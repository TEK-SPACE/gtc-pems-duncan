using System;
using Duncan.PEMS.Entities.General;
#pragma warning disable 1591

namespace Duncan.PEMS.Entities.Transactions
{
    public class CustomerTransactionModel
    {
        public DateTime DateTime { get; set; }
        [OriginalGridPosition(Position = 0)]
        public string DateTimeDisplay
        {
            get { return DateTime == DateTime.MinValue ? string.Empty : DateTime.ToString("g"); }
        }
        [OriginalGridPosition(Position = 1)]
        public long TransactionId { get; set; }
        public int? AreaId { get; set; }
        [OriginalGridPosition(Position = 2)]
        public string Area { get; set; }
        [OriginalGridPosition(Position = 3)]
        public string Street { get; set; }
        public int? AmountPaid { get; set; }

        [OriginalGridPosition(Position = 4)]
        public string AmountPaidDisplay { get; set; }
        public int? TimePaid { get; set; }

        [OriginalGridPosition(Position = 5)]
        public string TimePaidDisplay { get; set; }

        [OriginalGridPosition(Position = 12)]
        public string AssetType { get; set; }

        [OriginalGridPosition(Position = 6)]
        public int? AssetId { get; set; }

        [OriginalGridPosition(Position = 7)]
        public string AssetName { get; set; }

        [OriginalGridPosition(Position = 8)]
        public string TransactionType { get; set; }

        [OriginalGridPosition(Position = 9)]
        public string PaymentStatusType { get; set; }

        [OriginalGridPosition(Position = 10)]
        public string CcLast4 { get; set; }

        [OriginalGridPosition(Position = 13)]
        public int? GatewayId { get; set; }

        public long? SpaceId { get; set; }

        [OriginalGridPosition(Position = 14)]
        public string SpaceIdDisplay
        {
            get { return SpaceId != null ? SpaceId.ToString() : ""; }
        }

        [OriginalGridPosition(Position = 11)]
        public string SpaceStatus { get; set; }

        [OriginalGridPosition(Position = 15)]
        public DateTime? BatchDate { get; set; }
       
        public string SettleDate  //Have Datetime value in database
        {
            get
            {
               if(BatchDate.HasValue)
                   return BatchDate.Value.ToString("MM/dd/yyyy");
               else
                   return string.Empty;
            }
            //get
            //{
            //    DateTime dateTime;
            //    if (DateTime.TryParse(BatchDate, out dateTime))
            //        return dateTime.ToString("MM/dd/yyyy");
            //    else
            //        return string.Empty;
            //}
            //get { return SettleBatchID.HasValue ? SettleBatchID.Value.ToString("MM/dd/YYYY") : string.Empty; }
        }

        public int? ZoneId { get; set; }
        public string Suburb { get; set; }
        public string DemandArea { get; set; }
        public int? BayId { get; set; }
        public int? DiscountSchemaId { get; set; }
        //public string PaymentStatusType { get; set; }
        public int? Occupancy { get; set; }

        public long? SensorPaymentTransactionId { get; set; }
        public bool IsSensorTransaction { get; set; }
        public int? TimeType1 { get; set; }
        public int? TimeType2 { get; set; }
        public int? TimeType3 { get; set; }
        public int? TimeType4 { get; set; }
        public int? TimeType5 { get; set; }

        public long RowNumber { get; set; }
        public int Count { get; set; }
        public int? CustomerId { get; set; }

        public int? CardType { get; set; }
    }
}
