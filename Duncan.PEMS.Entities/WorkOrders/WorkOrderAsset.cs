using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Entities.WorkOrders
{
    public class WorkOrderAsset
    {
        public int AssetId { get; set; }
        public int AreaId { get; set; }
        public string AssetType { get; set; }
        public string AssetSubType { get; set; }
        public string AssetName { get; set; }
        public string AssetAreaName { get; set; }
        public DateTime? AssetLastPMDate { get; set; }
    }
}
