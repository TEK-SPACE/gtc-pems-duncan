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
    
    public partial class WorkOrderPart
    {
        public long WorkOrderPartId { get; set; }
        public long WorkOrderId { get; set; }
        public Nullable<long> PartId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string Notes { get; set; }
    
        public virtual Part Part { get; set; }
        public virtual WorkOrder1 WorkOrder { get; set; }
    }
}