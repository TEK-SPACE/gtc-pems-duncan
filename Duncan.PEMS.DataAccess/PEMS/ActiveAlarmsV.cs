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
    
    public partial class ActiveAlarmsV
    {
        public int CustomerID { get; set; }
        public int AreaID { get; set; }
        public Nullable<int> AssetID { get; set; }
        public Nullable<long> GlobalMeterID { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public string Street { get; set; }
        public string Area { get; set; }
        public string Zone { get; set; }
        public string Suburb { get; set; }
        public System.DateTime Time_of_Occurrence { get; set; }
        public System.DateTime Time_of_Report { get; set; }
        public Nullable<System.DateTime> Service_Target_Time { get; set; }
        public Nullable<int> AlarmUID { get; set; }
        public int AlarmCode { get; set; }
        public string AlarmCodeDesc { get; set; }
        public string AlarmSourceDesc { get; set; }
        public Nullable<int> AlarmSourceCode { get; set; }
        public string Alarm_Class { get; set; }
        public string AssetType { get; set; }
        public string AssetName { get; set; }
        public string State_of_Asset { get; set; }
        public string AlarmStatus { get; set; }
        public Nullable<int> TechnicianId { get; set; }
        public string TimeClass1 { get; set; }
        public string TimeClass2 { get; set; }
        public string TimeClass3 { get; set; }
        public string TimeClass4 { get; set; }
        public string TimeClass5 { get; set; }
        public string TargetService { get; set; }
        public Nullable<int> Minute_Duration_of_Alarm { get; set; }
        public Nullable<int> Time_Remaining_to_Service { get; set; }
        public string DemandArea { get; set; }
    }
}
