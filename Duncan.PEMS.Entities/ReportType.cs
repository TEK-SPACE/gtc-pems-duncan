//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duncan.PEMS.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReportType
    {
        public ReportType()
        {
            this.ReportQueries = new HashSet<ReportQuery>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<ReportQuery> ReportQueries { get; set; }
    }
}
