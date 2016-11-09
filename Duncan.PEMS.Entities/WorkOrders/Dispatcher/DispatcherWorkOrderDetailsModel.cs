using System.Collections.Generic;
using System.Web.Mvc;
using Duncan.PEMS.Entities.WorkOrders.Base;

namespace Duncan.PEMS.Entities.WorkOrders.Dispatcher
{
    /// <summary>
    /// Work order details model for the deispatcher screens
    /// </summary>
    public class DispatcherWorkOrderDetailsModel : BaseWorkOrderDetailsModel
    {
        public DispatcherWorkOrderDetailsModel()
       {
           WorkOrderEvents = new List<WorkOrderEvent>();
           WorkOrderImages = new List<WorkOrderImage>();
           WorkOrderAsset = new WorkOrderAsset();
           RepairDescriptions = new List<SelectListItem>();
           AvailableParts = new List<AvailablePart>();
           WorkOrderParts = new List<WorkOrderPart>();
           Technician = new Technicians.Technician();
           AvailableTechnicians = new List<Technicians.Technician>();
       }

        public int SelectedTechnicianId { get; set; }
        public Technicians.Technician Technician { get; set; }
        public List<Technicians.Technician> AvailableTechnicians { get; set; }

        public static string PartIdPrefix = "partId";
        public char Separator = '_';

    }
}
