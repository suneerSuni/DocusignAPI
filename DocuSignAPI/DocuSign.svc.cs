using DocuSign.Utils;
using FillTheDoc.DAL;
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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class DocuSignService : IDocuSignService
    {

        public string SendforESign(SendDocumentInfo docDetails, string placeHolderName = "", string saveFileAs = "", string tableStrXML = "", List<CLDocValue> value = null)
        {
            string DocuSignID=string.Empty;
            try
            {
                string base64WordDoc = new WordReader().FillValuesToDoc(Convert.FromBase64String(docDetails.File_Sign), tableStrXML, value);
                DocuSignDAL dalObj = new DocuSignDAL();
                docDetails.File_Sign = base64WordDoc;
                DocuSignID= dalObj.SendforESign(docDetails);
                return DocuSignID;
            }
            catch (Exception exception)
            {
                Utility.LogAction("Exception in ExecutePPPClosingDocument: " + exception.Message);
                throw exception;
            }
            
        }
    }

}

