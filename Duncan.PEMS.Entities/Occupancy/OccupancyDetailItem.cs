using System;

namespace Duncan.PEMS.Entities.Occupancy
{
    public class OccupancyDetailItem
    {
        public long SpaceId { get; set; }
        public string SpaceType { get; set; }
        public string Area { get; set; }
        public string Zone { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
        public long SensorId { get; set; }
        public long MeterId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int DemandArea { get; set; }
        public DateTime ArrivalTimestamp { get; set; }
        public DateTime? DepartureTimestamp { get; set; }

        public string PeakTime
        {
            get
            {
                string ret = "NO";
                if ( TimeType1 == "Peak" || TimeType2 == "Peak" || TimeType3 == "Peak" || TimeType4 == "Peak" ||
                     TimeType5 == "Peak" )
                    ret = "YES";
                return ret;
            }
        }

        public string EveningTime
        {
            get
            {
                string ret = "NO";
                if ( TimeType1 == "Evening" || TimeType2 == "Evening" || TimeType3 == "Evening" || TimeType4 == "Evening" ||
                     TimeType5 == "Evening" )
                    ret = "YES";
                return ret;
            }
        }

        public int ViolationMinutes { get; set; }
        public int ViolationSegmentCount { get; set; }
        public int OccupancyDuration { get; set; }
        public string OccupancyStatus { get; set; }
        public DateTime? OccupancyTimestamp { get; set; }
        public string OperationalStatus { get; set; }
        public string NonCompliantStatus { get; set; }
        public int UnusedPaidTime { get; set; }
        public DateTime? TimeCleared { get; set; }
        public DateTime? PaidTimeStart { get; set; }
        public int FreeParkingTime { get; set; }
        public DateTime? FirstPaymentTime { get; set; }
        public int FirstPaymentAmount { get; set; }
        public string FirstPaymentMethod { get; set; }
        public DateTime? LastPaymentTime { get; set; }
        public int LastPaymentAmount { get; set; }
        public string LastPaymentMethod { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public int TotalPaymentCount { get; set; }
        public string DiscountType { get; set; }
        public DateTime? PaidTimeEnd { get; set; }
        public int PaidTimeDuration { get; set; }
        public int GracePeriodUsed { get; set; }
        public string TimeType1 { get; set; }
        public string TimeType2 { get; set; }
        public string TimeType3 { get; set; }
        public string TimeType4 { get; set; }
        public string TimeType5 { get; set; }

        public int? BayNumber { get; set; }
        public string BayName { get; set; }

        public bool? Zeroout { get; set; } 
        public int? Minuteresold { get; set; }

    }
}