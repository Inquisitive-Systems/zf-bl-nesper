using System;
using System.Runtime.Serialization;

namespace ZF.BL.Nesper.Wcf.Service
{
    [DataContract]
    public class ZoneFoxFault
    {
        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string InnerException { get; set; }

        [DataMember]
        public string AdditionalDetails { get; set; }

        public static ZoneFoxFault CreateFrom(Exception ex)
        {
            var fault = new ZoneFoxFault();
            if (ex.StackTrace != null) 
                fault.StackTrace = ex.StackTrace;
            if (ex.InnerException != null) 
                fault.InnerException = ex.InnerException.Message;

            fault.Message = ex.Message;
            fault.AdditionalDetails = ex.ToString();

            return fault;
        }
    }
}