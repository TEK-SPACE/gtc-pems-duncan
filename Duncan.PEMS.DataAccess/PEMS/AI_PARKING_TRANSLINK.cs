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
    
    public partial class AI_PARKING_TRANSLINK
    {
        public int ParkingTransLinkId { get; set; }
        public int CustomerID { get; set; }
        public Nullable<int> ParkingId { get; set; }
        public Nullable<int> ExportId { get; set; }
        public Nullable<System.DateTime> StatusDateTime { get; set; }
        public string StatusReason { get; set; }
        public string StatusValue { get; set; }
        public Nullable<int> OfficerId { get; set; }
    
        public virtual AI_EXPORT AI_EXPORT { get; set; }
        public virtual AI_PARKING AI_PARKING { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual AI_OFFICERS AI_OFFICERS { get; set; }
    }
}
