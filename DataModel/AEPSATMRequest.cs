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
    
    public partial class AEPSATMRequest
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string MerchantID { get; set; }
        public decimal Amount { get; set; }
        public string TransactionID { get; set; }
        public string TransactionType { get; set; }
        public string ReturnUrl { get; set; }
        public long RefNumber { get; set; }
        public System.DateTime AddDate { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
    }
}
