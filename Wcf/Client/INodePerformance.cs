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

using System.ServiceModel;
using ZF.BL.Nesper.Wcf.Service;

namespace ZF.BL.Nesper.Wcf.Client
{
    [ServiceContract]
    public interface INodePerformance
    {
        [OperationContract]
        [FaultContract(typeof (ZoneFoxFault))]
        void HardwareInfoUpdate(
            string cpuNameAndSpeed,
            int numberOfCores,
            int totalRamInMb,
            int memoryBusSpeed,
            string platformName,
            string ipAddresses);

        [OperationContract]
        [FaultContract(typeof (ZoneFoxFault))]
        void PerformanceInfoUpdate(
            int cpuPercentageUsedByCurrentProcess,
            int memoryUsedByCurrentProcess,
            int freeSystemMemory,
            int eventProcessingRate,
            int alertProcessingRate,
            long totalEvents, long totalAlerts);
    }
}