using System;
namespace Duncan.PEMS.Entities.WebServices.FieldMaintenance
{
    public class CreateAlarmRequest
    {
        public long WorkOrderId { get; set; }
        public int AssetId { get; set; }
        public int AreaId { get; set; }
        public int CustomerId { get; set; }

        public int EventCode { get; set; }
        public int EventSource { get; set; }
        public string Notes { get; set; }
        public DateTime LocalTime { get; set; }

    }
}
