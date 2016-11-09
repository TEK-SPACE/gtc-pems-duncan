namespace Duncan.PEMS.Entities.WorkOrders.Dispatcher
{
   /// <summary>
   /// Model for the work load (dispatcher screens)
   /// </summary>
   public  class DispatcherWorkLoadListModel
    {
       /// <summary>
       /// First Name
       /// </summary>
       public string FirstName { get; set; }
       
       /// <summary>
       /// Last Name
       /// </summary>
       public string LastName { get; set; }

       /// <summary>
       /// Technician Id
       /// </summary>
       public int TechnicianId { get; set; }

       /// <summary>
       /// AssignedCount
       /// </summary>
       public int AssignedCount { get; set; }

       /// <summary>
       /// CompletedCount (for today)
       /// </summary>
       public int CompletedCount { get; set; }
    }
}
