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
    
    public partial class TerminalOnBoarding
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string AgentNo { get; set; }
        public int MerchantOnBoardingId { get; set; }
        public string MerchantID { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public string SIMNumber { get; set; }
        public string TerminalType { get; set; }
        public string DeviceModel { get; set; }
        public decimal MaxUsageDaily { get; set; }
        public decimal MaxUsageWeekly { get; set; }
        public decimal MaxUsage_montly { get; set; }
        public decimal VelocityCheckMinutes { get; set; }
        public decimal VelocityCheckCount { get; set; }
        public System.DateTime AddDate { get; set; }
        public System.DateTime EditDate { get; set; }
        public string Status { get; set; }
        public string APIStatus { get; set; }
        public bool IsActive { get; set; }
        public string APIRefId { get; set; }
        public string Reason { get; set; }
        public string TxnID { get; set; }
        public string LoginId { get; set; }
        public string EncPassword { get; set; }
        public string ModifyBy { get; set; }
    }
}