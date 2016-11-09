using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Entities.Users
{
    class NoCommunication
    {
        public string CustomerID { get; set; }
        public int AssetID { get; set; }
        public int MyProperty { get; set; }
        public string AssetName { get; set; }
        public string Street { get; set; }

        public List<NoCommunication> noCommunications { get; set; }
    }
}
