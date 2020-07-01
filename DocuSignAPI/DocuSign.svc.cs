using DocuSign.Utils;
using DocuSignAPI.Model;
using FillTheDoc.DAL;
using FillTheDoc.Model;
using FillTheDoc.Utils;
using Newtonsoft.Json;
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
        SVCResults results = new SVCResults();
        int addedDocDount = 2;
        Base64WordDocs b64Docs;
        List<FileDetail> ODCFiles;


        #region FillDocument Old
        //public string FillDocument(int id, string CUName, string CUEmail)
        //{
        //    string DocuSignID = string.Empty;
        //    try
        //    {
        //        string base64WordDoc;
        //        string filePath = string.Empty;
        //        string Type = "Type1";
        //        string folderPath = "Folder3";
        //        DocuSignDAL dalObj = new DocuSignDAL();
        //        DBConnector connector = new DBConnector();
        //        List<DocumentFields> dataList = connector.ReadDocuSignData(id);
        //        filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type1\\MSR New Membership.docx";

        //        if ((dataList.Count - 1) <= 3)
        //        {
        //            folderPath = "Folder3";
        //        }
        //        else if ((dataList.Count - 1) <= 6)
        //        {
        //            folderPath = "Folder6";
        //        }
        //        else if ((dataList.Count - 1) <= 9)
        //        {
        //            folderPath = "Folder9";
        //        }
        //        if (dataList.Count > 0)
        //        {

        //            if (!string.IsNullOrEmpty(Convert.ToString(dataList.ElementAt(0).BenDesiBeneficiary)) && dataList.ElementAt(0).MemberServiceCheckingAccChk == "True")
        //            {
        //                Type = "Type4";
        //                filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type4\\MSR New Membership.docx";
        //            }
        //            else
        //                if (!string.IsNullOrEmpty(Convert.ToString(dataList.ElementAt(0).BenDesiBeneficiary)))
        //            {
        //                Type = "Type2";
        //                filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type2\\MSR New Membership.docx";
        //            }
        //            else
        //                if (dataList.ElementAt(0).MemberServiceCheckingAccChk == "True")
        //            {
        //                Type = "Type3";
        //                filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type3\\MSR New Membership.docx";
        //            }
        //        }
        //        base64WordDoc = Convert.ToBase64String(File.ReadAllBytes(filePath));
        //        var obj = BuildDocuSignDocFields(dataList, Type, CUName, CUEmail);
        //        obj.FileBase64String = new WordReader().FillValuesToDoc(Convert.FromBase64String(base64WordDoc), "", obj);
        //        File.WriteAllBytes(@"E:\\test1.doc", Convert.FromBase64String(obj.FileBase64String));
        //        return obj.FileBase64String;
        //    }
        //    catch (Exception exception)
        //    {
        //        Utility.LogAction("Exception " + exception.Message);
        //        throw exception;
        //    }
        //}
        #endregion
        public SVCResults FillDocument(int id, string CUName, string CUEmail, string DocuName, int dCount, string accountNo)
        {
            string InvalidFileName = string.Empty;
            string docuSignID = string.Empty;
            try
            {
                DocuSignDAL dalObj = new DocuSignDAL();
                DBConnector connector = new DBConnector();
                List<DocumentFields> dataList = connector.ReadDocuSignData(id);
                Base64WordDocs b64Docs = new Base64WordDocs();
                b64Docs.fileDetails = new List<FileDetail>();
                FileDetail file;
                string folderPath = "Folder3";
                string RetFileString = string.Empty;
                SendDocumentInfo obj;
                if (DocuName == "RetailAccountChangeForm.docx")
                    obj = BuildDocuSignDocFieldsRetailAccountChange(dataList, CUName, CUEmail, accountNo);
                else if (DocuName == "OverdraftServicesConsentForm.docx")
                    obj = BuildDocuSignDocFieldsODC(dataList, accountNo);
                else
                    obj = BuildDocuSignDocFields(dataList, CUName, CUEmail, accountNo);
                switch (DocuName)
                {
                    case "MemberServiceRequest.docx":
                        if ((dataList.Count - 1) <= 3)
                        {
                            folderPath = "Folder3";
                            file = new FileDetail();
                            file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\MemberServiceRequest.docx";
                            file.name = "MemberServiceRequest.docx";
                            file.id = "1";
                            b64Docs.fileDetails.Add(file);
                        }
                        else if ((dataList.Count - 1) <= 6)
                        {
                            folderPath = "Folder6";
                            file = new FileDetail();
                            file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\MemberServiceRequest.docx";
                            file.name = "MemberServiceRequest.docx";
                            file.id = "1";
                            b64Docs.fileDetails.Add(file);
                        }
                        else if ((dataList.Count - 1) <= 9)
                        {
                            folderPath = "Folder9";
                            file = new FileDetail();
                            file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\MemberServiceRequest.docx";
                            file.name = "MemberServiceRequest.docx";
                            file.id = "1";
                            b64Docs.fileDetails.Add(file);
                        }

                        break;
                    case "BeneficiaryDesignation.docx":
                        file = new FileDetail();
                        file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\BeneficiaryDesignation.docx";
                        file.name = "BeneficiaryDesignation.docx";
                        file.id = "1";
                        b64Docs.fileDetails.Add(file);

                        break;
                    case "DirectDepositAccountVerification.docx":
                        file = new FileDetail();
                        file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\DirectDepositAccountVerification.docx";
                        file.name = "DirectDepositAccountVerification.docx";
                        file.id = "1";
                        b64Docs.fileDetails.Add(file);
                        break;
                    case "OverdraftServicesConsentForm.docx":
                        file = new FileDetail();
                        file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\OverdraftServicesConsentForm.docx";
                        file.name = "OverdraftServicesConsentForm.docx";
                        file.id = "1";
                        b64Docs.fileDetails.Add(file);
                        break;
                    case "RetailAccountChangeForm.docx":
                        if ((dataList.Count - 1) <= 3)
                        {
                            folderPath = "Folder3";
                            file = new FileDetail();
                            file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\RetailAccountChangeForm.docx";
                            file.name = "RetailAccountChangeForm.docx";
                            file.id = "1";
                            b64Docs.fileDetails.Add(file);
                        }
                        else if ((dataList.Count - 1) <= 6)
                        {
                            folderPath = "Folder6";
                            file = new FileDetail();
                            file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\RetailAccountChangeForm.docx";
                            file.name = "RetailAccountChangeForm.docx";
                            file.id = "1";
                            b64Docs.fileDetails.Add(file);
                        }
                        else if ((dataList.Count - 1) <= 9)
                        {
                            folderPath = "Folder9";
                            file = new FileDetail();
                            file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\RetailAccountChangeForm.docx";
                            file.name = "RetailAccountChangeForm.docx";
                            file.id = "1";
                            b64Docs.fileDetails.Add(file);
                        }

                        break;
                    case "AccountReceipt.docx":
                        if ((dataList.Count - 1) <= 4)
                        {
                            folderPath = "Folder3";
                            file = new FileDetail();
                            file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\AccountReceipt.docx";
                            file.name = "AccountReceipt.docx";
                            file.id = "1";
                            b64Docs.fileDetails.Add(file);
                        }
                        else if ((dataList.Count - 1) <= 7)
                        {
                            folderPath = "Folder6";
                            file = new FileDetail();
                            file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\AccountReceipt.docx";
                            file.name = "AccountReceipt.docx";
                            file.id = "1";
                            b64Docs.fileDetails.Add(file);
                        }
                        else if ((dataList.Count - 1) <= 10)
                        {
                            folderPath = "Folder9";
                            file = new FileDetail();
                            file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\AccountReceipt.docx";
                            file.name = "AccountReceipt.docx";
                            file.id = "1";
                            b64Docs.fileDetails.Add(file);
                        }
                        break;
                    case "AdverseAction.docx":
                        file = new FileDetail();
                        file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\AdverseAction.docx";
                        file.name = "AdverseAction.docx";
                        file.id = "1";
                        b64Docs.fileDetails.Add(file);
                        break;
                    case "AltraFoundationApplication.docx":
                        file = new FileDetail();
                        file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\AltraFoundationApplication.docx";
                        file.name = "AltraFoundationApplication.docx";
                        file.id = "1";
                        b64Docs.fileDetails.Add(file);
                        break;
                    case "IndexedMoneyMarketDisclosure.docx":
                        file = new FileDetail();
                        file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\IndexedMoneyMarketDisclosure.docx";
                        file.name = "IndexedMoneyMarketDisclosure.docx";
                        file.id = "1";
                        b64Docs.fileDetails.Add(file);
                        break;
                    default:
                        InvalidFileName = DocuName + " - not found!";
                        break;
                }
                var dFile = b64Docs.fileDetails.ElementAt(0);
                RetFileString = new WordReader().FillValuesToDoc(File.ReadAllBytes(dFile.path), "", obj);

                //File.WriteAllBytes(@"E:\\ZZZ\\" + dFile.name, Convert.FromBase64String(RetFileString));

                results.status = "Success";
                results.DocuSignID = "";
                results.ErrorMessage = "";
                results.InnerException = "";
                results.StackTrace = "";
                results.FilledDocument = RetFileString;
            }
            catch (Exception exception)
            {
                results.StackTrace = exception.StackTrace;
                results.status = "Error";
                results.DocuSignID = "";
                results.ErrorMessage = exception.Message;
                results.InnerException = Convert.ToString(exception.InnerException);
                if (!string.IsNullOrEmpty(InvalidFileName))
                    results.ErrorMessage += InvalidFileName;
                Utility.LogAction("SSNID: " + id + "; Exception: " + exception.Message);
            }
            return results;
        }
        public SVCResults SendforESign(int id, string CUName, string CUEmail)
        {
            string docuSignIDTRO = string.Empty;
            string docuSignIDTROJoint = string.Empty;
            try
            {
                DocuSignDAL dalObj = new DocuSignDAL();
                DBConnector connector = new DBConnector();
                List<DocumentFields> dataList = connector.ReadDocuSignData(id);
                b64Docs = new Base64WordDocs();
                b64Docs.fileDetails = new List<FileDetail>();
                FileDetail file;
                string folderPath = "Folder3";
                if (dataList.Count() > 0)
                {
                    
                    if ((dataList.Count - 1) <= 3)
                    {
                        folderPath = "Folder3";
                        file = new FileDetail();
                        file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\MemberServiceRequest.docx";
                        file.name = "MemberServiceRequest.docx";
                        file.id = "1";
                        b64Docs.fileDetails.Add(file);
                    }
                    else if ((dataList.Count - 1) <= 6)
                    {
                        folderPath = "Folder6";
                        file = new FileDetail();
                        file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\MemberServiceRequest.docx";
                        file.name = "MemberServiceRequest.docx";
                        file.id = "1";
                        b64Docs.fileDetails.Add(file);
                    }
                    else if ((dataList.Count - 1) <= 9)
                    {
                        folderPath = "Folder9";
                        file = new FileDetail();
                        file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\MemberServiceRequest.docx";
                        file.name = "MemberServiceRequest.docx";
                        file.id = "1";
                        b64Docs.fileDetails.Add(file);
                    }

                    //DirectDeposite changed to non signature

                    //file = new FileDetail();
                    //file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\DirectDepositAccountVerification.docx";
                    //file.name = "DirectDepositAccountVerification.docx";
                    //file.id = addedDocDount + "";
                    //b64Docs.fileDetails.Add(file);
                    //++addedDocDount;

                    if (!string.IsNullOrEmpty(Convert.ToString(dataList.ElementAt(0).BenDesiBeneficiary)))
                    {
                        file = new FileDetail();
                        file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\BeneficiaryDesignation.docx";
                        file.name = "BeneficiaryDesignation.docx";
                        file.id = addedDocDount + "";
                        b64Docs.fileDetails.Add(file);
                        ++addedDocDount;
                    }

                    var obj = BuildDocuSignDocFields(dataList, CUName, CUEmail);

                    foreach (var dFile in b64Docs.fileDetails)
                    {
                        dFile.base64WordDoc = new WordReader().FillValuesToDoc(File.ReadAllBytes(dFile.path), "", obj);
                    }

                    BuildDocuSignDocsODC(dataList, CUName, CUEmail);        //OverDraftConsentForms Repeat and Fill Logic
                    if (ODCFiles != null)
                        b64Docs.fileDetails.AddRange(ODCFiles);                 //Append ODC Form files with other Documents to send for DocuSign

                    if (dataList.ElementAt(0).ExistingOrNewMember == "New")
                    {
                        docuSignIDTROJoint = dalObj.SendforESign(obj, b64Docs.fileDetails.Where(f => f.id == "1").ToList());
                        connector.UpdateDocuSignSubmit(id, docuSignIDTROJoint, ConfigurationManager.AppSettings["DocumentName1"]);
                    }

                    if (b64Docs.fileDetails.Count() > 1)
                    {
                        var obj2 = obj;
                        obj2.JointSignerDetails = new List<SignerInfo>();

                        docuSignIDTRO = dalObj.SendforESign(obj2, b64Docs.fileDetails.Where(f => f.id != "1").ToList());
                        connector.UpdateDocuSignSubmit(id, docuSignIDTRO, ConfigurationManager.AppSettings["DocumentName"]);
                    }
                    

                    results.status = "Success";
                    results.DocuSignID = docuSignIDTROJoint + "$" + docuSignIDTRO;
                    results.ErrorMessage = "";
                    results.InnerException = "";
                    results.FilledDocument = "";
                    results.StackTrace = "";
                }
                else
                {
                    results.status = "Error";
                    results.DocuSignID = docuSignIDTROJoint + "$" + docuSignIDTRO;
                    results.ErrorMessage = "No data found.";
                    results.InnerException = "";
                    results.FilledDocument = "";
                    results.StackTrace = "";
                }
            }
            catch (Exception exception)
            {
                DBConnector connector = new DBConnector();
                connector.UpdateDocuSignSubmit(id, "", "", "Error");
                results.StackTrace = exception.StackTrace;
                results.status = "Error";
                results.DocuSignID = "";
                results.ErrorMessage = exception.Message;
                results.InnerException = Convert.ToString(exception.InnerException);
                results.FilledDocument = "";
                Utility.LogAction("SSNID: "+id+"; Exception: " + exception.Message);
            }
            return results;

        }

        private SendDocumentInfo BuildDocuSignDocFields(List<DocumentFields> dataList, string CUName, string CUEmail, string accountNo = "")
        {
            int recepientLoop = 0;

            SendDocumentInfo sDocInfo = new SendDocumentInfo();

            List<SignerInfo> signerDetails = new List<SignerInfo>();
            List<SignerInfo> jointSignerDetails = new List<SignerInfo>();

            List<CLDocValue> values = new List<CLDocValue>();

            if (dataList.Count() > 0)
            {
                values.Add(new CLDocValue { Key = "BENEFICIARYPOD", Value = dataList.ElementAt(0).BenDesiBeneficiary });
                //values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).BenDesiMemberNumber });
                //values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).BenDesiNameDate });
                //values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).BenDesiNameDOB });
                //values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).BenDesiNameRelationShip });
                //values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).BenDesiNameSSNNumber });
                //values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).DirectDepositVeriFullName });
                //values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).DirectDepositSavings });
                //values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).DirectDepositCheckings });

                #region signerDetails
                signerDetails.Add(new SignerInfo()
                {
                    ReciName = dataList.ElementAt(0).MemberServiceRequestFullName,
                    ReciEmail = dataList.ElementAt(0).MemberServiceEmail,
                    ReciId = Convert.ToString(++recepientLoop)
                });
                #endregion
                values.Add(new CLDocValue { Key = "DNAUser", Value = CUName });
                values.Add(new CLDocValue { Key = "date", Value = DateTime.Now.ToString("MM/dd/yyyy") });
                values.Add(new CLDocValue { Key = "Date", Value = DateTime.Now.ToString("MM/dd/yyyy") });
                values.Add(new CLDocValue { Key = "MemberFullName", Value = dataList.ElementAt(0).MemberServiceRequestFullName });
                values.Add(new CLDocValue { Key = "MemberNo", Value = dataList.ElementAt(0).MemberServiceRequestMemberInfo });
                values.Add(new CLDocValue { Key = "MemberName", Value = dataList.ElementAt(0).MemberServiceRequestFullName });
                values.Add(new CLDocValue { Key = "MailingAddress", Value = dataList.ElementAt(0).MemberServiceRequestMailingAddress });
                values.Add(new CLDocValue { Key = "IdType", Value = dataList.ElementAt(0).MemberServiceRequestIDType });
                values.Add(new CLDocValue { Key = "City", Value = dataList.ElementAt(0).MemberServiceCityStateZip1 });
                values.Add(new CLDocValue { Key = "MemberCity", Value = dataList.ElementAt(0).MemberCity });
                values.Add(new CLDocValue { Key = "MemberState", Value = dataList.ElementAt(0).MemberState });
                values.Add(new CLDocValue { Key = "MemberZip", Value = dataList.ElementAt(0).MemberZip });
                values.Add(new CLDocValue { Key = "IDNumber", Value = dataList.ElementAt(0).MemberServiceIDNumber });
                values.Add(new CLDocValue { Key = "PhysicalAddress", Value = dataList.ElementAt(0).MemberServiceRequestPhysicalAddress });
                values.Add(new CLDocValue { Key = "IdIssueStates", Value = dataList.ElementAt(0).MemberServiceRequestIDIssueState });
                values.Add(new CLDocValue { Key = "IssuingDate", Value = dataList.ElementAt(0).MemberServiceRequestIDIssueDate });
                values.Add(new CLDocValue { Key = "CityZip", Value = dataList.ElementAt(0).MemberServiceCityStateZip2 });
                values.Add(new CLDocValue { Key = "IDExpDate", Value = dataList.ElementAt(0).MemberServiceIDExpDate });
                values.Add(new CLDocValue { Key = "DOB", Value = !string.IsNullOrEmpty(dataList.ElementAt(0).MemberServiceDOB) ? dataList.ElementAt(0).MemberServiceDOB.Split(' ').ElementAt(0) : "" });
                values.Add(new CLDocValue { Key = "HomePhone", Value = dataList.ElementAt(0).MemberServiceHomePhone });
                values.Add(new CLDocValue { Key = "Email", Value = dataList.ElementAt(0).MemberServiceEmail });
                values.Add(new CLDocValue { Key = "CellPhone", Value = dataList.ElementAt(0).MemberServiceCell });
                values.Add(new CLDocValue { Key = "WorkPhone", Value = dataList.ElementAt(0).MemberServiceWorkPhone });
                values.Add(new CLDocValue { Key = "Employer", Value = dataList.ElementAt(0).MemberServiceEmployer });
                values.Add(new CLDocValue { Key = "OccupationTitle", Value = dataList.ElementAt(0).MemberServiceOccupationTitle });
                values.Add(new CLDocValue { Key = "SSNTIN", Value = dataList.ElementAt(0).RetailAccountChangeSSNNumber });
                values.Add(new CLDocValue { Key = "SSN", Value = dataList.ElementAt(0).RetailAccountChangeSSNNumber });
                values.Add(new CLDocValue { Key = "ScoreDetails", Value = dataList.ElementAt(0).ScoreDetails });


                #region Account Types
                int countShareSaving = 0;
                int countClubSaving = 0;
                int countChecking = 0;
                int countCertificate = 0;
                int countCertificate1 = 0;
                int countMoneyMarket = 0;
                int countHighRateSavings = 0;
                int countDepositeAccount1 = 0;
                int countDepositeAccount2 = 0;
                int countDepositeAccount3 = 0;
                string ShareSaving = string.Empty;
                string ClubSaving = string.Empty;
                string Checking = string.Empty;
                string Certificate = string.Empty;
                string Certificate1 = string.Empty;
                string MoneyMarket = string.Empty;
                string HighRateSaving = string.Empty;
                string DepositeAccount1 = string.Empty;
                string DepositeAccount2 = string.Empty;
                string DepositeAccount3 = string.Empty;

                List<string> SavingProducts = new List<string>();
                List<string> CheckingProducts = new List<string>();
                List<string> CertificateProducts = new List<string>();
                List<string> AllAccounts = new List<string>();
                if (!string.IsNullOrEmpty(dataList.ElementAt(0).SavingProducts))
                    SavingProducts = dataList.ElementAt(0).SavingProducts.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!string.IsNullOrEmpty(dataList.ElementAt(0).CheckingProducts))
                    CheckingProducts = dataList.ElementAt(0).CheckingProducts.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!string.IsNullOrEmpty(dataList.ElementAt(0).CertificateProducts))
                    CertificateProducts = dataList.ElementAt(0).CertificateProducts.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                AllAccounts.AddRange(SavingProducts);
                AllAccounts.AddRange(CheckingProducts);
                AllAccounts.AddRange(CertificateProducts);

                if (AllAccounts.Count() > 0)
                {
                    foreach (var account in AllAccounts)
                    {
                        if (account.IndexOf('-') > 0)
                        {
                            switch (account.Substring(0, account.IndexOf('-')).Trim())
                            {
                                case "Regular Savings":
                                case "Share Savings":
                                    if (countShareSaving < 3)
                                    {
                                        ShareSaving += account + "; ";
                                        countShareSaving++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                                case "Club Savings":
                                    if (countClubSaving < 3)
                                    {
                                        ClubSaving += account + "; ";
                                        countClubSaving++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                                case "A+ Checking":
                                case "A+ Platinum Checking":
                                case "Free Checking":
                                case "Platinum":
                                case "Checking":
                                    if (countChecking < 3)
                                    {
                                        Checking += account + "; ";
                                        countChecking++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                                case "6 month":
                                case "12 month":
                                case "13 month special":
                                case "13 month Rochester Special":
                                case "18 month":
                                case "24 month":
                                case "31 month special":
                                case "31 Month Rochester special":
                                case "36 month":
                                case "48 month":
                                case "60 month":
                                case "Graduation Certificate":
                                case "Grow Up Certificate":
                                case "First Time Homebuyers Certificate":
                                case "Certificate":
                                    if (countCertificate < 3)
                                    {
                                        Certificate += account + "; ";
                                        countCertificate++;
                                    }
                                    else if (countCertificate1 < 3)
                                    {
                                        Certificate1 += account + "; ";
                                        countCertificate1++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                                case "Best Life Money Market":
                                case "Indexed Money Market":
                                case "Money Market":
                                    if (countMoneyMarket < 3)
                                    {
                                        MoneyMarket += account + "; ";
                                        countMoneyMarket++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;

                                case "High Rate Savings":
                                    if (countHighRateSavings < 3)
                                    {
                                        HighRateSaving += account + "; ";
                                        countHighRateSavings++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                                default:
                                    if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                            }
                        }
                    }
                }
                values.Add(new CLDocValue { Key = "ShareSaving", Value = ShareSaving });
                values.Add(new CLDocValue { Key = "ClubSaving", Value = ClubSaving });
                values.Add(new CLDocValue { Key = "Checking", Value = Checking });
                values.Add(new CLDocValue { Key = "Certificate", Value = Certificate });
                values.Add(new CLDocValue { Key = "Certificate1", Value = Certificate1 });
                values.Add(new CLDocValue { Key = "MoneyMarket", Value = MoneyMarket });
                values.Add(new CLDocValue { Key = "HighRateSaving", Value = HighRateSaving });
                values.Add(new CLDocValue { Key = "DepositeAccount1", Value = DepositeAccount1 });
                values.Add(new CLDocValue { Key = "DepositeAccount2", Value = DepositeAccount2 });
                values.Add(new CLDocValue { Key = "DepositeAccount3", Value = DepositeAccount3 });
                values.Add(new CLDocValue { Key = "chkDepositeAccount3", Value = "true" });
                if (!string.IsNullOrEmpty(ShareSaving))
                    values.Add(new CLDocValue { Key = "ShareSavChk", Value = "true" });
                if (!string.IsNullOrEmpty(ClubSaving))
                    values.Add(new CLDocValue { Key = "ClubSavChk", Value = "true" });
                if (!string.IsNullOrEmpty(Checking))
                    values.Add(new CLDocValue { Key = "CheckingChk", Value = "true" });
                if (!string.IsNullOrEmpty(Certificate))
                    values.Add(new CLDocValue { Key = "chkcertificate", Value = "true" });
                if (!string.IsNullOrEmpty(Certificate1))
                    values.Add(new CLDocValue { Key = "chkcertificate2", Value = "true" });
                if (!string.IsNullOrEmpty(MoneyMarket))
                    values.Add(new CLDocValue { Key = "chkMoneyMarket", Value = "true" });
                if (!string.IsNullOrEmpty(HighRateSaving))
                    values.Add(new CLDocValue { Key = "ChkHighRateSaving", Value = "true" });
                if (!string.IsNullOrEmpty(DepositeAccount1))
                    values.Add(new CLDocValue { Key = "ChkDepositAccount1", Value = "true" });
                if (!string.IsNullOrEmpty(DepositeAccount2))
                    values.Add(new CLDocValue { Key = "ChkDepositAccount2", Value = "true" });
                if (!string.IsNullOrEmpty(DepositeAccount3))
                    values.Add(new CLDocValue { Key = "ChkDepositAccount3", Value = "true" });

                #endregion

                #region MemberServiceRequest, Services Section Filling with OD Account Label and Account Number
                if (!string.IsNullOrEmpty(dataList.ElementAt(0).OverDraftAccounts))
                {
                    List<string> ODAccounts = new List<string>();
                    ODAccounts = dataList.ElementAt(0).OverDraftAccounts.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (ODAccounts.Count() > 0)
                        values.Add(new CLDocValue { Key = $"OverDraft", Value = "true" });
                    for (int i = 1; i <= ODAccounts.Count(); i++)
                    {
                        if (i <= 4)
                        {
                            values.Add(new CLDocValue { Key = $"services{i}", Value = ODAccounts.ElementAt(i - 1) });
                        }
                    }
                }
                #endregion

                #region Direct Deposit & Account Verification Saving and Checking Filling
                values.Add(new CLDocValue { Key = "DDSavings", Value = dataList.ElementAt(0).SavingProducts });
                values.Add(new CLDocValue { Key = "DDChecking", Value = dataList.ElementAt(0).CheckingProducts });
                #endregion

                #region CreditScore
                if (!string.IsNullOrEmpty(dataList.ElementAt(0).CreditScore))
                {
                    values.Add(new CLDocValue { Key = "CreditScore", Value = dataList.ElementAt(0).CreditScore });
                    if (Convert.ToInt32(dataList.ElementAt(0).CreditScore) < 620)
                    {
                        values.Add(new CLDocValue { Key = "sectionAChk1", Value = "true" });
                        values.Add(new CLDocValue { Key = "sectionBChk1", Value = "true" });
                    }
                }
                #endregion

                #region Certificate
                //List<string> certAccNos = new List<string>();
                //certAccNos = dataList.ElementAt(0).CertificateAccountNumbers.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                //if (certAccNos.Count() > 0) //Need to Implement logic for downloading multiple documents
                //{
                values.Add(new CLDocValue { Key = "accountNo", Value = accountNo });
                //}
                #endregion

                #region AccountReceipt MemberName Filling
                for (int i = 0; i < dataList.Count(); i++)
                {
                    values.Add(new CLDocValue { Key = $"accountOwners{i + 1}", Value = dataList.ElementAt(i).MemberServiceRequestFullName });

                }
                #endregion
            }


            for (int loop = 1; loop < dataList.Count; loop++)
            {
                SetDocuSignFields(dataList[loop], values, jointSignerDetails, loop, Convert.ToString(++recepientLoop));
            }


            //switch (type)
            //{
            //    case "Type3":
            //    case "Type4":
            sDocInfo.CUSignerDetails = new List<SignerInfo>()
                    {
                new SignerInfo() {
                    ReciName =CUName,
                    ReciEmail=CUEmail,
                    ReciId=Convert.ToString(++recepientLoop)
                        }
                    };
            //        break;
            //    default:
            //        break;
            //}

            sDocInfo.DocuSignFields = values;
            sDocInfo.SignerDetails = signerDetails;
            sDocInfo.JointSignerDetails = jointSignerDetails;

            return sDocInfo;
        }
        private void SetDocuSignFields(DocumentFields dataList, List<CLDocValue> values, List<SignerInfo> jointSignerDetails, int loop, string reciID)
        {
            values.Add(new CLDocValue { Key = $"MemberName{loop}", Value = dataList.MemberServiceRequestFullName });
            values.Add(new CLDocValue { Key = $"SSN{loop}", Value = dataList.RetailAccountChangeSSNNumber });
            values.Add(new CLDocValue { Key = $"MailingAddress{loop}", Value = dataList.MemberServiceRequestMailingAddress });
            values.Add(new CLDocValue { Key = $"IdType{loop}", Value = dataList.MemberServiceRequestIDType });
            values.Add(new CLDocValue { Key = $"City{loop}", Value = dataList.MemberServiceCityStateZip1 });
            values.Add(new CLDocValue { Key = $"IDNumber{loop}", Value = dataList.MemberServiceIDNumber });
            values.Add(new CLDocValue { Key = $"PhysicalAddress{loop}", Value = dataList.MemberServiceRequestPhysicalAddress });
            values.Add(new CLDocValue { Key = $"IdIssueStates{loop}", Value = dataList.MemberServiceRequestIDIssueState });
            values.Add(new CLDocValue { Key = $"IssuingDate{loop}", Value = dataList.MemberServiceRequestIDIssueDate });
            values.Add(new CLDocValue { Key = $"CityZip{loop}", Value = dataList.MemberServiceCityStateZip2 });
            values.Add(new CLDocValue { Key = $"IDExpDate{loop}", Value = dataList.MemberServiceIDExpDate });
            values.Add(new CLDocValue { Key = $"NameRelationship{loop}", Value = dataList.BenDesiNameRelationShip });
            values.Add(new CLDocValue { Key = $"DOB{loop}", Value = !string.IsNullOrEmpty(dataList.MemberServiceDOB) ? dataList.MemberServiceDOB.Split(' ').ElementAt(0) : "" });
            values.Add(new CLDocValue { Key = $"HomePhone{loop}", Value = dataList.MemberServiceHomePhone });
            values.Add(new CLDocValue { Key = $"Email{loop}", Value = dataList.MemberServiceEmail });
            values.Add(new CLDocValue { Key = $"CellPhone{loop}", Value = dataList.MemberServiceCell });
            values.Add(new CLDocValue { Key = $"WorkPhone{loop}", Value = dataList.MemberServiceWorkPhone });
            values.Add(new CLDocValue { Key = $"Employer{loop}", Value = dataList.MemberServiceEmployer });
            values.Add(new CLDocValue { Key = $"OccupationTitlev{loop}", Value = dataList.MemberServiceOccupationTitle });

            values.Add(new CLDocValue { Key = $"ChangeMemberName{loop + 1}", Value = dataList.MemberServiceRequestFullName });
            values.Add(new CLDocValue { Key = $"ChangeSSNTIN{loop + 1}", Value = dataList.RetailAccountChangeSSNNumber });
            values.Add(new CLDocValue { Key = $"ChangeMailingAddress{loop + 1}", Value = dataList.RetailAccountChangeMailingAddress });
            values.Add(new CLDocValue { Key = $"ChangeCityStateZip{loop + 1}", Value = dataList.RetailAccountChangeCityStateZip });
            values.Add(new CLDocValue { Key = $"ChangeDriversLicIssueDate{loop + 1}", Value = dataList.RetailAccountChangeIssuedDate });
            values.Add(new CLDocValue { Key = $"ChangeHomePhone{loop + 1}", Value = dataList.RetailAccountChangePhone });
            values.Add(new CLDocValue { Key = $"ChangeWorkPhone{loop + 1}", Value = dataList.RetailAccountChangeWorkPhone });
            values.Add(new CLDocValue { Key = $"ChangeCellPhone{loop + 1}", Value = dataList.RetailAccountChangeCellPhone });
            values.Add(new CLDocValue { Key = $"ChangeEmployer{loop + 1}", Value = dataList.RetailAccountChangeEmployer });
            values.Add(new CLDocValue { Key = $"ChangeDateofBirth{loop + 1}", Value = !string.IsNullOrEmpty(dataList.RetailAccountChangeDOB) ? dataList.RetailAccountChangeDOB.Split(' ').ElementAt(0) : "" });
            values.Add(new CLDocValue { Key = $"ChangeOccupation{loop + 1}", Value = dataList.RetailAccountChangeOccupation });
            values.Add(new CLDocValue { Key = $"ChangeEmail{loop + 1}", Value = dataList.RetailAccountChangeEmail });

            jointSignerDetails.Add(new SignerInfo()
            {
                ReciName = dataList.MemberServiceRequestFullName,
                ReciEmail = dataList.MemberServiceEmail,
                ReciId = reciID
            });
        }

        #region ODC
        private SendDocumentInfo BuildDocuSignDocFieldsODC(List<DocumentFields> dataList, string accountNo = "")
        {
            SendDocumentInfo sDocInfo = new SendDocumentInfo();
            List<CLDocValue> values = new List<CLDocValue>();

            if (dataList.Count() > 0)
            {
                values.Add(new CLDocValue { Key = "MemberName", Value = dataList.ElementAt(0).MemberServiceRequestFullName });
                values.Add(new CLDocValue { Key = "ODAccountNumber", Value = accountNo });
            }

            sDocInfo.DocuSignFields = values;

            return sDocInfo;
        }
        private void BuildDocuSignDocsODC(List<DocumentFields> dataList, string CUName, string CUEmail, string accountNo = "")
        {
            ODCFiles = new List<FileDetail>();
            List<string> cotsAccounts = new List<string>();
            cotsAccounts = dataList.ElementAt(0).COTSAccountNumbers.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < cotsAccounts.Count(); i++)
            {
                FileDetail file = new FileDetail();
                file.path = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\OverdraftServicesConsentForm.docx";
                file.name = "OverdraftServicesConsentForm.docx";
                file.id = addedDocDount + "";

                int recepientLoop = 0;

                SendDocumentInfo sDocInfo = new SendDocumentInfo();

                List<SignerInfo> signerDetails = new List<SignerInfo>();
                List<SignerInfo> jointSignerDetails = new List<SignerInfo>();
                List<CLDocValue> values = new List<CLDocValue>();

                if (dataList.Count() > 0)
                {
                    #region signerDetails
                    signerDetails.Add(new SignerInfo()
                    {
                        ReciName = dataList.ElementAt(0).MemberServiceRequestFullName,
                        ReciEmail = dataList.ElementAt(0).MemberServiceEmail,
                        ReciId = Convert.ToString(++recepientLoop)
                    });
                    #endregion

                    values.Add(new CLDocValue { Key = "MemberName", Value = dataList.ElementAt(0).MemberServiceRequestFullName });
                    values.Add(new CLDocValue { Key = "ODAccountNumber", Value = cotsAccounts.ElementAt(i) });
                }

                sDocInfo.CUSignerDetails = new List<SignerInfo>()
                    {
                new SignerInfo() {
                    ReciName =CUName,
                    ReciEmail=CUEmail,
                    ReciId=Convert.ToString(++recepientLoop)
                        }
                    };

                sDocInfo.DocuSignFields = values;
                sDocInfo.SignerDetails = signerDetails;
                sDocInfo.JointSignerDetails = jointSignerDetails;

                file.base64WordDoc = new WordReader().FillValuesToDoc(File.ReadAllBytes(file.path), "", sDocInfo);
                ++addedDocDount;

                ODCFiles.Add(file);
            }
        }
        #endregion

        #region RetailAccount ChangeForm
        private SendDocumentInfo BuildDocuSignDocFieldsRetailAccountChange(List<DocumentFields> dataList, string CUName, string CUEmail, string accountNo = "")
        {
            int recepientLoop = 0;

            SendDocumentInfo sDocInfo = new SendDocumentInfo();

            List<SignerInfo> jointSignerDetails = new List<SignerInfo>();

            List<CLDocValue> values = new List<CLDocValue>();

            if (dataList.Count() > 0)
            {
                values.Add(new CLDocValue { Key = "NewMemberName1", Value = dataList.ElementAt(0).MemberServiceRequestFullName });
                values.Add(new CLDocValue { Key = "MemberNumber1", Value = dataList.ElementAt(0).RetailAccountChangeMemberNumber });
                values.Add(new CLDocValue { Key = "SSNTIN", Value = dataList.ElementAt(0).RetailAccountChangeSSNNumber });
                values.Add(new CLDocValue { Key = "MailingAddress1", Value = dataList.ElementAt(0).RetailAccountChangeMailingAddress });
                values.Add(new CLDocValue { Key = "CityStateZip1", Value = dataList.ElementAt(0).RetailAccountChangeCityStateZip });
                values.Add(new CLDocValue { Key = "DriversLicNo1", Value = dataList.ElementAt(0).RetailAccountChangeIDNumber });
                values.Add(new CLDocValue { Key = "DriversLicIssueDate1", Value = dataList.ElementAt(0).RetailAccountChangeIssuedDate });
                values.Add(new CLDocValue { Key = "DriversLicExpDate1", Value = dataList.ElementAt(0).RetailAccountChangeExpDate });
                values.Add(new CLDocValue { Key = "HomePhone1", Value = dataList.ElementAt(0).RetailAccountChangePhone });


                values.Add(new CLDocValue { Key = "WorkPhone1", Value = dataList.ElementAt(0).RetailAccountChangeWorkPhone });
                values.Add(new CLDocValue { Key = "CellPhone1", Value = dataList.ElementAt(0).RetailAccountChangeCellPhone });
                values.Add(new CLDocValue { Key = "Employer1", Value = dataList.ElementAt(0).RetailAccountChangeEmployer });
                values.Add(new CLDocValue { Key = "DateofBirth1", Value = dataList.ElementAt(0).RetailAccountChangeDOB });
                values.Add(new CLDocValue { Key = "Occupation1", Value = dataList.ElementAt(0).RetailAccountChangeOccupation });
                values.Add(new CLDocValue { Key = "Email1", Value = dataList.ElementAt(0).RetailAccountChangeEmail });

                #region Account Types
                int countShareSaving = 0;
                int countClubSaving = 0;
                int countChecking = 0;
                int countCertificate = 0;
                int countCertificate1 = 0;
                int countMoneyMarket = 0;
                int countHighRateSavings = 0;
                int countDepositeAccount1 = 0;
                int countDepositeAccount2 = 0;
                int countDepositeAccount3 = 0;
                string ShareSaving = string.Empty;
                string ClubSaving = string.Empty;
                string Checking = string.Empty;
                string Certificate = string.Empty;
                string Certificate1 = string.Empty;
                string MoneyMarket = string.Empty;
                string HighRateSaving = string.Empty;
                string DepositeAccount1 = string.Empty;
                string DepositeAccount2 = string.Empty;
                string DepositeAccount3 = string.Empty;

                List<string> SavingProducts = new List<string>();
                List<string> CheckingProducts = new List<string>();
                List<string> CertificateProducts = new List<string>();
                List<string> AllAccounts = new List<string>();
                if (!string.IsNullOrEmpty(dataList.ElementAt(0).SavingProducts))
                    SavingProducts = dataList.ElementAt(0).SavingProducts.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!string.IsNullOrEmpty(dataList.ElementAt(0).CheckingProducts))
                    CheckingProducts = dataList.ElementAt(0).CheckingProducts.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!string.IsNullOrEmpty(dataList.ElementAt(0).CertificateProducts))
                    CertificateProducts = dataList.ElementAt(0).CertificateProducts.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                AllAccounts.AddRange(SavingProducts);
                AllAccounts.AddRange(CheckingProducts);
                AllAccounts.AddRange(CertificateProducts);

                if (AllAccounts.Count() > 0)
                {
                    foreach (var account in AllAccounts)
                    {
                        if (account.IndexOf('-') > 0)
                        {
                            switch (account.Substring(0, account.IndexOf('-')).Trim())
                            {
                                case "Regular Savings":
                                case "Share Savings":
                                    if (countShareSaving < 3)
                                    {
                                        ShareSaving += account + "; ";
                                        countShareSaving++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                                case "Club Savings":
                                    if (countClubSaving < 3)
                                    {
                                        ClubSaving += account + "; ";
                                        countClubSaving++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                                case "A+ Checking":
                                case "A+ Platinum Checking":
                                case "Free Checking":
                                case "Platinum":
                                case "Checking":
                                    if (countChecking < 3)
                                    {
                                        Checking += account + "; ";
                                        countChecking++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                                case "6 month":
                                case "12 month":
                                case "13 month special":
                                case "13 month Rochester Special":
                                case "18 month":
                                case "24 month":
                                case "31 month special":
                                case "31 Month Rochester special":
                                case "36 month":
                                case "48 month":
                                case "60 month":
                                case "Graduation Certificate":
                                case "Grow Up Certificate":
                                case "First Time Homebuyers Certificate":
                                case "Certificate":
                                    if (countCertificate < 3)
                                    {
                                        Certificate += account + "; ";
                                        countCertificate++;
                                    }
                                    else if (countCertificate1 < 3)
                                    {
                                        Certificate1 += account + "; ";
                                        countCertificate1++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                                case "Best Life Money Market":
                                case "Indexed Money Market":
                                case "Money Market":
                                    if (countMoneyMarket < 3)
                                    {
                                        MoneyMarket += account + "; ";
                                        countMoneyMarket++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;

                                case "High Rate Savings":
                                    if (countHighRateSavings < 3)
                                    {
                                        HighRateSaving += account + "; ";
                                        countHighRateSavings++;
                                    }
                                    else if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                                default:
                                    if (countDepositeAccount1 < 3)
                                    {
                                        DepositeAccount1 += account + "; ";
                                        countDepositeAccount1++;
                                    }
                                    else if (countDepositeAccount2 < 3)
                                    {
                                        DepositeAccount2 += account + "; ";
                                        countDepositeAccount2++;
                                    }
                                    else if (countDepositeAccount3 < 3)
                                    {
                                        DepositeAccount3 += account + "; ";
                                        countDepositeAccount3++;
                                    }
                                    break;
                            }
                        }
                    }
                }
                values.Add(new CLDocValue { Key = "ShareSaving", Value = ShareSaving });
                values.Add(new CLDocValue { Key = "ClubSaving", Value = ClubSaving });
                values.Add(new CLDocValue { Key = "Checking", Value = Checking });
                values.Add(new CLDocValue { Key = "Certificate", Value = Certificate });
                values.Add(new CLDocValue { Key = "Certificate1", Value = Certificate1 });
                values.Add(new CLDocValue { Key = "MoneyMarket", Value = MoneyMarket });
                values.Add(new CLDocValue { Key = "HighRateSaving", Value = HighRateSaving });
                values.Add(new CLDocValue { Key = "DepositeAccount1", Value = DepositeAccount1 });
                values.Add(new CLDocValue { Key = "DepositeAccount2", Value = DepositeAccount2 });
                values.Add(new CLDocValue { Key = "DepositeAccount3", Value = DepositeAccount3 });
                values.Add(new CLDocValue { Key = "chkDepositeAccount3", Value = "true" });
                if (!string.IsNullOrEmpty(ShareSaving))
                    values.Add(new CLDocValue { Key = "ShareSavChk", Value = "true" });
                if (!string.IsNullOrEmpty(ClubSaving))
                    values.Add(new CLDocValue { Key = "ClubSavChk", Value = "true" });
                if (!string.IsNullOrEmpty(Checking))
                    values.Add(new CLDocValue { Key = "CheckingChk", Value = "true" });
                if (!string.IsNullOrEmpty(Certificate))
                    values.Add(new CLDocValue { Key = "chkcertificate", Value = "true" });
                if (!string.IsNullOrEmpty(Certificate1))
                    values.Add(new CLDocValue { Key = "chkcertificate2", Value = "true" });
                if (!string.IsNullOrEmpty(MoneyMarket))
                    values.Add(new CLDocValue { Key = "chkMoneyMarket", Value = "true" });
                if (!string.IsNullOrEmpty(HighRateSaving))
                    values.Add(new CLDocValue { Key = "ChkHighRateSaving", Value = "true" });
                if (!string.IsNullOrEmpty(DepositeAccount1))
                    values.Add(new CLDocValue { Key = "ChkDepositAccount1", Value = "true" });
                if (!string.IsNullOrEmpty(DepositeAccount2))
                    values.Add(new CLDocValue { Key = "ChkDepositAccount2", Value = "true" });
                if (!string.IsNullOrEmpty(DepositeAccount3))
                    values.Add(new CLDocValue { Key = "ChkDepositAccount3", Value = "true" });

                #endregion
            }

            for (int loop = 1; loop < dataList.Count(); loop++)
            {
                SetDocuSignFieldsRetailAccountChange(dataList[loop], values, (loop + 1));
            }

            sDocInfo.DocuSignFields = values;
            return sDocInfo;
        }
        private void SetDocuSignFieldsRetailAccountChange(DocumentFields dataList, List<CLDocValue> values, int loop)
        {
            values.Add(new CLDocValue { Key = $"Add{loop}", Value = "true" });
            values.Add(new CLDocValue { Key = $"ChangeMemberName{loop}", Value = dataList.MemberServiceRequestFullName });
            values.Add(new CLDocValue { Key = $"ChangeSSNTIN{loop}", Value = dataList.RetailAccountChangeSSNNumber });
            values.Add(new CLDocValue { Key = $"ChangeMailingAddress{loop}", Value = dataList.RetailAccountChangeMailingAddress });
            values.Add(new CLDocValue { Key = $"ChangeCityStateZip{loop}", Value = dataList.RetailAccountChangeCityStateZip });
            values.Add(new CLDocValue { Key = $"ChangeDriversLicNo{loop}", Value = dataList.RetailAccountChangeIDNumber });
            values.Add(new CLDocValue { Key = $"ChangeDriversLicIssueDate{loop}", Value = dataList.RetailAccountChangeIssuedDate });
            values.Add(new CLDocValue { Key = $"ChangeDriversLicExpDate{loop}", Value = dataList.RetailAccountChangeExpDate });
            values.Add(new CLDocValue { Key = $"ChangeHomePhone{loop}", Value = dataList.RetailAccountChangePhone });
            values.Add(new CLDocValue { Key = $"ChangeWorkPhone{loop}", Value = dataList.RetailAccountChangeWorkPhone });
            values.Add(new CLDocValue { Key = $"ChangeCellPhone{loop }", Value = dataList.RetailAccountChangeCellPhone });
            values.Add(new CLDocValue { Key = $"ChangeEmployer{loop}", Value = dataList.RetailAccountChangeEmployer });
            values.Add(new CLDocValue { Key = $"ChangeDateofBirth{loop}", Value = !string.IsNullOrEmpty(dataList.RetailAccountChangeDOB) ? dataList.RetailAccountChangeDOB.Split(' ').ElementAt(0) : "" });
            values.Add(new CLDocValue { Key = $"ChangeOccupation{loop}", Value = dataList.RetailAccountChangeOccupation });
            values.Add(new CLDocValue { Key = $"ChangeEmail{loop}", Value = dataList.RetailAccountChangeEmail });

        }
        #endregion

        public string test(string data)
        {
            List<SignerInfo> signers = JsonConvert.DeserializeObject<List<SignerInfo>>(data);
            return "";
        }

        public SVCResults TestSendForSign(int id)
        {
            return SendforESign(id, "CUSuneer", "midhun.claysys@gmail.com");
            //return SendforESign(3, "CUSuneer", "midhun.claysys@gmail.com");
        }
        public SVCResults TestFillDocument(string docName)
        {
            try
            {
                return FillDocument(3, "MIdhun K Jayan", "midhun.claysys@gmail.com", docName, 2, "123-456-7890");
            }
            catch (Exception exception)
            {
                results.status = "Error";
                results.DocuSignID = "";
                results.ErrorMessage = exception.Message;
                results.InnerException = Convert.ToString(exception.InnerException);
                results.FilledDocument = "";
                Utility.LogAction("Exception " + exception.Message);
                throw;
            }

        }
    }
}

