using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;


using Duncan.PEMS.SpaceStatus.Models;
using Duncan.PEMS.SpaceStatus.DataShapes;
using Duncan.PEMS.SpaceStatus.DataSuppliers;
using Duncan.PEMS.SpaceStatus.UtilityClasses;

namespace Duncan.PEMS.SpaceStatus.DataSuppliers
{

    public class PAMSpaceExpiryInformation
    {
        public int ClusterID { get; set; }
        public int PAMGraceMinutes { get; set; }

        public int MeterID { get; set; }
        public int Meter_imin { get; set; }
        public DateTime Meter_upTS { get; set; }

        public int BayID { get; set; }
        public ExpiryState BayExpiryState { get; set; }
        public TimeSpan BayExpiration_AsTimeSpan { get; set; }
        public DateTime BayExpiration_AsDateTime { get; set; }

        public PAMSpaceExpiryInformation()
        {
        }
    }

    /// <summary>
    /// Gets XML data from the PAM server
    /// </summary>
    public class PaymentXMLSource
    {
        private string _ApplicationPath;
        private CustomerConfig _CustomerConfig = null;
        private CustomerLogic _Customer = null;

        // Variables used for PAM TCP/UDP support
        private Socket UDPSocket = null;
        private EndPoint UDPServerEndPoint = null;
        private bool UseBigEndian = false;
        private string _PAMServerTCPAddress = "";
        private int _PAMServerTCPPort = 0;

        public PaymentXMLSource(string ApplicationPath, CustomerConfig customerCfg)
        {
            _CustomerConfig = customerCfg;
            _Customer = CustomerLogic.CustomerManager.GetLogic(customerCfg);
            _ApplicationPath = ApplicationPath;
        }

        private void PerformAsciiSortOnTable(DataTable tableToSort, int SortColumnIdx)
        {
            List<DataRow> SortedDataRows = new List<DataRow>();
            foreach (DataRow NextDataRow in tableToSort.Rows)
            {
                SortedDataRows.Add(NextDataRow);
            }
            // Now sort our array of DataRow info using custom sorter logic
            DataRowLogicalComparer sorter = new DataRowLogicalComparer(SortColumnIdx);
            SortedDataRows.Sort(sorter);

            // Clone the table structure and populate new table with the sorted data
            DataTable newDT = tableToSort.Clone();
            int rowCount = tableToSort.Rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                object[] arr = new object[tableToSort.Columns.Count];
                for (int j = 0; j < tableToSort.Columns.Count; j++)
                {
                    arr[j] = SortedDataRows[i][j];
                }
                DataRow data_row = newDT.NewRow();
                data_row.ItemArray = arr;
                newDT.Rows.Add(data_row);
            }

            // Now clear the tableToSort and repopulate with sorted data from cloned table
            tableToSort.Rows.Clear();
            for (int i = 0; i < newDT.Rows.Count; i++)
            {
                object[] arr = new object[tableToSort.Columns.Count];
                for (int j = 0; j < tableToSort.Columns.Count; j++)
                {
                    arr[j] = newDT.Rows[i][j];
                }

                DataRow data_row = tableToSort.NewRow();
                data_row.ItemArray = arr;
                tableToSort.Rows.Add(data_row);
            }
        }

        private DataRow[] GetMatchingRecords(DataTable tableToSearch, DataColumn searchColumn, string valueToMatch)
        {
            // The standard DataTable.Select might not be reliable enough for us? 
            // (In the past, have observed that it missed records that appeared to qualify, so I lost some trust in this functionality...)
            /*return tableToSearch.Select(searchColumn.ColumnName + "  = '" + searchValue + "'");*/

            // Perform a search on each row to see which records match our search criteria. 
            // And just to be extra cautious, we will trim leading/trailing whitespace that might be present
            valueToMatch = valueToMatch.Trim();
            List<DataRow> searchResults = new List<DataRow>();
            foreach (DataRow searchrow in tableToSearch.Rows)
            {
                if (string.Compare(searchrow[searchColumn].ToString().Trim(), valueToMatch) == 0)
                {
                    searchResults.Add(searchrow);
                }
            }
            return searchResults.ToArray();
        }

        public List<PAMSpaceExpiryInformation> GetPAMStatusForMeters_StronglyTyped(int CustomerId, List<int> listOfMeterIDs)
        {
            List<PAMSpaceExpiryInformation> result = new List<PAMSpaceExpiryInformation>();

            // Get payment dataset from PAM service
            DataSet dsPayments = GetPAMStatusForMeters(CustomerId, listOfMeterIDs);

            // Can't do any processing if there is nothing in the dataset
            if ((dsPayments == null) || (dsPayments.Tables.Count == 0))
            {
                return result;
            }

            // We will be converting the dataset to a strongly typed collection. To optimize this,
            // we will first resolve references to tables and columns that we will access multiple times
            DataTable tblCluster = dsPayments.Tables["Cluster"];
            DataColumn col_tblCluster_Cluster_Id = tblCluster.Columns["Cluster_Id"];
            DataColumn col_tblCluster_ID = tblCluster.Columns["ID"];
            DataColumn col_tblCluster_Grace = tblCluster.Columns["grace"];

            DataTable tblMeter = dsPayments.Tables["Meter"];
            DataColumn col_tblMeter_Cluster_Id = tblMeter.Columns["Cluster_Id"];
            DataColumn col_tblMeter_Meter_Id = tblMeter.Columns["Meter_Id"];
            DataColumn col_tblMeter_ID = tblMeter.Columns["ID"];
            DataColumn col_tblMeter_IMin = tblMeter.Columns["imin"];

            DataTable tblBay = dsPayments.Tables["Bay"];
            DataColumn col_tblBay_Meter_Id = tblBay.Columns["Meter_Id"];
            DataColumn col_tblBay_ID = tblBay.Columns["ID"];
            DataColumn col_tblBay_Expt = tblBay.Columns["expt"];

            // Loop through each cluster
            foreach (DataRow clusterRow in tblCluster.Rows)
            {
                // Loop through each meter of current cluster
                DataRow[] metersInCluster = GetMatchingRecords(tblMeter, col_tblMeter_Cluster_Id, clusterRow[col_tblCluster_Cluster_Id].ToString());
                foreach (DataRow meterRow in metersInCluster)
                {
                    // Loop through each bay of current meter
                    DataRow[] baysInMeter = GetMatchingRecords(tblBay, col_tblBay_Meter_Id, meterRow[col_tblMeter_Meter_Id].ToString());
                    foreach (DataRow bayRow in baysInMeter)
                    {
                        // Create new info object and add to result collection
                        PAMSpaceExpiryInformation infoData = new PAMSpaceExpiryInformation();
                        result.Add(infoData);

                        // Populate the Cluster ID from Cluster table
                        infoData.ClusterID = Convert.ToInt32(clusterRow[col_tblCluster_ID]);

                        // Populate the Grace from Cluster table -- but beware that it doesn't necessarily exist
                        double ExpiryTimeGraceInMinutes = 0;
                        if (col_tblCluster_Grace != null)
                            double.TryParse(clusterRow[col_tblCluster_Grace].ToString(), out ExpiryTimeGraceInMinutes);
                        infoData.PAMGraceMinutes = Convert.ToInt32(ExpiryTimeGraceInMinutes);

                        // Populate Meter ID and idle minutes from Meter table
                        infoData.MeterID = Convert.ToInt32(meterRow[col_tblMeter_ID]);
                        infoData.Meter_imin = Convert.ToInt32(meterRow[col_tblMeter_IMin]);

                        // Convert current time at the server to current time in the destination timezone
                        DateTime NowAtDestination = Convert.ToDateTime(this._CustomerConfig.DestinationTimeZoneDisplayName);// Duncan.PEMS.SpaceStatus.UtilityClasses.TimeZoneInfo.ConvertTimeZoneToTimeZone(DateTime.Now, _CustomerConfig.ServerTimeZone, _CustomerConfig.CustomerTimeZone);

                        // Populate last communication time for meter based on info in Meter table. Note however that
                        // PAM returns a garbage value for upTS. Instead, we will calculate the last communication time
                        // by subtracting the idle minutes from the current time (at customers timezone, not the server's timezone!)
                        infoData.Meter_upTS = NowAtDestination.AddMinutes((-1) * (infoData.Meter_imin));

                        // Populate Bay ID from Bay table
                        infoData.BayID = Convert.ToInt32(bayRow[col_tblBay_ID]);

                        // Populate bay expiration information based on data in the Bay table

                        // Irrespective of what the XML Server document says:
                        // expt is actually in terms of seconds since Jan 1st 2000 12:00AM
                        DateTime Ref = new DateTime(2000, 1, 1); //use beginning of 2000 as reference for ExpiryTime
                        TimeSpan TimeSinceRef = NowAtDestination - Ref;
                        double MeterExpiryTimeInSeconds = Convert.ToDouble(bayRow[col_tblBay_Expt]);
                        infoData.BayExpiration_AsTimeSpan = TimeSpan.FromSeconds(MeterExpiryTimeInSeconds - TimeSinceRef.TotalSeconds);

                        // Expired seconds is in units of seconds since 1/1/2000 at 00:00:00
                        // this value is expressed against the meter's local time
                        infoData.BayExpiration_AsDateTime = new DateTime(2000, 1, 1, 0, 0, 0).AddSeconds(MeterExpiryTimeInSeconds);

                        // Determine the bay expiration status, based on expiration timetasmp and grace period configurations, etc.

                        TimeSpan MeterExpiryGrace = new TimeSpan();
                        // Is grace period enabled in configuration, and also has a non-zero value from PAM?
                        if ((_CustomerConfig.GracePeriodEnabled) && (ExpiryTimeGraceInMinutes > 0))
                        {
                            // MeterExpiryGrace calculate current space grace expiry time available from PAMWebService.
                            double ExpiryTimeGraceInSeconds = ExpiryTimeGraceInMinutes * 60; //PAM returns in minutes. convert to seconds
                            ExpiryTimeGraceInSeconds = ExpiryTimeGraceInSeconds + MeterExpiryTimeInSeconds;
                            MeterExpiryGrace = TimeSpan.FromSeconds(ExpiryTimeGraceInSeconds - TimeSinceRef.TotalSeconds);
                        }

                        int ExpiryTimeToBeCriticalInSeconds = _CustomerConfig.ExpiryTimeToBeCriticalInSeconds;
                        // When the expt reads 0, it means that the meter is inoperational
                        if (MeterExpiryTimeInSeconds != 0)
                        {
                            /*BayRow["ExpiryTimeString"] = SpaceStatus.FormatShortTimeSpan(MeterExpiryTime, _CustomerConfig);*/
                            infoData.BayExpiryState = ExpiryState.Expired;
                            // if data is valid then go ahread and see what formatting to use:
                            if ((_CustomerConfig.GracePeriodEnabled) && (ExpiryTimeGraceInMinutes > 0))
                            {
                                // Grace period is enabled
                                if (MeterExpiryGrace.TotalSeconds > 0)
                                {
                                    // Expiraton is within the grace period
                                    infoData.BayExpiryState = ExpiryState.Grace;
                                    TimeSpan tsGraceLeft = new TimeSpan(MeterExpiryGrace.Ticks);
                                    /*BayRow["ExpiryTimeString"] = SpaceStatus.FormatShortTimeSpan(tsGraceLeft, _CustomerConfig);*/
                                }
                            }
                            else if (infoData.BayExpiration_AsTimeSpan.TotalSeconds < 0)
                            {
                                infoData.BayExpiryState = ExpiryState.Expired;
                            }
                            else if (infoData.BayExpiration_AsTimeSpan.TotalSeconds <= ExpiryTimeToBeCriticalInSeconds)
                                infoData.BayExpiryState = ExpiryState.Critical;
                            else if (infoData.BayExpiration_AsTimeSpan.TotalSeconds > 0)
                                infoData.BayExpiryState = ExpiryState.Safe;
                        }
                        else
                        {
                            // When the expt reads 0, it means that the meter is inoperational
                            // If data is invalid just mark as invalid.
                            /*BayRow["ExpiryTimeString"] = _CustomerConfig.InvalidBayString;*/
                            infoData.BayExpiryState = ExpiryState.Inoperational;
                        }
                    }
                }
            }

            // Finished with the dataset, so now we can release its resources
            dsPayments.Dispose();
            dsPayments = null;

            return result;
        }

        public DataSet GetPAMStatusForMeters(int CustomerId, List<int> listOfMeterIDs)
        {
            // DEBUG: We will need to have a mapping of MeterID-to-ClusterID relationships so we know the distinct clusters we need to query.  
            //        i.e. Given 5 meters, they could span anywhere from 1 to 5 different PAM clusters...
            //  but for now, since we don't have this relationship yet, we will query for each meter...
            bool queryEntireCustomer = false;
            List<int> oneMeterForEachDistinctCluster = new List<int>();
            List<int> distinctPAMCLusterIDs = new List<int>();
            foreach (int nextMeterID in listOfMeterIDs)
            {
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(_CustomerConfig))
                {
                    // Skip if its not the asset we're looking for
                    if (asset.MeterID != nextMeterID)
                        continue;

                    // We have to ignore it if there is no PAM Cluster ID (meaning it doesn't exist in PAM)
                    if (asset.PAMClusterID >= 0)
                    {
                        if (distinctPAMCLusterIDs.IndexOf(asset.PAMClusterID) == -1)
                        {
                            distinctPAMCLusterIDs.Add(asset.PAMClusterID);
                            oneMeterForEachDistinctCluster.Add(nextMeterID);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("There is no PAM cluster assigned for meter: " + nextMeterID.ToString());
                    }
                }
            }

            // If we are needing to query on more than 3 clusters, we might as well just make one query to PAM and get all data for entire customer at once,
            // which should end up being faster than multiple queries for less data...
            if (distinctPAMCLusterIDs.Count > 2)
            {
                // Set flag to query on entire customer
                queryEntireCustomer = true;
                // Remove all meters from the list to query, then just add a single "dummy" Meter ID
                oneMeterForEachDistinctCluster.Clear();
                oneMeterForEachDistinctCluster.Add(0);
            }

            DataSet ReturnDataset = new DataSet();
            List<int> MetersInResult = new List<int>();

            // Init the primary key values for clusters and meters (We will override the automatic values, because we may be combining multiple datasets into one)
            int nextClusterPrimaryKey = 0;
            int nextMeterPrimaryKey = 0;

            DataSet partialResult = null;
            foreach (int oneMeterIdForNextCluster in oneMeterForEachDistinctCluster)
            {
                // DEBUG:  // This is just for testing to speed up things -- this needs to be removed!
                /*
                if ((oneMeterIdForNextCluster >= 2000) && (oneMeterIdForNextCluster <= 2100))
                    continue;
                */

                // Get a dataset of the PAM cluster status corresponding to the meter id
              partialResult = GetPAMClusterStatusByMeterId(CustomerId, oneMeterIdForNextCluster, queryEntireCustomer);

                // If we don't have any data to look at, there's nothing more we can do
                if ((partialResult == null) || (partialResult.Tables["CLUSTER"] == null) || (partialResult.Tables["CLUSTER"].Rows.Count == 0))
                    continue;

                // Update the primary key (Cluster_id) of the CLUSTER table. Because there are relations, child record should automatically update as needed
                partialResult.Tables["CLUSTER"].Rows[0]["Cluster_id"] = nextClusterPrimaryKey;

                // We don't want duplicates in our final dataset, so remove any METER records in the
                // partial result that correspond to a meter already in the final response.  Note that
                // the dataset has built-in relationships so the accompanying BAY records will automatically
                // be removed also
                DataTable compareTbl = partialResult.Tables["METER"];
                int compareTblColIdx = compareTbl.Columns.IndexOf("ID");
                int compareMeterID = -1;
                int LoopHi = compareTbl.Rows.Count - 1;
                /*
                for (int loIdx = LoopHi; loIdx >= 0; loIdx--)
                {
                    DataRow nextRow = compareTbl.Rows[loIdx];
                */
                // We need to start by reindexing the temporary primary keys to negative numbers so they don't interfere
                // with our new numbering...
                int tempNegativePK = -1;
                foreach (DataRow nextRow in compareTbl.Rows)
                {
                    // Update the temp primary key to the next negative value
                    nextRow["Meter_id"] = tempNegativePK;
                    // Decrement the next temp primary key
                    tempNegativePK--;
                }
                partialResult.AcceptChanges();

                foreach (DataRow nextRow in compareTbl.Rows)
                {
                    compareMeterID = Convert.ToInt32(nextRow[compareTblColIdx]);

                    // Now delete this row if the meter id is already in the final dataset, or if the 
                    // meter id is not in the list of desired meter ids
                    if ((MetersInResult.Contains(compareMeterID)) || (listOfMeterIDs.IndexOf(compareMeterID) == -1))
                    {
                        nextRow.Delete();
                    }
                    else
                    {
                        // This meter id is desired, so make sure it is included in our list of meters in the response
                        if (!MetersInResult.Contains(compareMeterID))
                            MetersInResult.Add(compareMeterID);

                        // Update the primary key (Meter_id) of the METER table. Because there are relations, child record should automatically update as needed
                        nextRow["Meter_id"] = nextMeterPrimaryKey;

                        // Increment the primary key for the next meter (because we might be combining multiple datasets into one)
                        nextMeterPrimaryKey++;
                    }
                }

                // Now we can merge our partial response into the final response
                partialResult.AcceptChanges();
                ReturnDataset.Merge(partialResult);
                ReturnDataset.AcceptChanges();

                // Now we don't need the partial dataset anymore
                partialResult.Dispose();
                partialResult = null;

                // Increment the primary key for the next cluster (because we might be combining multiple datasets into one)
                nextClusterPrimaryKey++;
            }

            // DEBUG: Do we really need to sort the dataset?  Maybe we should sort in the strongly typed collection this eventually gets turned into?
            // And lets make sure the METER table in the final dataset is sorted by MeterID. But before sorting, we have to disable constraints
            ReturnDataset.EnforceConstraints = false;

            // We can only do a sort if our desired table exists
            if ((partialResult == null) || (partialResult.Tables["METER"] == null) || (partialResult.Tables["METER"].Rows.Count == 0))
            {
                // Meter table doesn't exist or is empty, so no sorting necessary
            }
            else
            {
                PerformAsciiSortOnTable(ReturnDataset.Tables["METER"], ReturnDataset.Tables["METER"].Columns.IndexOf("ID"));
            }

            // Return the final result
            ReturnDataset.AcceptChanges();
            return ReturnDataset;
        }

        private DataSet GetPAMClusterStatusByMeterId(int CustomerId, int MeterId, bool iQueryEntireCustomer)
        {
            // We query PAM with one MeterID as parameter, and it returns data for all meters that are in the same associated ClusterID

            DataSet ReturnDataset = null;

            try
            {
                // 2008.06.25 jla - Testing only! PAM Dev is slow right now, so we'll use a fake response file for now...
                // If fake response file doesn't exist, then of course we will get it from the server as usual
                string path = _ApplicationPath + "\\sampleresponse_" + _CustomerConfig.CustomerNameForFiles + ".xml";
                if (System.IO.File.Exists(_ApplicationPath + "\\sampleresponse_" + _CustomerConfig.CustomerNameForFiles + ".xml"))
                {
                    ReturnDataset = new DataSet();
                    ReturnDataset.ReadXml(_ApplicationPath + "\\sampleresponse_" + _CustomerConfig.CustomerNameForFiles + ".xml");
                }
                else
                {
                    // Obtain data via HTTP request if the customer is not configured for TCP/UDP PAM queries
                    if (String.IsNullOrEmpty(CustomerLogic.CustomerManager.Instance.PAMServerTCPAddress)) /*Note: Moved "_Customer.PAMServerTCPAddress" to CustomerManager object*/ 
                    {
                        ReturnDataset = new DataSet();

                        if (iQueryEntireCustomer == true)
                        {
                            string reformattedQuery = CustomerLogic.CustomerManager.Instance.PaymentXMLSource;
                            int loReplaceIdx = reformattedQuery.IndexOf("/meters/", 0, reformattedQuery.Length, StringComparison.InvariantCultureIgnoreCase);
                            if (loReplaceIdx != -1)
                            {
                                reformattedQuery = reformattedQuery.Remove(loReplaceIdx);
                                reformattedQuery = reformattedQuery + "/expiry";
                            }
                            ReturnDataset.ReadXml(String.Format(reformattedQuery, CustomerId.ToString(), MeterId.ToString()));
                        }
                        else
                        {
                            ReturnDataset.ReadXml(String.Format(CustomerLogic.CustomerManager.Instance.PaymentXMLSource, CustomerId.ToString(), MeterId.ToString()));
                        }
                    }
                    else
                    {
                        // Customer is configured for TCP/UDP PAM query

                        // Start a diagnostics timer
                        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                        sw.Start();

                        // Query PAM via TCP socket
                        string loErrMsg = "";
                        ReturnDataset = PAMClusterStatusRequestViaTCP(_CustomerConfig.CustomerId.ToString(), MeterId.ToString(), ref loErrMsg, iQueryEntireCustomer);

                        // Stop the diagnostic timer
                        sw.Stop();
                        System.Diagnostics.Debug.WriteLine("PAM TCP Query Elapsed: " + sw.Elapsed.ToString());

                        if (loErrMsg != "")
                            throw new AIException(AIExceptionCode.XMLServerNotAvailable, loErrMsg);
                    }
                }
                return ReturnDataset;
            }
            catch (Exception Ex)
            {
                throw (new AIException(AIExceptionCode.XMLServerNotAvailable, Ex.Message));
            }
        }

        #region PAM TCP/UDP Support
        private DataSet CreateEmptyPAMDataSet()
        {
            // Create DataSet
            DataSet dsPAMResponse = new DataSet("PAMResponse");

            // Add Cluster table to dataset
            DataTable dtCluster = new DataTable("Cluster");
            dsPAMResponse.Tables.Add(dtCluster);
            // Add desired columns as attributes
            DataColumn _NewColumn = dtCluster.Columns.Add("ID");
            _NewColumn.ColumnMapping = MappingType.Attribute;
            // Add hidden primary key column that autoincrements
            _NewColumn = dtCluster.Columns.Add("Cluster_id", typeof(System.Int32));
            _NewColumn.AutoIncrement = true;
            _NewColumn.AutoIncrementStep = 1;
            _NewColumn.ColumnMapping = MappingType.Hidden;
            dtCluster.PrimaryKey = new DataColumn[1] { dtCluster.Columns["Cluster_id"] };
            /* Start - N. Parvathy:17 May 2010 - Grace Period */
            //if (_CustomerConfig.GracePeriodEnabled)
            {//if grace period is enabled
                //add grace period attribute
                _NewColumn = dtCluster.Columns.Add("grace");
                _NewColumn.ColumnMapping = MappingType.Attribute;
            }
            /* N. Parvathy:17 May 2010 - Grace Period - End */

            // Add meter table to dataset
            DataTable dtMeter = new DataTable("Meter");
            dsPAMResponse.Tables.Add(dtMeter);
            // Add desired columns as attributes
            _NewColumn = dtMeter.Columns.Add("ID");
            _NewColumn.ColumnMapping = MappingType.Attribute;
            _NewColumn = dtMeter.Columns.Add("imin");
            _NewColumn.ColumnMapping = MappingType.Attribute;
            _NewColumn = dtMeter.Columns.Add("upTS");
            _NewColumn.ColumnMapping = MappingType.Attribute;
            // Add hidden primary key column that autoincrements
            _NewColumn = dtMeter.Columns.Add("Meter_id", typeof(System.Int32));
            _NewColumn.AutoIncrement = true;
            _NewColumn.AutoIncrementStep = 1;
            _NewColumn.ColumnMapping = MappingType.Hidden;
            dtMeter.PrimaryKey = new DataColumn[1] { dtMeter.Columns["Meter_id"] };
            // Add hidden column that will reference the primary key of the cluster table
            _NewColumn = dtMeter.Columns.Add("Cluster_id", typeof(System.Int32));
            _NewColumn.ColumnMapping = MappingType.Hidden;
            // Build data relation and add to the dataset
            System.Data.DataRelation _relation = new DataRelation("Cluster_Meter", dtCluster.Columns["Cluster_id"], dtMeter.Columns["Cluster_id"], true);
            _relation.Nested = true;
            dsPAMResponse.Relations.Add(_relation);

            // Add bay table to dataset
            DataTable dtBay = new DataTable("Bay");
            dsPAMResponse.Tables.Add(dtBay);
            // Add desired columns as attributes
            _NewColumn = dtBay.Columns.Add("ID");
            _NewColumn.ColumnMapping = MappingType.Attribute;
            _NewColumn = dtBay.Columns.Add("expt");
            _NewColumn.ColumnMapping = MappingType.Attribute;
            // Add hidden column that will reference the primary key of the meter table
            _NewColumn = dtBay.Columns.Add("Meter_id", typeof(System.Int32));
            _NewColumn.ColumnMapping = MappingType.Hidden;
            // Build data relation and add to the dataset
            _relation = new DataRelation("Meter_Bay", dtMeter.Columns["Meter_id"], dtBay.Columns["Meter_id"], true);
            _relation.Nested = true;
            dsPAMResponse.Relations.Add(_relation);

            // Accept changes to dataset
            dsPAMResponse.AcceptChanges();
            return dsPAMResponse;
        }

        public DataSet PAMClusterStatusRequestViaTCP(string iCustID, string iMeterID, ref string oErrMsg, bool iQueryEntireCustomer)
        {
            // PAM Request Structure: (14 bytes, little-Endian)
            //   uint8_t flags;      // 0 = normal, 1 = encryption
            //   uint8_t type;       // COMM_PAMBayStatusGet	= 5, COMM_PAMClusterStatus = 6, COMM_PAMMeterStatus = 7, "PAM Multi Cluster Expiry" = 18
            //   uint16_t areaNum;   // Area ID
            //   uint16_t cid;       // Customer ID
            //   uint16_t mid;       // Meter ID
            //   int16_t bayId;      // Bay ID
            //   int32_t time;       // Bay expiry/last-updated time in seconds since 1/1/2000 00:00:00

            // PAM Response Structure: (Dynamic length)
            //   uint8_t flags;           // 0 = normal, 1 = encryption
            //   uint8_t type;            // COMM_PAMBayStatusGet	= 5, COMM_PAMClusterStatus = 6, COMM_PAMMeterStatus = 7
            //   int16_t txnRespCode;     // Apache http response code
            //   int16_t ripnetRespCode;  // RIPNET http response code
            //   int16_t contentLen;	  // Length of content string      | It appear documentation is wrong, and contentLen is
            //   int16_t unknown;         // 2-bytes unknown purpose       | actually a 32-bit value (so the "unknown" slot is part of contentLen)
            //   string  contentString;   // Variable length content string
            //                               Contents are an array of this format: MT:<mid> <imin> <upTS> (<bayID> <expTm>,<bayID> <expTm>,… )\r

            UDPSocket = null;
            UDPServerEndPoint = null;
            UseBigEndian = false;
            _PAMServerTCPAddress = CustomerLogic.CustomerManager.Instance.PAMServerTCPAddress;
            _PAMServerTCPPort = 0;
            Int32.TryParse(CustomerLogic.CustomerManager.Instance.PAMServerTCPPort, out _PAMServerTCPPort);

            string PAMResponseStr = "";
            try
            {
                try
                {
                    OpenUDPSocket();
                }
                catch (Exception exdetail)
                {
                    throw new Exception("Failed in OpenUDPSocket(): " + exdetail.Message);
                }

                // Can't send command this way if there is no socket
                if (UDPSocket == null)
                {
                    throw new Exception("Unable to create/establish TCP Socket. Please check customer configuration.");
                }

                // Create binary structure containing request data
                byte[] srcBuffer = new byte[14];
                MemoryStream bufferStream = null;
                BinaryReader bufferReader = null;
                BinaryWriter bufferWriter = null;
                bufferStream = new MemoryStream(srcBuffer);
                bufferReader = new BinaryReader(bufferStream);
                bufferWriter = new BinaryWriter(bufferStream);

                // Set Common Header Data
                SetByte(0, 0, bufferWriter); // Not Encrypted
                if (iQueryEntireCustomer == true)
                    SetByte(18, 1, bufferWriter); // Type = "PAM Multi Cluster Expiry"
                else
                    SetByte(6, 1, bufferWriter); // Type = COMM_PAMClusterStatus

                // Set Common General Data
                SetWord(0, 2, bufferWriter);     // Area ID
                SetWord(Convert.ToUInt16(iCustID), 4, bufferWriter);  // Customer ID
                if (iQueryEntireCustomer == true)
                    SetWord(0, 6, bufferWriter); // Meter ID not applicable when querying for entire customer
                else
                    SetWord(Convert.ToUInt16(iMeterID), 6, bufferWriter); // Meter ID

                // Set Common PAM Operation Data
                SetSmallInt(0, 8, bufferWriter); // Bay ID -- Zero because N/A for cluster status request
                SetLongInt(0, 10, bufferWriter); // Time -- Zero because N/A for cluster status request

                // Free stream resources
                bufferReader.Close();
                bufferWriter.Close();
                bufferStream.Close();
                bufferStream.Dispose();

                // Before sending, read any left over garbage until the socket is empty
                byte[] trashBuffer = new byte[255];
                try
                {
                    FlushUDPSocket(trashBuffer);
                }
                catch (Exception exdetail)
                {
                    throw new Exception("Failed in FlushUDPSocket(): " + exdetail.Message);
                }

                // Write the byte array to the socket
                try
                {
                    int bytesSent = UDPSocket.SendTo(srcBuffer, UDPServerEndPoint);
                }
                catch (Exception exdetail)
                {
                    string extraDetails = "";
                    if (srcBuffer == null)
                        extraDetails += "; srcBuffer = null";
                    if (UDPServerEndPoint == null)
                        extraDetails += "; UDPServerEndPoint = null";
                    if (UDPSocket == null)
                        extraDetails += "; UDPSocket = null";
                    throw new Exception("Failed in UDPSocket.SendTo(): " + exdetail.Message + extraDetails);
                }

                // Receive response without blocking! (Also, we need to look for multiple packets, because they seem to arrive in 8KB buffered packets)
                int packetNumber = 1;
                bool AllPacketsReceived = false;
                bool firstPacketReceived = false;
                MemoryStream msFullResponse = new MemoryStream();
                byte[] loResponseBuffer = null;

                while (AllPacketsReceived == false)
                {
                    int loUDPDataAvail = 0;
                    /*int noMoreDataLoopCount = 0;*/
                    int loopMax = 5000; // Allows for 50 seconds waiting for first data to arrive
                    if (firstPacketReceived == true)
                        loopMax = 900; // Allows for 9 seconds for next data packet to arrive

                    for (int loNdx = 0; loNdx < loopMax; loNdx++)
                    {
                        if (loNdx > 0)
                            System.Threading.Thread.Sleep(10);

                        try
                        {
                            // Poll will return true if the connection has been terminated and the socket closed. Not a guarantee that our data is 
                            // ready. "Available" seems to be a better fit.

                            // 2012-8-23: New logic treats any available data as a complete packet, instead of imposing a 3 second per packet
                            // penalty just in case more data arrives.  Now, if more data arrives, it just means we will be reading more packets
                            // than previously, but at least the wait times are decreased!
                            if (loUDPDataAvail > 0)
                            {
                                firstPacketReceived = true;
                                //System.Diagnostics.Debug.Write("Data arrived (packet " + packetNumber.ToString() + "); ");
                                break;
                            }
                            else
                            {
                                loUDPDataAvail = UDPSocket.Available;
                            }

                            // 2012-8-23: This old logic imposed a minimum of 3 seconds for each packet.  For large clusters, this might needlessly add 15 seconds or so...
                            /*
                            if ((loUDPDataAvail == UDPSocket.Available) && (loUDPDataAvail > 0))
                            {
                                noMoreDataLoopCount++;
                                if (noMoreDataLoopCount >= 30) // allows for 3 second of no more data
                                {
                                    firstPacketReceived = true;
                                    break; // 3 seconds of no more data. That's long enough for us.
                                }
                            }
                            loUDPDataAvail = UDPSocket.Available;
                            */
                        }
                        catch (Exception loE1)
                        {
                            if (firstPacketReceived == true)
                                AllPacketsReceived = true;

                            firstPacketReceived = true;
                            string extraDetails = "";
                            if (UDPSocket == null)
                                extraDetails = "; UDPSocket = null";
                            throw new Exception("Failed in UDPSocket.Available polling loop: " + loE1.Message + extraDetails);
                        }
                    } // wait for data to arrive loop.

                    // If we waited out all of the loops above and don't have any data, we need to mark the 
                    // "AllPacketsReceived" flag, because essentially we have timed-out waiting for data to arrive
                    if (loUDPDataAvail == 0)
                    {
                        AllPacketsReceived = true;
                    }

                    // We have a packet's worth of data. Let's get it.
                    // Read from socket into byte array
                    if (loUDPDataAvail > 0)
                    {
                        loResponseBuffer = new byte[loUDPDataAvail];
                        try
                        {
                            int bytesRead = UDPSocket.ReceiveFrom(loResponseBuffer, loUDPDataAvail, SocketFlags.None, ref UDPServerEndPoint);
                        }
                        catch (Exception exdetail)
                        {
                            string extraDetails = "";
                            if (UDPServerEndPoint == null)
                                extraDetails += "; UDPServerEndPoint = null";
                            if (UDPSocket == null)
                                extraDetails += "; UDPSocket = null";
                            throw new Exception("Failed in UDPSocket.ReceiveFrom(): " + exdetail.Message + extraDetails);
                        }
                        packetNumber++;

                        // Write the byte array to the memory stream
                        try
                        {
                            msFullResponse.Write(loResponseBuffer, 0, loResponseBuffer.Length);
                        }
                        catch (Exception exdetail)
                        {
                            string extraDetails = "";
                            if (msFullResponse == null)
                                extraDetails += "; msFullResponse = null";
                            if (loResponseBuffer == null)
                                extraDetails += "; loResponseBuffer = null";
                            throw new Exception("Failed in msFullResponse.Write(): " + exdetail.Message + extraDetails);
                        }

                        // Now we will peek at the stream to see what it says the content length is,
                        // then we will compare against the size of the data we have so far.  If the 
                        // values match up nicely, we know we can immediately stop waiting for more data
                        // so we can be a bit more responsive.
                        // Start by retaining our current stream position
                        Int64 streamPos = msFullResponse.Position;
                        try
                        {
                            if (msFullResponse.Length > 10)
                            {
                                // Create a reader targeted at byte index 6, then read content length as 32-bit value
                                bufferReader = new BinaryReader(msFullResponse);
                                msFullResponse.Position = 6;
                                UInt32 InnerContentLength = bufferReader.ReadUInt32();
                                // Put the stream back to its original position before we peeked at its contents
                                msFullResponse.Position = streamPos;
                                if ((InnerContentLength > 0) && (InnerContentLength <= msFullResponse.Length - 10))
                                {
                                    // Yes, all expected data has arrived!  
                                    AllPacketsReceived = true;
                                }
                            }
                        }
                        catch
                        {
                            // Some error happened while trying to get a sneek peek at some of the data. We aren't expecting
                            // for this to happen, but let's ignore it, then put the stream back to its original position before 
                            // we tried to peek at its contents.  This might be an ignorable error that gets cleared up by letting
                            // the process finish normally
                            try { msFullResponse.Position = streamPos; }
                            catch { }
                        }
                    }
                }

                // Close the socket now that we read the response data
                try
                {
                    CloseUDPSocket();
                }
                catch (Exception exdetail)
                {
                    throw new Exception("Failed in CloseUDPSocket(): " + exdetail.Message);
                }

                // Specific validation check for better error messages
                if (msFullResponse == null)
                {
                    throw new Exception("Error: msFullResponse = null after CloseUDPSocket()");
                }

                // Specific validation check for better error messages
                if (msFullResponse.Length == 0)
                {
                    throw new Exception("Error: No data received on TCP/UDP socket");
                }


                // Now build a final response buffer from the memory stream, which may contain data from more than 1 packet
                msFullResponse.Capacity = Convert.ToInt32(msFullResponse.Length);
                loResponseBuffer = msFullResponse.ToArray();
                // Finished with this memory stream
                msFullResponse.Close();
                msFullResponse.Dispose();

                // Create stream to easy reading data from the byte array
                bufferStream = new MemoryStream(loResponseBuffer);
                bufferReader = new BinaryReader(bufferStream);
                bufferWriter = new BinaryWriter(bufferStream);

                // Read the Apache response code from the response buffer, then convert to HttpStatusCode
                Int16 ApacheResponseCodeValue = ReadSmallInt(2, bufferReader);
                HttpStatusCode ApacheResponseCode = HttpStatusCode.NotFound;
                try
                {
                    ApacheResponseCode = (HttpStatusCode)(Enum.Parse(typeof(HttpStatusCode), ApacheResponseCodeValue.ToString()));
                }
                catch { }

                // Log the response code that was extracted, because its useful information to have for diagnostics
                /*
                try
                {
                    Logging.AddTextToGenericLog(_ServerLogPathAndFileName, "RIPNET response code: " + ApacheResponseCodeValue.ToString() + " -- " + ApacheResponseCode.ToString());
                }
                catch { }
                */

                // Read the content length from the byte array
                // DEBUG: 2012-06-07 -- It seems that the documentation for the PAM UDP structure is incorrect.
                //        The content length is actually stored as a 32-bit value instead of 16-bit!
                /*UInt16 ContentLength = ReadWord(6, bufferReader);*/
                UInt32 ContentLength = ReadLongWord(6, bufferReader);

                // Extract the content string from the byte array
                try
                {
                    PAMResponseStr = ASCIIEncoding.ASCII.GetString(loResponseBuffer, 10, Convert.ToInt32(ContentLength));
                }
                catch (Exception exdetail)
                {
                    string extraDetails = "";
                    if (loResponseBuffer != null)
                        extraDetails += "; loResponseBuffer.Length = " + loResponseBuffer.Length.ToString();
                    extraDetails += "; Expected ContentLength = " + Convert.ToInt32(ContentLength).ToString();
                    throw new Exception("Failed to extract PAMResponseStr from UDP response content: " + exdetail.Message + extraDetails);
                }

                // Close stream resources
                bufferReader.Close();
                bufferWriter.Close();
                bufferStream.Close();
                bufferStream.Dispose();





                // Now we need to parse the response to build the dataset
                DataSet dsPAMResponse = CreateEmptyPAMDataSet();
                DataTable dtCluster = dsPAMResponse.Tables["Cluster"];
                DataTable dtMeter = dsPAMResponse.Tables["Meter"];
                DataTable dtBay = dsPAMResponse.Tables["Bay"];
                DataRow drCluster = null;
                DataRow drMeter = null;
                DataRow drBay = null;


                // If we queried UDP for entire customer info instead of single cluster info, our response is much different and needs to be parsed similar to XML responses
                if (iQueryEntireCustomer == true)
                {
                    parseUDPResponseAsXMLContent(PAMResponseStr, dsPAMResponse);
                }
                else
                {
                    string[] lines = PAMResponseStr.Split(new char[] { '\r' });
                    foreach (string nextLine in lines)
                    {
                        if ((nextLine.StartsWith("CL:")) && (nextLine.Length > 3))
                        {
                            // Create new row in Cluster table
                            drCluster = dtCluster.NewRow();
                            dtCluster.Rows.Add(drCluster);
                            drCluster["ID"] = nextLine.Substring(3).Trim();
                        }
                        else if ((nextLine.StartsWith("CU:")) && (drCluster != null))
                        {
                            // Customer number follows, but we don't care about it right now
                        }/* Start - N. Parvathy:17 May 2010 - Grace Period */
                        else if ((nextLine.StartsWith("GP:")) && (nextLine.Length > 3) && (drCluster != null))
                        {
                            // Grace Period arrives. store it in "grace" column
                            drCluster["grace"] = nextLine.Substring(3).Trim();

                        }/* N. Parvathy:17 May 2010 - Grace Period - End */
                        else if ((nextLine.StartsWith("MT:")) && (nextLine.Length > 3) && (drCluster != null))
                        {
                            // Extract meter information
                            int bayStartIdx = nextLine.IndexOf("(");
                            if (bayStartIdx == -1)
                            {
                                throw new Exception("Meter in PAM response has no space information" +
                                    "; Tried extracting from data: " + nextLine);
                            }
                            string meterDetails = "";
                            try
                            {
                                meterDetails = nextLine.Substring(3, bayStartIdx - 3);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Expected Meter Details not found: " + ex.Message +
                                    "; Tried extracting from data: " + nextLine);
                            }
                            string[] meterDetailElements = meterDetails.Split(new char[] { ' ' });
                            if (meterDetailElements.Length < 2)
                            {
                                throw new Exception("Expected Meter Details not found" +
                                    "; Tried extracting from data: " + nextLine);
                            }

                            // Create new row in Meter table
                            drMeter = dtMeter.NewRow();
                            dtMeter.Rows.Add(drMeter);
                            drMeter["ID"] = meterDetailElements[0]; // Meter ID
                            drMeter["imin"] = meterDetailElements[1]; // imin
                            drMeter["upTS"] = meterDetailElements[2]; // upTS
                            drMeter["Cluster_id"] = drCluster["Cluster_id"];

                            // Extract Bay information for current meter
                            int bayEndIdx = nextLine.IndexOf(")");
                            if (bayEndIdx == -1)
                            {
                                throw new Exception("Meter in PAM response has corrupt space information" +
                                    "; Tried extracting from data: " + nextLine);
                            }
                            string allBaysDetailsStr = nextLine.Substring(bayStartIdx + 1, bayEndIdx - (bayStartIdx + 1));
                            string[] allBayDetails = allBaysDetailsStr.Split(new char[] { ',' });
                            if (allBayDetails.Length == 0)
                            {
                                throw new Exception("Meter in PAM response has an empty list of spaces" +
                                    "; Tried extracting from data: " + nextLine);
                            }
                            foreach (string nextBayDetails in allBayDetails)
                            {
                                // Create new row in Bay table
                                drBay = dtBay.NewRow();
                                dtBay.Rows.Add(drBay);

                                string[] bayElements = nextBayDetails.Split(new char[] { ' ' });
                                if (bayElements.Length < 2)
                                {
                                    throw new Exception("Expected Space Details not found" +
                                        "; Tried extracting from data: " + nextLine);
                                }
                                drBay["ID"] = bayElements[0]; // Bay ID
                                // Expired seconds is in units of seconds since 1/1/2000 at 00:00:00
                                // this value is expressed against the meter's local time
                                drBay["expt"] = bayElements[1]; // expt
                                drBay["Meter_id"] = drMeter["Meter_id"];
                            }
                        }
                    }
                }

                // Rturn the dataset
                dsPAMResponse.AcceptChanges();
                return dsPAMResponse;
            }
            catch (Exception loException)
            {
                // Return the error message text
                string extraDetails = "";
                extraDetails += "; Corrupt PAMResponseStr = " + PAMResponseStr;

                oErrMsg = "Failed to parse PAMResponseStr: " + loException.Message + extraDetails +
                    "\r\nServer: TCP " + _PAMServerTCPAddress + " on Port " + _PAMServerTCPPort.ToString();
                return null;
            }
        }

        private void parseUDPResponseAsXMLContent(string PAMResponseStr, DataSet dsPAMResponse)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(PAMResponseStr);
            sw.Flush();
            ms.Position = 0;
            ms.Capacity = Convert.ToInt32(ms.Length);

            dsPAMResponse.ReadXml(ms, XmlReadMode.ReadSchema);

            sw.Close();
            sw.Dispose();
            ms.Close();
            ms.Dispose();
        }

        public void OpenUDPSocket()
        {
            // Shutdown any previous socket first
            CloseUDPSocket();

            // PAM supports both TCP and UDP. We will use TCP because it is more reliable
            IPAddress serverAddress = IPAddress.Parse(_PAMServerTCPAddress);
            UDPServerEndPoint = new IPEndPoint(serverAddress, Convert.ToInt32(_PAMServerTCPPort));

            // Create a TCP socket to the server
            /*UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);*/
            UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect socket (Required when using TCP; Not required for UDP)
            UDPSocket.Connect(UDPServerEndPoint);
        }

        public void CloseUDPSocket()
        {
            try
            {
                if (UDPSocket != null)
                {
                    UDPSocket.Shutdown(SocketShutdown.Both);
                    UDPSocket.Close();
                }
                UDPSocket = null;
                UDPServerEndPoint = null;
            }
            catch
            {
            }
        }

        private void FlushUDPSocket(byte[] iRawBuffer)
        {
            try
            {
                for (; ; )
                {
                    int loAvailable = UDPSocket.Available;
                    if (loAvailable == 0)
                        break; // no more.

                    if (loAvailable > iRawBuffer.Length)
                        loAvailable = iRawBuffer.Length; // can only get this many bytes at a time.
                    UDPSocket.ReceiveFrom(iRawBuffer, loAvailable, SocketFlags.None, ref UDPServerEndPoint);
                    System.Threading.Thread.Sleep(10);
                }
            }
            catch
            {
            }
        }

        // DEBUG: This code isn't finished yet, but it may be useful in the future?
        /*
        public DateTime GetRTCFromUDP(string iCustID, string iMeterID)
        {
            DateTime result = DateTime.MinValue;

            // We can't do anything if we aren't configured to use the UDP/TCP interface for PAM (PAM HTTP doesn't have RTC function)
            if (String.IsNullOrEmpty(CustomerLogic.CustomerManager.Instance.PAMServerTCPAddress))
                return result;

            // PAM Request Structure: (14 bytes, little-Endian)
            //   uint8_t flags;      // 0 = normal, 1 = encryption
            //   uint8_t type;       // COMM_PAMBayStatusGet = 5, COMM_PAMClusterStatus = 6, COMM_PAMMeterStatus = 7
            //   uint16_t areaNum;   // Area ID
            //   uint16_t cid;       // Customer ID
            //   uint16_t mid;       // Meter ID

            //   int16_t bayId;      // Bay ID
            //   int32_t time;       // Bay expiry/last-updated time in seconds since 1/1/2000 00:00:00

            // PAM Response Structure: (Dynamic length)
            //   uint8_t flags;           // 0 = normal, 1 = encryption
            //   uint8_t type;            // COMM_PAMHearBeat = 3, COMM_PAMBayStatusGet = 5, COMM_PAMClusterStatus = 6, COMM_PAMMeterStatus = 7
            //   int16_t txnRespCode;     // Apache http response code
            //   int16_t ripnetRespCode;  // RIPNET http response code
            //   int16_t contentLen;	  // Length of content string
            //   int16_t unknown;         // 2-bytes unknown purpose
            //   string  contentString;   // Variable length content string

            // CustomerID = 9991, Meter = 1001, Cluster = 999101

            // Can't send command this way if there is no socket
            if (UDPSocket == null)
            {
                return result;
            }

            // Create binary structure containing request data
            byte[] srcBuffer = new byte[8];
            MemoryStream bufferStream = null;
            BinaryReader bufferReader = null;
            BinaryWriter bufferWriter = null;
            bufferStream = new MemoryStream(srcBuffer);
            bufferReader = new BinaryReader(bufferStream);
            bufferWriter = new BinaryWriter(bufferStream);

            // Set Common Header Data
            SetByte(0, 0, bufferWriter); // Not Encrypted
            SetByte(8, 1, bufferWriter); // Type = RTC (8)

            // Set Common General Data
            SetWord(0, 2, bufferWriter);     // Area ID
            SetWord(Convert.ToUInt16(iCustID), 4, bufferWriter);  // Customer ID
            SetWord(Convert.ToUInt16(iMeterID), 6, bufferWriter); // Meter ID

            // Free stream resources
            bufferReader.Close();
            bufferWriter.Close();
            bufferStream.Close();
            bufferStream.Dispose();

            // Before sending, read any left over garbage until the socket is empty
            byte[] trashBuffer = new byte[255];
            FlushUDPSocket(trashBuffer);

            // Write the byte array to the socket
            try
            {
                int bytesSent = UDPSocket.SendTo(srcBuffer, UDPServerEndPoint);
            }
            catch (Exception loE1)
            {
                System.Diagnostics.Debug.WriteLine(loE1.Message);
                return result;
            }

            // Receive response without blocking!
            int loNdx;

            int loUDPDataAvail = 0;
            for (loNdx = 0; loNdx < 60; loNdx++)
            {
                System.Threading.Thread.Sleep(100);
                try
                {
                    // Poll will return true if the connection has been terminated and the socket closed. Not a guarantee that our data is 
                    // ready. "Available" seems to be a better fit.
                    if ((loUDPDataAvail == UDPSocket.Available) && (loUDPDataAvail > 0))
                        break; // 100 ms of no more data. That's long enough for us.
                    loUDPDataAvail = UDPSocket.Available;
                }
                catch (Exception loE1)
                {
                    System.Diagnostics.Debug.WriteLine(loE1.Message);
                    break;
                }
            } // wait for data to arrive loop.
            if (loUDPDataAvail == 0)
            {
                System.Diagnostics.Debug.WriteLine("SendRTCRequest Recvd only :" + loUDPDataAvail.ToString() + " out of " + srcBuffer.Length.ToString());
                return result; // not enough data here.
            }

            try
            {
                // have a packet's worth of data. Let's get it.
                // Read from socket into byte array
                byte[] loResponseBuffer = new byte[loUDPDataAvail];
                int bytesRead = UDPSocket.ReceiveFrom(loResponseBuffer, loUDPDataAvail, SocketFlags.None, ref UDPServerEndPoint);

                // Close the socket now that we read the response data
                CloseUDPSocket();

                // Create stream to easy reading data from the byte array
                bufferStream = new MemoryStream(loResponseBuffer);
                bufferReader = new BinaryReader(bufferStream);
                bufferWriter = new BinaryWriter(bufferStream);

                // Read the Apache response code from the response buffer, then convert to HttpStatusCode
                Int16 ApacheResponseCodeValue = ReadSmallInt(2, bufferReader);
                HttpStatusCode ApacheResponseCode = HttpStatusCode.NotFound;
                try
                {
                    ApacheResponseCode = (HttpStatusCode)(Enum.Parse(typeof(HttpStatusCode), ApacheResponseCodeValue.ToString()));
                }
                catch { }

                // Read the content length from the byte array
                Int16 ContentLength = ReadSmallInt(6, bufferReader);
                // Extract the content string from the byte array
                string iResponse = ASCIIEncoding.ASCII.GetString(loResponseBuffer, 10, ContentLength);

                // Close stream resources
                bufferReader.Close();
                bufferWriter.Close();
                bufferStream.Close();
                bufferStream.Dispose();
            }
            catch (Exception loE1)
            {
                System.Diagnostics.Debug.WriteLine(loE1.Message);
                return result;
            }
            return result;
        }
        */

        #endregion

        #region Binary Read Support
        public UInt16 ReadWord(int addr, BinaryReader br)
        {
            br.BaseStream.Position = addr;
            UInt16 result = br.ReadUInt16();
            // Swap Endian order if necessary
            if (UseBigEndian)
                result = EndianConversion.SwapUInt16(result);
            return result;
        }

        public Int16 ReadSmallInt(int addr, BinaryReader br)
        {
            br.BaseStream.Position = addr;
            Int16 result = br.ReadInt16();
            // Swap Endian order if necessary
            if (UseBigEndian)
                result = EndianConversion.SwapInt16(result);
            return result;
        }

        public UInt32 ReadLongWord(int addr, BinaryReader br)
        {
            br.BaseStream.Position = addr;
            UInt32 result = br.ReadUInt32();
            // Swap Endian order if necessary
            if (UseBigEndian)
                result = EndianConversion.SwapUInt32(result);
            return result;
        }

        public Int32 ReadLongInt(int addr, BinaryReader br)
        {
            br.BaseStream.Position = addr;
            Int32 result = br.ReadInt32();
            // Swap Endian order if necessary
            if (UseBigEndian)
                result = EndianConversion.SwapInt32(result);
            return result;
        }

        public byte ReadByte(int addr, BinaryReader br)
        {
            br.BaseStream.Position = addr;
            byte result = br.ReadByte();
            return result;
        }

        public sbyte ReadShortInt(int addr, BinaryReader br)
        {
            br.BaseStream.Position = addr;
            sbyte result = br.ReadSByte();
            return result;
        }
        #endregion

        #region Binary Write Support
        public void SetWord(UInt16 value, int addr, BinaryWriter bw)
        {
            bw.BaseStream.Position = addr;
            // Swap Endian order if necessary
            if (UseBigEndian)
                value = EndianConversion.SwapUInt16(value);
            bw.Write(value);
        }

        public void SetSmallInt(Int16 value, int addr, BinaryWriter bw)
        {
            bw.BaseStream.Position = addr;
            // Swap Endian order if necessary
            if (UseBigEndian)
                value = EndianConversion.SwapInt16(value);
            bw.Write(value);
        }

        public void SetLongWord(UInt32 value, int addr, BinaryWriter bw)
        {
            bw.BaseStream.Position = addr;
            // Swap Endian order if necessary
            if (UseBigEndian)
                value = EndianConversion.SwapUInt32(value);
            bw.Write(value);
        }

        public void SetLongInt(Int32 value, int addr, BinaryWriter bw)
        {
            bw.BaseStream.Position = addr;
            // Swap Endian order if necessary
            if (UseBigEndian)
                value = EndianConversion.SwapInt32(value);
            bw.Write(value);
        }

        public void SetByte(byte value, int addr, BinaryWriter bw)
        {
            bw.BaseStream.Position = addr;
            bw.Write(value);
        }

        public void SetShortInt(sbyte value, int addr, BinaryWriter bw)
        {
            bw.BaseStream.Position = addr;
            bw.Write(value);
        }
        #endregion
    }

    #region DataRow Sorting
    public sealed class DataRowLogicalComparer : System.Collections.Generic.IComparer<DataRow>
    {
        private static readonly System.Collections.Generic.IComparer<DataRow> _default = new DataRowLogicalComparer(0);
        private int _SortColumnIdx = 0;

        public DataRowLogicalComparer(int SortColumnIdx)
        {
            _SortColumnIdx = SortColumnIdx;
        }

        public static System.Collections.Generic.IComparer<DataRow> Default
        {
            get { return _default; }
        }

        public int Compare(DataRow s1, DataRow s2)
        {
            // Must use Ordinal comparison so it matches ASCII sorting
            string srcString1 = s1[_SortColumnIdx].ToString();
            string srcString2 = s2[_SortColumnIdx].ToString();
            return string.CompareOrdinal(srcString1, srcString2);
        }
    }
    #endregion

    #region Endian Conversion (Not currently needed, but quite useful)
    class EndianConversion
    {
        static EndianConversion()
        {
            _LittleEndian = BitConverter.IsLittleEndian;
        }

        public static short SwapInt16(short v)
        {
            return (short)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
        }

        public static ushort SwapUInt16(ushort v)
        {
            return (ushort)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
        }

        public static int SwapInt32(int v)
        {
            return (int)(((SwapInt16((short)v) & 0xffff) << 0x10) |
                          (SwapInt16((short)(v >> 0x10)) & 0xffff));
        }

        public static uint SwapUInt32(uint v)
        {
            return (uint)(((SwapUInt16((ushort)v) & 0xffff) << 0x10) |
                           (SwapUInt16((ushort)(v >> 0x10)) & 0xffff));
        }

        public static long SwapInt64(long v)
        {
            return (long)(((SwapInt32((int)v) & 0xffffffffL) << 0x20) |
                           (SwapInt32((int)(v >> 0x20)) & 0xffffffffL));
        }

        public static ulong SwapUInt64(ulong v)
        {
            return (ulong)(((SwapUInt32((uint)v) & 0xffffffffL) << 0x20) |
                            (SwapUInt32((uint)(v >> 0x20)) & 0xffffffffL));
        }

        public static bool IsBigEndian
        {
            get
            {
                return !_LittleEndian;
            }
        }

        public static bool IsLittleEndian
        {
            get
            {
                return _LittleEndian;
            }
        }

        private static readonly bool _LittleEndian;
    }
    #endregion

    public sealed class HistoricalPaymentRecordsSorter : System.Collections.Generic.IComparer<PaymentRecord>
    {
        private bool _sortAscending = true;

        private static readonly System.Collections.Generic.IComparer<PaymentRecord> _default = new HistoricalPaymentRecordsSorter(true);

        public HistoricalPaymentRecordsSorter(bool SortAscending)
        {
            _sortAscending = SortAscending;
        }

        public static System.Collections.Generic.IComparer<PaymentRecord> Default
        {
            get { return _default; }
        }

        public int Compare(PaymentRecord s1, PaymentRecord s2)
        {
            // We are sorting by date
            if (_sortAscending)
                return s1.TransactionDateTime.CompareTo(s2.TransactionDateTime);
            else
                return s2.TransactionDateTime.CompareTo(s1.TransactionDateTime);
        }
    }

    public class PaymentDatabaseSource
    {
        private CustomerLogic _Customer;

        public PaymentDatabaseSource(CustomerConfig customerDTO)
        {
            _Customer = CustomerLogic.CustomerManager.GetLogic(customerDTO);
        }

        public List<PaymentRecord> GetHistoricalPaymentDataForMeters_StronglyTyped(int CustomerId, List<int> listOfMeterIDs, DateTime StartTime, DateTime EndTime, bool SortAscending)
        {
            List<PaymentRecord> result = null;

            // Since SQL Server will choke if we try to include too many parameters, we will query for upto 500 meters at a time.
            // If we are needing data for more than 500 meters, then we will end up executing multiple queries to the database
            // and combining the results

            int nextIdxIntoListOfMeters = 0;
            int nextIdxIntoListOfMeterForUnionStatements = 0;
            while (nextIdxIntoListOfMeters < listOfMeterIDs.Count)
            {
                string loMeterDelim = string.Empty;
                int batchParamCount = 0;
                nextIdxIntoListOfMeterForUnionStatements = nextIdxIntoListOfMeters;

                // Use a DTO mapper to deserialize from database to object list
                StringBuilder sql = new StringBuilder();


                //// CASH ////
                sql.Append(
                    "select TransactionsCashId as TransactionId, CustomerId, MeterId, " +
                    "TransDateTime as TransactionDateTime, BayNumber, AmountInCents, " +
                    "TimePaid, TransactionType='CashId', 0 as Method " +
                    "from TransactionsCash " +
                    "where CustomerId = @CustomerId and MeterId in (");
                for (int loIdx = nextIdxIntoListOfMeters; loIdx < listOfMeterIDs.Count; loIdx++)
                {
                    sql.Append(loMeterDelim);
                    sql.Append("@MeterId_" + loIdx.ToString());
                    if (string.IsNullOrEmpty(loMeterDelim))
                        loMeterDelim = ", ";

                    // Increment the index into the meter list that has been accounted for and the number of items in this batch
                    nextIdxIntoListOfMeters++;
                    batchParamCount++;

                    // Don't want to have more than 500 parameters in a SQL query.  If there are a lot of meters, we may need to run multiple queries and
                    // combine the result
                    if (batchParamCount >= 500)
                    {
                        break;
                    }
                }
                sql.Append(")");
                sql.Append("and TransDateTime >= @StartTime and TransDateTime <= @EndTime ");

                //// CREDIT CARD ////
                sql.Append(
                    "union select TransactionsCreditCardId as TransactionId, CustomerId, MeterId, " +
                    "TransDateTime as TransactionDateTime, BayNumber, AmountInCents, " +
                    "TimePaid, TransactionType='CreditCardId', 1 as Method " +
                    "from TransactionsCreditCard " +
                    "where CustomerId = @CustomerId and MeterId in (");
                // Reset the delimiters and batch count because we need to use the same set of meter ids in our union statement
                loMeterDelim = string.Empty;
                batchParamCount = 0;
                // Start looping from "nextIdxIntoListOfMeterForUnionStatements" which matches "nextIdxIntoListOfMeters" at the start of this batch
                // Note: Do not increment "nextIdxIntoListOfMeterForUnionStatements" because the same start value needs to be used multiple times within this batch
                for (int loIdx = nextIdxIntoListOfMeterForUnionStatements; loIdx < listOfMeterIDs.Count; loIdx++)
                {
                    sql.Append(loMeterDelim);
                    sql.Append("@MeterId_" + loIdx.ToString());
                    if (string.IsNullOrEmpty(loMeterDelim))
                        loMeterDelim = ", ";

                    // Don't want to have more than 500 parameters in a SQL query.  If there are a lot of meters, we may need to run multiple queries and
                    // combine the result
                    batchParamCount++;
                    if (batchParamCount >= 500)
                    {
                        break;
                    }
                }

                sql.Append(")");
                sql.Append("and TransDateTime >= @StartTime and TransDateTime <= @EndTime ");

                //// MPARK ////
                sql.Append(
                    "union select TransactionsMParkId as TransactionId, CustomerId, MeterId, " +
                    "TransDateTime as TransactionDateTime, BayNumber, AmountInCents, " +
                    "TimePaid,TransactionType='MParkId', 2 as Method " +
                    "from TransactionsMPark " +
                    "where CustomerId = @CustomerId and MeterId in (");
                // Reset the delimiters and batch count because we need to use the same set of meter ids in our union statement
                loMeterDelim = string.Empty; 
                batchParamCount = 0;
                // Start looping from "nextIdxIntoListOfMeterForUnionStatements" which matches "nextIdxIntoListOfMeters" at the start of this batch
                // Note: Do not increment "nextIdxIntoListOfMeterForUnionStatements" because the same start value needs to be used multiple times within this batch
                for (int loIdx = nextIdxIntoListOfMeterForUnionStatements; loIdx < listOfMeterIDs.Count; loIdx++)
                {
                    sql.Append(loMeterDelim);
                    sql.Append("@MeterId_" + loIdx.ToString());
                    if (string.IsNullOrEmpty(loMeterDelim))
                        loMeterDelim = ", ";

                    // Don't want to have more than 500 parameters in a SQL query.  If there are a lot of meters, we may need to run multiple queries and
                    // combine the result
                    batchParamCount++;
                    if (batchParamCount >= 500)
                    {
                        break;
                    }
                }
                sql.Append(")");
                sql.Append("and TransDateTime >= @StartTime and TransDateTime <= @EndTime ");

                //// SMART CARD ////
                sql.Append(
                    "union select TransactionsSmartCardId as TransactionId, CustomerId, MeterId, " +
                    "TransDateTime as TransactionDateTime, BayNumber, AmountInCents, " +
                    "TimePaid, TransactionType='SmartCardId', 3 as Method " +
                    "from TransactionsSmartCard " +
                    "where CustomerId = @CustomerId and MeterId in (");
                // Reset the delimiters and batch count because we need to use the same set of meter ids in our union statement
                loMeterDelim = string.Empty;
                batchParamCount = 0;
                // Start looping from "nextIdxIntoListOfMeterForUnionStatements" which matches "nextIdxIntoListOfMeters" at the start of this batch
                // Note: Do not increment "nextIdxIntoListOfMeterForUnionStatements" because the same start value needs to be used multiple times within this batch
                for (int loIdx = nextIdxIntoListOfMeterForUnionStatements; loIdx < listOfMeterIDs.Count; loIdx++)
                {
                    sql.Append(loMeterDelim);
                    sql.Append("@MeterId_" + loIdx.ToString());
                    if (string.IsNullOrEmpty(loMeterDelim))
                        loMeterDelim = ", ";

                    // Don't want to have more than 500 parameters in a SQL query.  If there are a lot of meters, we may need to run multiple queries and
                    // combine the result
                    batchParamCount++;
                    if (batchParamCount >= 500)
                    {
                        break;
                    }
                }
                sql.Append(")");
                sql.Append("and TransDateTime >= @StartTime and TransDateTime <= @EndTime ");

                //// CASH KEY ////
                sql.Append(
                    "union select TransactionsCashKeyId as TransactionId, CustomerId, MeterId, " +
                    "TransDateTime as TransactionDateTime, BayNumber, AmountInCents, " +
                    "TimePaid, TransactionType='CashKeyId', 4 as Method " +
                    "from TransactionsCashKey " +
                    "where CustomerId = @CustomerId and MeterId in (");
                // Reset the delimiters and batch count because we need to use the same set of meter ids in our union statement
                loMeterDelim = string.Empty;
                batchParamCount = 0;
                // Start looping from "nextIdxIntoListOfMeterForUnionStatements" which matches "nextIdxIntoListOfMeters" at the start of this batch
                // Note: Do not increment "nextIdxIntoListOfMeterForUnionStatements" because the same start value needs to be used multiple times within this batch
                for (int loIdx = nextIdxIntoListOfMeterForUnionStatements; loIdx < listOfMeterIDs.Count; loIdx++)
                {
                    sql.Append(loMeterDelim);
                    sql.Append("@MeterId_" + loIdx.ToString());
                    if (string.IsNullOrEmpty(loMeterDelim))
                        loMeterDelim = ", ";

                    // Don't want to have more than 500 parameters in a SQL query.  If there are a lot of meters, we may need to run multiple queries and
                    // combine the result
                    batchParamCount++;
                    if (batchParamCount >= 500)
                    {
                        break;
                    }
                }
                sql.Append(")");
                sql.Append("and TransDateTime >= @StartTime and TransDateTime <= @EndTime ");

                //// FREE PARKING CREDIT (PAMTX type 10) ////
                sql.Append(
                    "union select SequenceId as TransactionId, CustomerId, MeterId, " +
                    "TransDateTime as TransactionDateTime, Bay as BayNumber, AmountCent as AmountInCents, " +
                    "TimeCredit as TimePaid, TransactionType='FreeParkingId', 5 as Method " +
                    "from PAMTx " +
                    "where CustomerId = @CustomerId and MeterId in (");
                // Reset the delimiters and batch count because we need to use the same set of meter ids in our union statement
                loMeterDelim = string.Empty;
                batchParamCount = 0;
                // Start looping from "nextIdxIntoListOfMeterForUnionStatements" which matches "nextIdxIntoListOfMeters" at the start of this batch
                // Note: Do not increment "nextIdxIntoListOfMeterForUnionStatements" because the same start value needs to be used multiple times within this batch
                for (int loIdx = nextIdxIntoListOfMeterForUnionStatements; loIdx < listOfMeterIDs.Count; loIdx++)
                {
                    sql.Append(loMeterDelim);
                    sql.Append("@MeterId_" + loIdx.ToString());
                    if (string.IsNullOrEmpty(loMeterDelim))
                        loMeterDelim = ", ";

                    // Don't want to have more than 500 parameters in a SQL query.  If there are a lot of meters, we may need to run multiple queries and
                    // combine the result
                    batchParamCount++;
                    if (batchParamCount >= 500)
                    {
                        break;
                    }
                }
                sql.Append(")");
                sql.Append("and TransDateTime >= @StartTime and TransDateTime <= @EndTime and TransactionType = 10 ");

                //// ZERO-OUT EVENTS (PAMTX type 8) ////
                sql.Append(
                    "union select SequenceId as TransactionId, CustomerId, MeterId, " +
                    "TransDateTime as TransactionDateTime, Bay as BayNumber, AmountCent as AmountInCents, " +
                    "0 as TimePaid, TransactionType='ZeroOutId', 6 as Method " +
                    "from PAMTx " +
                    "where CustomerId = @CustomerId and MeterId in (");
                // Reset the delimiters and batch count because we need to use the same set of meter ids in our union statement
                loMeterDelim = string.Empty;
                batchParamCount = 0;
                // Start looping from "nextIdxIntoListOfMeterForUnionStatements" which matches "nextIdxIntoListOfMeters" at the start of this batch
                // Note: Do not increment "nextIdxIntoListOfMeterForUnionStatements" because the same start value needs to be used multiple times within this batch
                for (int loIdx = nextIdxIntoListOfMeterForUnionStatements; loIdx < listOfMeterIDs.Count; loIdx++)
                {
                    sql.Append(loMeterDelim);
                    sql.Append("@MeterId_" + loIdx.ToString());
                    if (string.IsNullOrEmpty(loMeterDelim))
                        loMeterDelim = ", ";

                    // Don't want to have more than 500 parameters in a SQL query.  If there are a lot of meters, we may need to run multiple queries and
                    // combine the result
                    batchParamCount++;
                    if (batchParamCount >= 500)
                    {
                        break;
                    }
                }
                sql.Append(")");
                sql.Append("and TransDateTime >= @StartTime and TransDateTime <= @EndTime and TransactionType = 8 ");


                // And add the order-by
                sql.Append(" order by TransactionDateTime");
                if (!SortAscending)
                {
                    sql.Append(" desc");
                }

                using (SqlCommand command = _Customer.SharedSqlDao.GetSqlCommand(sql.ToString()))
                {
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@StartTime", StartTime));
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@EndTime", EndTime));

                    // Reset the batch count because we need to use the same set of meter ids to fill in the query's parameter values
                    batchParamCount = 0;
                    // Start looping from "nextIdxIntoListOfMeterForUnionStatements" which matches "nextIdxIntoListOfMeters" at the start of this batch
                    // Note: Do not increment "nextIdxIntoListOfMeterForUnionStatements" because the same start value needs to be used multiple times within this batch
                    for (int loIdx = nextIdxIntoListOfMeterForUnionStatements; loIdx < listOfMeterIDs.Count; loIdx++)
                    {
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@MeterId_" + loIdx.ToString(), listOfMeterIDs[loIdx]));

                        // Don't want to have more than 500 parameters in a SQL query.  If there are a lot of meters, we may need to run multiple queries and
                        // combine the result
                        batchParamCount++;
                        if (batchParamCount >= 500)
                        {
                            break;
                        }
                    }

                    // Get list of objects from the SQL query
                    List<PaymentRecord> partialResult = _Customer.SharedSqlDao.GetList<PaymentRecord>(command);

                    // Create final result object if needed
                    if (result == null)
                        result = new List<PaymentRecord>();

                    // Append the partial results to the full results
                    if ((partialResult != null) && (partialResult.Count > 0))
                    {
                        result.AddRange(partialResult);
                    }
                }
            }

            // Sort the result list, which might be comprised of the results from multiple queries
            result.Sort(new HistoricalPaymentRecordsSorter(SortAscending));

            // Return the final result, which may have been comprised of concatenated results from multiple DB queries
            return result;
        }
    }
}