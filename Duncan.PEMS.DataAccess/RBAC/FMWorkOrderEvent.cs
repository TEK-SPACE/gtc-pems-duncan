//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duncan.PEMS.DataAccess.RBAC
{
    using System;
    using System.Collections.Generic;
    
    public partial class FMWorkOrderEvent
    {
        public long WorkOrderEventId { get; set; }
        public long WorkOrderId { get; set; }
        public long EventId { get; set; }
        public int EventCode { get; set; }
        public System.DateTime EventDateTime { get; set; }
        public System.DateTime SLADue { get; set; }
        public string EventDesc { get; set; }
        public int AlarmTier { get; set; }
        public string Notes { get; set; }
        public bool Automated { get; set; }
        public bool Vandalism { get; set; }
        public int Status { get; set; }
    
        public virtual FMWorkOrder WorkOrder { get; set; }        
    }
}
