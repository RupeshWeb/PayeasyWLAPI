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
    
    public partial class LowBalanceNotify
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public decimal Amount { get; set; }
        public int Interval { get; set; }
        public System.DateTime AddDate { get; set; }
        public System.DateTime EditDate { get; set; }
        public bool IsActive { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
    }
}
