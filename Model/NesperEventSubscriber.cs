using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using com.espertech.esper.client;
using com.espertech.esper.events.bean;
using com.espertech.esper.events.map;
using log4net;
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Model
{
    internal sealed class NesperEventSubscriber
    {
        private readonly ILog _alertLog = LogManager.GetLogger(BlLog.AlertsLog);

        private readonly BlockingCollection<ActivityAlert> _alertBuffer;
        private readonly string _ruleId;

        public NesperEventSubscriber(BlockingCollection<ActivityAlert> alertBuffer, string ruleId)
        {
            _alertBuffer = alertBuffer ?? throw new ArgumentNullException(nameof(alertBuffer));
            _ruleId = ruleId ?? throw new ArgumentNullException(nameof(ruleId));
        }

        /// <summary>
        ///     This method must have this specific signature,
        ///     otherwise engine wouldn't recognize it in runtime.
        ///     It is called by the engine subscribers and
        ///     we retrieve our objects from the passed in args
        /// </summary>
        public void Update(object sender, UpdateEventArgs e)
        {
            if (e.NewEvents == null)
                throw new ArgumentNullException(nameof(e), "e.NewEvents");
            if (e.NewEvents.Length == 0)
                throw new InvalidOperationException("Engine event fired but no data provided about the event");
            
            var events = ExtractEvents(e);

            if (events.Count == 0)
                throw new InvalidOperationException("Failed to retrieve activity that triggered the alert");

            // We need to chunk events, in case there's too many of them
            // We saw one alert of 38 MB in production stored in ES
            // Chunk events by 200, make a new alert for each batch of events
            var chunkedEvents = events.ChunkBy(200);

            foreach (var chunk in chunkedEvents)
            {
                var alert = new ActivityAlert(_ruleId, DateTime.UtcNow, events.ToArray());
                var offset = new DateTimeOffset(DateTime.UtcNow);
                _alertLog.Warn($"New alert: {offset:dd-MM-yyyy HH:mm:ss}, rule id: {alert.RuleId}, user {alert.Events[0].User}, {chunk.Count} events");
                NesperPerformance.TotalAlerts++;
                _alertBuffer.Add(alert);
            }
        }

        private List<ActivityEvent> ExtractEvents(UpdateEventArgs e)
        {
            var list = new List<ActivityEvent>();

            // process dictionary of events
            if (e.NewEvents.First().GetType() == typeof(MapEventBean))
            {
                foreach (var mapBean in e.NewEvents)
                foreach (var kv in ((IDictionary<string, object>) mapBean.Underlying))
                {
                    // kv.Value can be 
                    //   - Many events, e.g. EventBean[]
                    //   - One events, e.g. BeanEventBean
                    //   - Null, e.g. when we looking for a missing event like A and NOT B in outer join

                    if (kv.Value == null)
                        continue; // not interesed in the missing event, carry on looping

                    // many events, e.g. when we do "select ev1, ev2, ev3 ... "
                    if (kv.Value is EventBean[] beans)
                    {
                        list.AddRange(beans.Select(x => (ActivityEvent) x.Underlying));
                    }

                    // one event, e.g. when we do "select ev1 from ..."
                    if (kv.Value is BeanEventBean bean)
                    {
                        list.Add((ActivityEvent) bean.Underlying);
                    }
                }
            }
            else // process events
            {
                list = (from row in e.NewEvents select (ActivityEvent) row.Underlying).ToList();
            }

            return list;
        }
    }
}