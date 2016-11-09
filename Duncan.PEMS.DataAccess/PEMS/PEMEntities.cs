namespace Duncan.PEMS.DataAccess.PEMS
{
    public partial class PEMEntities
    {
        //additional constructor used for forcing conneciton string setting for all factories. 
        public PEMEntities(string connString)
            : base(connString)
        {
        }
    }
}
