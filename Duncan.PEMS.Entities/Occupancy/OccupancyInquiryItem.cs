using System;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Occupancy
{

    public class OccupancyDDL
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
    public class OccupancyDDLString
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
    public class OccupancyInquiryItem
    {

        public DateTime ArrivalTime { get; set; }
        [OriginalGridPosition(Position = 0)]
        public string ArrivalTimeDisplay { get { return ArrivalTime == DateTime.MinValue ? string.Empty : ArrivalTime.ToString("g"); } }

        [OriginalGridPosition(Position = 1)]
        public string MeterName { get; set; }


        public int? TotalOccupiedMinute { get; set; }
        [OriginalGridPosition(Position = 2)]
        public string TotalOccupiedMinuteDisplay { get; set; }


        public int? TotalTimePaidMinute { get; set; }
        [OriginalGridPosition(Position = 3)]
        public string TotalTimePaidMinuteDisplay{get;set;}

        public int? TotalNumberOfPayments { get; set; }
        [OriginalGridPosition(Position = 4)]
        public string TotalNumberOfPayment
        {
            get { return TotalNumberOfPayments != null ? TotalNumberOfPayments.ToString() : ""; }
        }
        
        public int? TotalAmountInCent { get; set; }
        [OriginalGridPosition(Position = 5)]
        public string TotalAmountInCentDisplay {get;set;}


        public int? Minuteresold { get; set; }
        [OriginalGridPosition(Position = 6)]
        public string SecondsResold { get; set; }
        //public string SecondsResold
        //{
        //     get {
        //        if (Minuteresold != null)
        //        {
        //            if (Minuteresold.Value > 0)
        //                return Minuteresold.Value.ToString();
        //        }
        //        return ""; 
        //    }
        //}

        public int? RemaingPaidTimeMinute { get; set; }
        [OriginalGridPosition(Position = 7)]
        public string RemaingPaidTimeMinuteDisplay { get; set; }
        //public string RemaingPaidTimeMinuteDisplay
        //{
        //    get {
        //        if (RemaingPaidTimeMinute != null)
        //        {
        //            if (RemaingPaidTimeMinute.Value > 0)
        //                return RemaingPaidTimeMinute.Value.ToString();
        //        }
        //        return ""; 
        //    }
        //}

        public int? ViolationMinute { get; set; }
        [OriginalGridPosition(Position = 8)]
        public string ViolationMinuteDisplay { get;set; }
        //public string ViolationMinuteDisplay
        //{
        //    get {
        //        if (ViolationMinute != null)
        //        {
        //            if (ViolationMinute.Value > 0)
        //                return ViolationMinute.Value.ToString();
        //        }
        //        return ""; 
        //    }
        //}

        public DateTime? DepartureTime { get; set; }
        [OriginalGridPosition(Position = 9)]
        public string DepartureTimeDisplay
        { 
            get { return DepartureTime.HasValue ? DepartureTime.Value.ToString("g") : string.Empty; } 
        }


        //[OriginalGridPosition(Position = 9)]
        //public string OccupancyStatus { get; set; }


        [OriginalGridPosition(Position = 10)]
        public string OperationalStatus { get; set; }

        [OriginalGridPosition(Position = 11)]
        public int MeterId { get; set; }

        [OriginalGridPosition(Position = 12)]
        public int? SensorId { get; set; }

        [OriginalGridPosition(Position = 13)]
        public string SpaceType { get; set; }

        public long SpaceId { get; set; }
        [OriginalGridPosition(Position = 14)]
        public string DisplaySpaceId
        { 
            get { return SpaceId.ToString(); } 
        }

        [OriginalGridPosition(Position = 15)]
        public string Area { get; set; }

        public string DemandArea { get; set; }
        [OriginalGridPosition(Position = 16)]
        public string Street { get; set; }


        [OriginalGridPosition(Position = 17)]
        public string Zone { get; set; }

        [OriginalGridPosition(Position = 18)]
        public string Suburb { get; set; }

        [OriginalGridPosition(Position = 19)]
        public int? BayNumber { get; set; }

        [OriginalGridPosition(Position = 20)]
        public string BayName { get; set; }

        public string NonCompliantStatus { get; set; }
        public int? TimeType1 { get; set; }
        public int? TimeType2 { get; set; }
        public int? TimeType3 { get; set; }
        public int? TimeType4 { get; set; }
        public int? TimeType5 { get; set; }
        public string AssetName { get; set; }
        public int AssetId { get; set; }
        public string AssetType { get; set; }
        public int CustomerID { get; set; }
        public long RowNumber { get; set; }
        public int Count { get; set; }
        public long SensorPaymentTransactionId { get; set; }
    }
}