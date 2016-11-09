using System.Collections.Generic;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerDemandZonesModel : CustomerBaseModel
    {
        // Demand Zones
        public List<CustomerDemandZone> DemandZones { get; set; }
    }
}
