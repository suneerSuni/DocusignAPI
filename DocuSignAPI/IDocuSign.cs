using DocuSignAPI.Model;
using FillTheDoc.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DocuSignAPI
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract ]
    public interface IDocuSignService
    {
        
        [OperationContract]
        SVCResults SendforESign(int id, string CUName, string CUEmail);

        [OperationContract]
        SVCResults FillDocument(int id, string CUName, string CUEmail,string DocuName,int dCount,string accountNo);

        [OperationContract]
        string test(string data);

        [OperationContract]
        SVCResults TestSendForSign(int id);
        [OperationContract]
        SVCResults TestFillDocument(string docName);
        // TODO: Add your service operations here
    }
}
