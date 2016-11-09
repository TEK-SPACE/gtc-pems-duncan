using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Collections
{
    public class CollectionListModel
    {
        //grid props
        [OriginalGridPosition(Position = 0)]
        public string RouteName { get; set; }
        [OriginalGridPosition(Position = 1)]
        public long RouteId { get; set; }
        public string ConfigurationID { get { return RouteId.ToString(); } }
        [OriginalGridPosition(Position = 2)]
        public string Status { get; set; }
        public int StatusId { get; set; }
        public DateTime DateCreated { get; set; }
        [OriginalGridPosition(Position = 3)]
        public string DateCreatedDisplay { get { return DateCreated == DateTime.MinValue ? string.Empty : DateCreated.ToString("d"); } }
        public DateTime? DateActivated { get; set; }
        [OriginalGridPosition(Position = 4)]
        public string DateActivatedDisplay { get { return DateActivated.HasValue ? DateActivated.Value.ToString("d") : string.Empty; } }
        [OriginalGridPosition(Position = 5)]
        public int NumberOfMeters { get; set; }
    }

    public class ConfigDDLModel
    {
        public long Value { get; set; }
        public string Text { get; set; }
    }

    public class StatusDDLModel
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class VendorDDLModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }

    public class AggregationListModel
    {
        //grid props
        public DateTime? DateTime { get; set; }
        [OriginalGridPosition(Position = 1)]
        public string DateTimeDisplay { get { return DateTime.HasValue ? DateTime.Value.ToString("d") : string.Empty; } }

        public long DateTimeTicks
        {
            get { return DateTime.HasValue ? DateTime.Value.Ticks : 0; }
        }

        public long RouteId { get; set; }
        [OriginalGridPosition(Position = 2)]
        public string ConfigurationID { get { return RouteId.ToString(); } }
        [OriginalGridPosition(Position = 0)]
        public string RouteName { get; set; }
        public int? VendorId { get; set; }
        [OriginalGridPosition(Position = 3)]
        public string VendorName { get; set; }
        [OriginalGridPosition(Position = 4)]
        public int MetersToCollect { get; set; }
        [OriginalGridPosition(Position = 5)]
        public int MetersCollected { get; set; }
        [OriginalGridPosition(Position = 6)]
        public double? TotalCollectedMeter { get; set; }
        [OriginalGridPosition(Position = 7)]
        public double? TotalCollectedChip { get; set; }
        //[OriginalGridPosition(Position = 8)]
        public double? TotalCollectedVendor { get; set; }
        public int Variance { get; set; }
        [OriginalGridPosition(Position = 8)]
        public float? AmountDiff { get; set; }
        public int CustomerId { get; set; }

        public long CollectionRunReportId { get; set; }

        public bool? UnScheduled { get; set; }

    }

    public class GroupedAggregationListModel
    {
        //grid props
        public DateTime? DateTime { get; set; }
        [OriginalGridPosition(Position = 1)]
        public string DateTimeDisplay { get { return DateTime.HasValue ? DateTime.Value.ToString("d") : string.Empty; } }

        public long DateTimeTicks
        {
            get { return DateTime.HasValue ? DateTime.Value.Ticks : 0; }
        }

        //todo - GTC: update this to reflect the correct grid data for the grouped aggregation index page.
        public long RouteId { get; set; }
        [OriginalGridPosition(Position = 2)]
        public string ConfigurationID { get { return RouteId.ToString(); } }
        [OriginalGridPosition(Position = 0)]
        public string RouteName { get; set; }
        public int? VendorId { get; set; }
        [OriginalGridPosition(Position = 3)]
        public string VendorName { get; set; }
        [OriginalGridPosition(Position = 4)]
        public int MetersToCollect { get; set; }
        [OriginalGridPosition(Position = 5)]
        public int MetersCollected { get; set; }
        [OriginalGridPosition(Position = 6)]
        public double? TotalCollected { get; set; }
        public int Variance { get; set; }
        [OriginalGridPosition(Position = 7)]
        public float? AmountDiff { get; set; }
        public int CustomerId { get; set; }
    }



    public class CollectionConfiguration
    {
        public int CustomerId { get; set; }

        public string Area { get; set; }

        public string Zone { get; set; }

        public string Street { get; set; }

        public string Suburb { get; set; }

        public long Value { get; set; }
        public string Text { get; set; }

        public DateTime LocalTime { get; set; }

        [Required]
        [Display(Name = "Collection Name")]
        public string CollectionName { get; set; }

        public string Status { get; set; }
        public int StatusId { get; set; }
        public long CollectionId { get; set; }
        public string VendorName { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Vendor Required")]
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }

        [Display(Name = "Activation Date")]
        public DateTime? ActivationDate { get; set; }

        [Required]
        [Display(Name = "Days Between Collections")]
        public int DaysBtwCollections { get; set; }

        public bool SkipPublicHolidays { get; set; }
        public bool SkipSunday { get; set; }
        public bool SkipMonday { get; set; }
        public bool SkipTuesday { get; set; }
        public bool SkipWednesday { get; set; }
        public bool SkipThursday { get; set; }
        public bool SkipFriday { get; set; }
        public bool SkipSaturday { get; set; }
        public List<CollectionMeter> Meters { get; set; }
        public List<CollectionMeter> AllMeters { get; set; }
        public List<StatusDDLModel> StatusOptions { get; set; }
        public List<VendorDDLModel> VendorOptions { get; set; }

        public DateTime DateCreated { get; set; }
        public int? CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastEditedOn { get; set; }
        public int? LastEditedById { get; set; }
        public string LastEditedBy { get; set; }

        public bool SkipDaysOfWeek
        {
            get
            {
                if (SkipFriday || SkipMonday || SkipSaturday || SkipSunday || SkipThursday || SkipTuesday ||
                     SkipWednesday)
                    return true;
                return false;
            }
        }
    }

    public class AggregationMeterDetails : CollectionMeter
    {

        public int CustomerId { get; set; }
        public DateTime CollectionDate { get; set; }

        //meter / location information
        public DateTime? DateTime { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        //collection information
        public string CashBoxIdRemoved { get; set; }
        public string CashBoxIdInserted { get; set; }
        public DateTime? InsertionTimeMeter { get; set; }
        public DateTime RemovalTimeMeter { get; set; }
        public DateTime? InsertionTimeChip { get; set; }
        public DateTime? RemovalTimeChip { get; set; }
        public DateTime? ReadTime { get; set; }
        public string TransactionLogFileName { get; set; }
        public long TimeActive { get; set; }
        public int SequenceNumber { get; set; }
        public double PercentageFull { get; set; }
        public string NewCashboxIdInserted { get; set; }
        public int Version { get; set; }
        public int CollectorId { get; set; }
        public string OperatorId { get; set; }

        //REvenue Collected
        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? MeterAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? ChipAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? VendorAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double DifferenceMeterVendor { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double DifferenceMeterChip { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double DifferenceVendorChip { get; set; }

        //[DisplayFormat(DataFormatString = "{0:C}")]
        //[DataType(DataType.Currency)]
        //public double? TotalCollected { get; set; }
        public int CoinType1Count { get; set; }
        public int CoinType2Count { get; set; }
        public int CoinType3Count { get; set; }
        public int CoinType4Count { get; set; }
        public int CoinType5Count { get; set; }
        public int CoinType6Count { get; set; }
        public bool DisplayType1 { get; set; }
        public bool DisplayType2 { get; set; }
        public bool DisplayType3 { get; set; }
        public bool DisplayType4 { get; set; }
        public bool DisplayType5 { get; set; }
        public bool DisplayType6 { get; set; }
        public string DisplayNameType1 { get; set; }
        public string DisplayNameType2 { get; set; }
        public string DisplayNameType3 { get; set; }
        public string DisplayNameType4 { get; set; }
        public string DisplayNameType5 { get; set; }
        public string DisplayNameType6 { get; set; }

    }

    public class AggregationDetails
    {
        //agg detail

        public int CustomerId { get; set; }
        public DateTime? DateTime { get; set; }
        public long RouteId { get; set; }
        public string RouteName { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public int MetersToCollect { get; set; }
        //summary info

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double TotalCashCollected { get; set; }

        public double TotalMetersCollected { get; set; }
        public double TotalMetersNotCollected { get; set; }
        public double TotalUnscheduledMeterCollected { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double TotalReportedByMeter { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? TotalReportedByChip { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? TotalReportedByVendor { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double AverageReportedByMeter { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? AverageReportedByChip { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? AverageReportedByVendor { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? TotalDifferenceMeterToChip { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? TotalDifferenceMeterToVendor { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? TotalDifferenceVendorToChip { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? AverageDifferenceMeterToChip { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? AverageDifferenceMeterToVendor { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? AverageDifferenceVendorToChip { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? MaxAmtCollectedMeter { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? MaxAmtCollectedVendor { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public long? MaxAmtCollectedChip { get; set; }

        //meters
        public List<AggregationMeter> Meters { get; set; }

        public long CollectionRunReportID { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double? MaxAmtCollectedMeterInDollar
        {
            set
            {
                this.MaxAmtCollectedMeter = value;
            }
            get
            {
                return this.MaxAmtCollectedMeter.Value / 100;
            }
        }
    }

    public class CollectionMeter
    {
        public long CollectionRunMeterID { get; set; }
        [OriginalGridPosition(Position = 6)]
        public string Street { get; set; }
        public int? ZoneId { get; set; }
        [OriginalGridPosition(Position = 4)]
        public string Zone { get; set; }
        [OriginalGridPosition(Position = 0)]
        public int MeterId { get; set; }
        [OriginalGridPosition(Position = 1)]
        public string MeterName { get; set; }
        public int CustomerId { get; set; }
        public int AreaId { get; set; }
        public int? AreaId2 { get; set; }
        [OriginalGridPosition(Position = 3)]
        public string Area { get; set; }
        public long? CollectionRunId { get; set; }
        public string CollectionName { get; set; }
        [OriginalGridPosition(Position = 5)]
        public string Suburb { get; set; }
    }

    public class AggregationMeter : CollectionMeter
    {
        public DateTime? DateTime { get; set; }
        public string DateTimeDisplay { get { return DateTime.HasValue ? DateTime.Value.ToString("d") : string.Empty; } }
        public long DateTimeTicks { get { return DateTime.HasValue ? DateTime.Value.Ticks : 0; } }

        public DateTime? CollectionDateTime { get; set; }
        [OriginalGridPosition(Position = 2)]
        public string CollectionDateTimeDisplay { get { return CollectionDateTime.HasValue ? CollectionDateTime.Value.ToString("d") : string.Empty; } }
        public long CollectionDateTimeTicks { get { return CollectionDateTime.HasValue ? CollectionDateTime.Value.Ticks : 0; } }
        [OriginalGridPosition(Position = 7)]
        public double? AmtMeter { get; set; }
        //[OriginalGridPosition(Position = 8)]
        public double? AmtVendor { get; set; }
        [OriginalGridPosition(Position = 8)]
        public double? AmtChip { get; set; }
        public bool DifferenceFlag { get; set; }
    }

    public class MeterAdditionStatus
    {
        public CollectionRouteResult Result { get; set; }
        public int MeterId { get; set; }
        public long OriginalCollectionId { get; set; }
        public long ConflictingCollectionId { get; set; }

    }


    public class VCEIndexModel
    {
        public int CustomerId { get; set; }
        public List<VCEListModel> VCEs { get; set; }
    }


    public class VCEListModel
    {
        public int CollectionRunId { get; set; }
        public int CustomerId { get; set; }
        public string CollectionRunName { get; set; }
        public DateTime? ActivationDate { get; set; }
        public string ActivationDateDisplay { get { return ActivationDate.HasValue ? ActivationDate.Value.ToString("d") : string.Empty; } }
        public int TotalAmountReportedByMeter { get; set; }
        public int TotalAmountCounted { get; set; }
    }

    public class VCEDetailsModel
    {
        public int CollectionRunId { get; set; }
        public string CollectionRunName { get; set; }
        public int? CollectionRunReportId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? ActivationDate { get; set; }
        public string ActivationDateDisplay { get { return ActivationDate.HasValue ? ActivationDate.Value.ToString("d") : string.Empty; } }

        public string Coin1Name { get; set; }
        public string Coin2Name { get; set; }
        public string Coin3Name { get; set; }
        public string Coin4Name { get; set; }
        public string Coin5Name { get; set; }
        public string Coin6Name { get; set; }
        public string Coin7Name { get; set; }
        public string Coin8Name { get; set; }
        public int Coin1Count { get; set; }
        public int Coin2Count { get; set; }
        public int Coin3Count { get; set; }
        public int Coin4Count { get; set; }
        public int Coin5Count { get; set; }
        public int Coin6Count { get; set; }
        public int Coin7Count { get; set; }
        public int Coin8Count { get; set; }
        public int Coin1Value { get; set; }
        public int Coin2Value { get; set; }
        public int Coin3Value { get; set; }
        public int Coin4Value { get; set; }
        public int Coin5Value { get; set; }
        public int Coin6Value { get; set; }
        public int Coin7Value { get; set; }
        public int Coin8Value { get; set; }

        public bool Coin1Display { get; set; }
        public bool Coin2Display { get; set; }
        public bool Coin3Display { get; set; }
        public bool Coin4Display { get; set; }
        public bool Coin5Display { get; set; }
        public bool Coin6Display { get; set; }
        public bool Coin7Display { get; set; }
        public bool Coin8Display { get; set; }


        public int TotalValue { get; set; }
        public int TotalCoinCount { get; set; }


        public string ButtonText
        {
            get
            {
                if (CollectionRunReportId.HasValue)
                    return "Recount";
                return "Save";

            }
        }
    }


}
