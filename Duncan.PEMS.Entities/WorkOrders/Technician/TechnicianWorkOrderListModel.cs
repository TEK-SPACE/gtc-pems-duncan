using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.WorkOrders.Base;

#pragma warning disable 1591

namespace Duncan.PEMS.Entities.WorkOrders.Technician
{
    public class TechnicianWorkOrderListModel
    {

        public TechnicianWorkOrderListModel()
        {
            AssetIdOptions = new List<AssetIdentifier>();
        }
        public string SelectedAssetKey { get; set; }
        public List<TechnicianWorkOrderListItem> WorkOrders { get; set; }
        public string SelectedSort { get; set; }
        public string SelectedDirection { get; set; }
        public IEnumerable<SelectListItem> SortOptions { get; set; }
        /// <summary>
        /// List of all possible assets for an event - only used for PM events
        /// </summary>
        public List<AssetIdentifier> AssetIdOptions { get; set; }
    }

    public class TechnicianWorkOrderListItem : BaseWorkOrderListItem
    {
        public TechnicianWorkOrderListItem()
        {
            WorkOrderAsset = new WorkOrderAsset();
        }
        public string Class { get; set; }
    }
}
