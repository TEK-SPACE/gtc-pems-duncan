using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Entities.Customers
{
   
    public  class PAMConfigurationModel 
    {
        public int CustomerID { get; set; }
        public Nullable<bool> CustomerIDPAM { get; set; }
        public Nullable<bool> ResetImin { get; set; }
        public Nullable<bool> ExpTimeByPAM { get; set; }

    }

    public class PAMGracePeriodModel
    {
        public int CustomerID { get; set; }
        public int Clusterid { get; set; }

        public int GracePeriod { get; set; }


    }

    public  class PAMClusters
    {
        public int Clusterid { get; set; }
        public int Customerid { get; set; }
        public int MeterId { get; set; }
        public int Hostedbaystart { get; set; }
        public int Hostedbayend { get; set; }
        public string Description { get; set; }

     }
}
