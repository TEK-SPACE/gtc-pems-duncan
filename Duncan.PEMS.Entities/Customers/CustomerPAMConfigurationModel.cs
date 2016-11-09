using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerPAMConfigurationModel : CustomerBaseModel
    {
        public PAMConfigurationModel PAMActiveCust { get; set; }

        public List<PAMGracePeriodModel> PAMCluster { get; set; }
    }
}
