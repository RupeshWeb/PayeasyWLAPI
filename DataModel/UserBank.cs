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
    
    public partial class UserBank
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string BankName { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountNo { get; set; }
        public string IfscCode { get; set; }
        public string BranchName { get; set; }
        public string ImageUrl { get; set; }
        public System.DateTime AddDate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsBlock { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> ReplyDate { get; set; }
        public string Reply { get; set; }
    }
}
