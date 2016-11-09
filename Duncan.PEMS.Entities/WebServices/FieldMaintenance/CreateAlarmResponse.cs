using System;
namespace Duncan.PEMS.Entities.WebServices.FieldMaintenance
{
    public class CreateAlarmResponse : BaseAlarmResponse
    {
        public int EventUID { get; set; }
        public DateTime SlaDue { get; set; }

    }
}
