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
    
    public partial class MerchantOnBoardingDocument
    {
        public int ID { get; set; }
        public int MerchantOnBoardingId { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string Number { get; set; }
        public string DocumentURL { get; set; }
        public string FileName { get; set; }
        public string MIMEType { get; set; }
        public System.DateTime AddDate { get; set; }
        public System.DateTime EditDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}
