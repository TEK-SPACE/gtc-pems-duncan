namespace Duncan.PEMS.DataAccess.OracleDB
{
    /// <summary>
    /// 
    /// </summary>
    public class OracleDbEntitiesAbstract : OracleDBEntities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        public OracleDbEntitiesAbstract(string connectionName)
            : base(connectionName)
        {
        }
    }
}
