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
    
    public partial class Remitter
    {
        public long ID { get; set; }
        public string RTID { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public int PinCode { get; set; }
        public decimal MonthlyLimit { get; set; }
        public decimal AvailableLimit { get; set; }
        public System.DateTime AddDate { get; set; }
        public bool Isactive { get; set; }
        public int RecipientCount { get; set; }
        public int RecipientLimit { get; set; }
        public System.DateTime UpdateOn { get; set; }
        public string ARemitterID { get; set; }
        public decimal AAvailableLimit { get; set; }
        public string MWareRemitterID { get; set; }
    }
}