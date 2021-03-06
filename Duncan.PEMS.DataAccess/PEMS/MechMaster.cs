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
    
    public partial class MechMaster
    {
        public MechMaster()
        {
            this.MeterMaps = new HashSet<MeterMap>();
            this.MeterMapAudits = new HashSet<MeterMapAudit>();
        }
    
        public int MechId { get; set; }
        public string MechSerial { get; set; }
        public Nullable<int> MechType { get; set; }
        public Nullable<int> Customerid { get; set; }
        public string SimNo { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> InactiveRemarkID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public System.DateTime InsertedDate { get; set; }
        public string Notes { get; set; }
        public Nullable<int> MechIdNumber { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual InactiveRemark InactiveRemark { get; set; }
        public virtual MechanismMaster MechanismMaster { get; set; }
        public virtual ICollection<MeterMap> MeterMaps { get; set; }
        public virtual ICollection<MeterMapAudit> MeterMapAudits { get; set; }
    }
}
