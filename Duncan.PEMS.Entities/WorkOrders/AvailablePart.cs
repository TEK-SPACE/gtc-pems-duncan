
using System;

namespace Duncan.PEMS.Entities.WorkOrders
{
    public class AvailablePart
    {
        public long PartId { get; set; }
        public string PartName { get; set; }
        public int MeterGroup { get; set; }
        public string MeterGroupDisplay { get; set; }
        public int Category { get; set; }
        public string CategoryDisplay { get; set; }
        public string PartDesc { get; set; }
        public int CostInCents { get; set; }
        public string CostInCentsDisplay
        {
            get
            {
                //take the cents, convert it to dollars, then format it to currency
                return ((decimal)CostInCents / 100).ToString("C");
            }
        }

        public int Status { get; set; }
        public string StatusDisplay { get; set; }
        public string PartIdentifier { get { return PartId + "|" + PartName; } }

    }
}
