﻿using System;
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
        // Assume a max ceiling for JSON serrialised alert - 5 MB 
        private const int MaxAlertSizeInBytes = 5 * 1024 * 1024;

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
        ///     otherwise Nesper engine wouldn't recognize it in runtime.
        ///     It is called by the engine subscribers and
        ///     we retrieve our objects from the UpdateEventArgs object
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

            SendChunkedAlerts(events);
        }

        // Chunk alerts based on the size of events
        // Send a new alert each time when the output message size is greater than N MB
        // For perfomrance reasons we only estimate the size of result alert 
        private void SendChunkedAlerts(List<ActivityEvent> events)
        {
            int estimatedAlertSizeInBytes = 0; // temp variable to hold current estimated message size
            var tmpEvents = new List<ActivityEvent>(); // temp list to store copies of events during multiple iterations

            // scan all events
            foreach (var ev in events)
            {
                // calculate approximate size of this alert and add event to the temp list
                estimatedAlertSizeInBytes += EstimateEventSizeInBytes(ev);
                tmpEvents.Add(ev);

                // have we reached max alert size limit?
                if (estimatedAlertSizeInBytes > MaxAlertSizeInBytes)
                {
                    // yes, let's send this 
                    SendAlert(tmpEvents, estimatedAlertSizeInBytes);

                    // clear up temp state
                    tmpEvents.Clear();
                    estimatedAlertSizeInBytes = 0;
                }
            }

            // check if we have any leftover events for which we did not send an alert 
            if (tmpEvents.Count > 0)
                SendAlert(tmpEvents, estimatedAlertSizeInBytes);
        }

        // Add alert to a shared memory buffer
        private void SendAlert(List<ActivityEvent> tmpEvents, int estimatedAlertSizeInBytes)
        {
            var alert = new ActivityAlert(_ruleId, DateTime.UtcNow, tmpEvents.ToArray());
            var offset = new DateTimeOffset(DateTime.UtcNow);

            string logMessage = $"New alert: {offset:dd-MM-yyyy HH:mm:ss}, rule id: {alert.RuleId}, " +
                                $"user {alert.Events[0].User}, {tmpEvents.Count} events " +
                                $"of estimates size {estimatedAlertSizeInBytes} bytes";
            _alertLog.Warn(logMessage);

            NesperPerformance.TotalAlerts++;
            _alertBuffer.Add(alert);
        }

        // Produce a quick best guess of event size in bytes
        // Current size is a lower bound for json object required to represent serialised event
        private int EstimateEventSizeInBytes(ActivityEvent ev)
        {
            // assume there's more things in the event, such as
            // size of "dd-MM-yyyyTHH:mm:ss+01:00" datetime string,
            // other json object properties and potentially network metadata 
            const int supplement = 60; 

            int estimatedSize = supplement + ev.Machine.Length + ev.User.Length + ev.Application.Length + ev.Activity.Length + ev.Resource.Length;

            // every character takes at least 2 bytes in Unicode which we use in C#
            estimatedSize *= 2; 

            return estimatedSize;
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