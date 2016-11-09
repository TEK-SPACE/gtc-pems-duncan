using System;

namespace Duncan.PEMS.Entities.WorkOrders.Base
{
  /// <summary>
  /// Base work order list item definition
  /// </summary>
  public abstract  class BaseWorkOrderListItem
    {
        /// <summary>
        /// Id of the work order
        /// </summary>
        public long WorkOrderId { get; set; }
        /// <summary>
        /// Asset that the work order is assigned to
        /// </summary>
        public WorkOrderAsset WorkOrderAsset { get; set; }

        /// <summary>
        /// keep this property for sorting purposes, not settable though.
        /// </summary>
        public string AssetType { get { return WorkOrderAsset.AssetType; } }

        public string AssetName { get { return WorkOrderAsset.AssetName; } }
        public string AssetId { get { return WorkOrderAsset.AssetId.ToString(); } }

        public string AreaName { get { return WorkOrderAsset.AssetAreaName; } }
        public string AreaId { get { return WorkOrderAsset.AreaId.ToString(); } }
        /// <summary>
        /// Display version for localization
        /// </summary>
        public string AssetLastPMDateDisplay
        {
            get { return WorkOrderAsset.AssetLastPMDate.HasValue ? WorkOrderAsset.AssetLastPMDate.Value.ToString("g") : string.Empty; }
        }

        /// <summary>
        /// Location of the work order asset
        /// </summary>
        public string Location { get; set; }
      /// <summary>
      /// SLA Due value for the work order
      /// </summary>
        public DateTime Deadline { get; set; }

      /// <summary>
      /// Display version for localization
      /// </summary>
        public string DeadlineDisplay
        {
            get { return Deadline == DateTime.MinValue ? string.Empty : Deadline.ToString("g"); }
        }

      /// <summary>
      /// Time the work order was created
      /// </summary>
        public DateTime? NotificationTime { get; set; }

      /// <summary>
      /// Display version for localization
      /// </summary>
        public string NotificationTimeDisplay
        {
            get { return NotificationTime.HasValue ? NotificationTime.Value.ToString("g") : string.Empty; }
        }
    }
}
