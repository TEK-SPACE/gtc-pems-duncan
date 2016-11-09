using System;

namespace Duncan.PEMS.Entities.Transactions
{
    public class SensorTransactionModel
    {
        public long TransactionId { get; set; }
        public int AmountPaid { get; set; }
        public int Duration { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}