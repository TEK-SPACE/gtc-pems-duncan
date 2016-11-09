using System;

namespace Duncan.PEMS.Entities.Transactions
{
    public class CustomerTransactionSensorModel
    {
        // Sensor Information 
        public int? SensorId { get; set; }
        public string SensorName { get; set; }
        public int MeterId { get; set; }
        public string MeterName { get; set; }
        public int? GatewayId { get; set; }
        public string GatewayName { get; set; }
        public string Area { get; set; }
        public string Zone { get; set; }
        public string Suburb { get; set; }
        public string Street { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string DemandType { get; set; }

        // Transaction Detail
        public long? TransactionId { get; set; }
        public DateTime? TransactionDateTime { get; set; }
        public bool Peak { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public DateTime? DepartureDateTime { get; set; }
        public DateTime? ZeroOutTime { get; set; }
        public int? TotalAmountInCent { get; set; }
        public int? TotalNumberOfPayments { get; set; }
        public int? TotalTimePaidMinutes { get; set; }
        public int? TotalOccupiedMinutes { get; set; }
        public int? RemaingPaidTimeMinutes { get; set; }
        public int? FreeParkingTimeMinutes { get; set; }
        public int? GracePeriodMinutes { get; set; }
        public int? ViolationMinutes { get; set; }
        public string OperationalStatus { get; set; }
        public string NonComplianceStatus { get; set; }
        public string OccupancyStatus { get; set; }
        public bool DiscountSchemaUsed { get; set; }

        // Payment Detail
        public DateTime? FirstTxPaymentTime { get; set; }
        public DateTime? LastTxPaymentTime { get; set; }
        public DateTime? FirstTxStartTime { get; set; }
        public DateTime? LastTxStartTime { get; set; }
        public DateTime? FirstTxExpiryTime { get; set; }
        public DateTime? LastTxExpiryTime { get; set; }
        public int? FirstTxAmountInCent { get; set; }
        public int? LastTxAmountInCent { get; set; }
        public int? FirstTxTimePaidMinute { get; set; }
        public int? LastTxTimePaidMinute { get; set; }
        public string FirstTxPaymentMethod { get; set; }
        public string LastTxPaymentMethod { get; set; }
    }
}