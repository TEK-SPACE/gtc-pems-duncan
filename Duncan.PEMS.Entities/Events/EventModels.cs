using System;
using System.Collections.Generic;
using Duncan.PEMS.Entities.Alarms;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Events
{

    //in order to export and respect the custom grids in the DB, 
    //we have to add Original Position attributes, so we will have to add some "new" properties that respect that, 
    //since all these items use the event model as its base, but the positions on the grid are different. This is why you see duplicate properties.
    //NO NOT REMOVE THESE



    /// <summary>
    ///     Base Event Model
    /// </summary>
    public class EventModel
    {
        public int? CustomerID { get; set; }
        public DateTime DateTime { get; set; }
        public string DateTimeDisplay { get { return DateTime == DateTime.MinValue ? string.Empty : DateTime.ToString("g"); } }
        
        public string AssetType { get; set; }
        public string AssetName { get; set; }
        public int? AssetId { get; set; }
        public string EventClass { get; set; }
        public int? EventCode { get; set; }
        public string Area { get; set; }
        public string Zone { get; set; }
        public int? AreaId { get; set; }
        public int? ZoneId { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
        public string DemandArea { get; set; }
        public int? TimeType1 { get; set; }
        public int? TimeType2 { get; set; }
        public int? TimeType3 { get; set; }
        public int? TimeType4 { get; set; }
        public int? TimeType5 { get; set; }

        public long RowNumber { get; set; }
        public int Count { get; set; }
        public long? EventUID { get; set; }

    }

    public class SummaryEventModel : EventModel
    {
           [OriginalGridPosition(Position = 0)]
        public new string DateTimeDisplay { get { return DateTime == DateTime.MinValue ? string.Empty : DateTime.ToString("g"); } }
           [OriginalGridPosition(Position = 1)]
           public new long? EventUID { get; set; }
           [OriginalGridPosition(Position = 2)]
        public string Description { get; set; }
           [OriginalGridPosition(Position = 3)]
        public new string AssetType { get; set; }
           [OriginalGridPosition(Position = 4)]
           public new int? AssetId { get; set; }
           [OriginalGridPosition(Position = 4)]
        public new string AssetName { get; set; }
           [OriginalGridPosition(Position = 5)]
        public new string Street { get; set; }
           [OriginalGridPosition(Position = 6)]
        public new string EventClass { get; set; }
           [OriginalGridPosition(Position = 7)]
        public new int? EventCode { get; set; }
           [OriginalGridPosition(Position = 8)]
        public new string Area { get; set; }
           [OriginalGridPosition(Position = 9)]
        public new string Zone { get; set; }
           [OriginalGridPosition(Position = 10)]
        public new string Suburb { get; set; }
           [OriginalGridPosition(Position = 11)]
        public new string DemandArea { get; set; }
    }

    public class DiagnosticsEventModel : EventModel
    {
        [OriginalGridPosition(Position = 0)]
        public new string DateTimeDisplay { get { return DateTime == DateTime.MinValue ? string.Empty : DateTime.ToString("g"); } }
        [OriginalGridPosition(Position = 1)]
        public string ReceivedDateTimeDisplay { get { return ReceivedDateTime == DateTime.MinValue ? string.Empty : ReceivedDateTime.ToString("G"); } }
        [OriginalGridPosition(Position = 2)]
        public new long? EventUID { get; set; }

        [OriginalGridPosition(Position = 3)]
        public new int? AssetId { get; set; }
        [OriginalGridPosition(Position = 4)]
        public new string AssetName { get; set; }
       

        [OriginalGridPosition(Position = 5)]
        public string Type { get; set; }
        [OriginalGridPosition(Position = 6)]
        public string Value { get; set; }
        [OriginalGridPosition(Position = 7)]
        public new string Street { get; set; }
        [OriginalGridPosition(Position = 8)]
        public new string Area { get; set; }
        [OriginalGridPosition(Position = 9)]
        public new string Zone { get; set; }
        [OriginalGridPosition(Position = 10)]
        public new string Suburb { get; set; }
        [OriginalGridPosition(Position = 11)]
        public new string DemandArea { get; set; }

        public DateTime ReceivedDateTime { get; set; }
        public int? CoinRejectCount { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Voltage { get; set; }
        public decimal? SignalStrength { get; set; }
        public string Version { get; set; }

    }

    public class AlarmsEventModel : EventModel
    {
        [OriginalGridPosition(Position = 0)]
        public new string DateTimeDisplay { get { return DateTime == DateTime.MinValue ? string.Empty : DateTime.ToString("g"); } }
        [OriginalGridPosition(Position = 1)]
        public new long? EventUID { get; set; }
        [OriginalGridPosition(Position = 2)]
        public new string AssetType { get; set; }
        [OriginalGridPosition(Position = 3)]
        public new int? AssetId { get; set; }
        [OriginalGridPosition(Position = 3)]
        public new string AssetName { get; set; }
        [OriginalGridPosition(Position = 4)]
        public string TimeNotifiedDisplay { get { return TimeNotified == DateTime.MinValue ? string.Empty : TimeNotified.ToString("g"); } }
        [OriginalGridPosition(Position = 5)]
        public string TimeClearedDisplay { get { return TimeCleared.HasValue ? TimeCleared.Value.ToString("g") : string.Empty; } }
        [OriginalGridPosition(Position = 6)]
        public string TimeDueSLADisplay { get; set; }
        [OriginalGridPosition(Position = 7)]
        public string AlarmDescription { get; set; }
        [OriginalGridPosition(Position = 8)]
        public string Source { get; set; }
        [OriginalGridPosition(Position = 9)]
        public string AlarmSeverity { get; set; }
        [OriginalGridPosition(Position = 10)]
        public int? WorkOrderId { get; set; }
        [OriginalGridPosition(Position = 11)]
        public int? ResolutionCode { get; set; }
        [OriginalGridPosition(Position = 12)]
        public string Technician { get; set; }

        public DateTime? TimeDueSLA { get; set; }
        public DateTime TimeNotified { get; set; }
        public DateTime? TimeCleared { get; set; }
        public bool IsClosed { get; set; }
        public int? TotalMinutes { get; set; }
    }

    public class ConnectionEventModel : EventModel
    {
         [OriginalGridPosition(Position = 0)]
        public new string DateTimeDisplay { get { return DateTime == DateTime.MinValue ? string.Empty : DateTime.ToString("g"); } }
         [OriginalGridPosition(Position = 1)]
         public new long? EventUID { get; set; }
         [OriginalGridPosition(Position = 2)]
         public new string AssetType { get; set; }
         [OriginalGridPosition(Position = 3)]
         public new string AssetName { get; set; }
         [OriginalGridPosition(Position = 4)]
         public string ConnectionStatus { get; set; }
         [OriginalGridPosition(Position = 5)]
         public string ErrorDescription { get; set; }
         [OriginalGridPosition(Position = 6)]
         public string EndTimeDisplay { get { return EndTime == DateTime.MinValue ? string.Empty : EndTime.ToString("g"); } }
         [OriginalGridPosition(Position = 7)]
         public string Period { get; set; }
         [OriginalGridPosition(Position = 8)]
         public int Port { get; set; }

        public DateTime StartTime { get; set; }
          public string StartTimeDisplay { get { return StartTime == DateTime.MinValue ? string.Empty : StartTime.ToString("g"); } }
        public DateTime EndTime { get; set; }
    }

    public class TransactionEventModel : EventModel
    {
         [OriginalGridPosition(Position = 0)]
        public new string DateTimeDisplay { get { return DateTime == DateTime.MinValue ? string.Empty : DateTime.ToString("g"); } }
         [OriginalGridPosition(Position = 1)]
         public long TransactionId { get; set; }
         [OriginalGridPosition(Position = 2)]
         public new string AssetType { get; set; }
         [OriginalGridPosition(Position = 3)]
         public new string AssetName { get; set; }
         [OriginalGridPosition(Position = 4)]
         public string PaymentType { get; set; }
         [OriginalGridPosition(Position = 5)]
         public int? Bay { get; set; }
         [OriginalGridPosition(Position = 6)]
         public string TimePaidDisplay { get; set; }
         [OriginalGridPosition(Position = 7)]
         public string AmountPaidDisplay { get; set; }
         [OriginalGridPosition(Position = 8)]
         public string CardType { get; set; }
         [OriginalGridPosition(Position = 9)]
         public string CardStatus { get; set; }
         [OriginalGridPosition(Position = 10)]
         public int? ReceiptNumber { get; set; }

        public int? Amount { get; set; }
        public int? TimePaid { get; set; }
        public DateTime? TransactionTime { get; set; }
        public long? SensorPaymentTransactionId { get; set; }
        public bool IsSensorTransaction { get; set; }
    }

    public class CollectionCommEventModel : EventModel
    {
          [OriginalGridPosition(Position = 0)]
        public new string DateTimeDisplay { get { return DateTime == DateTime.MinValue ? string.Empty : DateTime.ToString("g"); } }
          [OriginalGridPosition(Position = 1)]
          public new long? EventUID { get; set; }
          [OriginalGridPosition(Position = 2)]
          public new int? AssetId { get; set; }
          [OriginalGridPosition(Position = 2)]
          public new string AssetName { get; set; }
          [OriginalGridPosition(Position = 3)]
          public string InsertionTimeDisplay { get { return InsertionTime.HasValue ? InsertionTime.Value.ToString("g") : string.Empty; } }
          [OriginalGridPosition(Position = 4)]
          public string AmountDisplay { get; set; }
          [OriginalGridPosition(Position = 5)]
          public string PreviousCBID { get; set; }
          [OriginalGridPosition(Position = 6)]
          public string NewCBID { get; set; }
          [OriginalGridPosition(Position = 7)]
          public int? SequenceNumber { get; set; }

        public double Amount { get; set; }
        public DateTime? InsertionTime { get; set; }


    }

    public class CollectionCBREventModel : EventModel
    {

        [OriginalGridPosition(Position = 0)]
        public new string DateTimeDisplay { get { return DateTime == DateTime.MinValue ? string.Empty : DateTime.ToString("g"); } }
        [OriginalGridPosition(Position = 1)]
        public new long? EventUID { get; set; }
        [OriginalGridPosition(Position = 2)]
        public new int? AssetId { get; set; }
        [OriginalGridPosition(Position = 2)]
        public new string AssetName { get; set; }
        [OriginalGridPosition(Position = 3)]
        public string RemovalTimeDisplay { get { return RemovalTime.HasValue ? RemovalTime.Value.ToString("g") : string.Empty; } }
        [OriginalGridPosition(Position = 4)]
        public string InsertionTimeDisplay { get { return InsertionTime == DateTime.MinValue ? string.Empty : InsertionTime.ToString("g"); } }
        [OriginalGridPosition(Position = 5)]
        public string TimeActive
        {
            get
            {
                if (RemovalTime.HasValue)
                {
                    TimeSpan timeSpan = RemovalTime.Value - InsertionTime;
                    string formatted;
                    if (timeSpan.TotalDays >= 1)
                    {
                        formatted = String.Format("{0:00}:{1:00}:{2:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
                    }
                    else
                    {
                        formatted = String.Format("{0:00}:{1:00}", timeSpan.Hours, timeSpan.Minutes);
                    }

                    return formatted;
                }

                return String.Empty;
            }
        }
        [OriginalGridPosition(Position = 6)]
        public string CBID { get; set; }
        [OriginalGridPosition(Position = 7)]
        public int SequenceNumber { get; set; }
        [OriginalGridPosition(Position = 8)]
        public double? AmtAuto { get; set; }
        [OriginalGridPosition(Position = 9)]
        public double? AmtManual { get; set; }
        [OriginalGridPosition(Position = 10)]
        public float? AmtDifference { get; set; }
        [OriginalGridPosition(Position = 11)]
        public string OperatorId { get; set; }
        [OriginalGridPosition(Position = 12)]
        public int? Version { get; set; }
        [OriginalGridPosition(Position = 13)]
        public string TransactionFileName { get; set; }

        public DateTime InsertionTime { get; set; }
        public DateTime? RemovalTime { get; set; }
    }

    public class EventDetails
    {
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        public string Area { get; set; }
        public string Zone { get; set; }
        public string Suburb { get; set; }
        public string Street { get; set; }
        public string AssetType { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string DemandArea { get; set; }
        public int? BaysAffected { get; set; }
        public long? EventId { get; set; }
        public int EventCode { get; set; }
        public int EventSource { get; set; }
        public string EventSourceDescription { get; set; }
        public string TechnicianId { get; set; }
        public DateTime EventDateTime { get; set; }
        public string DayOfWeek { get; set; }
        public string PartOfWeek { get; set; }
        public string Period { get; set; }
        public string AbbrDesc { get; set; }
        public string LongDesc { get; set; }
        public List<TimeType> TimeTypes { get; set; }
        public int? TimeType1 { get; set; }
        public int? TimeType2 { get; set; }
        public int? TimeType3 { get; set; }
        public int? TimeType4 { get; set; }
        public int? TimeType5 { get; set; }
        public bool Peak { get; set; }
    }
}