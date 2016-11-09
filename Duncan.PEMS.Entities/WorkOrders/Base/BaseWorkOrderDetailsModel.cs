

using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.WorkOrders.Base
{
    public abstract class BaseWorkOrderDetailsModel
    {
        public string Notes { get; set; }
        public DateTime Deadline { get; set; }
        public string DeadlineDisplay
        {
            get { return Deadline == DateTime.MinValue ? string.Empty : Deadline.ToString("g"); }
        }
        public string Location { get; set; }
        public long WorkOrderId { get; set; }

        public int AreaId { get; set; }
        public string AreaId2Display { get; set; }

        public WorkOrderAsset WorkOrderAsset { get; set; }
        public int Priority { get; set; }
        public string PriorityDisplay { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        //flag to determine if they need ot close all events for work order at once, or close individual events out
        public bool CloseWorkOrderEvents { get; set; }

        //repair descriptions
        public List<SelectListItem> RepairDescriptions { get; set; }
        public string SelectedRepairDescription { get; set; }

        //possible parts
        public List<AvailablePart> AvailableParts { get; set; }
        public string SelectedPart { get; set; }

        public string PartNote { get; set; }

        public DateTime CreationDate { get; set; }
        public string CreationDateDisplay
        {
            get { return CreationDate == DateTime.MinValue ? string.Empty : CreationDate.ToString("g"); }
        }

        public string CrossStreet { get; set; }
        public string Zone { get; set; }

        public string PastWorkOrders { get; set; }

        //list of images
        public List<WorkOrderImage> WorkOrderImages { get; set; }

        //list of events
        public List<WorkOrderEvent> WorkOrderEvents { get; set; }

        //lsit of parts
        public List<WorkOrderPart> WorkOrderParts { get; set; }

     
    }
}
