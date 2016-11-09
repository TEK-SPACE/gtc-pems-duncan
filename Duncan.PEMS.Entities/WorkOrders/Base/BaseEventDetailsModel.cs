using System.Collections.Generic;
namespace Duncan.PEMS.Entities.WorkOrders.Base
{
    /// <summary>
    /// Base model for work order events
    /// </summary>
    public abstract class BaseEventDetailsModel
    {
        /// <summary>
        /// Source of the Event
        /// </summary>
        public string Source { get; set; }
        
        /// <summary>
        /// Asset assigned to the event
        /// </summary>
        public WorkOrderAsset WorkOrderAsset { get; set; }
        
        /// <summary>
        /// Event Data
        /// </summary>
        public WorkOrderEvent WorkOrderEvent { get; set; }
       
        /// <summary>
        /// Images for the wokr order for this event
        /// </summary>
        public List<WorkOrderImage> WorkOrderImages { get; set; }
    }
}
