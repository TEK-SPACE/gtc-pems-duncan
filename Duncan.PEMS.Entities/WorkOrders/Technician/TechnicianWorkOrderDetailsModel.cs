using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Duncan.PEMS.Entities.WorkOrders.Base;

#pragma warning disable 1591

namespace Duncan.PEMS.Entities.WorkOrders.Technician
{
    public class TechnicianWorkOrderDetailsModel : BaseWorkOrderDetailsModel
    {
        public TechnicianWorkOrderDetailsModel()
        {
            WorkOrderEvents = new List<WorkOrderEvent>();
            WorkOrderImages = new List<WorkOrderImage>();
            WorkOrderAsset = new WorkOrderAsset();
            RepairDescriptions = new List<SelectListItem>();
            AvailableParts = new List<AvailablePart>();
            WorkOrderParts = new List<WorkOrderPart>();
        }

        public static string PartIdPrefix = "partId";
        public char Separator = '_';
    }
}
