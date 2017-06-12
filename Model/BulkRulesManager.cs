using System;
using System.Collections.Generic;
using System.Diagnostics;
using log4net;
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Model
{
    public class BulkRulesManager : IBulkRulesManager
    {
        private readonly IEventProcessor _eventProcessor;
        private readonly ILog _log;

        public BulkRulesManager(IEventProcessor eventProcessor)
        {
            if (eventProcessor == null) throw new ArgumentNullException(nameof(eventProcessor));

            _eventProcessor = eventProcessor;
            _log = LogManager.GetLogger(BlLog.EventProcessingLog);
        }

        public void LoadAll(Dictionary<string, string> idEplDictionary, bool exitOnFailure = false)
        {
            var logHelper = new LogHelper();
            logHelper.StartTimerFor("Rules insertion");
            foreach (var item in idEplDictionary)
            {
                try
                {
                    _eventProcessor.Load(item.Key, item.Value);
                    _log.Error($"Loaded rule id {item.Key}");
                }
                catch (ArgumentException)
                {
                    _log.Error($"Failed to load rule id {item.Key}");
                    if (exitOnFailure)
                        throw;
                }
            }
            logHelper.StopAndLogTime();
        }

        public void UnloadAll()
        {
            Stopwatch sw = Stopwatch.StartNew();
            _eventProcessor.UnloadAll();
            sw.Stop();
            _log.Debug($"All unloaded in {sw.ElapsedMilliseconds} ms");
        }
    }
}