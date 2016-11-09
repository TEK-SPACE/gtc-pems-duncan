using System;


namespace Duncan.PEMS.Entities.WorkOrders
{
    public class WorkOrderEvent
    {
        public long WorkOrderEventId { get; set; }
        public long WorkOrderId { get; set; }
        public long EventId { get; set; }
        /// <summary>
        /// Alarm Code
        /// </summary>
        public int EventCode { get; set; }
        public DateTime EventDateTime { get; set; }
        public string EventDateTimeDisplay
        {
            get { return EventDateTime == DateTime.MinValue ? string.Empty : EventDateTime.ToString("g"); }
        }

        /// <summary>
        /// Deadline
        /// </summary>
        public DateTime SLADue { get; set; }
        public string SLADueDisplay
        {
            get { return SLADue == DateTime.MinValue ? string.Empty : SLADue.ToString("g"); }
        }
        /// <summary>
        /// Alarm Category
        /// </summary>
        public string EventDesc { get; set; }
        /// <summary>
        /// Severity
        /// </summary>
        public int AlarmTier { get; set; }
        /// <summary>
        /// Fault Reported Notes
        /// </summary>
        public string Notes { get; set; }
        public bool Automated { get; set; }
        public bool Vandalism { get; set; }
        public int Status { get; set; }
        public string StatusDisplay { get; set; }
        public int Severity { get; set; }
        public string SeverityDisplay { get; set; }
    }
}
