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
    
    public partial class ActivityLog
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Number { get; set; }
        public string RequestType { get; set; }
        public System.DateTime AddDate { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string IpAddress { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
    }
}
