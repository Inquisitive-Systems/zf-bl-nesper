using System;
using System.ServiceModel;
using log4net;
using ZF.BL.Nesper.Utils;

namespace ZF.BL.Nesper.Wcf.Service
{
    public class WcfCall
    {
        private readonly ILog _exLog;

        public WcfCall()
        {
            _exLog = LogManager.GetLogger(BlLog.ExceptionLog);
        }

        public void Wrap(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
            }
        }

        public TResult Wrap<TResult>(Func<TResult> action)
        {
            if (action == null) throw new ArgumentNullException("action");

            try
            {
                return action.Invoke();
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
            }
        }

        private FaultException HandleException(Exception ex)
        {
            //we need to look at the exception inside here and determine what on earth to do with it.
            _exLog.Error(ex.ToString());
            ZoneFoxFault fault = ZoneFoxFault.CreateFrom(ex);

            string reason = !string.IsNullOrWhiteSpace(fault.InnerException) 
                ? string.Format("{0}", fault.InnerException) 
                : string.Format("{0}", fault.Message);

            return new FaultException<ZoneFoxFault>(fault, new FaultReason(reason));
        }
    }
}