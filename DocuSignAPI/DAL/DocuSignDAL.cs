using CredentialManagement;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.Utils;
using DocuSignAPI.Model;
using DocuSignService.Util;
using FillTheDoc.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
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
       
        string _downloadFileLocation = "";
        string _impUser = "";
        string _impPwd = "";
        string _impDomain = "";
        bool _isImpersonate = false;

      
       
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

            _downloadFileLocation = ConfigurationManager.AppSettings["DownloadFileLocation"];
            _impUser = ConfigurationManager.AppSettings["ImperosonationUser"];
            _impPwd = ConfigurationManager.AppSettings["ImperosonationPwd"];
            _impDomain = ConfigurationManager.AppSettings["ImperosonationDomain"];
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["IsImpersonate"]))
                _isImpersonate = ConfigurationManager.AppSettings["IsImpersonate"].ToLower() == "true" ? true : false;


            ApiClient apiClient = new ApiClient(_apiUrl);
            DocuSign.eSign.Client.Configuration.Default.ApiClient = apiClient;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        #region Public Methods

        public string SendforESign(SendDocumentInfo docDetails, List<FileDetail> fileDetails,string CUName,string CUEmail)
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

                envDef.Documents = new List<Document>();

                for (int i = 0; i < fileDetails.Count(); i++)
                {
                    Document doc = new Document();
                    doc.DocumentBase64 = fileDetails.ElementAt(i).base64WordDoc;
                    doc.Name = fileDetails.ElementAt(i).name;
                    doc.DocumentId = fileDetails.ElementAt(i).id;
                    doc.FileExtension = "docx";
                    envDef.Documents.Add(doc);
                }

                envDef.Recipients = new Recipients();
                envDef.Recipients.Signers = new List<Signer>();
                envDef.Recipients.InPersonSigners = new List<InPersonSigner>();

                if (Utility.IsEventLogged) Utility.LogAction("Creating recipient and sign tabs");

                foreach (var reci in docDetails.SignerDetails)
                {
                    if (docDetails.SignerDetails.ElementAt(0).IsInPerson.ToLower() == "true")
                    {
                        InPersonSigner signer = new InPersonSigner();
                        signer.Tabs = new Tabs();
                        signer.Tabs.SignHereTabs = new List<SignHere>();
                        signer.Tabs.DateSignedTabs = new List<DateSigned>();
                        signer.Tabs.TextTabs = new List<Text>();
                        signer.Tabs.DateTabs = new List<Date>();
                        signer.Tabs.CheckboxTabs = new List<Checkbox>();

                        //signer.Name = reci.ReciName;
                        //signer.Email = reci.ReciEmail;
                        //signer.RecipientId = reci.ReciId;
                        //signer.RoutingOrder = reci.ReciId;

                        signer.RecipientId = reci.ReciId;
                        signer.RoutingOrder = reci.ReciId;
                        signer.InPersonSigningType = "inPersonSigner";
                        signer.SignerName = reci.ReciName;
                        signer.HostName = CUName;
                        signer.HostEmail = CUEmail;
                        //signer.AutoNavigation = "true";
                        //signer.DefaultRecipient = "true";

                        SignHere signHere = new SignHere();
                        signHere.DocumentId = "1";
                        signHere.RecipientId = reci.ReciId;
                        signHere.AnchorString = "member$ign";
                        signHere.AnchorXOffset = "0";
                        signHere.AnchorYOffset = "0";
                        signHere.AnchorUnits = "inches";
                        signHere.AnchorIgnoreIfNotPresent = "false";

                        signer.Tabs.SignHereTabs.Add(signHere);

                        DateSigned signed = new DateSigned();
                        signed.DocumentId = "1";
                        signed.RecipientId = reci.ReciId;
                        signed.AnchorString = "member$date";
                        signed.AnchorXOffset = "0";
                        signed.AnchorYOffset = "0";
                        signed.AnchorUnits = "inches";
                        signed.AnchorIgnoreIfNotPresent = "true";

                        signer.Tabs.DateSignedTabs.Add(signed);

                        for (int j = 1; j <= 16; j++)
                        {
                            Text textTab = new Text();
                            textTab.DocumentId = "1";
                            textTab.RecipientId = "1";
                            textTab.AnchorString = "member1$1nput" + j;
                            textTab.TabLabel = "OptionalBeneficiaryInfo" + j;
                            textTab.AnchorXOffset = "0";
                            textTab.AnchorYOffset = "-3";
                            textTab.AnchorUnits = "pixels";
                            textTab.AnchorIgnoreIfNotPresent = "true";
                            textTab.AnchorMatchWholeWord = "true";
                            textTab.Required = "false";
                            textTab.Width = "75";
                            textTab.Height = "15";
                            textTab.FontSize = "Size10";
                            textTab.Font = "TimesNewRoman";
                            signer.Tabs.TextTabs.Add(textTab);
                        }


                        for (int k = 1; k <= 8; k++)
                        {
                            Date dateTab = new Date();
                            dateTab.DocumentId = "1";
                            dateTab.RecipientId = "1";
                            dateTab.AnchorString = "OBI$D01B$" + k;
                            dateTab.TabLabel = "OBIDate" + k;
                            dateTab.AnchorXOffset = "0";
                            dateTab.AnchorYOffset = "-3";
                            dateTab.AnchorUnits = "pixels";
                            dateTab.AnchorIgnoreIfNotPresent = "true";
                            dateTab.AnchorMatchWholeWord = "true";
                            dateTab.Required = "false";
                            dateTab.Width = "50";
                            dateTab.Height = "15";
                            dateTab.FontSize = "Size9";
                            dateTab.Font = "Arial";
                            signer.Tabs.DateTabs.Add(dateTab);
                        }

                        for (int c = 1; c <= 5; c++)
                        {
                            Checkbox checkBox = new Checkbox();
                            checkBox.DocumentId = "1";
                            checkBox.RecipientId = "1";
                            checkBox.AnchorString = "cu0$chk" + c;
                            checkBox.TabLabel = "MSRCheckBox" + c;
                            checkBox.AnchorXOffset = "0";
                            checkBox.AnchorYOffset = "0";
                            checkBox.AnchorUnits = "inches";
                            checkBox.AnchorIgnoreIfNotPresent = "true";

                            signer.Tabs.CheckboxTabs.Add(checkBox);
                        }

                        envDef.Recipients.InPersonSigners.Add(signer);
                    }
                    else
                    {

                        Signer signer = new Signer();
                        signer.Tabs = new Tabs();
                        signer.Tabs.SignHereTabs = new List<SignHere>();
                        signer.Tabs.DateSignedTabs = new List<DateSigned>();
                        signer.Tabs.TextTabs = new List<Text>();
                        signer.Tabs.DateTabs = new List<Date>();
                        signer.Tabs.CheckboxTabs = new List<Checkbox>();

                        signer.Name = reci.ReciName;
                        signer.Email = reci.ReciEmail;
                        signer.RecipientId = reci.ReciId;
                        signer.RoutingOrder = reci.ReciId;

                        SignHere signHere = new SignHere();
                        signHere.DocumentId = "1";
                        signHere.RecipientId = reci.ReciId;
                        signHere.AnchorString = "member$ign";
                        signHere.AnchorXOffset = "0";
                        signHere.AnchorYOffset = "0";
                        signHere.AnchorUnits = "inches";
                        signHere.AnchorIgnoreIfNotPresent = "false";

                        signer.Tabs.SignHereTabs.Add(signHere);

                        DateSigned signed = new DateSigned();
                        signed.DocumentId = "1";
                        signed.RecipientId = reci.ReciId;
                        signed.AnchorString = "member$date";
                        signed.AnchorXOffset = "0";
                        signed.AnchorYOffset = "0";
                        signed.AnchorUnits = "inches";
                        signed.AnchorIgnoreIfNotPresent = "true";

                        signer.Tabs.DateSignedTabs.Add(signed);

                        for (int j = 1; j <= 16; j++)
                        {
                            Text textTab = new Text();
                            textTab.DocumentId = "1";
                            textTab.RecipientId = "1";
                            textTab.AnchorString = "member1$1nput" + j;
                            textTab.TabLabel = "OptionalBeneficiaryInfo" + j;
                            textTab.AnchorXOffset = "0";
                            textTab.AnchorYOffset = "-3";
                            textTab.AnchorUnits = "pixels";
                            textTab.AnchorIgnoreIfNotPresent = "true";
                            textTab.AnchorMatchWholeWord = "true";
                            textTab.Required = "false";
                            textTab.Width = "75";
                            textTab.Height = "15";
                            textTab.FontSize = "Size10";
                            textTab.Font = "TimesNewRoman";
                            signer.Tabs.TextTabs.Add(textTab);
                        }


                        for (int k = 1; k <= 8; k++)
                        {
                            Date dateTab = new Date();
                            dateTab.DocumentId = "1";
                            dateTab.RecipientId = "1";
                            dateTab.AnchorString = "OBI$D01B$" + k;
                            dateTab.TabLabel = "OBIDate" + k;
                            dateTab.AnchorXOffset = "0";
                            dateTab.AnchorYOffset = "-3";
                            dateTab.AnchorUnits = "pixels";
                            dateTab.AnchorIgnoreIfNotPresent = "true";
                            dateTab.AnchorMatchWholeWord = "true";
                            dateTab.Required = "false";
                            dateTab.Width = "50";
                            dateTab.Height = "15";
                            dateTab.FontSize = "Size9";
                            dateTab.Font = "Arial";
                            signer.Tabs.DateTabs.Add(dateTab);
                        }

                        for (int c = 1; c <= 5; c++)
                        {
                            Checkbox checkBox = new Checkbox();
                            checkBox.DocumentId = "1";
                            checkBox.RecipientId = "1";
                            checkBox.AnchorString = "cu0$chk" + c;
                            checkBox.TabLabel = "MSRCheckBox" + c;
                            checkBox.AnchorXOffset = "0";
                            checkBox.AnchorYOffset = "0";
                            checkBox.AnchorUnits = "inches";
                            checkBox.AnchorIgnoreIfNotPresent = "true";

                            signer.Tabs.CheckboxTabs.Add(checkBox);
                        }

                        envDef.Recipients.Signers.Add(signer);
                    }
                }

                if (docDetails.JointSignerDetails != null && docDetails.JointSignerDetails.Count > 0)
                {
                    for (int i = 0; i < docDetails.JointSignerDetails.Count; i++)
                    {
                        if (docDetails.JointSignerDetails.ElementAt(i).IsInPerson.ToLower() == "true")
                        {
                            InPersonSigner jointSigner = new InPersonSigner();
                            jointSigner.Tabs = new Tabs();
                            jointSigner.Tabs.SignHereTabs = new List<SignHere>();
                            jointSigner.Tabs.DateSignedTabs = new List<DateSigned>();
                            jointSigner.Tabs.TextTabs = new List<Text>();
                            jointSigner.Tabs.DateTabs = new List<Date>();
                            jointSigner.Tabs.CheckboxTabs = new List<Checkbox>();

                            //jointSigner.Name = docDetails.JointSignerDetails[i].ReciName;
                            //jointSigner.Email = docDetails.JointSignerDetails[i].ReciEmail;
                            //jointSigner.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            //jointSigner.RoutingOrder = docDetails.JointSignerDetails[i].ReciId;

                            jointSigner.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            jointSigner.RoutingOrder = docDetails.JointSignerDetails[i].ReciId;
                            jointSigner.InPersonSigningType = "inPersonSigner";
                            jointSigner.SignerName = docDetails.JointSignerDetails[i].ReciName;
                            jointSigner.HostName = CUName;
                            jointSigner.HostEmail = CUEmail;

                            SignHere jointSignerTab = new SignHere();
                            jointSignerTab.DocumentId = "1";
                            jointSignerTab.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            jointSignerTab.AnchorString = "Joint0w$ign" + (i + 1);
                            jointSignerTab.AnchorXOffset = "0";
                            jointSignerTab.AnchorYOffset = "0";
                            jointSignerTab.AnchorUnits = "inches";
                            jointSignerTab.AnchorIgnoreIfNotPresent = "true";

                            jointSigner.Tabs.SignHereTabs.Add(jointSignerTab);

                            DateSigned jointSigned = new DateSigned();
                            jointSigned.DocumentId = "1";
                            jointSigned.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            jointSigned.AnchorString = "Joint0w$date" + (i + 1);
                            jointSigned.AnchorXOffset = "0";
                            jointSigned.AnchorYOffset = "0";
                            jointSigned.AnchorUnits = "inches";
                            jointSigned.AnchorIgnoreIfNotPresent = "true";

                            jointSigner.Tabs.DateSignedTabs.Add(jointSigned);

                            Text textTab = new Text();
                            textTab.DocumentId = "1";
                            textTab.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            textTab.AnchorString = "joint1$1nput";
                            textTab.AnchorXOffset = "0";
                            textTab.AnchorYOffset = "-3";
                            textTab.AnchorUnits = "pixels";
                            textTab.AnchorIgnoreIfNotPresent = "true";
                            textTab.AnchorMatchWholeWord = "true";
                            textTab.Required = "false";
                            textTab.Width = "75";
                            textTab.Height = "15";
                            textTab.FontSize = "Size10";
                            textTab.Font = "TimesNewRoman";
                            jointSigner.Tabs.TextTabs.Add(textTab);

                            Date dateTab = new Date();
                            dateTab.DocumentId = "1";
                            dateTab.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            dateTab.AnchorString = "joint$D01B$";
                            dateTab.TabLabel = "date1";
                            dateTab.AnchorXOffset = "0";
                            dateTab.AnchorYOffset = "-3";
                            dateTab.AnchorUnits = "pixels";
                            dateTab.AnchorIgnoreIfNotPresent = "true";
                            dateTab.AnchorMatchWholeWord = "true";
                            dateTab.Required = "false";
                            dateTab.Width = "50";
                            dateTab.Height = "15";
                            dateTab.FontSize = "Size9";
                            dateTab.Font = "Arial";
                            jointSigner.Tabs.DateTabs.Add(dateTab);

                            for (int c = 1; c <= 5; c++)
                            {
                                Checkbox checkBox = new Checkbox();
                                checkBox.DocumentId = "1";
                                checkBox.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                                checkBox.AnchorString = "cu0$chk" + c;
                                checkBox.TabLabel = "MSRCheckBox" + c;
                                checkBox.AnchorXOffset = "0";
                                checkBox.AnchorYOffset = "0";
                                checkBox.AnchorUnits = "inches";
                                checkBox.AnchorIgnoreIfNotPresent = "true";

                                jointSigner.Tabs.CheckboxTabs.Add(checkBox);
                            }

                            envDef.Recipients.InPersonSigners.Add(jointSigner);
                        }
                        else
                        {

                            Signer jointSigner = new Signer();
                            jointSigner.Tabs = new Tabs();
                            jointSigner.Tabs.SignHereTabs = new List<SignHere>();
                            jointSigner.Tabs.DateSignedTabs = new List<DateSigned>();
                            jointSigner.Tabs.TextTabs = new List<Text>();
                            jointSigner.Tabs.DateTabs = new List<Date>();
                            jointSigner.Tabs.CheckboxTabs = new List<Checkbox>();

                            jointSigner.Name = docDetails.JointSignerDetails[i].ReciName;
                            jointSigner.Email = docDetails.JointSignerDetails[i].ReciEmail;
                            jointSigner.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            jointSigner.RoutingOrder = docDetails.JointSignerDetails[i].ReciId;


                            SignHere jointSignerTab = new SignHere();
                            jointSignerTab.DocumentId = "1";
                            jointSignerTab.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            jointSignerTab.AnchorString = "Joint0w$ign" + (i + 1);
                            jointSignerTab.AnchorXOffset = "0";
                            jointSignerTab.AnchorYOffset = "0";
                            jointSignerTab.AnchorUnits = "inches";
                            jointSignerTab.AnchorIgnoreIfNotPresent = "true";

                            jointSigner.Tabs.SignHereTabs.Add(jointSignerTab);

                            DateSigned jointSigned = new DateSigned();
                            jointSigned.DocumentId = "1";
                            jointSigned.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            jointSigned.AnchorString = "Joint0w$date" + (i + 1);
                            jointSigned.AnchorXOffset = "0";
                            jointSigned.AnchorYOffset = "0";
                            jointSigned.AnchorUnits = "inches";
                            jointSigned.AnchorIgnoreIfNotPresent = "true";

                            jointSigner.Tabs.DateSignedTabs.Add(jointSigned);

                            Text textTab = new Text();
                            textTab.DocumentId = "1";
                            textTab.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            textTab.AnchorString = "joint1$1nput";
                            textTab.AnchorXOffset = "0";
                            textTab.AnchorYOffset = "-3";
                            textTab.AnchorUnits = "pixels";
                            textTab.AnchorIgnoreIfNotPresent = "true";
                            textTab.AnchorMatchWholeWord = "true";
                            textTab.Required = "false";
                            textTab.Width = "75";
                            textTab.Height = "15";
                            textTab.FontSize = "Size10";
                            textTab.Font = "TimesNewRoman";
                            jointSigner.Tabs.TextTabs.Add(textTab);

                            Date dateTab = new Date();
                            dateTab.DocumentId = "1";
                            dateTab.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                            dateTab.AnchorString = "joint$D01B$";
                            dateTab.TabLabel = "date1";
                            dateTab.AnchorXOffset = "0";
                            dateTab.AnchorYOffset = "-3";
                            dateTab.AnchorUnits = "pixels";
                            dateTab.AnchorIgnoreIfNotPresent = "true";
                            dateTab.AnchorMatchWholeWord = "true";
                            dateTab.Required = "false";
                            dateTab.Width = "50";
                            dateTab.Height = "15";
                            dateTab.FontSize = "Size9";
                            dateTab.Font = "Arial";
                            jointSigner.Tabs.DateTabs.Add(dateTab);

                            for (int c = 1; c <= 5; c++)
                            {
                                Checkbox checkBox = new Checkbox();
                                checkBox.DocumentId = "1";
                                checkBox.RecipientId = docDetails.JointSignerDetails[i].ReciId;
                                checkBox.AnchorString = "cu0$chk" + c;
                                checkBox.TabLabel = "MSRCheckBox" + c;
                                checkBox.AnchorXOffset = "0";
                                checkBox.AnchorYOffset = "0";
                                checkBox.AnchorUnits = "inches";
                                checkBox.AnchorIgnoreIfNotPresent = "true";

                                jointSigner.Tabs.CheckboxTabs.Add(checkBox);
                            }

                            envDef.Recipients.Signers.Add(jointSigner);
                        }
                    }
                }


                if (docDetails.CUSignerDetails != null && docDetails.CUSignerDetails.Count > 0)
                {
                    for (int i = 0; i < docDetails.CUSignerDetails.Count; i++)
                    {
                        Signer cuSigner = new Signer();
                        cuSigner.Tabs = new Tabs();
                        cuSigner.Tabs.SignHereTabs = new List<SignHere>();
                        cuSigner.Tabs.DateSignedTabs = new List<DateSigned>();
                        cuSigner.Tabs.TextTabs = new List<Text>();
                        cuSigner.Tabs.DateTabs = new List<Date>();
                        cuSigner.Tabs.CheckboxTabs = new List<Checkbox>();

                        cuSigner.Name = docDetails.CUSignerDetails[i].ReciName;
                        cuSigner.Email = docDetails.CUSignerDetails[i].ReciEmail;
                        cuSigner.RecipientId = docDetails.CUSignerDetails[i].ReciId;
                        cuSigner.RoutingOrder = docDetails.CUSignerDetails[i].ReciId;


                        SignHere cuSignerTab = new SignHere();
                        cuSignerTab.DocumentId = "1";
                        cuSignerTab.RecipientId = docDetails.CUSignerDetails[i].ReciId;
                        cuSignerTab.AnchorString = "Cu0w$ign";
                        cuSignerTab.AnchorXOffset = "0";
                        cuSignerTab.AnchorYOffset = "0";
                        cuSignerTab.AnchorUnits = "inches";
                        cuSignerTab.AnchorIgnoreIfNotPresent = "true";

                        cuSigner.Tabs.SignHereTabs.Add(cuSignerTab);

                        DateSigned cuSigned = new DateSigned();
                        cuSigned.DocumentId = "1";
                        cuSigned.RecipientId = docDetails.CUSignerDetails[i].ReciId;
                        cuSigned.AnchorString = "Cu0w$date";
                        cuSigned.AnchorXOffset = "0";
                        cuSigned.AnchorYOffset = "0";
                        cuSigned.AnchorUnits = "inches";
                        cuSigned.AnchorIgnoreIfNotPresent = "true";

                        cuSigner.Tabs.DateSignedTabs.Add(cuSigned);
                        for (int j = 1; j <= 6; j++)
                        {
                            Text textTab = new Text();
                            textTab.DocumentId = "1";
                            textTab.RecipientId = docDetails.CUSignerDetails[i].ReciId;
                            textTab.AnchorString = "cu1$1nput" + j;
                            textTab.TabLabel = "cuOfficialSec" + j;
                            textTab.AnchorXOffset = "0";
                            textTab.AnchorYOffset = "-3";
                            textTab.AnchorUnits = "pixels";
                            textTab.AnchorIgnoreIfNotPresent = "true";
                            textTab.AnchorMatchWholeWord = "true";
                            textTab.Required = "false";
                            textTab.Width = "75";
                            textTab.Height = "15";
                            textTab.FontSize = "Size10";
                            textTab.Font = "TimesNewRoman";
                            cuSigner.Tabs.TextTabs.Add(textTab);
                        }

                        for (int k = 1; k <= 3; k++)
                        {
                            Date dateTab = new Date();
                            dateTab.DocumentId = "1";
                            dateTab.RecipientId = docDetails.CUSignerDetails[i].ReciId;
                            dateTab.AnchorString = "cu$D01B$" + k;
                            dateTab.TabLabel = "cuDate" + k;
                            dateTab.AnchorXOffset = "0";
                            dateTab.AnchorYOffset = "-3";
                            dateTab.AnchorUnits = "pixels";
                            dateTab.AnchorIgnoreIfNotPresent = "true";
                            dateTab.AnchorMatchWholeWord = "true";
                            dateTab.Required = "false";
                            dateTab.Width = "50";
                            dateTab.Height = "15";
                            dateTab.FontSize = "Size9";
                            dateTab.Font = "Arial";
                            cuSigner.Tabs.DateTabs.Add(dateTab);
                        }

                        Text textTab1 = new Text();
                        textTab1.DocumentId = "1";
                        textTab1.RecipientId = docDetails.CUSignerDetails[i].ReciId;
                        textTab1.AnchorString = "benComment$1nput";
                        textTab1.TabLabel = "BenComment";
                        textTab1.AnchorXOffset = "0";
                        textTab1.AnchorYOffset = "-3";
                        textTab1.AnchorUnits = "pixels";
                        textTab1.AnchorIgnoreIfNotPresent = "true";
                        textTab1.AnchorMatchWholeWord = "true";
                        textTab1.Required = "false";
                        textTab1.Width = "75";
                        textTab1.Height = "15";
                        textTab1.FontSize = "Size10";
                        textTab1.Font = "TimesNewRoman";
                        cuSigner.Tabs.TextTabs.Add(textTab1);

                        Text textTab2 = new Text();
                        textTab2.DocumentId = "1";
                        textTab2.RecipientId = docDetails.CUSignerDetails[i].ReciId;
                        textTab2.AnchorString = "RetailComment$1nput";
                        textTab2.TabLabel = "RetailComment";
                        textTab2.AnchorXOffset = "0";
                        textTab2.AnchorYOffset = "-3";
                        textTab2.AnchorUnits = "pixels";
                        textTab2.AnchorIgnoreIfNotPresent = "true";
                        textTab2.AnchorMatchWholeWord = "true";
                        textTab2.Required = "false";
                        textTab2.Width = "75";
                        textTab2.Height = "15";
                        textTab2.FontSize = "Size10";
                        textTab2.Font = "TimesNewRoman";

                        cuSigner.Tabs.TextTabs.Add(textTab2);

                        Text textTab3 = new Text();
                        textTab3.DocumentId = "1";
                        textTab3.RecipientId = docDetails.CUSignerDetails[i].ReciId;
                        textTab3.AnchorString = "Retailemployeeinfo$1nput";
                        textTab3.TabLabel = "Retailemployeeinfo";
                        textTab3.AnchorXOffset = "0";
                        textTab3.AnchorYOffset = "-3";
                        textTab3.AnchorUnits = "pixels";
                        textTab3.AnchorIgnoreIfNotPresent = "true";
                        textTab3.AnchorMatchWholeWord = "true";
                        textTab3.Required = "false";
                        textTab3.Width = "75";
                        textTab3.Height = "15";
                        textTab3.FontSize = "Size10";
                        textTab3.Font = "TimesNewRoman";

                        cuSigner.Tabs.TextTabs.Add(textTab3);

                        for (int c = 1; c <= 5; c++)
                        {
                            Checkbox checkBox = new Checkbox();
                            checkBox.DocumentId = "1";
                            checkBox.RecipientId = docDetails.CUSignerDetails[i].ReciId;
                            checkBox.AnchorString = "cu0$chk" + c;
                            checkBox.TabLabel = "MSRCheckBox" + c;
                            checkBox.AnchorXOffset = "0";
                            checkBox.AnchorYOffset = "0";
                            checkBox.AnchorUnits = "inches";
                            checkBox.AnchorIgnoreIfNotPresent = "true";

                            cuSigner.Tabs.CheckboxTabs.Add(checkBox);
                        }

                        envDef.Recipients.Signers.Add(cuSigner);
                    }

                }

                envDef.Status = "sent";
                envDef.EnforceSignerVisibility = "true";

                if (Utility.IsEventLogged) Utility.LogAction("Sending Create and send envelope request");

                EnvelopesApi envelopesApi = new EnvelopesApi();
                EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

                return envelopeSummary.EnvelopeId;
            }
            catch (Exception ex)
            {
                Utility.LogAction("sending document to DocuSign failed with exception: " + ex.Message);
                throw;
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

        

        public void SaveToLocation(byte[] docBytes, string docuName,string MemberNumber,string SSN,string FirstName,string LastName,string RegAccountNo,int Id,string accountNo="",int Dcount=0)
        {
            try
            {
                string CompletedDateTime = DateTime.Now.ToString(@"MM-dd-yyyy").Replace('-', '/');
                string IndexFileName = "Index-" + DateTime.Now.ToString(@"MM-dd-yyyy") + ".csv";

                string logFilePath = _downloadFileLocation;
                string[] doc = docuName.Split('.');
                docuName = doc[0];
                string documentType = GetDocType(docuName);

                if ( (accountNo== "") || (Dcount==0))
                {
                    docuName = docuName + Id + ".docx";
                }
                else
                {
                    docuName = docuName + Id + Dcount + ".docx"; 
                }

                if (_isImpersonate)
                {
                    using (new Impersonator(_impUser, _impDomain, _impPwd))
                    {
                        if (!Directory.Exists(logFilePath))
                        {
                            Directory.CreateDirectory(logFilePath);
                            DirectoryInfo directoryInfo = new DirectoryInfo(logFilePath);
                            DirectorySecurity accessControl = directoryInfo.GetAccessControl();
                            accessControl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                            directoryInfo.SetAccessControl(accessControl);
                        }
                        using (StreamWriter streamWriter = new StreamWriter(string.Concat(logFilePath, "\\", IndexFileName), true))
                        {
                            
                         streamWriter.WriteLine(string.Concat(new string[] {
                         $@"{documentType}|{MemberNumber}|{SSN}|{LastName}|{FirstName}|{logFilePath}\{docuName}|{CompletedDateTime}|{RegAccountNo}"
                }));
                            streamWriter.Close();
                        }
                     
                        System.IO.File.WriteAllBytes(logFilePath + @"\" + docuName, docBytes);
                    }
                }
                else
                {
                    if (!Directory.Exists(logFilePath))
                    {
                        Directory.CreateDirectory(logFilePath);
                        DirectoryInfo directoryInfo = new DirectoryInfo(logFilePath);
                        DirectorySecurity accessControl = directoryInfo.GetAccessControl();
                        accessControl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        directoryInfo.SetAccessControl(accessControl);
                    }
                    using (StreamWriter streamWriter = new StreamWriter(string.Concat(logFilePath, "\\", IndexFileName), true))
                    {
                        streamWriter.WriteLine(string.Concat(new string[] {
                    $@"{documentType}|{MemberNumber}|{SSN}|{LastName}|{FirstName}|{logFilePath}\{docuName}|{CompletedDateTime}|{RegAccountNo}"
                }));
                        streamWriter.Close();
                    }
                                    
                    System.IO.File.WriteAllBytes(logFilePath + @"\" + docuName, docBytes);
                }
            }
            catch (Exception ex)
            {
                Utility.LogAction("Save to location failed with error: " + ex.Message);
                Utility.LogAction("StackTrace: " + ex.StackTrace);               
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

        #region savetoloc
       
        private string GetDocType(string docName)
        {
            string docType = string.Empty;
            switch (docName)
            {
                case "MemberServiceRequest":
                    docType = "Member Services Request";
                    break;
                case "BeneficiaryDesignation":
                    docType = "Beneficiary Designation";
                    break;
                case "OverdraftServicesConsentForm":
                    docType = "Overdraft Services Consent";
                    break;
                case "RetailAccountChangeForm":
                    docType = "Account Change Form";
                    break;
                default:
                    if (docName.Contains("OverdraftServicesConsentForm"))
                    {
                        docType = "Overdraft Services Consent";
                    }
                    else
                    {
                        docType = docName;
                    }
                    break;
            }
            return docType;
        }
        #endregion


    }
    #endregion

}
