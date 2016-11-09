
namespace Duncan.PEMS.Entities.Technicians
{
   public  class Technician
    {
        public string TechnicianContact { get; set; }
        public int? TechnicianKeyID { get; set; }
        public int TechnicianID { get; set; }
        public string TechnicianName { get; set; }
        public string TechnicianDisplay
        {
            get
            {
                if (TechnicianID > 0)
                    return TechnicianID.ToString() + ": " + TechnicianName;
                return "";
            }
        }
        public int TechnicianAssignedWorkOrderCount { get; set; }

       /// <summary>
       /// Number of completed work orders for today
       /// </summary>
       public int TechnicianCompletedWorkOrderCount { get; set; }

        public int CustomerId { get; set; }
    }
}
