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
        string SendforESign(int id, string CUName, string CUEmail);

        [OperationContract]
        string FillDocument(int id, string CUName, string CUEmail);

        [OperationContract]
        string test(string data);

        [OperationContract]
        string TestSendForSign();
        // TODO: Add your service operations here
    }
}
