using System.Collections.Generic;
using System.Web.Mvc;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.WorkOrders.Base;

namespace Duncan.PEMS.Entities.WorkOrders.Dispatcher
{
    /// <summary>
    /// Represents a create event model for the dispatch screens
    /// </summary>
    public class DispatcherCreateEventModel : BaseCreateEventModel
    {
        public DispatcherCreateEventModel()
        {
            AssetIdOptions = new List<AssetIdentifier>();
            FaultDescriptions = new List<SelectListItem>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Registration { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CrossStreet { get; set; }
    }
}
