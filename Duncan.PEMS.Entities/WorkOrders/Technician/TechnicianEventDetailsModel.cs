using System.Collections.Generic;
using Duncan.PEMS.Entities.WorkOrders.Base;

#pragma warning disable 1591

namespace Duncan.PEMS.Entities.WorkOrders.Technician
{
    public class TechnicianEventDetailsModel : BaseEventDetailsModel
    {
        public TechnicianEventDetailsModel()
        {
            WorkOrderAsset = new WorkOrderAsset();
            WorkOrderEvent = new WorkOrderEvent();
            WorkOrderImages = new List<WorkOrderImage>();
        }
    }
}
