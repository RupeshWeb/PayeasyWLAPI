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
    
    public partial class ValidateParameter
    {
        public int ID { get; set; }
        public int OperatorID { get; set; }
        public string Name { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public string FieldType { get; set; }
        public bool IsMandatory { get; set; }
        public System.DateTime AddDate { get; set; }
        public System.DateTime EditDate { get; set; }
        public bool IsActive { get; set; }
        public short Sort { get; set; }
        public string Pattern { get; set; }
        public bool HasGrouping { get; set; }
    }
}
