using DocuSign.Utils;
using FillTheDoc.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillTheDoc.Utils
{
    public class DBConnector
    {
        #region Properties
        private string _connectionString = ConfigurationManager.ConnectionStrings["connectSQL"].ConnectionString;

        private string _selectTemplateProcName = ConfigurationManager.AppSettings["SelectTemplateProcName"];

        private string _selectLoanDataProcName = ConfigurationManager.AppSettings["SelectLoanDataProcName"];
        private string _updateProcName = ConfigurationManager.AppSettings["UpdateLoanDataStatusProcName"];

        private string _selectDocuSignDataProcName = ConfigurationManager.AppSettings["SelectDocuSignDataProcName"];
        private string _selectDocuSignLoanGuarntyDataProcName = ConfigurationManager.AppSettings["SelectDocuSignLoanGuarantyDataProcName"];

        private string _updateDocuSignStatusProcName = ConfigurationManager.AppSettings["UpdateDocuSignStatusProcName"];
        private string _updateDocuSignLoanGuarantyStatusProcName = ConfigurationManager.AppSettings["UpdateDocuSignLoanGuarantyStatusProcName"];

        private string _selectDownloadDocProcName = ConfigurationManager.AppSettings["SelectDownloadDocProcName"];
        private string _selectDownloadLoanGuarntyDocProcName = ConfigurationManager.AppSettings["SelectDownloadLoanGuarantyDocProcName"];

        private string _insertDownloadStatusProcName = ConfigurationManager.AppSettings["UpdateDownloadDocStatusProcName"];
        private string _insertLoanGuarantyDownloadStatusProcName = ConfigurationManager.AppSettings["UpdateLoanGuarantyDownloadDocStatusProcName"];

        private SqlConnection _sqlCon = null;

        #endregion

        #region Methods




        public void UpdateStatus(string id, string status)
        {
            try
            {
                using (_sqlCon = new SqlConnection(_connectionString))
                {
                    _sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand(_updateProcName, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;

                    sql_cmnd.Parameters.AddWithValue("@Status", SqlDbType.NVarChar).Value = status;
                    sql_cmnd.Parameters.AddWithValue("@id", SqlDbType.NVarChar).Value = id;

                    sql_cmnd.ExecuteNonQuery();
                    _sqlCon.Close();

                    if (Utility.IsEventLogged) Utility.LogAction("Procedure executed successfully");
                }
            }
            catch (Exception ex)
            {
                Utility.LogAction("Updating status for id " + id + " failed with error " + ex.Message);
            }

        }

        public List<DocumentFields> ReadDocuSignData(int id)
        {
            try
            {
                List<DocumentFields> dataList = new List<DocumentFields>();

                using (_sqlCon = new SqlConnection(_connectionString))
                {
                    _sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand(_selectDocuSignDataProcName, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;

                    sql_cmnd.Parameters.AddWithValue("@PrimarySSNID", SqlDbType.VarChar).Value = id;

                    SqlDataReader reader = sql_cmnd.ExecuteReader();
                    DocumentFields data = null;

                    while (reader.Read())
                    {
                        data = new DocumentFields();

                        data.BenDesiBeneficiary = Convert.ToString(Value(reader, "BenDesiBeneficiary", "")); 
                        data.BenDesiNameRelationShip = Convert.ToString(Value(reader, "BenDesiNameRelationShip", ""));
                        data.BenDesiNameSSNNumber = Convert.ToString(Value(reader, "BenDesiNameSSNNumber", "")); 
                        data.BenDesiNameDOB = Convert.ToString(Value(reader, "BenDesiNameDOB", "")); 
                        data.BenDesiNameDate = Convert.ToString(Value(reader, "BenDesiNameDate", "")); 
                        data.BenDesiMemberNumber = Convert.ToString(Value(reader, "BenDesiMemberNumber", ""));




                        data.DirectDepositVeriFullName = Convert.ToString(Value(reader, "DirectDepositVeriFullName", ""));

                        data.DirectDepositCheckings = Convert.ToString(Value(reader, "DirectDepositCheckings", ""));
                        data.DirectDepositSavings = Convert.ToString(Value(reader, "DirectDepositSavings", "")); 
                        data.MemberServiceRequestMemberInfo = Convert.ToString(Value(reader, "MemberServiceRequestMemberInfo", "")); 
                        data.MemberServiceRequestFullName = Convert.ToString(Value(reader, "MemberServiceRequestFullName", "")); 
                        data.MemberServiceRequestMailingAddress = Convert.ToString(Value(reader, "MemberServiceRequestMailingAddress", "")); 
                        data.MemberServiceRequestIDType = Convert.ToString(Value(reader, "MemberServiceRequestIDType", ""));
                        data.MemberServiceCityStateZip1 = Convert.ToString(Value(reader, "MemberServiceCityStateZip1", ""));
                        data.MemberServiceIDNumber = Convert.ToString(Value(reader, "MemberServiceIDNumber", "")); 


                        data.MemberServiceRequestPhysicalAddress = Convert.ToString(Value(reader, "MemberServiceRequestPhysicalAddress", ""));
                        data.MemberServiceRequestIDIssueState = Convert.ToString(Value(reader, "MemberServiceRequestIDIssueState", "")); 
                        data.MemberServiceRequestIDIssueDate = Convert.ToString(Value(reader, "MemberServiceRequestIDIssueDate", ""));
                        data.MemberServiceCityStateZip2 = Convert.ToString(Value(reader, "MemberServiceCityStateZip2", ""));
                        data.MemberServiceIDExpDate =  Convert.ToString(Value(reader, "MemberServiceIDExpDate", "")); 
                        data.MemberServiceDOB = Convert.ToString(Value(reader, "MemberServiceDOB", "")); 
                        data.MemberServiceHomePhone = Convert.ToString(Value(reader, "MemberServiceHomePhone", "")); 

                        data.MemberServiceEmail = Convert.ToString(Value(reader, "MemberServiceEmail", "")); 
                        data.MemberServiceCell = Convert.ToString(Value(reader, "MemberServiceCell", "")); 
                        data.MemberServiceWorkPhone = Convert.ToString(Value(reader, "MemberServiceWorkPhone", ""));
                        data.MemberServiceFaxMachine = Convert.ToString(Value(reader, "MemberServiceFaxMachine", ""));
                        data.MemberServiceEmployer = Convert.ToString(Value(reader, "MemberServiceEmployer", "")); 
                        data.MemberServiceOccupationTitle = Convert.ToString(Value(reader, "MemberServiceOccupationTitle", "")); 
                        data.MemberServiceClubCheck = Convert.ToString(Value(reader, "MemberServiceClubCheck", ""));
                        data.MemberServiceCheckingAcc = Convert.ToString(Value(reader, "MemberServiceCheckingAcc", ""));


                        data.MemberServiceCheckingAccChk = Convert.ToString(Value(reader, "MemberServiceCheckingAccChk", "")); 
                        data.MemberServiceMoneyMarketChk = Convert.ToString(Value(reader, "MemberServiceMoneyMarketChk", "")); 
                        data.RetailAccountChangeMemberName = Convert.ToString(Value(reader, "RetailAccountChangeMemberName", "")); 
                        data.RetailAccountChangeMemberNumber = Convert.ToString(Value(reader, "RetailAccountChangeMemberNumber", "")); 
                        data.RetailAccountChangeSSNNumber = Convert.ToString(Value(reader, "RetailAccountChangeSSNNumber", ""));
                        data.RetailAccountChangeMailingAddress = Convert.ToString(Value(reader, "RetailAccountChangeMailingAddress", ""));
                        data.RetailAccountChangeCityStateZip = Convert.ToString(Value(reader, "RetailAccountChangeCityStateZip", "")); 

                        data.RetailAccountChangeIDNumber= Convert.ToString(Value(reader, "RetailAccountChangeIDNumber", "")); 
                        data.RetailAccountChangeIssuedDate =Convert.ToString(Value(reader, "RetailAccountChangeIssuedDate", "")); 
                        data.RetailAccountChangeExpDate =  Convert.ToString(Value(reader, "RetailAccountChangeExpDate", "")); 
                        data.RetailAccountChangePhone = Convert.ToString(Value(reader, "RetailAccountChangePhone", ""));
                        data.RetailAccountChangeWorkPhone = Convert.ToString(Value(reader, "RetailAccountChangeWorkPhone", "")); 
                        data.RetailAccountChangeCellPhone = Convert.ToString(Value(reader, "RetailAccountChangeCellPhone", ""));
                        data.RetailAccountChangeEmployer = Convert.ToString(Value(reader, "RetailAccountChangeEmployer", "")); 

                        data.RetailAccountChangeDOB =Convert.ToString(Value(reader, "RetailAccountChangeDOB", "")); 
                        data.RetailAccountChangeOccupation = Convert.ToString(Value(reader, "RetailAccountChangeOccupation", ""));
                        data.RetailAccountChangeEmail = Convert.ToString(Value(reader, "RetailAccountChangeEmail", "")); 

                        data.RetailAccountChangeClubCheck = Convert.ToString(Value(reader, "RetailAccountChangeClubCheck", "")); 
                        data.RetailAccountChangeCheckingAcc = Convert.ToString(Value(reader, "RetailAccountChangeCheckingAcc", "")); 
                        data.RetailAccountChangeAccChk = Convert.ToString(Value(reader, "RetailAccountChangeAccChk", ""));
                        data.RetailAccountChangeMoneyMarketChk = Convert.ToString(Value(reader, "RetailAccountChangeMoneyMarketChk", ""));

                        data.SavingProducts = Convert.ToString(Value(reader, "SavingProducts", "")); 
                        data.CheckingProducts = Convert.ToString(Value(reader, "CheckingProducts", ""));
                        data.CertificateProducts = Convert.ToString(Value(reader, "CertificateProducts", ""));
                        data.CertificateAccountNumbers = Convert.ToString(Value(reader, "CertificateAccountNumbers", ""));
                        data.COTSCount = Convert.ToString(Value(reader, "COTSCount", ""));
                        data.COTSAccountNumbers = Convert.ToString(Value(reader, "COTSAccountNumbers", ""));
                        data.OverDraftAccounts = Convert.ToString(Value(reader, "OverdraftAccounts", ""));

                        data.MemberCity = Convert.ToString(Value(reader, "MemberCity", ""));
                        data.MemberState = Convert.ToString(Value(reader, "MemberState", ""));
                        data.MemberZip = Convert.ToString(Value(reader, "MemberZip", ""));
                        data.CreditScore = Convert.ToString(Value(reader, "CreditScore", ""));
                        data.ScoreDetails = Convert.ToString(Value(reader,"ScoreDetails",""));

                        data.ExistingOrNewMember = Convert.ToString(Value(reader, "ExistingOrNewMember", ""));
                        data.IsInPersonSigner = Convert.ToString(Value(reader, "IsInPersonSigner", ""));


                        dataList.Add(data);

                    }
                    _sqlCon.Close();
                }
                if (Utility.IsEventLogged) Utility.LogAction("Procedure executed successfully");
                return dataList;
            }
            catch (Exception ex)
            {
                Utility.LogAction("Fetching DocuSign data for status " + id + " failed with error " + ex.Message);
                return new List<DocumentFields>();
            }
        }
        internal static T Value<T>(IDataRecord reader, string fldName, T defaultVal)
        {
            object o;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(fldName, StringComparison.OrdinalIgnoreCase))
                {
                    o = reader[i];
                    if (o != null && o != DBNull.Value)
                    {

                        return (T)Convert.ChangeType(o, typeof(T));
                    }
                    else
                        return defaultVal;

                }
            }
            return defaultVal;
        }
        public void UpdateDocuSignSubmit(int personId, string docID, string docName,string documentID, string status = "DocuSign sent")
        {
            try
            {
                using (_sqlCon = new SqlConnection(_connectionString))
                {
                    _sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand(_updateDocuSignStatusProcName, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;

                    sql_cmnd.Parameters.AddWithValue("@DocuSignID", SqlDbType.VarChar).Value = docID;
                    sql_cmnd.Parameters.AddWithValue("@DocuSignStatus", SqlDbType.VarChar).Value = status;
                    sql_cmnd.Parameters.AddWithValue("@PersonID", SqlDbType.Int).Value = personId;
                    sql_cmnd.Parameters.AddWithValue("@DocuName", SqlDbType.VarChar).Value = docName;
                    sql_cmnd.Parameters.AddWithValue("@DocumentID", SqlDbType.VarChar).Value = documentID;
                    sql_cmnd.ExecuteNonQuery();
                    _sqlCon.Close();

                    if (Utility.IsEventLogged) Utility.LogAction("Procedure executed successfully");
                }
            }
            catch (Exception ex)
            {
                Utility.LogAction("Updating status for id " + personId + " failed with error " + ex.Message);
            }
        }






        #endregion

    }
}