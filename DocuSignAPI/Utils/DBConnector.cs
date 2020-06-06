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

                        data.BenDesiBeneficiary = Convert.ToString(reader["BenDesiBeneficiary"]);
                        data.BenDesiNameRelationShip = Convert.ToString(reader["BenDesiNameRelationShip"]);
                        data.BenDesiNameSSNNumber = Convert.ToString(reader["BenDesiNameSSNNumber"]);
                        data.BenDesiNameDOB = Convert.ToString(reader["BenDesiNameDOB"]);
                        data.BenDesiNameDate = Convert.ToString(reader["BenDesiNameDate"]);
                        data.BenDesiMemberNumber = Convert.ToString(reader["BenDesiMemberNumber"]);
                       
                        
                       
                        
                        data.DirectDepositVeriFullName = Convert.ToString(reader["DirectDepositVeriFullName"]);

                        data.DirectDepositCheckings = Convert.ToString(reader["DirectDepositCheckings"]);
                        data.DirectDepositSavings = Convert.ToString(reader["DirectDepositSavings"]);
                        data.MemberServiceRequestMemberInfo = Convert.ToString(reader["MemberServiceRequestMemberInfo"]);
                        data.MemberServiceRequestFullName = Convert.ToString(reader["MemberServiceRequestFullName"]);
                        data.MemberServiceRequestMailingAddress = Convert.ToString(reader["MemberServiceRequestMailingAddress"]);
                        data.MemberServiceRequestIDType = Convert.ToString(reader["MemberServiceRequestIDType"]);
                        data.MemberServiceCityStateZip1 = Convert.ToString(reader["MemberServiceCityStateZip1"]);
                        data.MemberServiceIDNumber = Convert.ToString(reader["MemberServiceIDNumber"]);


                        data.MemberServiceRequestPhysicalAddress = Convert.ToString(reader["MemberServiceRequestPhysicalAddress"]);
                        data.MemberServiceRequestIDIssueState = Convert.ToString(reader["MemberServiceRequestIDIssueState"]);
                        data.MemberServiceRequestIDIssueDate = Convert.ToString(reader["MemberServiceRequestIDIssueDate"]);
                        data.MemberServiceCityStateZip2 = Convert.ToString(reader["MemberServiceCityStateZip2"]);
                        data.MemberServiceIDExpDate = Convert.ToString(reader["MemberServiceIDExpDate"]);
                        data.MemberServiceDOB = Convert.ToString(reader["MemberServiceDOB"]);
                        data.MemberServiceHomePhone = Convert.ToString(reader["MemberServiceHomePhone"]);

                        data.MemberServiceEmail = Convert.ToString(reader["MemberServiceEmail"]);
                        data.MemberServiceCell = Convert.ToString(reader["MemberServiceCell"]);
                        data.MemberServiceWorkPhone = Convert.ToString(reader["MemberServiceWorkPhone"]);
                        data.MemberServiceEmployer = Convert.ToString(reader["MemberServiceEmployer"]);
                        data.MemberServiceOccupationTitle = Convert.ToString(reader["MemberServiceOccupationTitle"]);
                        data.MemberServiceClubCheck = Convert.ToString(reader["MemberServiceClubCheck"]);
                        data.MemberServiceCheckingAcc = Convert.ToString(reader["MemberServiceCheckingAcc"]);


                        data.MemberServiceCheckingAccChk = Convert.ToString(reader["MemberServiceCheckingAccChk"]);
                        data.MemberServiceMoneyMarketChk = Convert.ToString(reader["MemberServiceMoneyMarketChk"]);
                        data.RetailAccountChangeMemberName = Convert.ToString(reader["RetailAccountChangeMemberName"]);
                        data.RetailAccountChangeMemberNumber = Convert.ToString(reader["RetailAccountChangeMemberNumber"]);
                        data.RetailAccountChangeSSNNumber = Convert.ToString(reader["RetailAccountChangeSSNNumber"]);
                        data.RetailAccountChangeMailingAddress = Convert.ToString(reader["RetailAccountChangeMailingAddress"]);
                        data.RetailAccountChangeCityStateZip = Convert.ToString(reader["RetailAccountChangeCityStateZip"]);


                        data.RetailAccountChangeIssuedDate = Convert.ToString(reader["RetailAccountChangeIssuedDate"]);
                        data.RetailAccountChangePhone = Convert.ToString(reader["RetailAccountChangePhone"]);
                        data.RetailAccountChangeWorkPhone = Convert.ToString(reader["RetailAccountChangeWorkPhone"]);
                        data.RetailAccountChangeCellPhone = Convert.ToString(reader["RetailAccountChangeCellPhone"]);
                        data.RetailAccountChangeEmployer = Convert.ToString(reader["RetailAccountChangeEmployer"]);

                        data.RetailAccountChangeDOB = Convert.ToString(reader["RetailAccountChangeDOB"]);
                        data.RetailAccountChangeOccupation = Convert.ToString(reader["RetailAccountChangeOccupation"]);
                        data.RetailAccountChangeEmail = Convert.ToString(reader["RetailAccountChangeEmail"]);

                        data.RetailAccountChangeClubCheck = Convert.ToString(reader["RetailAccountChangeClubCheck"]);
                        data.RetailAccountChangeCheckingAcc = Convert.ToString(reader["RetailAccountChangeCheckingAcc"]);
                        data.RetailAccountChangeAccChk = Convert.ToString(reader["RetailAccountChangeAccChk"]);
                        data.RetailAccountChangeMoneyMarketChk = Convert.ToString(reader["RetailAccountChangeMoneyMarketChk"]);

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

        public void UpdateDocuSignSubmit(string id, string docID, string status)
        {
            try
            {
                using (_sqlCon = new SqlConnection(_connectionString))
                {
                    _sqlCon.Open();
                    SqlCommand sql_cmnd = new SqlCommand(_updateDocuSignStatusProcName, _sqlCon);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;

                    sql_cmnd.Parameters.AddWithValue("@DocID", SqlDbType.VarChar).Value = docID;
                    sql_cmnd.Parameters.AddWithValue("@Status", SqlDbType.VarChar).Value = status;
                    sql_cmnd.Parameters.AddWithValue("@Id", SqlDbType.VarChar).Value = id;
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

       

       


        #endregion

    }
}