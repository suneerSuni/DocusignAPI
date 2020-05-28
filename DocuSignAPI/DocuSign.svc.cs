using DocuSign.Utils;
using FillTheDoc.DAL;
using FillTheDoc.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

        public string SendforESign(SendDocumentInfo docDetails,  string tableStrXML = "", List<CLDocValue> value = null,List<SignerInfo> jointEmail = null, List<SignerInfo> cuEmail = null)
        {
            string DocuSignID=string.Empty;
            try
            {
                string base64WordDoc;
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["GetFileFromPath"]))
                {
                    base64WordDoc = Convert.ToBase64String(File.ReadAllBytes(string.Concat(ConfigurationManager.AppSettings["FilePath"])));
                }
                else
                {
                    base64WordDoc = docDetails.File_Sign;
                }
                DocuSignDAL dalObj = new DocuSignDAL();
                docDetails.File_Sign = new WordReader().FillValuesToDoc(Convert.FromBase64String(base64WordDoc), tableStrXML, value);
                DocuSignID = dalObj.SendforESign(docDetails, jointEmail, cuEmail);
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

