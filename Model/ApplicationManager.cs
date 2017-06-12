using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using com.espertech.esper.client;
using log4net;
using log4net.Config;
using ZF.BL.Nesper.Utils;
using ZF.BL.Nesper.Wcf.Client;
using ZF.BL.Nesper.Wcf.Service;
using Version = ZF.BL.Nesper.Utils.Version;

namespace ZF.BL.Nesper.Model
{
    public class ApplicationManager
    {
        private AlertDispatcher _alertConsumer;
        private ILog _log, _exLog;
        private LogHelper _logHelper;
        private IEventProcessor _nesper;
        private NesperWorkerThread _nesperWorker;
        private IBulkRulesManager _rulesMgr;
        private WcfHost _ruleWcfHost, _bulkRulesWcfHost;
        private AlertProducerProxy _alertProducerProxy;
        private PerformanceProxy _performanceProxy;

        public void Start()
        {
            PathLocator.SetCurrentDirectoryPathFromExecutingAssembly();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            SetupLoggers();

            _logHelper.LogWelcomeMessage();
            SetupNesper();
            SetupWcf();
            SetupTimers();
            _logHelper.LogFooter();

            StartDeque();
        }

        public void Stop()
        {
            _log.Info("BL is shutting down");

            // sequence is important

            // workers
            if (_nesperWorker != null) _nesperWorker.StopThread();
            _log.Info("Nesper worker thread is stopped");

            if (_alertConsumer != null) _alertConsumer.StopThread();
            _log.Info("Alert consumer thread is stopped");

            // WCF proxies
            if (_alertProducerProxy != null) _alertProducerProxy.Dispose();
            _log.Info("WCF alert producer proxy is disposed");

            if (_performanceProxy != null) _performanceProxy.Dispose();
            _log.Info("WCF performance proxy is disposed");
            
            // WCF hosting
            if (_ruleWcfHost != null) _ruleWcfHost.Stop();
            _log.Info("WCF rule service hosting is stopped");

            if (_bulkRulesWcfHost != null) _bulkRulesWcfHost.Stop();
            _log.Info("WCF bulk rules service hosting is stopped");
            
            _log.Info("Bye-bye.");
            LogManager.Shutdown();
        }

        private void SetupLoggers()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("BL.log4net.config"));
            _log = LogManager.GetLogger(BlLog.EventProcessingLog);
            _exLog = LogManager.GetLogger(BlLog.ExceptionLog);
            _logHelper = new LogHelper();
        }

        private void SetupNesper()
        {
            _logHelper.StartTimerFor("Nesper setup");
            var alertBuffer = new BlockingCollection<ActivityAlert>();

            _nesper = new NesperAdapter(alertBuffer);
            _nesper.Initialize();
            _alertProducerProxy = new AlertProducerProxy();
            _alertConsumer = new AlertDispatcher(alertBuffer, _alertProducerProxy);
            _alertConsumer.StartThread();

            string cfgName = "Release";
#if DEBUG
            cfgName = "Debug";
#endif
            string versionNesper = Version.GetAssemblyVersionFor<EPStatement>();
            _log.Debug($"Nesper               v.{versionNesper}");
            _log.Debug($"Configuration:       {cfgName}");

            _logHelper.StopAndLogTime();
        }

        private void StartDeque()
        {
            Task.Factory.StartNew(() =>
                {
                    string msmqName = ConfigurationManager.AppSettings["msmqName"];
                    bool dryRun = Convert.ToBoolean(ConfigurationManager.AppSettings["dryRun"]);

                    if (!dryRun)
                    {
                        _log.Debug("Dry run disabled. Waiting for any rule...");
                        _nesper.WaitUntilRuleAdded();
                    }

                    _nesperWorker = new NesperWorkerThread(_nesper, msmqName);
                    _nesperWorker.StartThread();

                    _performanceProxy = new PerformanceProxy();
                    var nesperPerformance = new NesperPerformance(_log, _performanceProxy);
                    string reportEveryNSec = ConfigurationManager.AppSettings["PerformanceSaveIntervalInSeconds"];
                    nesperPerformance.Start(Convert.ToInt32(reportEveryNSec));
                });
        }

        private void SetupWcf()
        {
            _logHelper.StartTimerFor("Wcf services");

            IRule ruleService = new RuleService(_nesper);
            _ruleWcfHost = new WcfHost(ruleService, "RuleService");
            _ruleWcfHost.Start();

            _rulesMgr = new BulkRulesManager(_nesper);
            IBulkRules syncService = new BulkRulesService(_rulesMgr);
            _bulkRulesWcfHost = new WcfHost(syncService, "BulkRulesService");
            _bulkRulesWcfHost.Start();

            _logHelper.StopAndLogTime();
        }

        private void SetupTimers()
        {
            var firstEventTimer = new FirstEventReceivedTimer();
            firstEventTimer.Start();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            _exLog.Error("Uncaught exception  in application domain", ex);
            LogManager.Shutdown();
        }
    }
}