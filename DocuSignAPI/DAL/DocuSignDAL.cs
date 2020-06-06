using CredentialManagement;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.Utils;
using FillTheDoc.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Credential = CredentialManagement.Credential;

namespace FillTheDoc.DAL
{
    public class DocuSignDAL
    {
        #region Properties

        string _apiUrl = "";
        string _password = "";
        string _email = "";
        string _integratorKey = "";

        bool _canExecuteRequest = true;
        #endregion

        public DocuSignDAL()
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseCredentialStore"]))
            {
                if (!GetFromCredentialStore()) _canExecuteRequest = false;
            }
            else
            {
                _email = ConfigurationManager.AppSettings["DocuSignUsername"];
                _password = ConfigurationManager.AppSettings["DocuSignPassword"];
            }
            _integratorKey = ConfigurationManager.AppSettings["DocuSignIntegratorKey"];
            _apiUrl = ConfigurationManager.AppSettings["DocuSignBasePath"];

            ApiClient apiClient = new ApiClient(_apiUrl);
            DocuSign.eSign.Client.Configuration.Default.ApiClient = apiClient;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        #region Public Methods

        public string SendforESign(SendDocumentInfo docDetails)
        {
            try
            {
                if (!_canExecuteRequest)
                {
                    throw new Exception("Failed to retrieve Credentials from Credential store");
                }

                if (Utility.IsEventLogged) Utility.LogAction("Request Send document");

                string authHeader = "{\"Username\":\"" + _email + "\", \"Password\":\"" + _password + "\", \"IntegratorKey\":\"" + _integratorKey + "\"}";
                DocuSign.eSign.Client.Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

                string accountId = (!string.IsNullOrEmpty(Utility.DocuSignAccountID)) ? Utility.DocuSignAccountID : GetAccountID();

                EnvelopeDefinition envDef = new EnvelopeDefinition();
                envDef.EmailSubject = ConfigurationManager.AppSettings["DocuSignEmailSubject"];
                envDef.EmailBlurb = ConfigurationManager.AppSettings["DocuSignEmailBody"];
                if (Utility.IsEventLogged) Utility.LogAction("Creating document object");

                Document doc = new Document();
                doc.DocumentBase64 = docDetails.File_Sign;
                doc.Name = ConfigurationManager.AppSettings["DocumentName"];
                doc.DocumentId = "1";
                doc.FileExtension = "docx";

                envDef.Documents = new List<Document>();
                envDef.Documents.Add(doc);

                envDef.Recipients = new Recipients();
                envDef.Recipients.Signers = new List<Signer>();
                

                if (Utility.IsEventLogged) Utility.LogAction("Creating recipient and sign tabs");

                foreach (var reci in docDetails.SignerDetails)
                {
                    Signer signer = new Signer();

                    signer.Name = reci.ReciName;
                    signer.Email = reci.ReciEmail;
                    signer.RecipientId = reci.ReciId;
                    signer.RoutingOrder = reci.ReciId;
                    //signer.IdCheckConfigurationName= "ID Check";
                    signer.Tabs = new Tabs();
                    signer.Tabs.SignHereTabs = new List<SignHere>();
                    SignHere signHere = new SignHere();

                    signHere.DocumentId = "1";
                    signHere.RecipientId = "1";
                    signHere.AnchorString = "member$ign";
                    signHere.AnchorXOffset = "0";
                    signHere.AnchorYOffset = "0";
                    signHere.AnchorUnits = "inches";
                    signHere.AnchorIgnoreIfNotPresent = "false";
                    signer.Tabs.SignHereTabs.Add(signHere);
                    envDef.Recipients.Signers.Add(signer);
                    int jointOwnerCount = 0;
                    if (docDetails.JointSignerDetails != null && docDetails.JointSignerDetails.Count>0)
                    {
                        jointOwnerCount = docDetails.JointSignerDetails.Count;
                        for (int i = 0; i < docDetails.JointSignerDetails.Count; i++)
                        {
                            Signer jointSigner = new Signer();

                            signer.Name = docDetails.JointSignerDetails[i].ReciName;
                            signer.Email = docDetails.JointSignerDetails[i].ReciEmail;
                            signer.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            signer.RoutingOrder = Convert.ToString((i + 2));
                            //signer.IdCheckConfigurationName= "ID Check";
                            signer.Tabs = new Tabs();
                            signer.Tabs.SignHereTabs = new List<SignHere>();
                            SignHere jointSignerTab = new SignHere();
                            jointSignerTab.DocumentId = "1";
                            jointSignerTab.RecipientId = "1";
                            jointSignerTab.AnchorString = "Joint0w$ign" + (i + 1);
                            jointSignerTab.AnchorXOffset = "0";
                            jointSignerTab.AnchorYOffset = "0";
                            jointSignerTab.AnchorUnits = "inches";
                            jointSignerTab.AnchorIgnoreIfNotPresent = "true";
                            jointSigner.Tabs.SignHereTabs.Add(jointSignerTab);
                            envDef.Recipients.Signers.Add(jointSigner);
                        }
                    }

                    if (docDetails.CUSignerDetails != null && docDetails.CUSignerDetails.Count > 0)
                    {
                        for (int i = 0; i < docDetails.CUSignerDetails.Count; i++)
                        {
                            Signer jointSigner = new Signer();

                            signer.Name = docDetails.CUSignerDetails[i].ReciName;
                            signer.Email = docDetails.CUSignerDetails[i].ReciEmail;
                            signer.RecipientId = docDetails.CUSignerDetails[i].ReciId;
                            signer.RoutingOrder = Convert.ToString((i + 2+ jointOwnerCount));
                            //signer.IdCheckConfigurationName= "ID Check";
                            signer.Tabs = new Tabs();
                            signer.Tabs.SignHereTabs = new List<SignHere>();
                            SignHere jointSignerTab = new SignHere();
                            jointSignerTab.DocumentId = "1";
                            jointSignerTab.RecipientId = "1";
                            jointSignerTab.AnchorString = "Joint0w$ign" + (i + 1);
                            jointSignerTab.AnchorXOffset = "0";
                            jointSignerTab.AnchorYOffset = "0";
                            jointSignerTab.AnchorUnits = "inches";
                            jointSignerTab.AnchorIgnoreIfNotPresent = "true";
                            jointSigner.Tabs.SignHereTabs.Add(jointSignerTab);
                            envDef.Recipients.Signers.Add(jointSigner);
                        }
                    }

                }

                envDef.Status = "sent";

                if (Utility.IsEventLogged) Utility.LogAction("Sending Create and send envelope request");

                EnvelopesApi envelopesApi = new EnvelopesApi();
                EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

                return envelopeSummary.EnvelopeId;
            }
            catch (Exception ex)
            {
                Utility.LogAction("sending document to DocuSign failed with exception: " + ex.Message);
                return string.Empty;
            }
        }
       
        public string RequestSignedDocument(string envelopeId)
        {
            try
            {
                if (!_canExecuteRequest)
                {
                    throw new Exception("Failed to retrieve Credentials from Credential store");
                }


                if (Utility.IsEventLogged) Utility.LogAction("Request signed document");

                string authHeader = "{\"Username\":\"" + _email + "\", \"Password\":\"" + _password + "\", \"IntegratorKey\":\"" + _integratorKey + "\"}";
                DocuSign.eSign.Client.Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

                EnvelopesApi envelopesApi = new EnvelopesApi();

                string accountId = (!string.IsNullOrEmpty(Utility.DocuSignAccountID)) ? Utility.DocuSignAccountID : GetAccountID();

                if (Utility.IsEventLogged) Utility.LogAction("Sending request to get the envelope");

                Envelope envelope = envelopesApi.GetEnvelope(accountId, envelopeId);

                envelopesApi = new EnvelopesApi();
                byte[] signedDocArray = null;
                if (envelope.Status.ToLower() == "completed")
                {
                    if (Utility.IsEventLogged) Utility.LogAction("Sending request to get the list of documents");

                    EnvelopeDocumentsResult docsList = envelopesApi.ListDocuments(accountId, envelopeId);

                    if (docsList.EnvelopeDocuments.Count > 0)
                    {
                        if (Utility.IsEventLogged) Utility.LogAction("Sending request to get the signed document");

                        MemoryStream docStream = (MemoryStream)envelopesApi.GetDocument(accountId, docsList.EnvelopeId, docsList.EnvelopeDocuments[0].DocumentId);

                        signedDocArray = docStream.ToArray();
                        //SaveToLocation(signedDocArray);
                    }

                }
                return signedDocArray != null ? Convert.ToBase64String(signedDocArray) : "";
            }
            catch (Exception ex)
            {
                Utility.LogAction("sending request to get the signed documet failed with exception: " + ex.Message);
                return string.Empty;
            }

        }

        #endregion


        #region Private Methods

       
        private bool GetFromCredentialStore()
        {
            try
            {
                Credential credential = new Credential();
                credential.Target = ConfigurationManager.AppSettings["CredentialStoreKey"];
                credential.Type = CredentialType.Generic;
                credential.PersistanceType = PersistanceType.Enterprise;
                if (!credential.Load())
                {
                    Utility.LogAction("Failed to load credential from credential manager ");
                    return false;
                }
                _email = credential.Username;
                _password = credential.Password;
                return true;
            }
            catch (Exception ex)
            {
                Utility.LogAction("Credential manager failed with exception: " + ex.Message);
                return false;
            }

        }

        private string GetAccountID()
        {

            if (Utility.IsEventLogged) Utility.LogAction("Get Account ID");

            string authHeader = "{\"Username\":\"" + _email + "\", \"Password\":\"" + _password + "\", \"IntegratorKey\":\"" + _integratorKey + "\"}";
            DocuSign.eSign.Client.Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

            if (Utility.IsEventLogged) Utility.LogAction("Auth Login");

            AuthenticationApi authApi = new AuthenticationApi();
            LoginInformation loginInfo = authApi.Login();

            return Utility.DocuSignAccountID = loginInfo.LoginAccounts[0].AccountId;
        }

        #endregion
    }
}