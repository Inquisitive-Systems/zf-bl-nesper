using System;

namespace ZF.BL.Nesper.Wcf.Client
{
    public class AlertProducerProxy : IAlert, IDisposable
    {
        private readonly GenericProxy<IAlert> _proxy;
        private readonly IAlert _service;
        private volatile bool _isDisposed;

        public AlertProducerProxy()
        {
            _proxy = new GenericProxy<IAlert>();
            _service = _proxy.ChannelFactory.CreateChannel();
        }

        public bool Produce(byte[] compressedJsonAlerts)
        {
            return _proxy.InvokeRemoteAction(() => _service.Produce(compressedJsonAlerts));
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
                    _proxy.Dispose();

                _isDisposed = true;
            }
        }
    }
}