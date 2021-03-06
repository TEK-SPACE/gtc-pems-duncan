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
    
    public partial class CollRoute
    {
        public CollRoute()
        {
            this.CollRouteManualAmounts = new HashSet<CollRouteManualAmount>();
            this.CollRouteSeqs = new HashSet<CollRouteSeq>();
            this.MeterMaps = new HashSet<MeterMap>();
        }
    
        public int CollRouteId { get; set; }
        public int CustomerId { get; set; }
        public string DisplayName { get; set; }
        public string Comment { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual ICollection<CollRouteManualAmount> CollRouteManualAmounts { get; set; }
        public virtual ICollection<CollRouteSeq> CollRouteSeqs { get; set; }
        public virtual ICollection<MeterMap> MeterMaps { get; set; }
    }
}
