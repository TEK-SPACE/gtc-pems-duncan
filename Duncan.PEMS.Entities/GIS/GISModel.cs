using Duncan.PEMS.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.GIS
{
    public class GISModel
    {

        //*******************************************************************************************************
        //public string[] selectedItemIds { get; set; }
        // public IEnumerable<SelectListItem> items { get; set; }

        //*******************************************************************************************************

        //** AI Chicago model variables

        [OriginalGridPosition(Position = 19)]
        public string AssetId { get; set; }  //** This field is displaying converted string format value for 19 digit parking no.

        public DateTime? startDateTime { get; set; }

        public DateTime? endDateTime { get; set; }

        [OriginalGridPosition(Position = 9)]
        public string startTime { get; set; }

        [OriginalGridPosition(Position = 4)]
        public string endTime { get; set; }

        [OriginalGridPosition(Position = 3)]
        public string dateOnly { get; set; }

        [OriginalGridPosition(Position = 8)]
        public string officerActivity { get; set; }

        [OriginalGridPosition(Position = 13)]
        public string assetModelDesc { get; set; }

        [OriginalGridPosition(Position = 18)]
        public string totalRevenue { get; set; }

        [OriginalGridPosition(Position = 0)]
        public int? officerID { get; set; }
        public DateTime? activityDate { get; set; }
        public DateTime? activityDateTime { get; set; }
        public string Remark_1 { get; set; }
        public string Remark_2 { get; set; }
        public string Remark_3 { get; set; }

        [OriginalGridPosition(Position = 1)]
        public string officerName { get; set; }

        [OriginalGridPosition(Position = 7)]
        public string activityDateOff { get; set; }


        //** NSC GIS model variables
        public long Value { get; set; }
        public string Text { get; set; }

        public Boolean selected { get; set; }

        public List<string> LstassetType { get; set; }
        public string assetType { get; set; }
        public string LocationType { get; set; }
        public string LocationTypeId { get; set; }

        //Grid Variables
        public int CustomerID { get; set; }
        public int ZoneID { get; set; }
        [OriginalGridPosition(Position = 16)]
        public string ZoneName { get; set; }
        public int AreaID { get; set; }
        [OriginalGridPosition(Position = 17)]
        public string AreaName { get; set; }
        public int MeterID { get; set; }

        [OriginalGridPosition(Position = 10)]
        public string longAssetID { get; set; }

        [OriginalGridPosition(Position = 11)]
        public string MeterName { get; set; }

        public string AssetStateDesc { get; set; }
        [OriginalGridPosition(Position = 15)]
        public string Location { get; set; }
        public int? BayStart { get; set; }
        public int? BayEnd { get; set; }
        public int? MeterGroup { get; set; }
        [OriginalGridPosition(Position = 12)]
        public string MeterGroupDesc { get; set; }

        [OriginalGridPosition(Position = 5)]
        public double? Latitude { get; set; }

        [OriginalGridPosition(Position = 6)]
        public double? Longitude { get; set; }
        public double? LocLatitude { get; set; }
        public double? LocLongitude { get; set; }
        public long citationID { get; set; }
        public string DisplayName { get; set; }

        [OriginalGridPosition(Position = 2)]
        public long IssueNo { get; set; }
        [OriginalGridPosition(Position = 14)]
        public string DemandZoneDesc { get; set; }
        public int DemandZoneId { get; set; }
        [OriginalGridPosition(Position = 19)]
        public long AssetID { get; set; } //** This field is for comparing long type in linq queries

        public int OccupancyStatusID { get; set; }
        [OriginalGridPosition(Position = 20)]
        public string OccupancyStatusDesc { get; set; }

        public bool NonCommStatusDesc { get; set; } //** added on May 30th 2016 for finding NonCommSensor

        public int? NonCompliantStatus { get; set; }
        [OriginalGridPosition(Position = 22)]
        public string NonCompliantStatusDesc { get; set; }

        public int OperationalStatusId { get; set; }
        [OriginalGridPosition(Position = 21)]
        public string OperationalStatusDesc { get; set; }

        public int EventCode { get; set; }
        public string EventDescVerbose { get; set; }
        public string TransactionDate { get; set; }
        public string TransType { get; set; }
        public int? cashAmount { get; set; }
        public int? creditCardAmount { get; set; }
        public int? smartCardAmount { get; set; }
        public int? cellAmount { get; set; }
        public string revCategory { get; set; }

        public string errorMsg { get; set; }  //** Sairam added on 25th sep 2014;

        public long? BayID { get; set; }
        public int? BayNumber { get; set; }
        public int? TimeZoneID { get; set; }
        public bool HasSensor { get; set; }
        public string PaymentType { get; set; }
        public DateTime? CurrentMeterTime { get; set; }
        public DateTime? NonSensorEventTime { get; set; }
        public string PaymentTime { get; set; }
        public string ViolationType { get; set; }
        public int? amountInCent { get; set; }
        public string amountInCentStr { get; set; }
        public string MeterType { get; set; }


        public string startDateTimeMob { get; set; }

        public string endDateTimeMob { get; set; }
    }

    public class GISCitationModel
    {

        public long AssetID { get; set; } //** This field is for comparing long type in linq queries
        public int MeterID { get; set; }
        public string startTime { get; set; }
        public int CustomerID { get; set; }
        public DateTime? startDateTime { get; set; }
        public DateTime? endDateTime { get; set; }


        [OriginalGridPosition(Position = 0)]
        public int? officerID { get; set; }

        [OriginalGridPosition(Position = 1)]
        public string officerName { get; set; }

        [OriginalGridPosition(Position = 2)]
        public long IssueNo { get; set; }

        [OriginalGridPosition(Position = 3)]
        public string dateOnly { get; set; }

        [OriginalGridPosition(Position = 4)]
        public string endTime { get; set; }

        [OriginalGridPosition(Position = 5)]
        public double? Latitude { get; set; }

        [OriginalGridPosition(Position = 6)]
        public double? Longitude { get; set; }

    }

    public class GISOfficerLocationModel
    {
        public long AssetID { get; set; } //** This field is for comparing long type in linq queries
        public int MeterID { get; set; }
        public string startTime { get; set; }
        public int CustomerID { get; set; }
        public DateTime? startDateTime { get; set; }
        public DateTime? endDateTime { get; set; }
        public string activityDateOff { get; set; }
        public string officerActivity { get; set; }
        public DateTime? activityDateTime { get; set; }

        [OriginalGridPosition(Position = 0)]
        public int? officerID { get; set; }

        [OriginalGridPosition(Position = 1)]
        public string officerName { get; set; }

        [OriginalGridPosition(Position = 2)]
        public double? Latitude { get; set; }

        [OriginalGridPosition(Position = 3)]
        public double? Longitude { get; set; }

        [OriginalGridPosition(Position = 4)]
        public DateTime? activityDate { get; set; }

    }

    public class GISOfficerRouteModel
    {
        [OriginalGridPosition(Position = 0)]
        public int? officerID { get; set; }

        [OriginalGridPosition(Position = 1)]
        public string officerName { get; set; }

        [OriginalGridPosition(Position = 2)]
        public string officerActivity { get; set; }

        [OriginalGridPosition(Position = 3)]
        public string dateOnly { get; set; }

        [OriginalGridPosition(Position = 4)]
        public string startTime { get; set; }

        public string endTime { get; set; }

        [OriginalGridPosition(Position = 5)]
        public double? Latitude { get; set; }

        [OriginalGridPosition(Position = 6)]
        public double? Longitude { get; set; }

    }

    //** Generic Group class
    public class Group<T, K>
    {
        public K Key;
        public IEnumerable<T> Values;
    }
}