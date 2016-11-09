using System;

namespace Duncan.PEMS.Entities.WebServices.FieldMaintenance
{
    public class CloseAlarmResponse : BaseAlarmResponse
    {
        public int AssetId { get; set; }
        public int AreaId { get; set; }
        public int CustomerId { get; set; }
        public int EventUID { get; set; }
        public int EventCode { get; set; }
        public DateTime LocalTime { get; set; }
        public bool Closed { get; set; }
    }
}
