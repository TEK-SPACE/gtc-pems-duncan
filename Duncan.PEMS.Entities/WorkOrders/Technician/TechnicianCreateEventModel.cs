
using System.Collections.Generic;
using System.Web.Mvc;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.WorkOrders.Base;

namespace Duncan.PEMS.Entities.WorkOrders.Technician
{
   public class TechnicianCreateEventModel : BaseCreateEventModel   
    {
       public TechnicianCreateEventModel()
       {
           WorkOrderImages = new List<WorkOrderImage>();
           AssetIdOptions = new List<AssetIdentifier>();
           FaultDescriptions = new List<SelectListItem>();
       }
        //list of images for this work order
        public List<WorkOrderImage> WorkOrderImages { get; set; }
    }
}
