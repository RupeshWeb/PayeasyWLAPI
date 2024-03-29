//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class MerchantOnBoarding
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string AgentNo { get; set; }
        public string ApplicationNumber { get; set; }
        public string FirmName { get; set; }
        public string FirmAddress { get; set; }
        public string FirmPinCode { get; set; }
        public string FirmState { get; set; }
        public string FirmCity { get; set; }
        public string FirmPanNo { get; set; }
        public string FirmGSTIN { get; set; }
        public string BusinessType { get; set; }
        public string EstablishedYear { get; set; }
        public string BusinessNature { get; set; }
        public string MerchantCategoryCodeId { get; set; }
        public string MerchantTypeId { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public string Nationality { get; set; }
        public bool IsOwnHouse { get; set; }
        public System.DateTime AddDate { get; set; }
        public System.DateTime EditDate { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public string APIStatus { get; set; }
        public string KYCStatus { get; set; }
        public string APIRefId { get; set; }
        public string Reason { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string TxnID { get; set; }
        public string MerchantId { get; set; }
        public string RRN { get; set; }
        public string ModifyBy { get; set; }
    }
}
