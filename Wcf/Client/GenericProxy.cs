using System;
using System.ServiceModel;

namespace ZF.BL.Nesper.Wcf.Client
{
    public class GenericProxy<T> : ClientBase<T>, IDisposable where T : class
    {
        private volatile bool _isDisposed;

        public GenericProxy()
        {
        }

        public GenericProxy(string endpointCfgName) : base(endpointCfgName)
        {
        }

        public TResult InvokeRemoteAction<TResult>(Func<TResult> function)
        {
            return function.Invoke();
        }

        public void InvokeRemoteAction(Action action)
        {
            action.Invoke();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    CloseOrAbort();

                _isDisposed = true;
            }
        }

        private void CloseOrAbort()
        {
            bool success = false;
            try
            {
                if (State != CommunicationState.Faulted)
                {
                    Close();
                    success = true;
                }
            }
            finally
            {
                if (!success)
                    Abort();
            }
        }
    }
}