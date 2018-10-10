using System;
using System.ServiceModel;
using log4net;
using ZF.BL.Nesper.Model;
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Wcf.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RuleService : IRule
    {
        private readonly IEventProcessor _adapter;
        private readonly ILog _log;
        private readonly WcfCall _wcfCall;

        public RuleService(IEventProcessor adapter)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");

            _adapter = adapter;
            _log = LogManager.GetLogger(BlLog.EventProcessingLog);
            _wcfCall = new WcfCall();
        }

        public bool Add(string id, string epl)
        {
            _log.Debug("Adding rule");

            return _wcfCall.Wrap(() =>
                {
                    if (string.IsNullOrEmpty(epl)) throw new ArgumentNullException("epl");
                    if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

                    var logHelper = new LogHelper();
                    logHelper.LogRuleDetail(epl, id);

                    _log.Debug(id + " > loading rule to NEsper");
                    _adapter.Load(id, epl);

                    return true;
                });
        }

        public bool Update(string id, string epl)
        {
            _log.Debug("Updating rule");

            return _wcfCall.Wrap(() =>
                {
                    if (string.IsNullOrEmpty(epl)) throw new ArgumentNullException("epl");
                    if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

                    _adapter.Unload(id);
                    return Add(id, epl);
                });
        }

        public bool Remove(string id)
        {
            _log.Debug("Removing rule");
            return _wcfCall.Wrap(() =>
                {
                    if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

                    _log.Debug(id + " > unloading rule from NEsper");
                    _adapter.Unload(id);

                    return true;
                });
        }

        public bool Validate(string id, string epl)
        {
            // allow id to be null or empty

            _log.Debug("Validating rule");

            return _wcfCall.Wrap(() =>
                {
                    if (string.IsNullOrEmpty(epl)) throw new ArgumentNullException("epl");

                    var logHelper = new LogHelper();
                    logHelper.LogRuleDetail(epl, id);

                    _log.Debug(id + " > Validating rule in NEsper");
                    _adapter.Validate(id, epl);

                    return true;
                });
        }
    }
}