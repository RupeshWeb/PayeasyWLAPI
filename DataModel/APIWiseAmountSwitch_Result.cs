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
    
    public partial class APIWiseAmountSwitch_Result
    {
        public int ID { get; set; }
        public short APIID { get; set; }
        public int OperatorID { get; set; }
        public string Amount { get; set; }
        public short NextAPIID { get; set; }
        public System.DateTime AddDate { get; set; }
        public System.DateTime EditDate { get; set; }
        public bool IsActive { get; set; }
    }
}
