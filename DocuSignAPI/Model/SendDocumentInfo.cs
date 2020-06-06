using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FillTheDoc.Model
{
    [DataContract]
    public class SendDocumentInfo
    {
        
        [DataMember]
        public string File_Sign { get; set; }
        [DataMember]
        public string FileType { get; set; }
        //[DataMember]
        public List<SignerInfo> SignerDetails { get; set; }
        //[DataMember]
        public List<SignerInfo> JointSignerDetails { get; set; }
        //[DataMember]
        public List<SignerInfo> CUSignerDetails { get; set; }

        public List<CLDocValue> DocuSignFields { get; set; }

        public string FileBase64String { get; set; }

    }
    [DataContract]
    public class SignerInfo
    {
        [DataMember]
        public string ReciName { get; set; }
        [DataMember]
        public string ReciEmail { get; set; }
        [DataMember]
        public string ReciId { get; set; }
        // public List<SignatureDetails> SignaturePosition { get; set; }
    }


    [DataContract]
    public class SignatureDetails
    {
        [DataMember]
        public string SignaturePos_X { get; set; }
        [DataMember]
        public string SignaturePos_Y { get; set; }
        [DataMember]
        public string SignaturePage { get; set; }
    }

                   
    public class DocumentFields
    {
        private string _benDesiBeneficiary;
        public string BenDesiBeneficiary
        {
            get { return _benDesiBeneficiary ?? string.Empty; }
            set { _benDesiBeneficiary = value; }

        }

        private string _benDesiNameRelationShip;
        public string BenDesiNameRelationShip
        {
            get { return _benDesiNameRelationShip ?? string.Empty; }
            set { _benDesiNameRelationShip = value; }

        }

        private string _benDesiNameSSNNumber;
        public string BenDesiNameSSNNumber
        {
            get { return _benDesiNameSSNNumber ?? string.Empty; }
            set { _benDesiNameSSNNumber = value; }

        }

        private string _benDesiNameDOB;
        public string BenDesiNameDOB
        {
            get { return _benDesiNameDOB ?? string.Empty; }
            set { _benDesiNameDOB = value; }

        }

        private string _benDesiNameDate;
        public string BenDesiNameDate
        {
            get { return _benDesiNameDate ?? string.Empty; }
            set { _benDesiNameDate = value; }

        }

        private string _directDepositVeriFullName;
        public string DirectDepositVeriFullName
        {
            get { return _directDepositVeriFullName ?? string.Empty; }
            set { _directDepositVeriFullName = value; }

        }
        public string ToPascalCase(string s)
        {

            string result = string.Empty;
            if (!string.IsNullOrEmpty(s))
            {
                result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
            }
            return result;
        }

        private string _benDesiMemberNumber;
        public string BenDesiMemberNumber
        {
            get { return _benDesiMemberNumber ?? string.Empty; }
            set { _benDesiMemberNumber = value; }

        }
     

        private string _directDepositSavings;
        public string DirectDepositSavings
        {
            get { return _directDepositSavings ?? string.Empty; }
            set { _directDepositSavings = value; }

        }

        private string _DirectDepositCheckings;
        public string DirectDepositCheckings
        {
            get { return _DirectDepositCheckings ?? string.Empty; }
            set { _DirectDepositCheckings = value; }

        }

        private string _memberServiceRequestMemberInfo;
        public string MemberServiceRequestMemberInfo
        {
            get { return _memberServiceRequestMemberInfo ?? string.Empty; }
            set { _memberServiceRequestMemberInfo = value; }

        }

        private string _memberServiceRequestFullName;
        public string MemberServiceRequestFullName
        {
            get { return _memberServiceRequestFullName ?? string.Empty; }
            set { _memberServiceRequestFullName = value; }

        }

        private string _memberServiceRequestMailingAddress;
        public string MemberServiceRequestMailingAddress
        {
            get { return _memberServiceRequestMailingAddress ?? string.Empty; }
            set { _memberServiceRequestMailingAddress = value; }

        }

        private string _memberServiceRequestIDType;
        public string MemberServiceRequestIDType
        {
            get { return _memberServiceRequestIDType ?? string.Empty; }
            set { _memberServiceRequestIDType = value; }

        }

        private string _memberServiceCityStateZip1;
        public string MemberServiceCityStateZip1
        {
            get { return _memberServiceCityStateZip1 ?? string.Empty; }
            set { _memberServiceCityStateZip1 = value; }

        }



        private string _memberServiceIDNumber;
        public string MemberServiceIDNumber
        {
            get { return _memberServiceIDNumber ?? string.Empty; }
            set { _memberServiceIDNumber = value; }

        }


        private string _memberServiceRequestPhysicalAddress;
        public string MemberServiceRequestPhysicalAddress
        {
            get { return _memberServiceRequestPhysicalAddress ?? string.Empty; }
            set { _memberServiceRequestPhysicalAddress = value; }

        }

        private string _memberServiceRequestIDIssueState;
        public string MemberServiceRequestIDIssueState
        {
            get { return _memberServiceRequestIDIssueState ?? string.Empty; }
            set { _memberServiceRequestIDIssueState = value; }

        }

        private string _memberServiceRequestIDIssueDate;
        public string MemberServiceRequestIDIssueDate
        {
            get { return _memberServiceRequestIDIssueDate ?? string.Empty; }
            set { _memberServiceRequestIDIssueDate = value; }

        }

        private string _memberServiceCityStateZip2;
        public string MemberServiceCityStateZip2
        {
            get { return _memberServiceCityStateZip2 ?? string.Empty; }
            set { _memberServiceCityStateZip2 = value; }

        }

        private string _MemberServiceIDExpDate;
        public string MemberServiceIDExpDate
        {
            get { return _MemberServiceIDExpDate ?? string.Empty; }
            set { _MemberServiceIDExpDate = value; }

        }

        private string _MemberServiceDOB;
        public string MemberServiceDOB
        {
            get { return _MemberServiceDOB ?? string.Empty; }
            set { _MemberServiceDOB = value; }

        }

        private string _MemberServiceHomePhone;
        public string MemberServiceHomePhone
        {
            get { return _MemberServiceHomePhone ?? string.Empty; }
            set { _MemberServiceHomePhone = value; }

        }

        private string _MemberServiceEmail;
        public string MemberServiceEmail
        {
            get { return _MemberServiceEmail ?? string.Empty; }
            set { _MemberServiceEmail = value; }

        }

        private string _MemberServiceCell;
        public string MemberServiceCell
        {
            get { return _MemberServiceCell ?? string.Empty; }
            set { _MemberServiceCell = value; }

        }
        private string _MemberServiceWorkPhone;
        public string MemberServiceWorkPhone
        {
            get { return _MemberServiceWorkPhone ?? string.Empty; }
            set { _MemberServiceWorkPhone = value; }

        }

        private string _MemberServiceEmployer;
        public string MemberServiceEmployer
        {
            get { return _MemberServiceEmployer ?? string.Empty; }
            set { _MemberServiceEmployer = value; }

        }

        private string _MemberServiceOccupationTitle;

        public string MemberServiceOccupationTitle
        {
            get { return _MemberServiceOccupationTitle ?? string.Empty; }
            set { _MemberServiceOccupationTitle = value; }

        }

        private string _MemberServiceClubCheck;

        public string MemberServiceClubCheck
        {
            get { return _MemberServiceClubCheck ?? string.Empty; }
            set { _MemberServiceClubCheck = value; }

        }

        private string _MemberServiceCheckingAcc;

        public string MemberServiceCheckingAcc
        {
            get { return _MemberServiceCheckingAcc ?? string.Empty; }
            set { _MemberServiceCheckingAcc = value; }

        }


        private string _MemberServiceCheckingAccChk;
        public string MemberServiceCheckingAccChk
        {
            get { return _MemberServiceCheckingAccChk ?? string.Empty; }
            set { _MemberServiceCheckingAccChk = value; }

        }

        private string _MemberServiceMoneyMarketChk;
        public string MemberServiceMoneyMarketChk
        {
            get { return _MemberServiceMoneyMarketChk ?? string.Empty; }
            set { _MemberServiceMoneyMarketChk = value; }

        }

        private string _RetailAccountChangeMemberName;
        public string RetailAccountChangeMemberName
        {
            get { return _RetailAccountChangeMemberName ?? string.Empty; }
            set { _RetailAccountChangeMemberName = value; }

        }

        private string _RetailAccountChangeMemberNumber;
        public string RetailAccountChangeMemberNumber
        {
            get { return _RetailAccountChangeMemberNumber ?? string.Empty; }
            set { _RetailAccountChangeMemberNumber = value; }

        }

        private string _RetailAccountChangeSSNNumber;
        public string RetailAccountChangeSSNNumber
        {
            get { return _RetailAccountChangeSSNNumber ?? string.Empty; }
            set { _RetailAccountChangeSSNNumber = value; }

        }

        private string _RetailAccountChangeMailingAddress;
        public string RetailAccountChangeMailingAddress
        {
            get { return _RetailAccountChangeMailingAddress ?? string.Empty; }
            set { _RetailAccountChangeMailingAddress = value; }

        }


        private string _RetailAccountChangeCityStateZip;
        public string RetailAccountChangeCityStateZip
        {
            get { return _RetailAccountChangeCityStateZip ?? string.Empty; }
            set { _RetailAccountChangeCityStateZip = value; }

        }


        private string _RetailAccountChangeIssuedDate;
        public string RetailAccountChangeIssuedDate
        {
            get { return _RetailAccountChangeIssuedDate ?? string.Empty; }
            set { _RetailAccountChangeIssuedDate = value; }

        }


        private string _RetailAccountChangePhone;
        public string RetailAccountChangePhone
        {
            get { return _RetailAccountChangePhone ?? string.Empty; }
            set { _RetailAccountChangePhone = value; }

        }


        private string _RetailAccountChangeWorkPhone;
        public string RetailAccountChangeWorkPhone
        {
            get { return _RetailAccountChangeWorkPhone ?? string.Empty; }
            set { _RetailAccountChangeWorkPhone = value; }

        }


        private string _RetailAccountChangeCellPhone;
        public string RetailAccountChangeCellPhone
        {
            get { return _RetailAccountChangeCellPhone ?? string.Empty; }
            set { _RetailAccountChangeCellPhone = value; }

        }

        private string _RetailAccountChangeEmployer;
        public string RetailAccountChangeEmployer
        {
            get { return _RetailAccountChangeEmployer ?? string.Empty; }
            set { _RetailAccountChangeEmployer = value; }

        }


        private string _RetailAccountChangeDOB;
        public string RetailAccountChangeDOB
        {
            get { return _RetailAccountChangeDOB ?? string.Empty; }
            set { _RetailAccountChangeDOB = value; }

        }


        private string _RetailAccountChangeOccupation;
        public string RetailAccountChangeOccupation
        {
            get { return _RetailAccountChangeOccupation ?? string.Empty; }
            set { _RetailAccountChangeOccupation = value; }

        }


        private string _RetailAccountChangeEmail;
        public string RetailAccountChangeEmail
        {
            get { return _RetailAccountChangeEmail ?? string.Empty; }
            set { _RetailAccountChangeEmail = value; }

        }


        private string _RetailAccountChangeClubCheck;
        public string RetailAccountChangeClubCheck
        {
            get { return _RetailAccountChangeClubCheck ?? string.Empty; }
            set { _RetailAccountChangeClubCheck = value; }

        }

        private string _RetailAccountChangeCheckingAcc;
        public string RetailAccountChangeCheckingAcc
        {
            get { return _RetailAccountChangeCheckingAcc ?? string.Empty; }
            set { _RetailAccountChangeCheckingAcc = value; }

        }

        private string _RetailAccountChangeAccChk;
        public string RetailAccountChangeAccChk
        {
            get { return _RetailAccountChangeAccChk ?? string.Empty; }
            set { _RetailAccountChangeAccChk = value; }

        }

        private string _RetailAccountChangeMoneyMarketChk;
        public string RetailAccountChangeMoneyMarketChk
        {
            get { return _RetailAccountChangeMoneyMarketChk ?? string.Empty; }
            set { _RetailAccountChangeMoneyMarketChk = value; }

        }
        


    }

    

}
