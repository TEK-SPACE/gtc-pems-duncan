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
    
    public partial class EventCodesAudit
    {
        public int AuditID { get; set; }
        public int CustomerID { get; set; }
        public int EventSource { get; set; }
        public int EventCode { get; set; }
        public int AlarmTier { get; set; }
        public string EventDescAbbrev { get; set; }
        public string EventDescVerbose { get; set; }
        public Nullable<int> SLAMinutes { get; set; }
        public Nullable<bool> IsAlarm { get; set; }
        public Nullable<int> EventType { get; set; }
        public int UserId { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }
        public Nullable<bool> ApplySLA { get; set; }
        public Nullable<int> EventCategory { get; set; }
    }
}
