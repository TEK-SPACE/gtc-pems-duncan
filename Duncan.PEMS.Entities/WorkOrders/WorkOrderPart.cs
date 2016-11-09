using System;

namespace Duncan.PEMS.Entities.WorkOrders
{
   public  class WorkOrderPart
    {
        public long WorkOrderPartId { get; set; }
        public long WorkOrderId { get; set; }
        public long PartId { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public string PartDesc { get; set; }
        public string PartName { get; set; }
        public int CostInCents { get; set; }
       public string CostInCentsDisplay
       {
           get
           {
               //take the cents, convert it to dollars, then format it to currency
               return Math.Floor((decimal)CostInCents / 100).ToString("C");
           }
       }
    }
}
