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

        public string SendforESign(SendDocumentInfo docDetails, string signerInfoJSon, string tableStrXML = "", List<CLDocValue> value = null, int jointCount = 0, string Type = "")
        {
            string DocuSignID = string.Empty;
            try
            {
                string base64WordDoc;
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["GetFileFromPath"]))
                {
                    string filePath = string.Empty;
                    string folderPath = "Folder3";
                    if (jointCount <= 3)
                    {
                        folderPath = "Folder3";
                    }
                    else if (jointCount <= 6)
                    {
                        folderPath = "Folder6";
                    }
                    else if (jointCount <= 9)
                    {
                        folderPath = "Folder9";
                    }
                    switch (Type)
                    {
                        case "Type1":

                            filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type1.Docx";
                            break;
                        case "Type2":
                            filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type2.Docx";
                            break;
                        case "Type3":
                            filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type3.Docx";
                            break;
                        case "Type4":
                            filePath = string.Concat(ConfigurationManager.AppSettings["FilePath"]) + "\\" + folderPath + "\\Type4.Docx";
                            break;
                        default:
                            break;
                    }
                    base64WordDoc = Convert.ToBase64String(File.ReadAllBytes(filePath));
                }
                else
                {
                    base64WordDoc = docDetails.File_Sign;
                }


                DocuSignDAL dalObj = new DocuSignDAL();
                DBConnector connector = new DBConnector();
               List<DocumentFields> dataList = connector.ReadDocuSignData(ConfigurationManager.AppSettings["SelectDocuSignDataStatus"]);
                docDetails.File_Sign = new WordReader().FillValuesToDoc(Convert.FromBase64String(base64WordDoc),"", BuildDocuSignDocFields(dataList));
                DocuSignID = dalObj.SendforESign(docDetails);
                return DocuSignID;
            }
            catch (Exception exception)
            {
                Utility.LogAction("Exception " + exception.Message);
                throw exception;
            }

        }
        private List<CLDocValue> BuildDocuSignDocFields(List<DocumentFields> dataList)
        {
            List<CLDocValue> values = new List<CLDocValue>();
            if (dataList.Count()>0)
            {
                //values.Add(new CLDocValue { Key = "SBALoanDate", Value = DateTime.Now.ToString("MM/dd/yyyy") });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).BenDesiBeneficiary });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).BenDesiMemberNumber });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).BenDesiNameDate });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).BenDesiNameDOB });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).BenDesiNameRelationShip });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).BenDesiNameSSNNumber });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).DirectDepositVeriFullName });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).DirectDepositSavings });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).DirectDepositCheckings });


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
                values.Add(new CLDocValue { Key = "DOB", Value = dataList.ElementAt(0).MemberServiceDOB });
                values.Add(new CLDocValue { Key = "HomePhone", Value = dataList.ElementAt(0).MemberServiceHomePhone });
                values.Add(new CLDocValue { Key = "Email", Value = dataList.ElementAt(0).MemberServiceEmail });
                values.Add(new CLDocValue { Key = "CellPhone", Value = dataList.ElementAt(0).MemberServiceCell });
                values.Add(new CLDocValue { Key = "WorkPhone", Value = dataList.ElementAt(0).MemberServiceWorkPhone });
                values.Add(new CLDocValue { Key = "Employer", Value = dataList.ElementAt(0).MemberServiceEmployer });
                values.Add(new CLDocValue { Key = "OccupationTitle", Value = dataList.ElementAt(0).MemberServiceOccupationTitle });





                values.Add(new CLDocValue { Key = "NewMemberName1", Value = dataList.ElementAt(0).RetailAccountChangeMemberName });
                values.Add(new CLDocValue { Key = "MemberNumber1", Value = dataList.ElementAt(0).RetailAccountChangeMemberNumber });
                values.Add(new CLDocValue { Key = "SSNTIN", Value = dataList.ElementAt(0).RetailAccountChangeSSNNumber });
                values.Add(new CLDocValue { Key = "MailingAddress1", Value = dataList.ElementAt(0).RetailAccountChangeMailingAddress });
                values.Add(new CLDocValue { Key = "CityStateZip1", Value = dataList.ElementAt(0).RetailAccountChangeCityStateZip });
                values.Add(new CLDocValue { Key = "DriversLicIssueDate1", Value = dataList.ElementAt(0).RetailAccountChangeIssuedDate });
                values.Add(new CLDocValue { Key = "HomePhone1", Value = dataList.ElementAt(0).RetailAccountChangePhone });
                values.Add(new CLDocValue { Key = "WorkPhone1", Value = dataList.ElementAt(0).RetailAccountChangeWorkPhone });
                values.Add(new CLDocValue { Key = "CellPhone1", Value = dataList.ElementAt(0).RetailAccountChangeCellPhone });
                values.Add(new CLDocValue { Key = "Employer1", Value = dataList.ElementAt(0).RetailAccountChangeEmployer });
                values.Add(new CLDocValue { Key = "DateofBirth1", Value = dataList.ElementAt(0).RetailAccountChangeDOB });
                values.Add(new CLDocValue { Key = "Occupation1", Value = dataList.ElementAt(0).RetailAccountChangeOccupation });
                values.Add(new CLDocValue { Key = "Email1", Value = dataList.ElementAt(0).RetailAccountChangeEmail });

                values.Add(new CLDocValue { Key = "ClubSav", Value = dataList.ElementAt(0).RetailAccountChangeClubCheck });
                values.Add(new CLDocValue { Key = "ClubSav2", Value = dataList.ElementAt(0).RetailAccountChangeClubCheck });
                values.Add(new CLDocValue { Key = "ClubSaving", Value = dataList.ElementAt(0).MemberServiceClubCheck });
                values.Add(new CLDocValue { Key = "club", Value = dataList.ElementAt(0).MemberServiceClubCheck });
                values.Add(new CLDocValue { Key = "CheckingProd", Value = dataList.ElementAt(0).RetailAccountChangeCheckingAcc });
                values.Add(new CLDocValue { Key = "CheckingProd2", Value = dataList.ElementAt(0).RetailAccountChangeCheckingAcc });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).MemberServiceCheckingAcc });

                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).MemberServiceCheckingAccChk });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).MemberServiceMoneyMarketChk });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).RetailAccountChangeAccChk });
                values.Add(new CLDocValue { Key = "", Value = dataList.ElementAt(0).RetailAccountChangeMoneyMarketChk });
            }
            if (dataList.Count()==1)
            {

               

                
                

            }
            else if (dataList.Count() == 2)
            {
                values.Add(new CLDocValue { Key = "MemberName1", Value = dataList.ElementAt(0).MemberServiceRequestFullName });
                values.Add(new CLDocValue { Key = "SSN1", Value = dataList.ElementAt(0).RetailAccountChangeSSNNumber });
                values.Add(new CLDocValue { Key = "MailingAddress1", Value = dataList.ElementAt(0).MemberServiceRequestMailingAddress });
                values.Add(new CLDocValue { Key = "IdType1", Value = dataList.ElementAt(0).MemberServiceRequestIDType });
                values.Add(new CLDocValue { Key = "City1", Value = dataList.ElementAt(0).MemberServiceCityStateZip1 });
                values.Add(new CLDocValue { Key = "IDNumber1", Value = dataList.ElementAt(0).MemberServiceIDNumber });
                values.Add(new CLDocValue { Key = "PhysicalAddress1", Value = dataList.ElementAt(0).MemberServiceRequestPhysicalAddress });
                values.Add(new CLDocValue { Key = "IdIssueStates1", Value = dataList.ElementAt(0).MemberServiceRequestIDIssueState });
                values.Add(new CLDocValue { Key = "IssuingDate1", Value = dataList.ElementAt(0).MemberServiceRequestIDIssueDate });
                values.Add(new CLDocValue { Key = "CityZip1", Value = dataList.ElementAt(0).MemberServiceCityStateZip2 });
                values.Add(new CLDocValue { Key = "IDExpDate1", Value = dataList.ElementAt(0).MemberServiceIDExpDate });
                values.Add(new CLDocValue { Key = "DOB1", Value = dataList.ElementAt(0).MemberServiceDOB });
                values.Add(new CLDocValue { Key = "HomePhone1", Value = dataList.ElementAt(0).MemberServiceHomePhone });
                values.Add(new CLDocValue { Key = "Email1", Value = dataList.ElementAt(0).MemberServiceEmail });
                values.Add(new CLDocValue { Key = "CellPhone1", Value = dataList.ElementAt(0).MemberServiceCell });
                values.Add(new CLDocValue { Key = "WorkPhone1", Value = dataList.ElementAt(0).MemberServiceWorkPhone });
                values.Add(new CLDocValue { Key = "Employer1", Value = dataList.ElementAt(0).MemberServiceEmployer });
                values.Add(new CLDocValue { Key = "OccupationTitle1", Value = dataList.ElementAt(0).MemberServiceOccupationTitle });

                values.Add(new CLDocValue { Key = "ChangeMemberName2", Value = dataList.ElementAt(0).RetailAccountChangeMemberName });   
                values.Add(new CLDocValue { Key = "ChangeSSNTIN2", Value = dataList.ElementAt(0).RetailAccountChangeSSNNumber });
                values.Add(new CLDocValue { Key = "ChangeMailingAddress2", Value = dataList.ElementAt(0).RetailAccountChangeMailingAddress });
                values.Add(new CLDocValue { Key = "ChangeCityStateZip2", Value = dataList.ElementAt(0).RetailAccountChangeCityStateZip });
                values.Add(new CLDocValue { Key = "ChangeDriversLicIssueDate2", Value = dataList.ElementAt(0).RetailAccountChangeIssuedDate });
                values.Add(new CLDocValue { Key = "ChangeHomePhone2", Value = dataList.ElementAt(0).RetailAccountChangePhone });
                values.Add(new CLDocValue { Key = "ChangeWorkPhone2", Value = dataList.ElementAt(0).RetailAccountChangeWorkPhone });
                values.Add(new CLDocValue { Key = "ChangeCellPhone2", Value = dataList.ElementAt(0).RetailAccountChangeCellPhone });
                values.Add(new CLDocValue { Key = "ChangeEmployer2", Value = dataList.ElementAt(0).RetailAccountChangeEmployer });
                values.Add(new CLDocValue { Key = "ChangeDateofBirth2", Value = dataList.ElementAt(0).RetailAccountChangeDOB });
                values.Add(new CLDocValue { Key = "ChangeOccupation2", Value = dataList.ElementAt(0).RetailAccountChangeOccupation });
                values.Add(new CLDocValue { Key = "ChangeEmail2", Value = dataList.ElementAt(0).RetailAccountChangeEmail });

            }
            else if (dataList.Count() == 3)
            {


            }
            else if (dataList.Count() == 4)
            {

            }
            else if (dataList.Count() == 5)
            {

            }
            else if (dataList.Count() == 6)
            {

            }
            else if (dataList.Count() == 7)
            {

            }
            else if (dataList.Count() == 8)
            {

            }
            else if (dataList.Count() == 9)
            {

            }
            else if (dataList.Count() == 10)
            {

            }
            return values;
        }
        public string test(string data)
        {
          
            List<SignerInfo> signers = JsonConvert.DeserializeObject<List<SignerInfo>>(data);

            return data;
        }
    }

}

