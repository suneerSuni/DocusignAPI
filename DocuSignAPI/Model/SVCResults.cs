using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DocuSignAPI.Model
{
    /// <summary>
    /// Throw the exception details to client
    /// </summary>
    [DataContract]
    public class SVCResults
    {
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string DocuSignID { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public string InnerException { get; set; }
        [DataMember]
        public string FilledDocument { get; set; }
        [DataMember]
        public string StackTrace { get; set; }
    }
}