using System;
using Duncan.PEMS.Entities.General;
#pragma warning disable 1591

namespace Duncan.PEMS.Entities.Transactions
{
  
    public class CustomerSensorTransactionModel
    {
        public DateTime DateTime { get; set; }
        [OriginalGridPosition(Position = 0)]
        public string DateTimeDisplay
        {
            get { return DateTime == DateTime.MinValue ? string.Empty : DateTime.ToString("G"); }
        }
        [OriginalGridPosition(Position = 1)]
        public long TransactionId { get; set; }

        [OriginalGridPosition(Position = 2)]
        public string AssetType { get; set; }

        [OriginalGridPosition(Position = 3)]
        public int? AssetId { get; set; }

        [OriginalGridPosition(Position = 4)]
        public string AssetName { get; set; }

        public int? AreaId { get; set; }
        [OriginalGridPosition(Position = 5)]
        public string Area { get; set; }

        [OriginalGridPosition(Position = 6)]
        public string Street { get; set; }


        [OriginalGridPosition(Position = 7)]
        public string TransactionType { get; set; }


        [OriginalGridPosition(Position = 8)]
        public string SpaceStatus { get; set; }

        public long? SpaceId { get; set; }

        [OriginalGridPosition(Position = 9)]
        public string SpaceIdDisplay
        {
            get { return SpaceId != null ? SpaceId.ToString() : ""; }
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
