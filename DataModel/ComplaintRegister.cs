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
    
    public partial class ComplaintRegister
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public string AgentId { get; set; }
        public string MobileNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ComplaintType { get; set; }
        public string Disposition { get; set; }
        public string Description { get; set; }
        public long TransactionId { get; set; }
        public System.DateTime AddDate { get; set; }
        public System.DateTime EditDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string UpdatedBy { get; set; }
        public string Extra1 { get; set; }
    }
}
