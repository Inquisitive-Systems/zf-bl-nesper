/*
ZoneFox Business Layer event processor based on GNU NEsper
Copyright (C) 2018 ZoneFox

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

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
            int freeSystemMemory, int eventProcessingRate, int alertProcessingRate, long totalEvents, long totalAlerts)
        {
            _proxy.InvokeRemoteAction(() =>
                                      _service.PerformanceInfoUpdate(
                                          cpuPercentageUsedByCurrentProcess, memoryUsedByCurrentProcess,
                                          freeSystemMemory, eventProcessingRate, alertProcessingRate, totalEvents, totalAlerts));
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