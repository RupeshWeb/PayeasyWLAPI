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
    
    public partial class Recipient
    {
        public long ID { get; set; }
        public long RTID { get; set; }
        public string RPTID { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string AccountNo { get; set; }
        public string IFSC { get; set; }
        public string AccountType { get; set; }
        public string BankName { get; set; }
        public System.DateTime AddDate { get; set; }
        public string Status { get; set; }
        public bool IsVerified { get; set; }
        public string OtpVerify { get; set; }
        public Nullable<System.DateTime> VerifyDate { get; set; }
        public bool IsDeleted { get; set; }
        public string OtpDeleted { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public bool IsValidate { get; set; }
        public bool IsBlock { get; set; }
        public string AadharID { get; set; }
        public string MWareID { get; set; }
    }
}
