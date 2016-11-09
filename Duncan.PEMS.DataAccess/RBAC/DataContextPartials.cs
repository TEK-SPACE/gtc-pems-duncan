
namespace Duncan.PEMS.DataAccess.RBAC
{
        public partial class MaintenanceEntities
        {
            //additional constructor used for forcing conneciton string setting for all factories. 
            public MaintenanceEntities(string connString)
                : base(connString)
            {
            }
        }
}
