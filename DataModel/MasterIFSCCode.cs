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
    
    public partial class MasterIFSCCode
    {
        public long Id { get; set; }
        public string BankName { get; set; }
        public string IfscCode { get; set; }
        public bool IsDown { get; set; }
        public bool IsImpsAllow { get; set; }
        public bool IsNeftAllow { get; set; }
        public bool IsActive { get; set; }
        public string IIN { get; set; }
        public string Channels { get; set; }
        public string BankCode { get; set; }
        public string ImageUrl { get; set; }
    }
}
