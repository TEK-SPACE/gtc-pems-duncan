using System;

namespace Duncan.PEMS.Entities.WebServices.FieldMaintenance
{
   public class CloseAlarmRequest
    {
        public int AssetId { get; set; }
        public int AreaId { get; set; }
        public int CustomerId { get; set; }
        public long EventUID { get; set; }
        public int EventCode { get; set; }
        public DateTime LocalTime { get; set; }

    }
}
