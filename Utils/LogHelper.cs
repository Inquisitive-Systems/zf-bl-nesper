using System;
using System.Diagnostics;
using log4net;

namespace ZF.BL.Nesper.Utils
{
    public class LogHelper
    {
        private readonly ILog _log;
        private string _name;
        private Stopwatch _startupStopWatch;
        private Stopwatch _sw;

        public LogHelper()
        {
            _log = LogManager.GetLogger(BlLog.EventProcessingLog);
        }

        public void LogRuleDetail(string epl, string id)
        {
            _log.Debug(id);
            _log.Debug(epl);
        }

        public void LogWelcomeMessage()
        {
            _startupStopWatch = Stopwatch.StartNew();

            _log.Info("====================================================");
            _log.Info("===================***ZoneFox***====================");
            _log.Info("====================================================");
            _log.Info("                 ZoneFox.Bl.Nesper");

            string versionBl = Version.GetAssemblyVersionFor<LogHelper>();

            _log.Info("                     v." + versionBl);
            _log.Info("----------------------------------------------------");
        }

        public void LogFooter()
        {
            //Footer
            _log.Info("----------------------------------------------------");
            _log.Info($"ZoneFox.Bl.Nesper started in {_startupStopWatch.Elapsed.ToPrettySeconds()}");
            _log.Info("----------------------------------------------------");
        }

        public void StartTimerFor(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            _name = name;
            _sw = Stopwatch.StartNew();
        }

        public void StopAndLogTime()
        {
            _sw.Stop();
            string msg = $"{_name} done in {_sw.Elapsed.ToPrettySeconds()}";
            _log.Debug(msg);
        }
    }
}