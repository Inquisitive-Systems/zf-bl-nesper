using System.ServiceModel;
using ZF.BL.Nesper.Wcf.Service;

namespace ZF.BL.Nesper.Wcf.Client
{
    [ServiceContract]
    public interface IAlert
    {
        [OperationContract]
        [FaultContract(typeof (ZoneFoxFault))]
        bool Produce(byte[] compressedJsonAlerts);
    }
}