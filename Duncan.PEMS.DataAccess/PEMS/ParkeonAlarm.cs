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
    
    public partial class ParkeonAlarm
    {
        public int Id { get; set; }
        public string Park { get; set; }
        public string Meter { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public System.DateTime OccurredDate { get; set; }
        public System.DateTime ReceivedDate { get; set; }
        public string WorkOrderId { get; set; }
    }
}
