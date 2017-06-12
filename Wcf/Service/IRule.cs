using System.ServiceModel;

namespace ZF.BL.Nesper.Wcf.Service
{
    [ServiceContract]
    public interface IRule
    {
        [OperationContract]
        [FaultContract(typeof (ZoneFoxFault))]
        bool Add(string id, string epl);

        [OperationContract]
        [FaultContract(typeof (ZoneFoxFault))]
        bool Update(string id, string epl);

        [OperationContract]
        [FaultContract(typeof (ZoneFoxFault))]
        bool Remove(string id);

        [OperationContract]
        [FaultContract(typeof (ZoneFoxFault))]
        bool Validate(string id, string epl);
    }
}