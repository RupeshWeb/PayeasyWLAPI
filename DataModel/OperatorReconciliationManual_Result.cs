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
    
    public partial class OperatorReconciliationManual_Result
    {
        public short ServiceID { get; set; }
        public short APIID { get; set; }
        public string APIName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> RtCommAmt { get; set; }
        public Nullable<decimal> RtChargeAmt { get; set; }
        public decimal AdminCommAmt { get; set; }
        public Nullable<decimal> AdminChargeAmt { get; set; }
        public Nullable<decimal> ProfitLoss { get; set; }
    }
}
