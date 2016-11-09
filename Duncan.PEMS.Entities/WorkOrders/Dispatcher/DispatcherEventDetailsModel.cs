using System;
using System.Collections.Generic;
using Duncan.PEMS.Entities.WorkOrders.Base;

namespace Duncan.PEMS.Entities.WorkOrders.Dispatcher
{
    public class DispatcherEventDetailsModel: BaseEventDetailsModel
    {
        public DispatcherEventDetailsModel()
        {
            WorkOrderAsset = new WorkOrderAsset();
            WorkOrderEvent = new WorkOrderEvent();
            WorkOrderImages = new List<WorkOrderImage>();
        }

        //closeure, repair info
        //repair code
        public int? RepairCode { get; set; }
        public string RepairDescription { get; set; }
        //vandalism
        public bool? IsVandalism { get; set; }
        public string VandalismDisplay {get { return IsVandalism.GetValueOrDefault() ? "Y" : "N"; }}
        //closed by
        public string ClosedBy { get; set; }
        //time cleared
        public DateTime? TimeCleared { get; set; }
        public string TimeClearedDisplay { get { return TimeCleared.HasValue ? TimeCleared.Value.ToString() : string.Empty; } }
        //closure notes
        public string ClosureNotes { get; set; }
    }

}

