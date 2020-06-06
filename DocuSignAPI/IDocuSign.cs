﻿using FillTheDoc.Model;
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
        string SendforESign(SendDocumentInfo docDetails, string signerInfoJSon, string tableStrXML,  List<CLDocValue> value,int jointCount, string Type);

        [OperationContract]
        string test(string data);
        // TODO: Add your service operations here
    }
}
