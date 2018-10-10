using System;
using System.Collections.Generic;
using System.ServiceModel;
using log4net;
using ZF.BL.Nesper.Model;
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Wcf.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BulkRulesService : IBulkRules
    {
        private readonly IBulkRulesManager _bulkRulesManagerMgr;
        private readonly ILog _log;
        private readonly WcfCall _wcfCall;

        public BulkRulesService(IBulkRulesManager bulkRulesManagerMgr)
        {
            if (bulkRulesManagerMgr == null) throw new ArgumentNullException("bulkRulesManagerMgr");

            _bulkRulesManagerMgr = bulkRulesManagerMgr;
            _log = LogManager.GetLogger(BlLog.EventProcessingLog);
            _wcfCall = new WcfCall();
        }

        public bool ReloadRules(Dictionary<string, string> idEplDictionary)
        {
            return _wcfCall.Wrap(() =>
                {
                    _log.Warn("Received request to reload all rules");

                    _bulkRulesManagerMgr.UnloadAll();

                    _bulkRulesManagerMgr.LoadAll(idEplDictionary);

                    return true;
                });
        }
    }
}