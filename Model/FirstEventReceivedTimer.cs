using System;
using System.Timers;
using log4net;
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Model
{
    public class FirstEventReceivedTimer
    {
        private readonly ILog _log;

        public FirstEventReceivedTimer()
        {
            _log = LogManager.GetLogger(BlLog.EventProcessingLog);
        }

        public void Start()
        {
            var timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
            timer.Elapsed += delegate
                {
                    if (NesperPerformance.TotalEvents > 1)
                    {
                        _log.Warn("First event received");
                        timer.Enabled = false;
                        timer.Dispose();
                    }
                };
            timer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
            timer.Enabled = true;
            GC.KeepAlive(timer);
        }
    }
}