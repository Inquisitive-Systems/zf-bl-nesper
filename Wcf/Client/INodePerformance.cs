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