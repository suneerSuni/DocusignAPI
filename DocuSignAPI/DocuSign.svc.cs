using DocuSign.Utils;
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
        public string FillDocument(int id,string CUName,string CUEmail )
        {
            string DocuSignID = string.Empty;
            try
            {
                string base64WordDoc;
                string filePath = string.Empty;
                string Type = "Type1";
                string folderPath = "Folder3";
                DocuSignDAL dalObj = new DocuSignDAL();
                DBConnector connector = new DBConnector();
                List<DocumentFields> dataList = connector.ReadDocuSignData(id);
                filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type1\\MSR New Membership.docx";

                if ((dataList.Count - 1) <= 3)
                {
                    folderPath = "Folder3";
                }
                else if ((dataList.Count - 1) <= 6)
                {
                    folderPath = "Folder6";
                }
                else if ((dataList.Count - 1) <= 9)
                {
                    folderPath = "Folder9";
                }
                if (dataList.Count > 0)
                {

                    if (!string.IsNullOrEmpty(Convert.ToString(dataList.ElementAt(0).BenDesiBeneficiary)) && dataList.ElementAt(0).MemberServiceCheckingAccChk == "True")
                    {
                        Type = "Type4";
                        filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type4\\MSR New Membership.docx";
                    }
                    else
                        if (!string.IsNullOrEmpty(Convert.ToString(dataList.ElementAt(0).BenDesiBeneficiary)))
                    {
                        Type = "Type2";
                        filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type2\\MSR New Membership.docx";
                    }
                    else
                        if (dataList.ElementAt(0).MemberServiceCheckingAccChk == "True")
                    {
                        Type = "Type3";
                        filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type3\\MSR New Membership.docx";
                    }
                }
                base64WordDoc = Convert.ToBase64String(File.ReadAllBytes(filePath));
                var obj = BuildDocuSignDocFields(dataList, Type, CUName, CUEmail);
                obj.FileBase64String = new WordReader().FillValuesToDoc(Convert.FromBase64String(base64WordDoc), "", obj);
                File.WriteAllBytes(@"E:\\test1.doc", Convert.FromBase64String(obj.FileBase64String));
                return obj.FileBase64String;
            }
            catch (Exception exception)
            {
                Utility.LogAction("Exception " + exception.Message);
                throw exception;
            }
        }

        public string SendforESign(int id, string CUName, string CUEmail)
        {
            string DocuSignID = string.Empty;
            try
            {
                string base64WordDoc;
                string filePath = string.Empty;
                string Type = "Type1";
                DocuSignDAL dalObj = new DocuSignDAL();
                DBConnector connector = new DBConnector();
                List<DocumentFields> dataList = connector.ReadDocuSignData(id);
                string folderPath = "Folder3";
                filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type1\\MSR New Membership.docx";
                
                if ((dataList.Count-1) <= 3)
                {
                    folderPath = "Folder3";
                }
                else if ((dataList.Count - 1) <= 6)
                {
                    folderPath = "Folder6";
                }
                else if ((dataList.Count - 1) <= 9)
                {
                    folderPath = "Folder9";
                }
                if (dataList.Count > 0)
                {
                    
                    if (!string.IsNullOrEmpty(Convert.ToString(dataList.ElementAt(0).BenDesiBeneficiary)) && dataList.ElementAt(0).MemberServiceCheckingAccChk=="True")
                    {
                        Type = "Type4";
                        filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type4\\MSR New Membership.docx";
                    }
                    else
                        if (!string.IsNullOrEmpty(Convert.ToString(dataList.ElementAt(0).BenDesiBeneficiary)))
                    {
                        Type = "Type2";
                        filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type2\\MSR New Membership.docx";
                    }
                    else
                        if (dataList.ElementAt(0).MemberServiceCheckingAccChk == "True")
                    {
                        Type = "Type3";
                        filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type3\\MSR New Membership.docx";
                    }
                }

                base64WordDoc = Convert.ToBase64String(File.ReadAllBytes(filePath));
                var obj = BuildDocuSignDocFields(dataList, Type, CUName, CUEmail);
                obj.FileBase64String = new WordReader().FillValuesToDoc(Convert.FromBase64String(base64WordDoc), "", obj);
                DocuSignID = dalObj.SendforESign(obj);
                connector.UpdateDocuSignSubmit(id, DocuSignID, ConfigurationManager.AppSettings["DocumentName"]);
                return DocuSignID;
            }
            catch (Exception exception)
            {
                Utility.LogAction("Exception " + exception.Message);
                throw exception;
            }

        }

        private SendDocumentInfo BuildDocuSignDocFields(List<DocumentFields> dataList, string type,string CUName, string CUEmail)
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
                values.Add(new CLDocValue { Key = "date", Value = DateTime.Now.ToString("MM/dd/yyyy") });
                values.Add(new CLDocValue { Key = "MemberFullName", Value = dataList.ElementAt(0).MemberServiceRequestFullName });
                values.Add(new CLDocValue { Key = "MemberNo", Value = dataList.ElementAt(0).MemberServiceRequestMemberInfo });
                values.Add(new CLDocValue { Key = "MemberName", Value = dataList.ElementAt(0).MemberServiceRequestFullName });
                values.Add(new CLDocValue { Key = "MailingAddress", Value = dataList.ElementAt(0).MemberServiceRequestMailingAddress });
                values.Add(new CLDocValue { Key = "IdType", Value = dataList.ElementAt(0).MemberServiceRequestIDType });
                values.Add(new CLDocValue { Key = "City", Value = dataList.ElementAt(0).MemberServiceCityStateZip1 });
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

                values.Add(new CLDocValue { Key = "NewMemberName1", Value = dataList.ElementAt(0).MemberServiceRequestFullName });
                values.Add(new CLDocValue { Key = "MemberNumber1", Value = dataList.ElementAt(0).RetailAccountChangeMemberNumber });
                values.Add(new CLDocValue { Key = "SSNTIN", Value = dataList.ElementAt(0).RetailAccountChangeSSNNumber });
                values.Add(new CLDocValue { Key = "SSN", Value = dataList.ElementAt(0).RetailAccountChangeSSNNumber });
                values.Add(new CLDocValue { Key = "MailingAddress1", Value = dataList.ElementAt(0).RetailAccountChangeMailingAddress });
                values.Add(new CLDocValue { Key = "CityStateZip1", Value = dataList.ElementAt(0).RetailAccountChangeCityStateZip });
                values.Add(new CLDocValue { Key = "DriversLicIssueDate1", Value = dataList.ElementAt(0).RetailAccountChangeIssuedDate });
                values.Add(new CLDocValue { Key = "HomePhone1", Value = dataList.ElementAt(0).RetailAccountChangePhone });
                values.Add(new CLDocValue { Key = "WorkPhone1", Value = dataList.ElementAt(0).RetailAccountChangeWorkPhone });
                values.Add(new CLDocValue { Key = "CellPhone1", Value = dataList.ElementAt(0).RetailAccountChangeCellPhone });
                values.Add(new CLDocValue { Key = "Employer1", Value = dataList.ElementAt(0).RetailAccountChangeEmployer });
                values.Add(new CLDocValue { Key = "DateofBirth1", Value = !string.IsNullOrEmpty(dataList.ElementAt(0).RetailAccountChangeDOB) ? dataList.ElementAt(0).RetailAccountChangeDOB.Split(' ').ElementAt(0) : "" });
                values.Add(new CLDocValue { Key = "Occupation1", Value = dataList.ElementAt(0).RetailAccountChangeOccupation });
                values.Add(new CLDocValue { Key = "Email1", Value = dataList.ElementAt(0).RetailAccountChangeEmail });


                values.Add(new CLDocValue { Key = "ClubSavChk", Value = dataList.ElementAt(0).MemberServiceClubCheck });
                values.Add(new CLDocValue { Key = "ClubSav2Chk", Value = dataList.ElementAt(0).MemberServiceClubCheck });

                values.Add(new CLDocValue { Key = "CheckingProd", Value = dataList.ElementAt(0).MemberServiceCheckingAcc });
                values.Add(new CLDocValue { Key = "CheckingProd2", Value = dataList.ElementAt(0).MemberServiceCheckingAcc });
                values.Add(new CLDocValue { Key = "CheckingProd3", Value = dataList.ElementAt(0).MemberServiceCheckingAcc });

                values.Add(new CLDocValue { Key = "Checking", Value = dataList.ElementAt(0).MemberServiceCheckingAccChk });
                values.Add(new CLDocValue { Key = "checking", Value = dataList.ElementAt(0).MemberServiceCheckingAccChk });

                values.Add(new CLDocValue { Key = "chkMoneyMarket", Value = dataList.ElementAt(0).MemberServiceMoneyMarketChk });
                values.Add(new CLDocValue { Key = "MoneyMarket", Value = dataList.ElementAt(0).MemberServiceMoneyMarketChk });

               
               
            }

            for (int loop = 1; loop < dataList.Count; loop++)
            {
                SetDocuSignFields(dataList[loop], values, jointSignerDetails, loop, Convert.ToString(++recepientLoop), type);
            }

           
            switch (type)
            {
                case "Type3":
                case "Type4":
                    sDocInfo.CUSignerDetails = new List<SignerInfo>()
                    {
                new SignerInfo() {
                    ReciName =CUName,
                    ReciEmail=CUEmail,
                    ReciId=Convert.ToString(++recepientLoop)
                        }
                    };
                    break;
                default:
                    break;
            }

            sDocInfo.DocuSignFields = values;
            sDocInfo.SignerDetails = signerDetails;
            sDocInfo.JointSignerDetails = jointSignerDetails;

            return sDocInfo;
        }

        private void SetDocuSignFields(DocumentFields dataList, List<CLDocValue> values, List<SignerInfo> jointSignerDetails, int loop, string reciID, string type)
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

        public string test(string data)
        {
            List<SignerInfo> signers = JsonConvert.DeserializeObject<List<SignerInfo>>(data);
            return data;
        }

        public string TestSendForSign()
        {
            return SendforESign(3,"CUSuneer","suneer.pa@claysys.net");
        }
    }
}

