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

    public class SensingDatabaseSource
    {
        private CustomerConfig _CustomerCfg;
        private CustomerLogic _Customer;

        public SensingDatabaseSource(CustomerConfig customerCfg)
        {
            _CustomerCfg = customerCfg;
            _Customer = CustomerLogic.CustomerManager.GetLogic(_CustomerCfg);
        }

        public List<HistoricalSensingRecord> GetHistoricalVehicleSensingDataForMeters_StronglyTyped(int CustomerId, List<int> listOfMeterIDs, DateTime StartTime, DateTime EndTime, bool SortAscending)
        {
            List<HistoricalSensingRecord> result = null;
            if (listOfMeterIDs.Count == 0)
                return result;

            // Since SQL Server will choke if we try to include too many parameters, we will query for upto 500 meters at a time.
            // If we are needing data for more than 500 meters, then we will end up executing multiple queries to the database
            // and combining the results

            // Use a DTO mapper to deserialize from database to object list
            /*if (_CustomerCfg.VehSensingProvider == CustomerConfig.VehSensingProviders.GTC)*/
            {
                int nextIdxIntoListOfMeters = 0;
                while (nextIdxIntoListOfMeters < listOfMeterIDs.Count)
                {
                    string loMeterDelim = string.Empty;
                    int batchParamCount = 0;
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand command = _Customer.SharedSqlDao.GetSqlCommand(string.Empty))
                    {
                        sql.Append(
                            "Select psoa.LastStatus, psoa.LastUpdatedTS, ps.BayNumber, ps.MeterId, ps.CustomerID, ps.ParkingSpaceId," +
                            " psoa.ParkingSpaceOccupancyId, psoa.RecCreationDate" +
                            " from ParkingSpaces ps" +
                            " left join ParkingSpaceOccupancy pso on ps.ParkingSpaceId = pso.ParkingSpaceId" +
                            " left join ParkingSpaceOccupancyAudit psoa on pso.ParkingSpaceOccupancyId = psoa.ParkingSpaceOccupancyId" +
                            " where ps.CustomerId = @CustomerId" +
                            " and psoa.LastUpdatedTS >= @StartTime and psoa.LastUpdatedTS < @EndTime" +
                            " and ps.MeterId in (");

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
                        sql.Append(")");
                        sql.Append(" order by psoa.LastUpdatedTS");
                        if (!SortAscending)
                        {
                            sql.Append(" desc");
                        }

                        command.CommandText = sql.ToString();
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@StartTime", StartTime));
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@EndTime", EndTime));

                        // Get list of objects from the SQL query
                        List<HistoricalSensingRecord> partialResult = _Customer.SharedSqlDao.GetList<HistoricalSensingRecord>(command);

                        // Create final result object if needed
                        if (result == null)
                            result = new List<HistoricalSensingRecord>();

                        // Append the partial results to the full results
                        if ((partialResult != null) && (partialResult.Count > 0))
                        {
                            result.AddRange(partialResult);
                        }
                    }
                }
            }
            /*
            else
            {
                throw new Exception("Non-GTC vehicle sensing providers not implemented yet");
            }
            */

            // We must sort the result list, which may have been comprised of concatenated results from multiple DB queries
            result.Sort(new HistoricalSensingRecordsSorter(SortAscending));

            // Return the final result, which may have been comprised of concatenated results from multiple DB queries
            return result;
        }

        public List<CurrentSpaceOccupancyInformation> GetCurrentVehicleSensingDataForMeters_StronglyTyped(int CustomerId, List<int> listOfMeterIDs)
        {
            List<CurrentSpaceOccupancyInformation> result = null;
            if (listOfMeterIDs.Count == 0)
                return result;

            // Since SQL Server will choke if we try to include too many parameters, we will query for upto 500 meters at a time.
            // If we are needing data for more than 500 meters, then we will end up executing multiple queries to the database
            // and combining the results

            // Use a DTO mapper to deserialize from database to object list
            /*if (_CustomerCfg.VehSensingProvider == CustomerConfig.VehSensingProviders.GTC)*/
            {
                int nextIdxIntoListOfMeters = 0;
                while (nextIdxIntoListOfMeters < listOfMeterIDs.Count)
                {
                    string loMeterDelim = string.Empty;
                    int batchParamCount = 0;
                    StringBuilder sql = new StringBuilder();
                    using (SqlCommand command = _Customer.SharedSqlDao.GetSqlCommand(string.Empty))
                    {
                        sql.Append("Select pso.LastStatus, pso.LastUpdatedTS, ps.BayNumber, ps.MeterId, ps.HasSensor," +
                            " ps.CustomerID, ps.ParkingSpaceId, pso.ParkingSpaceOccupancyId" +
                            " from ParkingSpaces ps left join ParkingSpaceOccupancy pso on ps.ParkingSpaceId = pso.ParkingSpaceId" +
                            " where ps.CustomerId = @CustomerId and ps.MeterId in (");

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
                        sql.Append(")");

                        command.CommandText = sql.ToString();
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));

                        // Get list of objects from the SQL query
                        List<CurrentSpaceOccupancyInformation> partialResult = _Customer.SharedSqlDao.GetList<CurrentSpaceOccupancyInformation>(command);

                        // Create final result object if needed
                        if (result == null)
                            result = new List<CurrentSpaceOccupancyInformation>();

                        // Append the partial results to the full results
                        if ((partialResult != null) && (partialResult.Count > 0))
                        {
                            result.AddRange(partialResult);
                        }
                    }
                }
            }
            /*
            else
            {
                throw new Exception("Non-GTC vehicle sensing providers not implemented yet");
            }
            */

            // Return the final result, which may have been comprised of concatenated results from multiple DB queries
            return result;
        }


        public List<SensorEventAndCommsRecord> GetOccupancyDataForOpsAdminMaintExceptions(int CustomerId, List<int> listOfMeterIDs, OpsAdminMaintExceptionsReportParameters reportParams)
        {
            List<SensorEventAndCommsRecord> result = null;
            if (listOfMeterIDs.Count == 0)
                return result;

            /*
            if (_CustomerCfg.VehSensingProvider != CustomerConfig.VehSensingProviders.GTC)
                throw new Exception("Non-GTC vehicle sensing providers not implemented yet");
            */

            // Since SQL Server will choke if we try to include too many parameters, we will query for upto 500 meters at a time.
            // If we are needing data for more than 500 meters, then we will end up executing multiple queries to the database
            // and combining the results

            int nextIdxIntoListOfMeters = 0;
            while (nextIdxIntoListOfMeters < listOfMeterIDs.Count)
            {
                string loMeterDelim = string.Empty;
                int batchParamCount = 0;
                StringBuilder sql = new StringBuilder();
                using (SqlCommand command = _Customer.SharedSqlDao.GetSqlCommand(string.Empty))
                {
                    // Are we gathering the latest data instead of historical data?
                    if (reportParams.ReportOnHistoricalData == false)
                    {
                        // So that our query doesn't get way too complex, we will just gather data from the neweset record for each meter,
                        // and then the report will need to evaluate the elapsed times to see if it qualifies as "not recent enough", etc.
                        sql.Append(
                            "select m.MeterID, m.MeterName, ps.BayNumber," +
                            " (case when mm.AreaId2 is not null then mm.AreaId2 else ps.AreaId end) as AreaID, " +
                            " ps.HasSensor, m.MaxBaysEnabled, m.MeterGroup, m.MeterType," +
                            " (select top 1 mc.MeterTime from MeterComm as mc where mc.CustomerId = m.CustomerId and mc.AreaId = m.AreaID and mc.MeterId = m.MeterID" +
                            " order by mc.MeterTime desc) as LastCommunication," +
                            " (select top 1 mc.Message from MeterComm as mc where mc.CustomerId = m.CustomerId and mc.AreaId = m.AreaID and mc.MeterId = m.MeterID" +
                            " order by mc.MeterTime desc) as LastCommunicationMessage," +
                            " (select top 1 pso2.LastUpdatedTS from ParkingSpaceOccupancy as pso2 where pso2.ParkingSpaceID = ps.ParkingSpaceID" +
                            " order by pso2.LastUpdatedTS) as LastSensorStatusTime," +
                            " (select top 1 pso3.LastStatus from ParkingSpaceOccupancy as pso3 where pso3.ParkingSpaceID = ps.ParkingSpaceID" +
                            " order by pso3.LastUpdatedTS) as LastSensorStatus" +
                            " from ParkingSpaces as ps" +
                            " left join Meters as m on ps.MeterId = m.MeterId and ps.CustomerId = m.CustomerId and ps.AreaId = m.AreaId" +
                            " left join MeterMap as mm on m.MeterId = mm.MeterId and m.CustomerId = mm.CustomerId and m.AreaId = mm.AreaId" +
                            " where m.CustomerId = @CustomerId" +
                            " and ps.HasSensor = 1 and m.MeterGroup = 0"); // and m.MaxBaysEnabled >= 0
                        sql.Append(" and m.MeterId in (");
                        loMeterDelim = string.Empty;
                        batchParamCount = 0;
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
                        sql.Append(")");
                        sql.Append(" order by 1, 3, 11");

                        command.CommandText = sql.ToString();
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));
                    }
                    else if (reportParams.ReportOnHistoricalData == true)
                    {
                        sql.Append(
                            "select m.MeterID, m.MeterName, ps.BayNumber," +
                            " (case when mm.AreaId2 is not null then mm.AreaId2 else ps.AreaId end) as AreaID, " +
                            " ps.HasSensor, m.MaxBaysEnabled, m.MeterGroup, m.MeterType," +
                            " psoa.LastUpdatedTS as LastSensorStatusTime," +
                            " psoa.LastStatus as LastSensorStatus" +
                            " from ParkingSpaces as ps" +
                            " left join Meters as m on ps.MeterId = m.MeterId and ps.CustomerId = m.CustomerId and ps.AreaId = m.AreaId" +
                            " left join MeterMap as mm on m.MeterId = mm.MeterId and m.CustomerId = mm.CustomerId and m.AreaId = mm.AreaId" +
                            " left join ParkingSpaceOccupancy as pso on ps.ParkingSpaceID = pso.ParkingSpaceID" +
                            " left join ParkingSpaceOccupancyAudit as psoa on pso.ParkingSpaceOccupancyId = psoa.ParkingSpaceOccupancyId" +
                            " where m.CustomerId = @CustomerId" +
                            " and ps.HasSensor = 1 and m.MeterGroup = 0"); //  and m.MaxBaysEnabled >= 0

                        sql.Append(" and m.MeterId in (");
                        loMeterDelim = string.Empty;
                        batchParamCount = 0;
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
                        sql.Append(")");
                        sql.Append(" and psoa.LastUpdatedTS >= @StartTime and psoa.LastUpdatedTS <= @EndTime");
                        sql.Append(" order by 1, 3, 9");

                        command.CommandText = sql.ToString();
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@StartTime", reportParams.StartTime));
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@EndTime", reportParams.EndTime));
                    }

                    // Get list of objects from the SQL query
                    List<SensorEventAndCommsRecord> partialResult = _Customer.SharedSqlDao.GetList<SensorEventAndCommsRecord>(command);

                    // Create final result object if needed
                    if (result == null)
                        result = new List<SensorEventAndCommsRecord>();

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

        public List<SensorHeartbeatRecord> GetCommunicationDataForOpsAdminMaintExceptions(int CustomerId, List<int> listOfMeterIDs, OpsAdminMaintExceptionsReportParameters reportParams)
        {
            List<SensorHeartbeatRecord> result = null;
            if (listOfMeterIDs.Count == 0)
                return result;

            /*
            if (_CustomerCfg.VehSensingProvider != CustomerConfig.VehSensingProviders.GTC)
                throw new Exception("Non-GTC vehicle sensing providers not implemented yet");
            */

            // Since SQL Server will choke if we try to include too many parameters, we will query for upto 500 meters at a time.
            // If we are needing data for more than 500 meters, then we will end up executing multiple queries to the database
            // and combining the results

            int nextIdxIntoListOfMeters = 0;
            while (nextIdxIntoListOfMeters < listOfMeterIDs.Count)
            {
                string loMeterDelim = string.Empty;
                int batchParamCount = 0;
                StringBuilder sql = new StringBuilder();

                using (SqlCommand command = _Customer.SharedSqlDao.GetSqlCommand(string.Empty))
                {
                    // Are we gathering the latest data instead of historical data?
                    if (reportParams.ReportOnHistoricalData == false)
                    {
                        // So that our query doesn't get way too complex, we will just gather data from the neweset record for each meter,
                        // and then the report will need to evaluate the elapsed times to see if it qualifies as "not recent enough", etc.
                        sql.Append(
                            "select m.MeterID, m.MeterName, ps.BayNumber," +
                            " (case when mm.AreaId2 is not null then mm.AreaId2 else ps.AreaId end) as AreaID, " +
                            " ps.HasSensor, m.MaxBaysEnabled, m.MeterGroup, m.MeterType," +
                            " (select top 1 mc.MeterTime from MeterComm as mc where mc.CustomerId = m.CustomerId and mc.AreaId = m.AreaID and mc.MeterId = m.MeterID" +
                            " order by mc.MeterTime desc) as LastCommunication," +
                            " (select top 1 mc.Message from MeterComm as mc where mc.CustomerId = m.CustomerId and mc.AreaId = m.AreaID and mc.MeterId = m.MeterID" +
                            " order by mc.MeterTime desc) as LastCommunicationMessage" +
                            " from ParkingSpaces as ps" +
                            " left join Meters as m on ps.MeterId = m.MeterId and ps.CustomerId = m.CustomerId and ps.AreaId = m.AreaId" +
                            " left join MeterMap as mm on m.MeterId = mm.MeterId and m.CustomerId = mm.CustomerId and m.AreaId = mm.AreaId" +
                            " where m.CustomerId = @CustomerId" +
                            " and ps.HasSensor = 1 and m.MeterGroup = 0"); //and m.MaxBaysEnabled >= 0

                        sql.Append(" and m.MeterId in (");
                        loMeterDelim = string.Empty;
                        batchParamCount = 0;
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
                        sql.Append(")");
                        sql.Append(" order by 1, 3, 9");

                        command.CommandText = sql.ToString();
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));
                    }
                    else if (reportParams.ReportOnHistoricalData == true)
                    {
                        // So that our query doesn't get way too complex, we will just gather data from the neweset record for each meter,
                        // and then the report will need to evaluate the elapsed times to see if it qualifies as "not recent enough", etc.
                        sql.Append(
                            "select m.MeterID, m.MeterName, ps.BayNumber," +
                            " (case when mm.AreaId2 is not null then mm.AreaId2 else ps.AreaId end) as AreaID, " +
                            " ps.HasSensor, m.MaxBaysEnabled, m.MeterGroup, m.MeterType," +
                            " mc.MeterTime as LastCommunication, mc.Message as LastCommunicationMessage" +
                            " from ParkingSpaces as ps" +
                            " left join Meters as m on ps.MeterId = m.MeterId and ps.CustomerId = m.CustomerId and ps.AreaId = m.AreaId" +
                            " left join MeterMap as mm on m.MeterId = mm.MeterId and m.CustomerId = mm.CustomerId and m.AreaId = mm.AreaId" +
                            " left join MeterComm as mc on m.MeterId = mc.MeterId and m.CustomerId = mc.CustomerId" + // and m.AreaId = mc.MeterId"
                            " where m.CustomerId = @CustomerId" +
                            " and ps.HasSensor = 1 and m.MeterGroup = 0"); //and m.MaxBaysEnabled >= 0

                        sql.Append(" and m.MeterId in (");
                        loMeterDelim = string.Empty;
                        batchParamCount = 0;
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
                        sql.Append(")");
                        sql.Append(" and mc.MeterTime >= @StartTime and mc.MeterTime <= @EndTime");
                        sql.Append(" order by 1, 3, 9");

                        command.CommandText = sql.ToString();
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@StartTime", reportParams.StartTime));
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@EndTime", reportParams.EndTime));
                    }

                    // Get list of objects from the SQL query
                    List<SensorHeartbeatRecord> partialResult = _Customer.SharedSqlDao.GetList<SensorHeartbeatRecord>(command);

                    // Create final result object if needed
                    if (result == null)
                        result = new List<SensorHeartbeatRecord>();

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

        public List<SensorBatteryDiagnostics> GetBatteryVoltageDataForOpsAdminMaintExceptions(int CustomerId, List<int> listOfMeterIDs, OpsAdminMaintExceptionsReportParameters reportParams)
        {
            List<SensorBatteryDiagnostics> result = null;
            if (listOfMeterIDs.Count == 0)
                return result;

            /*
            if (_CustomerCfg.VehSensingProvider != CustomerConfig.VehSensingProviders.GTC)
                throw new Exception("Non-GTC vehicle sensing providers not implemented yet");
            */

            // Since SQL Server will choke if we try to include too many parameters, we will query for upto 500 meters at a time.
            // If we are needing data for more than 500 meters, then we will end up executing multiple queries to the database
            // and combining the results

            int nextIdxIntoListOfMeters = 0;
            while (nextIdxIntoListOfMeters < listOfMeterIDs.Count)
            {
                string loMeterDelim = string.Empty;
                int batchParamCount = 0;
                StringBuilder sql = new StringBuilder();

                using (SqlCommand command = _Customer.SharedSqlDao.GetSqlCommand(string.Empty))
                {
                    // Are we gathering the latest data instead of historical data?
                    if (reportParams.ReportOnHistoricalData == false)
                    {
                        sql.Append(
                            "select m.MeterID, m.MeterName, ps.BayNumber, " +
                            " (case when mm.AreaId2 is not null then mm.AreaId2 else ps.AreaId end) as AreaID, " +
                            " ps.HasSensor, m.MaxBaysEnabled, m.MeterGroup, m.MeterType," +
                            " (select top 1 md.DiagnosticValue from MeterDiagnostic md" +
                            " where m.MeterID = md.MeterID and m.CustomerID = md.CustomerID and m.AreaID = md.AreaID" +
                            " and md.DiagnosticType = 10" +
                            " order by md.DiagTime desc) as DryBattCurrV," +
                            " (select top 1 md.DiagnosticValue from MeterDiagnostic md" +
                            " where m.MeterID = md.MeterID and m.CustomerID = md.CustomerID and m.AreaID = md.AreaID" +
                            " and md.DiagnosticType = 12" +
                            " order by md.DiagTime desc) as RechargeBattCurrV," +
                            " (select top 1 md.DiagnosticValue from MeterDiagnostic md" +
                            " where m.MeterID = md.MeterID and m.CustomerID = md.CustomerID and m.AreaID = md.AreaID" +
                            " and md.DiagnosticType = 9" +
                            " order by md.DiagTime desc) as DryBattMinV," +
                            " (select top 1 md.DiagnosticValue from MeterDiagnostic md" +
                            " where m.MeterID = md.MeterID and m.CustomerID = md.CustomerID and m.AreaID = md.AreaID" +
                            " and md.DiagnosticType = 11" +
                            " order by md.DiagTime desc) as RechargeBattMinV," +
                            " (select top 1 md.DiagTime from MeterDiagnostic md" +
                            " where m.MeterID = md.MeterID and m.CustomerID = md.CustomerID and m.AreaID = md.AreaID" +
                            " and md.DiagnosticType in (9, 10, 11, 12)" +
                            " order by md.DiagTime desc) as TimestampOfLatestBatteryVoltageReport" +
                            " from ParkingSpaces as ps" +
                            " left join Meters as m on ps.MeterId = m.MeterId and ps.CustomerId = m.CustomerId and ps.AreaId = m.AreaId" +
                            " left join MeterMap as mm on m.MeterId = mm.MeterId and m.CustomerId = mm.CustomerId and m.AreaId = mm.AreaId" +
                            " where m.CustomerId = @CustomerId" +
                            " and ps.HasSensor = 1 and m.MeterGroup = 0"); //and m.MaxBaysEnabled >= 0

                        sql.Append(" and m.MeterId in (");
                        loMeterDelim = string.Empty;
                        batchParamCount = 0;
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
                        sql.Append(")");

                        command.CommandText = sql.ToString();
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));
                    }
                    else if (reportParams.ReportOnHistoricalData == true)
                    {
                        sql.Append(
                            "select m.MeterID, m.MeterName, ps.BayNumber, " +
                            " (case when mm.AreaId2 is not null then mm.AreaId2 else ps.AreaId end) as AreaID, " +
                            " ps.HasSensor, m.MaxBaysEnabled, m.MeterGroup, m.MeterType," +
                            " md.DiagnosticValue, md.DiagnosticType, " +
                            " md.DiagTime as TimestampOfLatestBatteryVoltageReport" +
                            " from ParkingSpaces as ps" +
                            " left join Meters as m on ps.MeterId = m.MeterId and ps.CustomerId = m.CustomerId and ps.AreaId = m.AreaId" +
                            " left join MeterMap as mm on m.MeterId = mm.MeterId and m.CustomerId = mm.CustomerId and m.AreaId = mm.AreaId" +
                            " left join MeterDiagnostic as md on m.MeterID = md.MeterId and m.CustomerID = md.CustomerID and m.AreaID = md.AreaID" +
                            " where m.CustomerId = @CustomerId" +
                            " and ps.HasSensor = 1 and m.MeterGroup = 0" + //and m.MaxBaysEnabled >= 0
                            " and md.DiagnosticType in (9, 10, 11, 12)");

                        sql.Append(" and m.MeterId in (");
                        loMeterDelim = string.Empty;
                        batchParamCount = 0;
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
                        sql.Append(")");
                        sql.Append(" and md.DiagTime >= @StartTime and md.DiagTime <= @EndTime");

                        command.CommandText = sql.ToString();
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@StartTime", reportParams.StartTime));
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@EndTime", reportParams.EndTime));
                    }


                    // Get list of objects from the SQL query
                    List<SensorBatteryDiagnostics> partialResult = _Customer.SharedSqlDao.GetList<SensorBatteryDiagnostics>(command);

                    // Create final result object if needed
                    if (result == null)
                        result = new List<SensorBatteryDiagnostics>();

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


        #region Support for simulating vehicle sensor events from meter
        private string InsertVehicleSensingEventInDatabase(int CustomerId, int MeterID, int BayNumber, int AreaID, Int64 ParkingSpaceId_Internal, bool IsOccupied)
        {
            try
            {
                // Get current timestamp at customer's timezone
                DateTime NowAtDestination = Convert.ToDateTime(this._CustomerCfg.DestinationTimeZoneDisplayName);// Duncan.PEMS.SpaceStatus.UtilityClasses.TimeZoneInfo.ConvertTimeZoneToTimeZone(DateTime.Now, this._CustomerCfg.ServerTimeZone, this._CustomerCfg.CustomerTimeZone);

                // Build query to see if there is already an existing record 
                StringBuilder sql = new StringBuilder();
                sql.Append("Select pso.ParkingSpaceOccupancyId, pso.LastStatus, pso.LastUpdatedTS, ps.BayNumber, ps.MeterId," +
                    " ps.CustomerID, ps.ParkingSpaceId, pso.ParkingSpaceOccupancyId" +
                    " from ParkingSpaces ps, ParkingSpaceOccupancy pso" +
                    " where ps.CustomerId = @CustomerId and ps.ParkingSpaceId = pso.ParkingSpaceId and ps.MeterId in (");
                sql.Append("@MeterId");
                sql.Append(")");

                // Execute query to check for existing record that should be updated
                int intRecordsAffected = 0;
                using (SqlCommand command = _Customer.SharedSqlDao.GetSqlCommand(sql.ToString()))
                {
                    // Set query parameters
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@MeterId", MeterID));

                    // Execute SQL statement
                    intRecordsAffected = _Customer.SharedSqlDao.GetResultRowCount(command);
                }

                // Prepare to insert/update CURRENT sensor status
                string sqlCommandText = "";
                if (intRecordsAffected == 0)
                {
                    // No existing record, so insert new one
                    sqlCommandText = "insert into ParkingSpaceOccupancy (ParkingSpaceId, LastStatus, LastUpdatedTS) Values (" +
                        "@ParkingSpaceId, @LastStatus, @LastUpdatedTS)";
                }
                else
                {
                    // A record exists, so update it with new info
                    sqlCommandText = "update ParkingSpaceOccupancy set LastStatus = @LastStatus, LastUpdatedTS = @LastUpdatedTS " +
                      "where ParkingSpaceId = @ParkingSpaceId";
                }

                // Perform the Insert or Update into ParkingSpaceOccupancy table
                using (SqlCommand command = _Customer.SharedSqlDao.GetSqlCommand(sqlCommandText))
                {
                    // Set query parameters
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@ParkingSpaceId", ParkingSpaceId_Internal));
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@LastUpdatedTS", NowAtDestination));

                    if (IsOccupied)
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@LastStatus", 1));
                    else
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@LastStatus", 2)); // Not intuitive, but meter team is using "2 = vacant" in the DB!

                    // Execute SQL statement
                    intRecordsAffected = _Customer.SharedSqlDao.ExecuteNonQuery(command);
                }


                // Before we insert a historical sensor record, we must get the appropriate ParkingSpaceOccupancyID
                sql = new StringBuilder();
                sql.Append("Select pso.ParkingSpaceOccupancyId from ParkingSpaces ps, ParkingSpaceOccupancy pso" +
                    " where ps.CustomerId = @CustomerId and ps.ParkingSpaceId = pso.ParkingSpaceId and ps.MeterId in (");
                sql.Append("@MeterId");
                sql.Append(")");
                Int64 ParkingSpaceOccupancyId = 0;
                using (SqlCommand command = _Customer.SharedSqlDao.GetSqlCommand(sql.ToString()))
                {
                    // Set query parameters
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@CustomerId", CustomerId));
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@MeterId", MeterID));

                    // Execute SQL statement
                    ParkingSpaceOccupancyId = _Customer.SharedSqlDao.GetSingleBigInt(command);
                }


                // We must also insert a historical sensor status record in the audit table
                sqlCommandText = "insert into ParkingSpaceOccupancyAudit (ParkingSpaceOccupancyId, LastStatus, LastUpdatedTS, RecCreationDate) Values (" +
                    "@ParkingSpaceOccupancyId, @LastStatus, @LastUpdatedTS, GetDate())";
                using (SqlCommand command = _Customer.SharedSqlDao.GetSqlCommand(sqlCommandText))
                {
                    // Set query parameters
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@ParkingSpaceOccupancyId", ParkingSpaceOccupancyId));
                    command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@LastUpdatedTS", NowAtDestination));

                    if (IsOccupied)
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@LastStatus", 1));
                    else
                        command.Parameters.Add(_Customer.SharedSqlDao.CreateParameter("@LastStatus", 2)); // Not intuitive, but meter team is using "2 = vacant" in the DB!

                    // Execute SQL statement
                    intRecordsAffected = _Customer.SharedSqlDao.ExecuteNonQuery(command);
                }

                if (IsOccupied)
                    return "OK. Occupied event recorded at " + NowAtDestination.ToString("yyyy-MM-dd") + " @ " + NowAtDestination.ToString("hh:mm:ss tt"); // ToShortTimeString();
                else
                    return "OK. Vacancy event recorded at " + NowAtDestination.ToString("yyyy-MM-dd") + " @ " + NowAtDestination.ToString("hh:mm:ss tt"); // ToShortTimeString();
            }
            catch (Exception loException)
            {
                return "Failed to simulate event due to error: " + loException.Message;
            }
        }

        public string SimulateVehicleSensingEvent(int CustomerId, int MeterID, int BayNumber, int AreaID, Int64 ParkingSpaceId_Internal, bool IsOccupied)
        {
            // The UDP method would be preferred, but for some reason its not yielding a change to data in database (even though PARM response is OK)
            // As a work-around, we will manipulate the database directly, but that means PAM/DataBus might not know about stuff we do behind the scenes...
            return InsertVehicleSensingEventInDatabase(CustomerId, MeterID, BayNumber, AreaID, ParkingSpaceId_Internal, IsOccupied);

            #region Deprecated
            // Developed against Liberty SSM vehicle sensor info at: http://jirssb01.cm.pam:9090/display/RPMS/UDP+Listener+Request-Response+Structures#UDPListenerRequest-ResponseStructures-54
            /*
            UDP Request type: 54 - Space Occupancy Status

            Request packet byte layout:
            1-2         3-4   5-6   7-8   9-10         11             12-15
            CommHeader  AID   CID   MID   BayNumber    SpaceStatus    MeterTime

              CommHeader:
                Type = 54 (Space status)
                Flag = 0 (not encrypted)
  
              Space Status values:
                0	Vacant
                1	Occupied
                2	Sensor Broken
                3	Unknown  
        
            Response packet byte layout:
            1-10
            GResH (Same as response for PAM cluster status, etc)
            */

            // This code sent info to UDP without error, but didn't yield the expected change in DB
            /*
            UDPSocket = null;
            UDPServerEndPoint = null;
            UseBigEndian = false;
            _PAMServerTCPAddress = this._Customer.PAMServerTCPAddress;
            _PAMServerTCPPort = 0;
            Int32.TryParse(this._Customer.PAMServerTCPPort, out _PAMServerTCPPort);

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
                byte[] srcBuffer = new byte[15];
                MemoryStream bufferStream = null;
                BinaryReader bufferReader = null;
                BinaryWriter bufferWriter = null;
                bufferStream = new MemoryStream(srcBuffer);
                bufferReader = new BinaryReader(bufferStream);
                bufferWriter = new BinaryWriter(bufferStream);

                // Set Common Header Data
                SetByte(0, 0, bufferWriter); // Flag = 0 (Not Encrypted)
                    SetByte(54, 1, bufferWriter); // Type = 54 (Space Occupancy Status)

                // Set Common General Data
                SetWord(Convert.ToUInt16(AreaID), 2, bufferWriter);       // Area ID 
                SetWord(Convert.ToUInt16(CustomerId), 4, bufferWriter);  // Customer ID
                SetWord(Convert.ToUInt16(MeterID), 6, bufferWriter);     // Meter ID
                SetWord(Convert.ToUInt16(BayNumber), 8, bufferWriter);   // Bay Number

                // Set occupancy status
                if (IsOccupied)
                    SetByte(1, 10, bufferWriter);
                else
                    SetByte(0, 10, bufferWriter); 

                // Determine the current timestamp in customer's timezone
                DateTime NowAtDestination = UtilityClasses.TimeZoneInfo.ConvertTimeZoneToTimeZone(DateTime.Now, this._CustomerCfg.ServerTimeZone, this._CustomerCfg.CustomerTimeZone);
                TimeSpan loHundredNanoSeconds = new TimeSpan(NowAtDestination.Ticks - new DateTime(2000, 1, 1, 0, 0, 0).Ticks);

                // Set timestamp
                //SetLongWord(Convert.ToUInt32(loHundredNanoSeconds.TotalSeconds), 11, bufferWriter); // Timestamp of vehicle sensor event 
                SetLongInt(Convert.ToInt32(loHundredNanoSeconds.TotalSeconds), 11, bufferWriter); // Timestamp of vehicle sensor event 

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
                    int noMoreDataLoopCount = 0;
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
                                break;
                            }
                            else
                            {
                                loUDPDataAvail = UDPSocket.Available;
                            }
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
                            break;
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
                            if (msFullResponse.Length >= 10)
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
                                else if (InnerContentLength == 0)
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
                ////try
                ////{
                ////    Logging.AddTextToGenericLog(_ServerLogPathAndFileName, "RIPNET response code: " + ApacheResponseCodeValue.ToString() + " -- " + ApacheResponseCode.ToString());
                ////}
                ////catch { }

                // Read the content length from the byte array
                UInt32 ContentLength = ReadLongWord(6, bufferReader);

                // Close stream resources
                bufferReader.Close();
                bufferWriter.Close();
                bufferStream.Close();
                bufferStream.Dispose();

                if (IsOccupied)
                    return ApacheResponseCode.ToString() + ". Occupied event recorded at " + NowAtDestination.ToString("yyyy-MM-dd") + " @ " + NowAtDestination.ToShortTimeString();
                else
                    return ApacheResponseCode.ToString() + ". Vacancy event recorded at " + NowAtDestination.ToString("yyyy-MM-dd") + " @ " + NowAtDestination.ToShortTimeString();
            }
            catch (Exception loException)
            {
                return "Failed to simulate event due to error: " + loException.Message;
            }
            */
            #endregion
        }

        #region Deprecated
        /*
    // Variables used for PAM TCP/UDP support
    private Socket UDPSocket = null;
    private EndPoint UDPServerEndPoint = null;
    private bool UseBigEndian = false;
    private string _PAMServerTCPAddress = "";
    private int _PAMServerTCPPort = 0;
    */

        /*
        private void OpenUDPSocket()
        {
            // Shutdown any previous socket first
            CloseUDPSocket();

            // PAM supports both TCP and UDP. We will use TCP because it is more reliable
            IPAddress serverAddress = IPAddress.Parse(_PAMServerTCPAddress);
            UDPServerEndPoint = new IPEndPoint(serverAddress, Convert.ToInt32(_PAMServerTCPPort));

            // Create a TCP socket to the server
            UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect socket (Required when using TCP; Not required for UDP)
            UDPSocket.Connect(UDPServerEndPoint);
        }

        private void CloseUDPSocket()
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
        */
        #endregion

        #endregion

        #region Binary Read Support (Deprecated)
        /*
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
    */
        #endregion

        #region Binary Write Support (Deprecated)
        /*
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
    */
        #endregion
    }

    public sealed class HistoricalSensingRecordsSorter : System.Collections.Generic.IComparer<HistoricalSensingRecord>
    {
        private bool _sortAscending = true;

        private static readonly System.Collections.Generic.IComparer<HistoricalSensingRecord> _default = new HistoricalSensingRecordsSorter(true);

        public HistoricalSensingRecordsSorter(bool SortAscending)
        {
            _sortAscending = SortAscending;
        }

        public static System.Collections.Generic.IComparer<HistoricalSensingRecord> Default
        {
            get { return _default; }
        }

        public int Compare(HistoricalSensingRecord s1, HistoricalSensingRecord s2)
        {
            // We are sorting by date
            if (_sortAscending)
                return s1.DateTime.CompareTo(s2.DateTime);
            else
                return s2.DateTime.CompareTo(s1.DateTime);
        }
    }

}