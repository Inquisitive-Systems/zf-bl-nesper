using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using com.espertech.esper.client;
using com.espertech.esper.client.deploy;
using log4net;
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Model
{
    public class NesperAdapter : IEventProcessor
    {
        private readonly BlockingCollection<ActivityAlert> _alertBuffer;
        private readonly ILog _log;
        private readonly ManualResetEvent _ruleAddedHandle = new ManualResetEvent(false);
        public Dictionary<string, NesperModuleItem> LoadedModules { get; private set; }
        private EventSender _activityEventSender;
        private EPServiceProvider _engine;
        private bool _isInitialized;

        // dictionary to keep track on the modules associated with a rule
        // if a rule contains EPL with our [DO NOT FIRE ALERT BLOCK START] blocks
        // we load into Nesper rules, but we don't subscribe to the alerts
        // the problem lies when we need to update the rule
        // for this we need to unload all the module statements in between the [*START]...[*STOP]

        public NesperAdapter(BlockingCollection<ActivityAlert> alertBuffer)
        {
            if (alertBuffer == null) throw new ArgumentNullException("alertBuffer");

            _isInitialized = false;
            _alertBuffer = alertBuffer;
            _log = LogManager.GetLogger(BlLog.EventProcessingLog);
            LoadedModules = new Dictionary<string, NesperModuleItem>();
        }

        public long EventsCounter
        {
            get { return _engine.EPRuntime.NumEventsEvaluated; }
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            //NOTE This call internally depend on libraries 
            //will throw an error if NEsper libs not found
            EPServiceProviderManager.PurgeDefaultProvider();
            var cfg = new Configuration();
            var evConfig = new ConfigurationEventTypeLegacy();
            evConfig.PropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;
            cfg.AddEventType<ActivityEvent>(typeof(ActivityEvent).Name, evConfig);
            cfg.EngineDefaults.Threading.IsInternalTimerEnabled = true;
            cfg.EngineDefaults.Threading.IsInsertIntoDispatchPreserveOrder = false;
            cfg.EngineDefaults.Threading.IsListenerDispatchPreserveOrder = false;
            cfg.EngineDefaults.Logging.IsEnableExecutionDebug = false;
            cfg.EngineDefaults.Logging.IsEnableADO = false;
            cfg.EngineDefaults.Logging.IsEnableQueryPlan = false;
            cfg.EngineDefaults.Logging.IsEnableTimerDebug = false;
            cfg.EngineDefaults.ViewResources.IsShareViews = false;
            cfg.AddImport<StringUtil>();

            _engine = EPServiceProviderManager.GetDefaultProvider(cfg);
            _engine.StatementStateChange += OnStatementChanged;

            _activityEventSender = _engine.EPRuntime.GetEventSender(typeof (ActivityEvent).Name);
            _isInitialized = true;
        }

        public void Load(string id, string epl)
        {
            InternalValidate(id, epl);

            EPStatement statement;
            try
            {
                statement = InsertEplStatementIntoNesper(id, epl);
            }
            catch (Exception ex)
            {
                string msg = "Failed to create a rule, based on EPL epl: " + epl;
                _log.Error(msg, ex);
                throw new ArgumentException(msg, ex);
            }

            //add subscriber for alerts
            _log.Info("Attaching subscriber");
            ILog alertLog = LogManager.GetLogger(BlLog.AlertsLog);
            var subscriber = new NesperEventSubscriber(_alertBuffer, id, alertLog);

			//Old method for attaching to nesper
            //statement.Subscriber = subscriber;

			//New method for attaching to nesper
            if(statement != null)
                statement.Events += subscriber.Update;
        }

        public void Validate(string id, string epl)
        {
            // id can be empty or null for validation call
            InternalValidate(epl);

            try
            {
                // check if the input contains any our delimiters
                // such as [DO NOT FIRE ALERT BLOCK START] ... [DO NOT FIRE ALERT BLOCK STOP] 
                var parser = new EplParser();
                bool shouldParse = parser.HasSubscriptionMarkers(epl);
                
                // Check if a block is present
                if (shouldParse) // yes
                {
                    EplParsedTuple tuple = parser.Parse(epl);

                    // Compile stage 1
                    // Read block of statements that doesn't require alert subscriber into module
                    Module module = _engine.EPAdministrator.DeploymentAdmin.Parse(tuple.EplScript);

                    // iterate through each epl separated by a semicolon ";"
                    foreach (var item in module.Items)
                    {
                        _engine.EPAdministrator.CompileEPL(item.Expression);
                    }
                    
                    // Compile stage 2
                    _engine.EPAdministrator.CompileEPL(tuple.StatementToFireAlert);
                    
                }
                else // no parsing needed, just compile whatever we have
                {
                    _engine.EPAdministrator.CompileEPL(epl);
                }
            }
            catch (Exception ex)
            {
                string newMessage = string.Concat("Rule ", id, " is invalid");
                throw new ArgumentException(newMessage, ex);
            }
        }

        public void Unload(string ruleId)
        {
            InternalValidate(ruleId);

            // check if the rule has associated module rules
			// NOTE this may need to be a checksum instead of the id
			if (LoadedModules.ContainsKey(ruleId))
            {
                LoadedModules[ruleId].UseCount--;

                if (LoadedModules[ruleId].UseCount <= 0)
                {
                    string id = LoadedModules[ruleId].Id;
                    _engine.EPAdministrator.DeploymentAdmin.Undeploy(id);
                    LoadedModules.Remove(ruleId);
                }
            }

            EPStatement statement = _engine.EPAdministrator.GetStatement(ruleId);
            if (statement == null) return;

            statement.Stop();
            statement.RemoveAllEventHandlers();
            statement.Dispose();
        }

        public void UnloadAll()
        {
            InternalValidate();

            _engine.EPAdministrator.StopAllStatements();
            _engine.EPAdministrator.DestroyAllStatements();

            LoadedModules.Clear();
        }

        public void PropagateEvent(ActivityEvent e)
        {
            NesperPerformance.TotalEvents++;
            _activityEventSender.SendEvent(e);
        }

        public void PropagateEvents(params ActivityEvent[] events)
        {
            NesperPerformance.TotalEvents += events.Length;

            for (int i = 0; i < events.Length; i++)
                _activityEventSender.SendEvent(events[i]);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void WaitUntilRuleAdded()
        {
            _ruleAddedHandle.WaitOne();
            Thread.Sleep(1000); // allow a bit of time in case many rules are being loaded
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _engine == null) return;
            _engine.Dispose();
            _engine = null;
        }

        private EPStatement InsertEplStatementIntoNesper(string id, string epl)
        {
            _log.Info($"Epl: {epl}");

            InternalValidate();

            // check if the input contains any our delimiters
            // such as [DO NOT FIRE ALERT BLOCK START] ... [DO NOT FIRE ALERT BLOCK STOP] 
            var parser = new EplParser();
            bool shouldParse = parser.HasSubscriptionMarkers(epl);

            EPStatement epStatement = null;

            // Check if a block is present
            if (shouldParse) // yes
            {
                _log.Info("Scripting block found");

                // read the two parts
                // first should just be imported into NEsper
                // without adding any subscriber 
                // second part should contain a single epl that
                // will fire an alert
                EplParsedTuple tuple = parser.Parse(epl);

                var md5 = new Md5();
                string hash = md5.GetHashAsHex(tuple.EplScript);
                var kvp = LoadedModules.FirstOrDefault(x => x.Value.Checksum == hash);
                bool isNewModule = kvp.Value == null;

                if (isNewModule)
                {
                    _log.Info("Creating module from script");

                    var result = _engine.EPAdministrator.DeploymentAdmin.ParseDeploy(tuple.EplScript);
                    var mi = new NesperModuleItem
                        {
                            Id = result.DeploymentId,
                            UseCount = 1,
                            Checksum = hash
                        };

                    _log.Info($"Scripting block contained {result.Statements.Count} statements");
                    
                    LoadedModules.Add(id, mi);
                }
                else
                {
                    kvp.Value.UseCount++;
                }

                // create epl that will be linked with a subscriber
                _log.Info("Creating statement that fires alert");
                if(!string.IsNullOrWhiteSpace(tuple.StatementToFireAlert)) //we can now potentially have some statements which don't actually need to be attached.
                    epStatement = _engine.EPAdministrator.CreateEPL(tuple.StatementToFireAlert, id);
            }
            else
            {
                _log.Info("Creating statement that fires alert");
                epStatement = _engine.EPAdministrator.CreateEPL(epl, id);
                _log.Info("Created id: " + epStatement.Name);
            }

            return epStatement;
        }

        private void OnStatementChanged(object sender, StatementStateEventArgs e)
        {
            if (e == null) throw new ArgumentNullException("e");
            _ruleAddedHandle.Set();
            _log.Debug($"{e.Statement.State}> {e.Statement.Name}");
        }

        private void InternalValidate(string id, string epl)
        {
            if (string.IsNullOrEmpty(epl)) throw new ArgumentNullException(nameof(epl));

            InternalValidate(id);
        }

        private void InternalValidate(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

            InternalValidate();
        }

        private void InternalValidate()
        {
            if (_engine == null) throw new InvalidOperationException("Engine has not bee initialised");
        }
    }
}