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
    
    public partial class AssetPendingReason
    {
        public AssetPendingReason()
        {
            this.AssetPendings = new HashSet<AssetPending>();
            this.CashBoxAudits = new HashSet<CashBoxAudit>();
            this.ConfigProfileSpaceAudits = new HashSet<ConfigProfileSpaceAudit>();
            this.DataKeyAudits = new HashSet<DataKeyAudit>();
            this.GatewaysAudits = new HashSet<GatewaysAudit>();
            this.MechMasterAudits = new HashSet<MechMasterAudit>();
            this.MeterMapAudits = new HashSet<MeterMapAudit>();
            this.MetersAudits = new HashSet<MetersAudit>();
            this.ParkingSpacesAudits = new HashSet<ParkingSpacesAudit>();
            this.SensorMappingAudits = new HashSet<SensorMappingAudit>();
            this.SensorsAudits = new HashSet<SensorsAudit>();
        }
    
        public int AssetPendingReasonId { get; set; }
        public string AssetPendingReasonDesc { get; set; }
    
        public virtual ICollection<AssetPending> AssetPendings { get; set; }
        public virtual ICollection<CashBoxAudit> CashBoxAudits { get; set; }
        public virtual ICollection<ConfigProfileSpaceAudit> ConfigProfileSpaceAudits { get; set; }
        public virtual ICollection<DataKeyAudit> DataKeyAudits { get; set; }
        public virtual ICollection<GatewaysAudit> GatewaysAudits { get; set; }
        public virtual ICollection<MechMasterAudit> MechMasterAudits { get; set; }
        public virtual ICollection<MeterMapAudit> MeterMapAudits { get; set; }
        public virtual ICollection<MetersAudit> MetersAudits { get; set; }
        public virtual ICollection<ParkingSpacesAudit> ParkingSpacesAudits { get; set; }
        public virtual ICollection<SensorMappingAudit> SensorMappingAudits { get; set; }
        public virtual ICollection<SensorsAudit> SensorsAudits { get; set; }
    }
}
