using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using Newtonsoft.Json;
using ZF.BL.Nesper.Msmq;
using ZF.BL.Nesper.Utils;
using ZF.BL.Nesper.Wcf.Client;

namespace ZF.BL.Nesper.Model
{
    public class AlertDispatcher
    {
        // handle bursts of alerts as one batch, make this rather small so that the repos can save this
        public const int BufferSize = 20;      // number of items
        public const int TakeTimeout = 1000;    // msec
        public const int MaxLastSaved = 2000;   //msec

        private DateTime _lastProduced;
        private Thread _thread;

        private readonly BlockingCollection<ActivityAlert> _sharedBuffer;
        private readonly TimeSpan _takeTimout, _maxLastSavedTimeSpan;
        private volatile bool _isCanceled;
        private readonly IAlert _proxy;
        private readonly ILog _exLog;

        public AlertDispatcher(BlockingCollection<ActivityAlert> sharedBuffer, IAlert alertProducerProxy)
        {
            if (sharedBuffer == null) throw new ArgumentNullException("sharedBuffer");
            if (alertProducerProxy == null) throw new ArgumentNullException("alertProducerProxy");

            _sharedBuffer = sharedBuffer;
            _isCanceled = false;
            _proxy = alertProducerProxy;

            _lastProduced = DateTime.UtcNow;
            _takeTimout = TimeSpan.FromMilliseconds(TakeTimeout);
            _maxLastSavedTimeSpan = TimeSpan.FromMilliseconds(MaxLastSaved);
            _exLog = LogManager.GetLogger(BlLog.ExceptionLog);
        }

        public ActivityAlert ConsumeItem()
        {
            ActivityAlert item;
            if (_sharedBuffer.TryTake(out item, _takeTimout))
            {
                return item;
            }
            return null;
        }

        public void StartThread()
        {
            _thread = new Thread(WorkToDo)
            {
                Name = "Alerts consumer thread",
                Priority = ThreadPriority.AboveNormal
            };

            _thread.Start();
        }

        private void WorkToDo()
        {
            var compressor = new Lz4();

            while (!_isCanceled)
            {
                byte[] jsonBytes = new byte[0];
                byte[] compressedBytes = new byte[0];
                long alertListCount = 0;
                long eventListCount = 0;
                try
                {
                    var alerts = ConsumeInSmallBatch();

                    // return to the while if there is nothing to save
                    if (alerts.Item1.Count == 0) continue;
                    
                    //debug messaging incase anything goes wrong here.
                    alertListCount = alerts.Item1.Count;
                    eventListCount = alerts.Item1.Sum(x => x.Events.Length);

                    string json = JsonConvert.SerializeObject(alerts.Item1);
                    jsonBytes = Encoding.Unicode.GetBytes(json);
                    compressedBytes = compressor.Compress(jsonBytes);

                    _proxy.Produce(compressedBytes);
                }
                catch (Exception ex)
                {
                    _exLog.Error($"alertListCount: {alertListCount} ");
                    _exLog.Error($"eventListCount: {eventListCount} ");
                    _exLog.Error($"jsonBytes: {jsonBytes.Length} ");
                    _exLog.Error($"compressedBytes: {compressedBytes.Length} ");
                    _exLog.Error(ex);
                }
            }
        }

        // Conditions to return 
        // - Buffer is full
        // - Consume timeout
        // - Last saved was too far ago
        public Tuple<List<ActivityAlert>, AlertProduceReason> ConsumeInSmallBatch()
        {
            var alerts = new List<ActivityAlert>();
            var reason = AlertProduceReason.Undefined;

            while (true)
            {
                ActivityAlert alert = ConsumeItem();
                if (alert != null) alerts.Add(alert);

                if (alerts.Count >= BufferSize) reason = AlertProduceReason.MaxBufferSizeReached;
                if (alert == null) reason = AlertProduceReason.ConsumeTimeout;
                if (DateTime.UtcNow > _lastProduced.Add(_maxLastSavedTimeSpan)) reason = AlertProduceReason.LastSavedTooLongAgo;

                if (reason != AlertProduceReason.Undefined)
                {
                    _lastProduced = DateTime.UtcNow;
                    return new Tuple<List<ActivityAlert>, AlertProduceReason>(alerts, reason);
                }
            }
        }

        public void StopThread()
        {
            _isCanceled = true;
            _thread.Join();
        }
    }
}