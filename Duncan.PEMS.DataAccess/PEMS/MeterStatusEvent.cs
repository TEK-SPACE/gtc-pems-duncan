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
    
    public partial class MeterStatusEvent
    {
        public Nullable<long> GlobalMeterID { get; set; }
        public int CustomerID { get; set; }
        public int AreaID { get; set; }
        public int MeterId { get; set; }
        public int State { get; set; }
        public System.DateTime TimeOfOccurrance { get; set; }
        public System.DateTime TimeOfNotification { get; set; }
        public Nullable<int> EventSource { get; set; }
    
        public virtual Meter Meter { get; set; }
        public virtual MeterServiceStatu MeterServiceStatu { get; set; }
    }
}
