using System.Collections.Generic;
using System.Web.Mvc;
using Duncan.PEMS.Entities.Assets;

namespace Duncan.PEMS.Entities.WorkOrders.Base
{
    /// <summary>
    /// Base class for work order create event models.
    /// </summary>
    public abstract class BaseCreateEventModel
    {
        /// <summary>
        /// Selected asset | area for this event
        /// </summary>
        public string SelectedAssetKey { get; set; }
        /// <summary>
        /// List of all possible assets for an event
        /// </summary>
        public List<AssetIdentifier> AssetIdOptions { get; set; }
        /// <summary>
        /// List of all possible faults for a work order event
        /// </summary>
        public List<SelectListItem> FaultDescriptions { get; set; }
        /// <summary>
        /// The selected event code (fault description) that represents the issue
        /// </summary>
        public string SelectedFaultDescription { get; set; }
        /// <summary>
        /// Notes for this specific work order event
        /// </summary>
        public string Notes { get; set; }
    }
}
