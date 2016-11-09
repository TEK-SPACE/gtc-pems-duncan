/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             __________________________________________________________________________________________________
 * 01/17/2014       Sergey Ostrerov                 DPTXPEMS-218 - AlarmCode-0 Details page - Server Error in '/' Application
 * 02/10/2014       Sergey Ostrerov                 JIRA: DPTXPEMS-225  Added CustomerId to where clause for any selects from PEMS Areas table 
 * 02/18/2014       Sergey Ostrerov                       DPTXPEMS-213 - Time formatting:Alarm Duration.
 * *****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Alarms;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using WebMatrix.WebData;
using TimeType = Duncan.PEMS.Entities.Alarms.TimeType;

namespace Duncan.PEMS.Business.Alarms
{
    /// <summary>
    /// The <see cref="Duncan.PEMS.Business.Alarms"/> namespace contains classes for managing alarms.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }





    /// <summary>
    /// Business logic class for Alarms.  Inherits from <see cref="BaseFactory"/>
    /// </summary>
    public class AlarmFactory : BaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Constructor for <see cref="AlarmFactory"/> 
        /// </summary>
        /// <param name="connectionStringName">Connection string name pointing to PEMS instance for this factory</param>
        public AlarmFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        protected  IQueryable<TSD> Tsds;

        /// <summary>
        /// Sets the target service designations for the customer passed in.
        /// </summary>
        /// <param name="customerID">ID of the customer to populate the Target service designations</param>
        private  void SetTSDS(int customerID)
        {
            var tsds = PemsEntities.TargetServiceDesignations
                           .Where(x => x.CustomerId == customerID)
                           .Join(PemsEntities.TargetServiceDesignationMasters, tsd => tsd.TargetServiceDesignationDesc, tsdm => tsdm.TargetServiceDesignationDesc, (tsd, tsdm) => new { tsd, tsdm })
                           .Select(x => new TSD()
                           {
                               Id = x.tsd.TargetServiceDesignationId,
                               Description = x.tsd.TargetServiceDesignationDesc,
                               MasterId = x.tsdm.TargetServiceDesignationId
                           });
            Tsds = tsds;
        }

        #region "Details"

        /// <summary>
        /// Gets an active alarm based on the parameters passed in
        /// </summary>
        public AlarmModel GetActiveAlarmDetails(int customerID, int areaID, int meterID, int eventCode, int eventSource, DateTime occuranceTime, DateTime localTime)
        {
            var alarmModel = new AlarmModel();
            try
            {
                var alarm = PemsEntities.ActiveAlarms.FirstOrDefault(x => x.CustomerID == customerID && x.AreaID == areaID && x.MeterId == meterID && x.EventCode == eventCode && x.EventSource == eventSource && x.TimeOfOccurrance == occuranceTime);
                alarmModel = GetActiveAlarmDetails(customerID, alarm, localTime);
            }
            catch (Exception ex)
            {
            }

            return alarmModel;
        }

        /// <summary>
        /// Gets an active alarm based on the parameters passed in
        /// </summary>
        public AlarmModel GetActiveAlarmDetails(int customerID, int eventId, DateTime localTime)
        {
            var alarmModel = new AlarmModel();
            try
            {
                var alarm = PemsEntities.ActiveAlarms.FirstOrDefault( x => x.CustomerID == customerID && x.EventUID == eventId );
                alarmModel = GetActiveAlarmDetails(customerID, alarm, localTime);
            }
            catch( Exception ex )
            {
            }

            return alarmModel;
        }

        /// <summary>
        /// Gets an active alarm based on the parameters passed in
        /// </summary>
        private AlarmModel GetActiveAlarmDetails(int customerID, ActiveAlarm alarm, DateTime localTime)
        {
            var alarmModel = new AlarmModel();
            if ( alarm != null )
            {
                //Get the meter map for this alarm
                MeterMap metermap =
                    PemsEntities.MeterMaps.FirstOrDefault(
                        x =>
                        x.Areaid == alarm.AreaID && x.Customerid == alarm.CustomerID &&
                        x.MeterId == alarm.MeterId );

                //Asset / Location Information
                alarmModel.EventSource = alarm.EventSource;
                alarmModel.Latitude = alarm.Meter == null ? 0 : alarm.Meter.Latitude ?? 0;
                alarmModel.Longitude = alarm.Meter == null ? 0 : alarm.Meter.Longitude ?? 0;
                alarmModel.MeterGroupId = alarm.Meter == null ? 0 : alarm.Meter.MeterGroup ?? 0;
                alarmModel.AssetClass = alarm.Meter == null ? AssetClass.Unknown : AssetFactory.GetAssetClass( alarm.Meter.MeterGroup );
                alarmModel.AssetType = alarm.Meter == null ? string.Empty : (new AssetFactory(ConnectionStringName)).GetAssetTypeDescription(alarm.Meter.MeterGroup, customerID);
                alarmModel.CustomerId = customerID;
                //name ans state are based on class
                OperationalStatu opStatus = null;

                if (alarmModel.AssetClass == AssetClass.Sensor)
                {
                    alarmModel.AssetName = PemsEntities.Sensors.FirstOrDefault(x => x.SensorID == metermap.SensorID) ==null? "": PemsEntities.Sensors.FirstOrDefault(x => x.SensorID == metermap.SensorID).SensorName;
                    opStatus = PemsEntities.OperationalStatus.FirstOrDefault( x => x.OperationalStatusId == metermap.Sensor.OperationalStatus );
                    alarmModel.OperationalStatus = opStatus == null ? "" : opStatus.OperationalStatusDesc ?? "";
                }
                else if (alarmModel.AssetClass == AssetClass.Gateway)
                {
                    alarmModel.AssetName = PemsEntities.Gateways.FirstOrDefault(x => x.GateWayID == metermap.GatewayID) == null ? "" : PemsEntities.Gateways.FirstOrDefault(x => x.GateWayID == metermap.GatewayID).Description;
                    opStatus = PemsEntities.OperationalStatus.FirstOrDefault(x => x.OperationalStatusId == metermap.Gateway.OperationalStatus);
                    alarmModel.OperationalStatus = opStatus == null ? "" : opStatus.OperationalStatusDesc ?? "";
                }
                    //everything else falls under meter for now
                else
                {
                    alarmModel.AssetName = alarm.Meter == null ? "" : alarm.Meter.MeterName;
                    opStatus = PemsEntities.OperationalStatus.FirstOrDefault(x => x.OperationalStatusId == metermap.Meter.OperationalStatusID);
                    alarmModel.OperationalStatus = opStatus == null ? "" : opStatus.OperationalStatusDesc ?? "";
                }
                alarmModel.AssetID = alarm.Meter.MeterId;
                alarmModel.Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == metermap.Customerid && x.AreaID == metermap.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == metermap.Customerid && x.AreaID == metermap.AreaId2).AreaName;
                alarmModel.AreaId = metermap.AreaId2; //alarm.Meter.Area.AreaID;
                alarmModel.Zone = metermap.Zone == null ? string.Empty : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metermap.ZoneId && metermap.Customerid == x.customerID) == null ? string.Empty : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metermap.ZoneId && metermap.Customerid == x.customerID).ZoneName;
                alarmModel.Suburb = metermap == null? string.Empty: metermap.CustomGroup11 == null ? string.Empty : metermap.CustomGroup11.DisplayName;
                alarmModel.Street = alarm.Meter.Location;
                alarmModel.BaysAffected = alarm.Meter.MaxBaysEnabled ?? 0;

                //Alarm / Meter information
                alarmModel.AlarmID = alarm.AlarmUID ?? 0;
                alarmModel.AlarmCode = alarm.EventCode1.EventCode1;
                alarmModel.AlarmDesription = alarm.EventCode1.EventDescAbbrev;
                alarmModel.AlarmDesriptionVerbose = alarm.EventCode1.EventDescVerbose;
                alarmModel.ServiceTargetTime = alarm.SLADue ?? DateTime.MinValue;
                alarmModel.AlarmStatus = "Open";
                var alarmStatus = PemsEntities.AlarmStatus.FirstOrDefault( x => x.AlarmStatusId == 2 );
                if ( alarmStatus != null )
                    alarmModel.AlarmStatus = alarmStatus.AlarmStatusDesc;
                alarmModel.TimeNotified = alarm.TimeOfNotification;
                alarmModel = DetermineSlaValues(alarmModel, alarm, localTime);
                alarmModel.TimeOccured = alarm.TimeOfOccurrance;
                //Time Types here

                AddTimeTypes( alarm, alarmModel );
                alarmModel.TimeType1 = alarm.TimeType1 ?? 0;
                alarmModel.TimeType2 = alarm.TimeType2 ?? 0;
                alarmModel.TimeType3 = alarm.TimeType3 ?? 0;
                alarmModel.TimeType4 = alarm.TimeType4 ?? 0;
                alarmModel.TimeType5 = alarm.TimeType5 ?? 0;
                alarmModel.AlarmSource = PemsEntities.EventSources.FirstOrDefault( x => x.EventSourceCode == alarm.EventSource ).EventSourceDesc;
                alarmModel.AlarmSeverity = alarm.EventCode1.AlarmTier1 == null ? string.Empty : alarm.EventCode1.AlarmTier1.TierDesc;
                alarmModel.EntryNotes = alarm.Notes;
                alarmModel.IsClosed = false;
                alarmModel.CustomerId = customerID;
                //Closure Information -  this alarm hasnt been closed, so most of them are not needed
                //closed by is current user
                alarmModel.ClosedBy = WebSecurity.CurrentUserId;
                alarmModel.ClosedByName = WebSecurity.CurrentUserName;
            }

            alarmModel.ResolutionCodes = GetResolutionCodes();
            alarmModel.TechnicianIDs = GetAlarmTechnicians();
            return alarmModel;
        }

        /// <summary>
        /// Updates the alarm model to include the correct time  types for this active alarm.
        /// </summary>
        private void AddTimeTypes(ActiveAlarm alarm, AlarmModel alarmModel)
        {
            alarmModel.TimeTypes = new List<TimeType>();
            if (alarm.TimeType1 != null)
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == alarm.TimeType1);
                if (tt != null)
                    alarmModel.TimeTypes.Add(new TimeType {Description = tt.TimeTypeDesc, Id = tt.TimeTypeId});
            }
            if (alarm.TimeType2 != null)
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == alarm.TimeType2);
                if (tt != null)
                    alarmModel.TimeTypes.Add(new TimeType {Description = tt.TimeTypeDesc, Id = tt.TimeTypeId});
            }
            if (alarm.TimeType3 != null)
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == alarm.TimeType3);
                if (tt != null)
                    alarmModel.TimeTypes.Add(new TimeType {Description = tt.TimeTypeDesc, Id = tt.TimeTypeId});
            }
            if (alarm.TimeType4 != null)
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == alarm.TimeType4);
                if (tt != null)
                    alarmModel.TimeTypes.Add(new TimeType {Description = tt.TimeTypeDesc, Id = tt.TimeTypeId});
            }
            if (alarm.TimeType5 != null)
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == alarm.TimeType5);
                if (tt != null)
                    alarmModel.TimeTypes.Add(new TimeType {Description = tt.TimeTypeDesc, Id = tt.TimeTypeId});
            }
        }

        /// <summary>
        /// Determines she SLA times for the alarm model based on the active alarm passed in and the local time.
        /// </summary>
        private AlarmModel DetermineSlaValues(AlarmModel alarmModel, ActiveAlarm alarm, DateTime localTime)
        {
            //set the defaults
            alarmModel.ServiceDesignation = "N/A";
            alarmModel.ServiceDesignationId = -1;
            alarmModel.TimeRemainingTillTargetTime = 0;
            alarmModel.TimeRemainingTillTargetTimeDisplay = "0";

            SetTSDS(alarm.CustomerID);
            if (alarm.SLADue.HasValue)
            {
                //set the time remaining until the sla is due - have to set this against the local customer time
                var diffTime = (alarm.SLADue.Value - localTime);
                alarmModel.TimeRemainingTillTargetTime = diffTime.TotalHours;
                if (diffTime.TotalHours > 0)
                    alarmModel.TimeRemainingTillTargetTimeDisplay = FormatHelper.FormatTimeFromMinutes(diffTime.TotalMinutes);
                //calculate the target  service designnation here as well
                var masterId = GetTsdMasterId(diffTime.TotalHours);
                //now determine the correct TSD Master
                var tds = Tsds.FirstOrDefault(x => x.MasterId == masterId);
                if (tds != null)
                {
                    alarmModel.ServiceDesignation = tds.Description;
                    alarmModel.ServiceDesignationId = tds.Id;
                }
            }
            return alarmModel;
        }

        /// <summary>
        /// Gets details of a historic alarm
        /// </summary>
        /// <returns></returns>
        public AlarmModel GetHistoricAlarmDetails(int customerID, int eventId)
        {
            var alarmModel = new AlarmModel();
            try
            {
                var alarm = PemsEntities.HistoricalAlarms.FirstOrDefault( x => x.CustomerID == customerID && x.EventUID == eventId );
                alarmModel = GetHistoricalAlarmDetails( customerID, alarm );
            }
            catch( Exception ex )
            {
            }

            return alarmModel;
        }

        /// <summary>
        /// Gets details of a historic alarm
        /// </summary>
        public AlarmModel GetHistoricAlarmDetails(int customerID, int areaID, int meterID, int eventCode, int eventSource, DateTime occuranceTime)
        {
            var alarmModel = new AlarmModel();
            try
            {
                var alarm = PemsEntities.HistoricalAlarms.FirstOrDefault(x => x.CustomerID == customerID && x.AreaID == areaID && x.MeterId == meterID && x.EventCode == eventCode && x.EventSource == eventSource && x.TimeOfOccurrance == occuranceTime);
              alarmModel =  GetHistoricalAlarmDetails( customerID, alarm );
            }
            catch (Exception ex)
            {
            }

            

            return alarmModel;
        }

        /// <summary>
        /// Gets details of a historic alarm
        /// </summary>
        private AlarmModel GetHistoricalAlarmDetails(int customerID, HistoricalAlarm alarm)
        {
            var alarmModel = new AlarmModel();
            if ( alarm != null )
            {
                //Get the meter map for this alarm
                var metermap =
                    PemsEntities.MeterMaps.FirstOrDefault( x => x.Areaid == alarm.AreaID && x.Customerid == alarm.CustomerID && x.MeterId == alarm.MeterId );
                var alarmEventCode = PemsEntities.EventCodes.FirstOrDefault( x => x.EventCode1 == alarm.EventCode && x.CustomerID == customerID );
                //Asset / Location Information
                alarmModel.Latitude = alarm.Meter == null ? 0 : alarm.Meter.Latitude ?? 0;
                alarmModel.Longitude = alarm.Meter == null ? 0 : alarm.Meter.Longitude ?? 0;
                alarmModel.MeterGroupId = alarm.Meter == null ? 0 : alarm.Meter.MeterGroup ?? 0;
                alarmModel.AssetClass = alarm.Meter == null ? AssetClass.Unknown : AssetFactory.GetAssetClass(alarm.Meter.MeterGroup);
                alarmModel.AssetType = alarm.Meter == null ? string.Empty : (new AssetFactory(ConnectionStringName)).GetAssetTypeDescription(alarm.Meter.MeterGroup, customerID);

                OperationalStatu opStatus = null;
                //name ans state are based on class
                if (alarmModel.AssetClass == AssetClass.Sensor)
                {
                    alarmModel.AssetName = PemsEntities.Sensors.FirstOrDefault(x => x.SensorID == metermap.SensorID) == null ? "" : PemsEntities.Sensors.FirstOrDefault(x => x.SensorID == metermap.SensorID).SensorName;
                    opStatus = PemsEntities.OperationalStatus.FirstOrDefault(x => x.OperationalStatusId == metermap.Sensor.OperationalStatus);
                    alarmModel.OperationalStatus = opStatus == null ? "" : opStatus.OperationalStatusDesc ?? "";
                }
                else if (alarmModel.AssetClass == AssetClass.Gateway)
                {
                    alarmModel.AssetName = PemsEntities.Gateways.FirstOrDefault(x => x.GateWayID == metermap.GatewayID) == null ? "" : PemsEntities.Gateways.FirstOrDefault(x => x.GateWayID == metermap.GatewayID).Description;
                    opStatus = PemsEntities.OperationalStatus.FirstOrDefault(x => x.OperationalStatusId == metermap.Gateway.OperationalStatus);
                    alarmModel.OperationalStatus = opStatus == null ? "" : opStatus.OperationalStatusDesc ?? "";
                }
                //everything else falls under meter for now
                else
                {
                    alarmModel.AssetName = alarm.Meter == null ? "" : alarm.Meter.MeterName;
                    opStatus = PemsEntities.OperationalStatus.FirstOrDefault(x => x.OperationalStatusId == metermap.Meter.OperationalStatusID);
                    alarmModel.OperationalStatus = opStatus == null ? "" : opStatus.OperationalStatusDesc ?? "";
                }
                alarmModel.AssetID = alarm.Meter.MeterId;
                alarmModel.Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == metermap.Customerid && x.AreaID == metermap.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.AreaID == metermap.AreaId2 && x.CustomerID == metermap.Customerid).AreaName;//alarm.Meter.Area.AreaName;
                alarmModel.AreaId = metermap.AreaId2;// alarm.Meter.Area.AreaID;
                alarmModel.Zone = metermap.Zone == null ? string.Empty : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metermap.ZoneId && metermap.Customerid == x.customerID) == null ? string.Empty : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metermap.ZoneId && metermap.Customerid == x.customerID).ZoneName;
                alarmModel.Suburb = metermap == null ? string.Empty : metermap.CustomGroup11 == null ? string.Empty : metermap.CustomGroup11.DisplayName;
                alarmModel.Street = alarm.Meter.Location;
                alarmModel.BaysAffected = alarm.Meter.MaxBaysEnabled ?? 0;

                //Alarm / Meter information
                alarmModel.AlarmID = alarm.AlarmUID ?? 0;
                alarmModel.AlarmCode = alarmEventCode == null ? 0 : alarmEventCode.EventCode1;
                alarmModel.AlarmDesription = alarmEventCode == null ? string.Empty : alarmEventCode.EventDescAbbrev;
                alarmModel.AlarmDesriptionVerbose = alarmEventCode == null ? string.Empty : alarmEventCode.EventDescVerbose;
                alarmModel.ServiceTargetTime = alarm.SLADue ?? DateTime.MinValue;
                alarmModel.AlarmStatus = "Closed";
                var alarmStatus = PemsEntities.AlarmStatus.FirstOrDefault( x => x.AlarmStatusId == 3 );
                if ( alarmStatus != null )
                    alarmModel.AlarmStatus = alarmStatus.AlarmStatusDesc;
                alarmModel.TimeNotified = alarm.TimeOfNotification;
                alarmModel.TimeCleared = alarm.TimeOfClearance ?? DateTime.MinValue;
               
                //SLA Values
                //calculation for historical alarm time remaining: Service Target time - Time Cleared. NOTE, this is different for active alarms. using time cleared instead of current local time
                //set the defaults
                alarmModel.TimeRemainingTillTargetTime = 0;
                alarmModel.TimeRemainingTillTargetTimeDisplay = Constants.TimeFormats.timeFormatToDisplay_HHH_MM;

                if (alarm.SLADue.HasValue)
                {
                    if (!String.IsNullOrEmpty(alarm.SLADue.Value.ToString()))
                    {
                        TimeSpan diffTime = (alarm.SLADue.Value - alarm.TimeOfClearance.Value);
                        if (diffTime.TotalHours > 0)
                            alarmModel.TimeRemainingTillTargetTimeDisplay = FormatHelper.FormatTimeFromMinutes(Math.Round(diffTime.TotalMinutes, 0), (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HHH_MM);
                    }
                }
                else
                {
                    alarmModel.TimeRemainingTillTargetTimeDisplay = FormatHelper.FormatTimeFromMinutes(0, (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HHH_MM);
                }

                //Target Service Designation - note, we are NOT regenerating this, just using what is assigned to the 
                alarmModel.ServiceDesignation = "N/A";
                alarmModel.ServiceDesignationId = -1;
                if (alarm.TargetServiceDesignation1 != null)
                {
                    alarmModel.ServiceDesignation = alarm.TargetServiceDesignation1.TargetServiceDesignationDesc;
                    alarmModel.ServiceDesignationId = alarm.TargetServiceDesignation1.TargetServiceDesignationId;
                }

                alarmModel.TimeOccured = alarm.TimeOfOccurrance;
                AddTimeTypes( alarm, alarmModel );
                alarmModel.TimeType1 = alarm.TimeType1 ?? 0;
                alarmModel.TimeType2 = alarm.TimeType2 ?? 0;
                alarmModel.TimeType3 = alarm.TimeType3 ?? 0;
                alarmModel.TimeType4 = alarm.TimeType4 ?? 0;
                alarmModel.TimeType5 = alarm.TimeType5 ?? 0;
                alarmModel.AlarmSource = PemsEntities.EventSources.FirstOrDefault( x => x.EventSourceCode == alarm.EventSource ).EventSourceDesc;
                alarmModel.AlarmSeverity = alarmEventCode == null ? string.Empty : alarmEventCode.AlarmTier1 == null ? string.Empty : alarmEventCode.AlarmTier1.TierDesc;
                alarmModel.EntryNotes = alarm.Notes;
                alarmModel.CustomerId = customerID;
                //Closure Information 
                var maintenanceEvent = PemsEntities.SFMeterMaintenanceEvents.FirstOrDefault(
                    x => x.AreaId == alarm.Meter.AreaID && x.CustomerId == customerID && x.MeterId == alarm.MeterId && x.WorkOrderID == alarm.WorkOrderId);
                alarmModel.ResolutionCode = maintenanceEvent == null ? 1 : maintenanceEvent.MaintenanceCode;
                alarmModel.ResolutionCodeDesc = maintenanceEvent == null
                                                    ? ""
                                                    : PemsEntities.MaintenanceCodes.FirstOrDefault( x => x.MaintenanceCode1 == maintenanceEvent.MaintenanceCode ) == null
                                                          ? ""
                                                          : PemsEntities.MaintenanceCodes.FirstOrDefault( x => x.MaintenanceCode1 == maintenanceEvent.MaintenanceCode ).Description;

               //RJH: Set ClosureNotification = TimeCleared if no maintenance event.
                alarmModel.ClosureNotification = maintenanceEvent == null ? alarmModel.TimeCleared : maintenanceEvent.EventDateTime;
               
                alarmModel.TechnicianID = maintenanceEvent == null ? -1 : maintenanceEvent.TechnicianId;
                if (alarmModel.TechnicianID > 0)
                {
                    alarmModel.TechnicianName =
                        PemsEntities.TechnicianDetails.FirstOrDefault(x => x.TechnicianId == alarmModel.TechnicianID) ==
                        null
                            ? ""
                            : PemsEntities.TechnicianDetails.FirstOrDefault(
                                x => x.TechnicianId == alarmModel.TechnicianID).Name;
                }
                alarmModel.ClosureNotes = alarm.ClosureNote;
                alarmModel.ClosedBy = alarm.ClearedByUserId ?? 0;
                if ( alarm.ClearedByUserId != null )
                {
                    var user = ( new UserFactory() ).GetUserById( alarm.ClearedByUserId.Value );
                    if ( user != null )
                        alarmModel.ClosedByName = user.Username;
                }
                alarmModel.IsClosed = true;
            }

            alarmModel.ResolutionCodes = GetResolutionCodes();
            alarmModel.TechnicianIDs = GetAlarmTechnicians();

            return alarmModel;
        }

        /// <summary>
        /// Adds the time types to the alarm model based on the historic alarm passed in.
        /// </summary>
        private  void AddTimeTypes(HistoricalAlarm alarm, AlarmModel alarmModel)
        {
            alarmModel.TimeTypes = new List<TimeType>();
            if (alarm.TimeType1 != null)
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == alarm.TimeType1);
                if (tt != null)
                    alarmModel.TimeTypes.Add(new TimeType { Description = tt.TimeTypeDesc, Id = tt.TimeTypeId });
            }
            if (alarm.TimeType2 != null)
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == alarm.TimeType2);
                if (tt != null)
                    alarmModel.TimeTypes.Add(new TimeType { Description = tt.TimeTypeDesc, Id = tt.TimeTypeId });
            }
            if (alarm.TimeType3 != null)
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == alarm.TimeType3);
                if (tt != null)
                    alarmModel.TimeTypes.Add(new TimeType { Description = tt.TimeTypeDesc, Id = tt.TimeTypeId });
            }
            if (alarm.TimeType4 != null)
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == alarm.TimeType4);
                if (tt != null)
                    alarmModel.TimeTypes.Add(new TimeType { Description = tt.TimeTypeDesc, Id = tt.TimeTypeId });
            }
            if (alarm.TimeType5 != null)
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == alarm.TimeType5);
                if (tt != null)
                    alarmModel.TimeTypes.Add(new TimeType { Description = tt.TimeTypeDesc, Id = tt.TimeTypeId });
            }
        }

        /// <summary>
        /// Clears an alarm. Creates a historic alarm, maintenance event, and removed the current active alarm from the system.
        /// </summary>
        public void ClearAlarm(AlarmModel model)
        {
            //get the original alarm item
            var alarm =
                PemsEntities.ActiveAlarms.FirstOrDefault(
                    x =>
                    x.CustomerID == model.CustomerId && x.AreaID == model.AreaId && x.MeterId == model.AssetID &&
                    x.EventCode == model.AlarmCode && x.EventSource == model.EventSource &&
                    x.TimeOfOccurrance == model.TimeOccured);
            if (alarm != null)
            {
                //first, we have to check for a work order, and if it doesnt exist, create one so we can link the new historical alarm to the sf maint event.
                if (!alarm.WorkOrderId.HasValue)
                {
                    var wOrder = new WorkOrder
                        {
                            AssignedBy = WebSecurity.CurrentUserId,
                            AssignedTS = DateTime.Now,
                            CustomerID = model.CustomerId,
                            TechnicianID = model.TechnicianID
                        };
                    PemsEntities.WorkOrders.Add(wOrder);
                    PemsEntities.SaveChanges();

                    //now assign the alarm the newly created work order id
                    alarm.WorkOrderId = wOrder.WorkOrderId;
                    PemsEntities.SaveChanges();
                }
                
                //now lets calculate the service designation
                SetTSDS(alarm.CustomerID);
                var masterId = GetTsdMasterId(model.TimeCleared, alarm.SLADue);
                //calculate the target  service designnation here as well
                var tds = Tsds.FirstOrDefault(x => x.MasterId == masterId);

                //we will try to update an existing historical alarm if it already exists, otherwise we will insert a new one. 
              //there is some old data that has duplicates in the historical and active, so this gracefully takes care of that, since all we want to do if the historical alarm exist is update the record with the active alarm information.
                var ExistinghistoricAlarm =
                    PemsEntities.HistoricalAlarms.FirstOrDefault(
                        x => x.CustomerID == alarm.CustomerID && x.AreaID == alarm.AreaID && x.MeterId == alarm.MeterId
                             && x.EventCode == alarm.EventCode && x.EventSource == alarm.EventSource &&
                             x.TimeOfOccurrance == alarm.TimeOfOccurrance && x.EventState == 0);

                //if it was found, we have to delete the historical item and re-create one, since the eventUID is an identity column, we cant change it, but its not part of the primary key, so it might need to change.
                //So, we have to try to get the hsitorical alarm, delete it, then try to create a new one.
                if (ExistinghistoricAlarm != null)
                {
                    PemsEntities.HistoricalAlarms.Remove(ExistinghistoricAlarm);
                  PemsEntities.SaveChanges();
                }
                //Now we need to create a new one
                    //we have to insert a historical alarm
                    var historicAlarm = new HistoricalAlarm
                        {
                            AlarmUID = alarm.AlarmUID,
                            AreaID = alarm.AreaID,
                            ClearedByUserId = model.ClosedBy,
                            ClearingEventUID = 0,
                            ClosureNote = model.ClosureNotes,
                            CustomerID = alarm.CustomerID,
                            EventCode = alarm.EventCode,
                            EventSource = alarm.EventSource,
                            EventState = 0,
                            EventUID = (int) (alarm.EventUID ?? 0),
                            GlobalMeterId = alarm.GlobalMeterID,
                            MeterId = alarm.MeterId,
                            Notes = alarm.Notes,
                            SLADue = alarm.SLADue,
                            TimeOfClearance = model.TimeCleared,
                            TimeOfNotification = alarm.TimeOfNotification,
                            TimeOfOccurrance = alarm.TimeOfOccurrance,
                            TimeType1 = alarm.TimeType1,
                            TimeType2 = alarm.TimeType2,
                            TimeType3 = alarm.TimeType3,
                            TimeType4 = alarm.TimeType4,
                            TimeType5 = alarm.TimeType5,
                            WorkOrderId = alarm.WorkOrderId
                        };
                    //now determine the correct TSD Master
                    if (tds != null)
                        historicAlarm.TargetServiceDesignation = tds.Id;

                    PemsEntities.HistoricalAlarms.Add(historicAlarm);
                    PemsEntities.SaveChanges();

                //we also need ot create a valid SFMeterMAintenanceEvent
                var maintenanceEvent = new SFMeterMaintenanceEvent
                    {
                        AreaId = alarm.AreaID,
                        CustomerId = alarm.CustomerID,
                        EventDateTime = model.ClosureNotification,
                        GlobalMeterId = alarm.GlobalMeterID,
                        MaintenanceCode = model.ResolutionCode,
                        MeterId = alarm.MeterId,
                        TechnicianId = model.TechnicianID,
                        WorkOrderID = alarm.WorkOrderId
                    };
                PemsEntities.SFMeterMaintenanceEvents.Add(maintenanceEvent);
                PemsEntities.SaveChanges();

                //now we must delete the alarm we jsut cleared
                PemsEntities.ActiveAlarms.Remove(alarm);
                PemsEntities.SaveChanges();
            }
        }

        #endregion
        /// <summary>
        /// Determines the Master ID for a Target Service Designation based on the total number of hours passed in.
        /// </summary>
        private  int GetTsdMasterId(double totalHours)
        {
            int masterId = -1;
            //Current time in customer time zone earlier than SLADue - Open-Compliant (1)
            //Current = 8:45    //SLADue  = 11:00
            if (totalHours > 2)
                masterId = (int)TargetServiceDesignations.OpenCompliant;

            //Current time in customer time zone is before but less than 2 hours to SLA Due - non-compliant in 2 hours (4)
            //Current = 9:15    //SLADue = 11:00
            else if (1 < totalHours && totalHours <= 2)
                masterId = (int)TargetServiceDesignations.OpenNonCompliant2Hours;

                //As above but less than an hour - non-compliant in 1 hour (3)
            //Current = 10:15   //SLADue  = 11:00
            else if (0 < totalHours && totalHours <= 1)
                masterId = (int)TargetServiceDesignations.OpenNonCompliant1Hour;

                //Current time in customer time zone later than SLADue - Open-non-compliant (2)
            //Current = 11:15 //SLADue = 11:00
            else if (totalHours < 0)
                masterId = (int)TargetServiceDesignations.OpenNonCompliant;
            return masterId;
        }

        /// <summary>
        /// Determines the Master ID for a Target Service Designation based on the  datetimes passed in
        /// </summary>
        private int GetTsdMasterId(DateTime timeCleared, DateTime? slaDue)
        {
            var masterId = (int)TargetServiceDesignations.ClosedNonCompliant; //default it to non compliant
            //Time Of Clearance earlier than SLADue (5)
            //ToC = 10:10    //SLADue  = 11:00
            if (slaDue.HasValue && timeCleared < slaDue)
                masterId = (int)TargetServiceDesignations.ClosedCompliant;
            return masterId;
        }

        #region "Listing"

        /// <summary>
        /// Gets all alarm objects from the data source
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultOrderBy"></param>
        /// <returns></returns>
        public  List<SpAlarmModel> GetAlarms(DataSourceRequest request, string defaultOrderBy)
        {
            string paramValues = string.Empty;
            var spParams = GetSpParams(request,  defaultOrderBy, out paramValues);
            IEnumerable<SpAlarmModel> customers =PemsEntities.Database.SqlQuery<SpAlarmModel>("sp_GetActiveAlarms " + paramValues, spParams);
            return customers.ToList();
        }
        #endregion

        #region "Filters"

        /// <summary>
        /// Gets the AssetType for the filters NOTE: As of 3-5-2013 the table for this data did not exist in the database
        /// </summary>
        /// <returns></returns>
        public List<AlarmDDLModel> GetAlarmAssetTypes(int customerId)
        {
            var ddlItems = new List<AlarmDDLModel>();

            try
            {
                var assetTypesQuery = from at in PemsEntities.AssetTypes
                                      where at.CustomerId == customerId && at.IsDisplay == true
                                      select new AlarmDDLModel
                                                 {
                                                     Value = at.MeterGroupId,
                                                     Text = at.MeterGroupDesc
                                                 };

                List<AlarmDDLModel> assetTypes = assetTypesQuery.ToList();

                if ( assetTypes.Any() )
                    ddlItems = assetTypes;
            }

            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAlarmAssetTypes", ex);
                ddlItems = new List<AlarmDDLModel>();
            }

            return ddlItems;
        }

        /// <summary>
        /// Gets the values for the Alarm Status dropdown on the listing page
        /// </summary>
        /// <returns></returns>
        public  List<AlarmDDLModel> GetAlarmStatusValues()
        {
            //LookUpItem tables stores most of the drop-down list values. Items in this table are grouped by PropertyGroup
            var items = new List<AlarmStatu>();
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                items = PemsEntities.AlarmStatus.ToList();
                if (items.Any())
                    ddlItems.AddRange(
                        items.Select(item => new AlarmDDLModel {Value = item.AlarmStatusId, Text = item.AlarmStatusDesc}));

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAlarmStatusValues", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }
     
        /// <summary>
        /// Gets the values for the Alarm Status dropdown on the listing page
        /// </summary>
        /// <returns><see cref="List{AlarmDDLModel}"/> of Alarm Statuses for cutomer</returns>
        public  List<AlarmDDLModel> GetTargetServices(int customerID)
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.TargetServiceDesignations.Where(x=>x.CustomerId == customerID);
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.TargetServiceDesignationId, Text = item.TargetServiceDesignationDesc }));
            }

            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetTargetServices", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }
      
        /// <summary>
        /// Gets a list of items for the Asset State dropdown filter
        /// </summary>
        /// <returns><see cref="List{AlarmDDLModel}"/></returns>
        public  List<AlarmDDLModel> GetAssetStates()
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.OperationalStatus.ToList();
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.OperationalStatusId, Text = item.OperationalStatusDesc }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAssetStates", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }
       
        /// <summary>
        /// Gets the TimeType for the filters
        /// </summary>
        /// <returns></returns>
        public  List<AlarmDDLModel> GetAlarmTimeTypes()
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.TimeTypes;
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.TimeTypeId, Text = item.TimeTypeDesc }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAlarmTimeTypes", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Gets the TimeType for the filters
        /// </summary>
        /// <returns></returns>
        public  List<AlarmDDLModel> GetResolutionCodes()
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                ddlItems.Add(new AlarmDDLModel { Value = -1, Text = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.DropDownDefault, "Select One") });
                var items = PemsEntities.MaintenanceCodes;
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.MaintenanceCode1, Text = item.Description }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetResolutionCodes", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Gets the list for the Alarm Severity filter
        /// </summary>
        /// <returns></returns>
        public  List<AlarmDDLModel> GetAlarmServities()
        {

            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.AlarmTiers;
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.Tier, Text = item.TierDesc }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAlarmServities", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Gets the values for the Technician ID filter
        /// </summary>
        /// <returns></returns>
        public  List<AlarmDDLModel> GetAlarmTechnicians()
        {

            var ddlItems = new List<AlarmDDLModel>();
            try
            {

                // (new ResourceFactory()).GetLocalizedTitle(Constants.LocaleResourceTypes.DropDownDefault,"Select One")
                //add the initial select one option
               // ddlItems.Add(new AlarmDDLModel { Value = -1, Text = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.DropDownDefault, "Select One") });
                var items = PemsEntities.TechnicianDetails;
                if (items.Any())
                    //DONT REFACTOR TO LINQ, this has to be foreach or it wont work. - due to the ToString
                    foreach (var item in items)
                        ddlItems.Add(new AlarmDDLModel { Value = item.TechnicianId, Text = item.TechnicianId.ToString() + ": " +item.Name  });
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAlarmTechnicians", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Getsl all the EventSource values for the Event Source filter
        /// </summary>
        /// <returns></returns>
        public  List<AlarmDDLModel> GetAlarmSources()
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.EventSources;
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.EventSourceCode, Text = item.EventSourceDesc }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAlarmSources", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Gets a list of items for the Alarm dropdown list
        /// </summary>
        /// <returns></returns>
        public  List<AlarmDDLModel> GetAlarmFilters(int customerId)
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.EventCodes.Where(x => x.IsAlarm == true && x.CustomerID == customerId);
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.EventCode1, Text = item.EventDescVerbose }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAlarmFilters", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Gets a list of Zone items for the Alarm filters
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public  List<AlarmDDLModel> GetAlarmZones(int customerID)
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.Zones.Where(x => x.customerID == customerID);
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.ZoneId, Text = item.ZoneName }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAlarmZones", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Gets a list of Areas for the Areas dropdown list
        /// </summary>
        /// <returns></returns>
        public  List<AlarmDDLModel> GetAlarmAreas(int customerID)
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.Areas.Where(x => x.CustomerID == customerID);
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.AreaID, Text = item.AreaName }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAlarmAreas", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Gets a list of SubAreas for the Suburbs dropdown list
        /// </summary>
        /// <returns></returns>
        public  List<AlarmDDLModel> GetAlarmSubAreas(int customerID)
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.SubAreas.Where(x => x.CustomerID == customerID);
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.SubAreaID, Text = item.SubAreaName }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAlarmSubAreas", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }
        #endregion
    
    }
}
