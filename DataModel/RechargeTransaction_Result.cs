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
    
    public partial class RechargeTransaction_Result
    {
        public string Provider { get; set; }
        public string ServiceType { get; set; }
        public string RefId { get; set; }
        public long OrderId { get; set; }
        public string TxnDate { get; set; }
        public string TxnTime { get; set; }
        public string UpdateOn { get; set; }
        public string UpdateOnTime { get; set; }
        public string CustomerNumber { get; set; }
        public string IfscCode { get; set; }
        public string AccountNo { get; set; }
        public string PayMode { get; set; }
        public decimal TxnAmount { get; set; }
        public decimal Charges { get; set; }
        public decimal Margin { get; set; }
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> TDS { get; set; }
        public decimal TotalAmount { get; set; }
        public string TxnStatus { get; set; }
        public string Reason { get; set; }
        public string UTRNo { get; set; }
        public short ServiceId { get; set; }
        public int OperatorId { get; set; }
    }
}
