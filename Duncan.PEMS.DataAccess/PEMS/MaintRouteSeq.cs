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
    
    public partial class MaintRouteSeq
    {
        public int MaintRouteId { get; set; }
        public int MaintRouteSeq1 { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    
        public virtual MaintRoute MaintRoute { get; set; }
    }
}
