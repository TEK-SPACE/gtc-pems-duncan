using System;

namespace Duncan.PEMS.Entities.MaintenanceGroups
{
    public class ListMaintenanceGroupModel
    {
        public string Name { get; set; }
        public string InternalName { get; set; }
        public string DisplayName { get; set; }
        public int Id { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnDisplay { get { return CreatedOn == DateTime.MinValue ? string.Empty : CreatedOn.ToString("d"); } }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedOnDisplay { get { return UpdatedOn == DateTime.MinValue ? string.Empty : UpdatedOn.ToString("d"); } }
        public string PemsConnectionStringName { get; set; }
        public string MaintenanceConnectionStringName { get; set; }
        public string ReportingConnectionStringName { get; set; }
    }
}
