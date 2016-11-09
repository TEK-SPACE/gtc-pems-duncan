using System;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.WorkOrders.Dispatcher
{
    /// <summary>
    /// Dispatcher list item. this is a stored proc / view model, so has to be flat.
    /// </summary>
    public class DispatcherWorkOrderListModel 
    {
        ////////////Filters////////////
        public DateTime CreationDate { get; set; }
        public string WorkOrderIdDisplay { get { return WorkOrderId.ToString(); } }
        public int? AssignmentState { get; set; }
        public string Suburb { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CompletedDateDisplay { get { return CompletedDate.HasValue ? CompletedDate.Value.ToString("g") : string.Empty; } }
        public DateTime Deadline { get; set; }
        public DateTime? AssignedDate { get; set; }
        public int HighestPriorityId { get; set; }

        ////////////Grid Fields////////////
        [OriginalGridPosition(Position = 1)]
        public long WorkOrderId { get; set; }
        [OriginalGridPosition(Position = 2)]
        public string Customer { get; set; }
        [OriginalGridPosition(Position = 3)]
        public string AssetType { get; set; }
        [OriginalGridPosition(Position = 4)]
        public int AssetId { get; set; }
        [OriginalGridPosition(Position = 5)]
        public string Street { get; set; }
        [OriginalGridPosition(Position = 6)]
        public string Area { get; set; }
        [OriginalGridPosition(Position = 7)]
        public string Zone { get; set; }
        [OriginalGridPosition(Position = 8)]
        public string HighestPriorityDisplay { get; set; }
        [OriginalGridPosition(Position = 9)]
        public string CreationDateDisplay { get { return CreationDate == DateTime.MinValue ? string.Empty : CreationDate.ToString("g"); } }
        [OriginalGridPosition(Position = 10)]
        public string DeadlineDisplay { get { return Deadline == DateTime.MinValue ? string.Empty : Deadline.ToString("g"); } }
        [OriginalGridPosition(Position = 11)]
        public int EventCount { get; set; }
        [OriginalGridPosition(Position = 12)]
        public string WorkOrderState { get; set; }
        [OriginalGridPosition(Position = 13)]
        public string Technician { get; set; }
        [OriginalGridPosition(Position = 14)]
        public string AssignedDateDisplay { get { return AssignedDate.HasValue ? AssignedDate.Value.ToString("g") : string.Empty; } }
    }
}
