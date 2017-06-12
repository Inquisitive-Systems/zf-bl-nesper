using System;

namespace ZF.BL.Nesper.Wcf.Client
{
    public class PerformanceProxy : INodePerformance, IDisposable
    {
        private readonly GenericProxy<INodePerformance> _proxy;
        private readonly INodePerformance _service;
        private volatile bool _isDisposed;

        public PerformanceProxy()
        {
            _proxy = new GenericProxy<INodePerformance>();
            _service = _proxy.ChannelFactory.CreateChannel();
        }

        public void HardwareInfoUpdate(
            string cpuNameAndSpeed, int numberOfCores, int totalRamInMb,
            int memoryBusSpeed, string platformName, string ipAddresses)
        {
            _proxy.InvokeRemoteAction(() =>
                                      _service.HardwareInfoUpdate(
                                          cpuNameAndSpeed, numberOfCores, totalRamInMb,
                                          memoryBusSpeed, platformName, ipAddresses));
        }

        public void PerformanceInfoUpdate(
            int cpuPercentageUsedByCurrentProcess, int memoryUsedByCurrentProcess,
            int freeSystemMemory, int eventProcessingRate, int alertProcessingRate)
        {
            _proxy.InvokeRemoteAction(() =>
                                      _service.PerformanceInfoUpdate(
                                          cpuPercentageUsedByCurrentProcess, memoryUsedByCurrentProcess,
                                          freeSystemMemory, eventProcessingRate, alertProcessingRate));
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