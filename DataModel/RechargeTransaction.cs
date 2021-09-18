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
    
    public partial class RechargeTransaction
    {
        public long ID { get; set; }
        public int OperatorID { get; set; }
        public int UserID { get; set; }
        public string UserDetails { get; set; }
        public string Number { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Amount { get; set; }
        public decimal CommPer { get; set; }
        public decimal CommVal { get; set; }
        public decimal CommAmt { get; set; }
        public decimal OtherCharge { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal ServiceChargeVal { get; set; }
        public decimal ServiceChargeAmt { get; set; }
        public decimal Cost { get; set; }
        public decimal ClosingBalance { get; set; }
        public string TransactionMode { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public System.DateTime EditDate { get; set; }
        public string Status { get; set; }
        public short ServiceID { get; set; }
        public string ServiceName { get; set; }
        public short APIID { get; set; }
        public string TransactionID { get; set; }
        public string Reason { get; set; }
        public string ApiLogs { get; set; }
        public bool IsProcessed { get; set; }
        public string EXTRA1 { get; set; }
        public string EXTRA2 { get; set; }
        public string EXTRA3 { get; set; }
        public string EXTRA4 { get; set; }
        public string EXTRA5 { get; set; }
        public string OperatorRef { get; set; }
        public string APIRef { get; set; }
        public bool RevertTran { get; set; }
        public string IpAddress { get; set; }
        public string EXTRA6 { get; set; }
        public string EXTRA7 { get; set; }
        public string EXTRA8 { get; set; }
        public string EXTRA9 { get; set; }
        public decimal CommAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public int CircleID { get; set; }
        public string LapuNumber { get; set; }
        public string EXTRA10 { get; set; }
        public string RetryAPiName { get; set; }
        public string ActionName { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<decimal> AFCommAmount { get; set; }
        public decimal AFChangeAmount { get; set; }
        public int AffiliateUserID { get; set; }
        public Nullable<decimal> GSTAmount { get; set; }
        public Nullable<decimal> TDSAmount { get; set; }
        public Nullable<bool> AepsCustomerConsent { get; set; }
        public string AepsCustomerConsentText { get; set; }
    }
}
