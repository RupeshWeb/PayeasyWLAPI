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
    
    public partial class DmtTransactionLimitAPI_Result
    {
        public long ID { get; set; }
        public int OperatorID { get; set; }
        public int UserID { get; set; }
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public short APIID { get; set; }
        public short ServiceID { get; set; }
        public string Status { get; set; }
    }
}