//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duncan.PEMS.DataAccess.PEMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReportDetail1
    {
        public int Id { get; set; }
        public int RepId { get; set; }
        public string RepColumn { get; set; }
        public string ColData { get; set; }
    
        public virtual ReportMaster ReportMaster { get; set; }
    }
}