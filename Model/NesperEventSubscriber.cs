using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using com.espertech.esper.client;
using com.espertech.esper.events.bean;
using com.espertech.esper.events.map;
using log4net;

namespace ZF.BL.Nesper.Model
{
    internal sealed class NesperEventSubscriber
    {
        private readonly BlockingCollection<ActivityAlert> _alertBuffer;
        private readonly ILog _alertLog;
        private readonly string _ruleId;

        public NesperEventSubscriber(BlockingCollection<ActivityAlert> alertBuffer, string ruleId, ILog alertLog)
        {
            if (alertBuffer == null) throw new ArgumentNullException("alertBuffer");
            if (ruleId == null) throw new ArgumentNullException("ruleId");
            if (alertLog == null) throw new ArgumentNullException("alertLog");

            _alertLog = alertLog;
            _alertBuffer = alertBuffer;
            _ruleId = ruleId;
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
                throw new ArgumentNullException("e", "e.NewEvents");
            if (e.NewEvents.Length == 0)
                throw new InvalidOperationException("Engine event fired but no data provided about the event");
            
            var list = new List<ActivityEvent>();
            if (e.NewEvents.First().GetType() == typeof (MapEventBean))
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
                        var beans = kv.Value as EventBean[];
                        if (beans != null)
                        {
                            list.AddRange(beans.Select(x => (ActivityEvent) x.Underlying));
                        }

                        // one event, e.g. when we do "select ev1 from ..."
                        var bean = kv.Value as BeanEventBean;
                        if (bean != null)
                        {
                            list.Add((ActivityEvent)bean.Underlying);
                        }
                    }
            }
            else
            {
                list = (from row in e.NewEvents select (ActivityEvent)row.Underlying).ToList();
            }

            if (list.Count == 0)
                throw new InvalidOperationException("Failed to retrieve activity that triggered the alert");

            var alert = new ActivityAlert(_ruleId, DateTime.UtcNow, list.ToArray());

            var offset = new DateTimeOffset(DateTime.UtcNow);

            _alertLog.Warn($"{offset.ToString("dd-MM-yyyy HH:mm:ss")}, {alert.RuleId}, {alert.Events[0].User}");

            NesperPerformance.TotalAlerts++;
            _alertBuffer.Add(alert);
        }
    }
}