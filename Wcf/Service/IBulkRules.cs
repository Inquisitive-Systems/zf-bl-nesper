using System.Collections.Generic;
using System.ServiceModel;

namespace ZF.BL.Nesper.Wcf.Service
{
    [ServiceContract]
    public interface IBulkRules
    {
        [OperationContract]
        [FaultContract(typeof (ZoneFoxFault))]
        bool ReloadRules(Dictionary<string, string> idEplDictionary);
    }
}