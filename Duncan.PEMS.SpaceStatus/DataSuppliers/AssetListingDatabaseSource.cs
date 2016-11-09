using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


using Duncan.PEMS.SpaceStatus.Models;
using Duncan.PEMS.SpaceStatus.DataShapes;
using Duncan.PEMS.SpaceStatus.DataSuppliers;
using Duncan.PEMS.SpaceStatus.UtilityClasses;

namespace Duncan.PEMS.SpaceStatus.DataSuppliers
{

    public class AssetListingDatabaseSource
    {
        private CustomerConfig _CustomerCfg;
        private CustomerLogic _Customer;

        public AssetListingDatabaseSource(CustomerConfig customerCfg)
        {
            _CustomerCfg = customerCfg;
            _Customer = CustomerLogic.CustomerManager.GetLogic(_CustomerCfg);
        }

        public List<AssetListingInformation> GetAssetListingData_StronglyTyped(int CustomerId, List<int> listOfMeterIDs, bool SortAscending)
        {
            List<AssetListingInformation> result = null;
            if (listOfMeterIDs.Count == 0)
                return result;

            // Since SQL Server will choke if we try to include too many parameters, we will query for upto 500 meters at a time.
            // If we are needing data for more than 500 meters, then we will end up executing multiple queries to the database
            // and combining the results

            // Use a DTO mapper to deserialize from database to object list
            int nextIdxIntoListOfMeters = 0;
            while (nextIdxIntoListOfMeters < listOfMeterIDs.Count)
            {
                string loMeterDelim = string.Empty;
                int batchParamCount = 0;
                StringBuilder sql = new StringBuilder();
                using (SqlCommand command = _Customer.SharedSqlDao.GetSqlCommand(string.Empty))
                {
                    sql.Append(
                              "select m.MeterID, ps.BayNumber, " +
                              " (case when mm.AreaID2 is null then m.AreaID else mm.AreaID2 end) as 'AreaID'," +
                              " m.Location, m.latitude, m.longitude" +
                              " from parkingspaces as ps" +
                              " left join meters as m on ps.meterid=m.meterid and m.customerid=ps.customerid and m.areaid=ps.areaid" +
                              " left join metermap as mm on ps.meterid=mm.meterid and mm.customerid=m.customerid and mm.areaid=m.areaid" +
                              " where m.CustomerId = @CustomerId" +
                              " and m.MeterId in (");

                    for (int loIdx = nextIdxIntoListOfMeters; loIdx < listOfMeterIDs.Count; loIdx++)
                    {
                        sql.Append(loMeterDelim);
                        sql.Append("@MeterId_" + loIdx.ToString());
                        if (string.IsNullOrEmpty(loMeterDelim))
                            loMeterDelim = ", ";

                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@MeterId_" + loIdx.ToString(), listOfMeterIDs[loIdx]));

                        nextIdxIntoListOfMeters++;
                        batchParamCount++;

                        // Don't want to have more than 500 parameters in a SQL query.  If there are a lot of meters, we may need to run multiple queries and
                        // combine the result
                        if (batchParamCount >= 500)
                        {
                            break;
                        }
                    }
                    sql.Append(") order by 1, 2");

                    command.CommandText = sql.ToString();
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));

                    // Get list of objects from the SQL query
                    List<AssetListingInformation> partialResult = _Customer.SharedSqlDao.GetList<AssetListingInformation>(command);

                    // Create final result object if needed
                    if (result == null)
                        result = new List<AssetListingInformation>();

                    // Append the partial results to the full results
                    if ((partialResult != null) && (partialResult.Count > 0))
                    {
                        result.AddRange(partialResult);
                    }
                }
            }

            // Return the final result, which may have been comprised of concatenated results from multiple DB queries
            return result;
        }
    }
}