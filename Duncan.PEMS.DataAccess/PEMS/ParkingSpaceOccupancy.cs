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
    
    public partial class ParkingSpaceOccupancy
    {
        public ParkingSpaceOccupancy()
        {
            this.ParkingSpaceOccupancyAudits = new HashSet<ParkingSpaceOccupancyAudit>();
        }
    
        public long ParkingSpaceOccupancyId { get; set; }
        public long ParkingSpaceId { get; set; }
        public int LastStatus { get; set; }
        public System.DateTime LastUpdatedTS { get; set; }
        public Nullable<int> GCustomerid { get; set; }
        public Nullable<int> GAreaId { get; set; }
        public Nullable<int> GMeterId { get; set; }
        public byte[] DiagnosticData { get; set; }
    
        public virtual OccupancyStatu OccupancyStatu { get; set; }
        public virtual ParkingSpace ParkingSpace { get; set; }
        public virtual ICollection<ParkingSpaceOccupancyAudit> ParkingSpaceOccupancyAudits { get; set; }
    }
}
