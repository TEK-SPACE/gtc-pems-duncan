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
    
    public partial class zzzParkingSpace
    {
        public Nullable<int> MeterID { get; set; }
        public int Customerid { get; set; }
        public bool HasSensor { get; set; }
        public int OccupancyStatus { get; set; }
        public string LastPaymentSource { get; set; }
        public string SpaceStatus { get; set; }
        public string ViolationType { get; set; }
        public Nullable<int> AmountInCents { get; set; }
        public Nullable<System.DateTime> ExpiryTime { get; set; }
        public Nullable<System.DateTime> SensorEventTime { get; set; }
        public Nullable<System.DateTime> PresentMeterTime { get; set; }
        public Nullable<System.DateTime> NonSensorEventTime { get; set; }
        public string MeterName { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<int> timezoneid { get; set; }
        public string MeterType { get; set; }
        public Nullable<int> ZoneID { get; set; }
        public string ZoneName { get; set; }
        public Nullable<int> AreaId { get; set; }
        public string areaname { get; set; }
        public Nullable<int> clusterid { get; set; }
        public Nullable<int> GracePeriodMinute { get; set; }
        public Nullable<int> expseconds { get; set; }
        public Nullable<int> baynumber { get; set; }
        public int imin { get; set; }
        public string Location { get; set; }
        public Nullable<long> ParkingSpaceID { get; set; }
    }
}
