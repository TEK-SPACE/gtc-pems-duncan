
using System.Collections.Generic;


namespace Duncan.PEMS.Entities.WorkOrders.Dispatcher
{
   public class DispatcherBulkUpdateModel
    {

       public DispatcherBulkUpdateModel()
       {
           AvailableTechnicians = new List<Technicians.Technician>();
       }
       public string[] WorkOrderIds { get; set; }
       public int SelectedTechnicianId { get; set; }
       public List<Technicians.Technician> AvailableTechnicians { get; set; }
    }
  
}
