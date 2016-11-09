/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________            __________________________________________________________________________________________________
 * 01/15/2014       Sergey Ostrerov                DPTXPEMS-178 - Event Inquiry-Time Due SLA column shows N/A or 00:00
 * 02/06/2014       R Howard                       JIRA: DPTXPEMS-225  Added CustomerId to where clause for any selects from PEMS Areas table
 * 02/06/2014       Sergey Ostrerov                JIRA: DPTXPEMS-213  TimePaid formatted 00:00:00
 * *****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.Entities.Events;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Utilities;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using NLog;
using DayOfWeek = System.DayOfWeek;
using TimeType = Duncan.PEMS.Entities.Alarms.TimeType;

namespace Duncan.PEMS.Business.Events
{
    public class EventModelFactory : BaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Factory constructor taking a connection string name.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        public EventModelFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        /// <summary>
        /// Gets summary list items based on a data source request and any associated filters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultOrderBy"></param>
        /// <returns></returns>
        public List<SummaryEventModel> GetSummaryEventModels([DataSourceRequest] DataSourceRequest request,string defaultOrderBy)
        {
            string paramValues;
            //add the view specific name
            request.Filters.Add( new FilterDescriptor {Member = "viewName", Value = "pv_EventsSummary", Operator = FilterOperator.IsEqualTo} );
            SqlParameter[] spParams = GetSpParams( request, defaultOrderBy, out paramValues );
            IEnumerable<SummaryEventModel> items = PemsEntities.Database.SqlQuery<SummaryEventModel>( "sp_GetEventsItems " + paramValues, spParams );
            return items.ToList();
        }

        /// <summary>
        /// Gets diagnostic list items based on a data source request and any associated filters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultOrderBy"></param>
        /// <returns></returns>
        public List<DiagnosticsEventModel> GetDiagnosticEventModels([DataSourceRequest] DataSourceRequest request, string defaultOrderBy)
        {
            string paramValues;
            //add the view specific name
            request.Filters.Add(new FilterDescriptor { Member = "viewName", Value = "pv_EventsDiagnostics", Operator = FilterOperator.IsEqualTo });
            var spParams = GetSpParams(request, defaultOrderBy, out paramValues);
            IEnumerable<DiagnosticsEventModel> items = PemsEntities.Database.SqlQuery<DiagnosticsEventModel>("sp_GetEventsItems " + paramValues, spParams);
            return items.ToList();
        }

        /// <summary>
        /// Gets alarm list items based on a data source request and any associated filters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultOrderBy"></param>
        /// <returns></returns>
        public List<AlarmsEventModel> GetAlarmEventModels([DataSourceRequest] DataSourceRequest request,
                                                          string defaultOrderBy)
        {
            string paramValues;
            //add the view specific name
            request.Filters.Add(new FilterDescriptor
                {
                    Member = "viewName",
                    Value = "pv_EventsAllAlarms",
                    Operator = FilterOperator.IsEqualTo
                });
            var spParams = GetSpParams(request, defaultOrderBy, out paramValues);
            IEnumerable<AlarmsEventModel> items =
                PemsEntities.Database.SqlQuery<AlarmsEventModel>("sp_GetEventsItems " + paramValues, spParams);

            var alarmsEventModels = items.ToList();
            foreach (var eventModel in alarmsEventModels)
            {
                if (eventModel.TimeDueSLA.HasValue)
                {
                    TimeSpan timeSpan = new TimeSpan();
                    timeSpan = TimeSpan.FromMinutes((double)eventModel.TotalMinutes);
                    if (eventModel.TotalMinutes < 0)
                        eventModel.TimeDueSLADisplay = "00:00:00";
                    else
                        eventModel.TimeDueSLADisplay = timeSpan.ToString();
                }
                else
                    eventModel.TimeDueSLADisplay = "N/A";
            }

            return alarmsEventModels;
        }

        /// <summary>
        /// Gets connection event list items based on a data source request and any associated filters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultOrderBy"></param>
        /// <returns></returns>
        public List<ConnectionEventModel> GetConnectionEventModels([DataSourceRequest] DataSourceRequest request, string defaultOrderBy)
        {
            string paramValues;
            //add the view specific name
            request.Filters.Add(new FilterDescriptor { Member = "viewName", Value = "pv_EventsGSMConnectionLogs", Operator = FilterOperator.IsEqualTo });
            var spParams = GetSpParams(request, defaultOrderBy, out paramValues);
            IEnumerable<ConnectionEventModel> items = PemsEntities.Database.SqlQuery<ConnectionEventModel>("sp_GetEventsItems " + paramValues, spParams);
            var connectionEventModels = items.ToList();

            foreach (var eventModel in connectionEventModels)
            {
                TimeSpan span = eventModel.EndTime - eventModel.StartTime;
                eventModel.Period = FormatHelper.FormatTimeFromMinutes( span.TotalMinutes );
            }

            return connectionEventModels;
        }

        /// <summary>
        /// Gets transaction event list items based on a data source request and any associated filters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultOrderBy"></param>
        /// <returns></returns>
        public List<TransactionEventModel> GetTransactionEventModels([DataSourceRequest] DataSourceRequest request, string defaultOrderBy)
        {
            string paramValues;
            //add the view specific name
            request.Filters.Add(new FilterDescriptor { Member = "viewName", Value = "pv_EventsTransactions", Operator = FilterOperator.IsEqualTo });
            var spParams = GetSpParams(request, defaultOrderBy, out paramValues);
            IEnumerable<TransactionEventModel> items = PemsEntities.Database.SqlQuery<TransactionEventModel>("sp_GetEventsItems " + paramValues, spParams);
            var transactionEventModels = items.ToList();

            foreach (var eventModel in transactionEventModels)
            {
                eventModel.AmountPaidDisplay = FormatHelper.FormatCurrency( eventModel.Amount );
                eventModel.TimePaidDisplay = FormatHelper.FormatTimeFromSeconds(eventModel.TimePaid);
            }

            return transactionEventModels;
        }

        /// <summary>
        /// Gets collection communication event list items based on a data source request and any associated filters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultOrderBy"></param>
        /// <returns></returns>
        public List<CollectionCommEventModel> GetCollectionCommEventModels([DataSourceRequest] DataSourceRequest request, string defaultOrderBy)
        {
            string paramValues;
            //add the view specific name
            request.Filters.Add(new FilterDescriptor { Member = "viewName", Value = "pv_EventsCollectionCommEvent", Operator = FilterOperator.IsEqualTo });
            var spParams = GetSpParams(request, defaultOrderBy, out paramValues);
            IEnumerable<CollectionCommEventModel> items = PemsEntities.Database.SqlQuery<CollectionCommEventModel>("sp_GetEventsItems " + paramValues, spParams);
            var collectionCommEventModels = items.ToList();

            foreach( var eventModel in collectionCommEventModels )
            {
                eventModel.AmountDisplay = FormatHelper.FormatCurrency( eventModel.Amount );
            }

            return collectionCommEventModels;
        }

        /// <summary>
        /// Gets collection cbre events list items based on a data source request and any associated filters
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultOrderBy"></param>
        /// <returns></returns>
        public List<CollectionCBREventModel> GetCollectionCBREventModels([DataSourceRequest] DataSourceRequest request, string defaultOrderBy)
        {
            string paramValues;
            //add the view specific name
            request.Filters.Add(new FilterDescriptor { Member = "viewName", Value = "pv_EventsCollectionCBR", Operator = FilterOperator.IsEqualTo });
            var spParams = GetSpParams(request, defaultOrderBy, out paramValues);
            IEnumerable<CollectionCBREventModel> items = PemsEntities.Database.SqlQuery<CollectionCBREventModel>("sp_GetEventsItems " + paramValues, spParams);
            return items.ToList();
        }

        /// <summary>
        /// Gets the details for a specific event
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="currentCityId"></param>
        /// <returns></returns>
        public EventDetails GetEventDetails(int eventID, int currentCityId)
        {
            EventDetails eventDetails;
                eventDetails = PemsEntities.EventLogs
                                   .Where( eventLog => eventLog.EventUID == eventID && eventLog.CustomerID == currentCityId )
                                   .Select( e => new EventDetails()
                                                     {
                                                         AssetName = e.Meter.MeterName,
                                                         AssetId = e.MeterId,
                                                         Area = PemsEntities.MeterMaps.FirstOrDefault(mm => mm.MeterId == e.MeterId && mm.Customerid == e.CustomerID && mm.Areaid == e.AreaID).AreaId2 == null ? ""
                                                         : PemsEntities.Areas.FirstOrDefault(a => a.CustomerID == e.CustomerID && a.AreaID == PemsEntities.MeterMaps.FirstOrDefault(mm => mm.MeterId == e.MeterId && mm.Customerid == e.CustomerID && mm.Areaid == e.AreaID).AreaId2) == null ? ""
                                                         : PemsEntities.Areas.FirstOrDefault(a => a.CustomerID == e.CustomerID && a.AreaID == PemsEntities.MeterMaps.FirstOrDefault(mm => mm.MeterId == e.MeterId && mm.Customerid == e.CustomerID && mm.Areaid == e.AreaID).AreaId2).AreaName,
                                                         AbbrDesc = PemsEntities.EventCodes.FirstOrDefault(ec => ec.EventCode1 == e.EventCode && ec.CustomerID == e.CustomerID).EventDescAbbrev,
                                                         LongDesc = PemsEntities.EventCodes.FirstOrDefault(ec => ec.EventCode1 == e.EventCode && ec.CustomerID == e.CustomerID).EventDescVerbose,
                                                         AssetType = e.Meter.MeterGroup1.MeterGroupDesc,
                                                         BaysAffected = e.Meter.MaxBaysEnabled,
                                                         EventDateTime = e.EventDateTime,
                                                         DemandArea = PemsEntities.DemandZones.FirstOrDefault(dz => dz.DemandZoneId == PemsEntities.Meters.FirstOrDefault(m => m.MeterId == e.MeterId && m.CustomerID == e.CustomerID && m.AreaID == e.AreaID).DemandZone).DemandZoneDesc,
                                                         EventId = e.EventUID,
                                                         EventCode = e.EventCode,
                                                         EventSource = e.EventSource,
                                                         EventSourceDescription = PemsEntities.EventSources.FirstOrDefault(ec => ec.EventSourceCode == e.EventSource ) == null ? ""
                                                         : PemsEntities.EventSources.FirstOrDefault(ec => ec.EventSourceCode == e.EventSource).EventSourceDesc,
                                                         Latitude = e.Meter.Latitude,
                                                         Longitude = e.Meter.Longitude,
                                                         Street = e.Meter.Location,
                                                         Suburb = PemsEntities.MeterMaps.FirstOrDefault(mm => mm.MeterId == e.MeterId && mm.Customerid == e.CustomerID && mm.Areaid == e.AreaID).CustomGroup11.DisplayName,
                                                         TechnicianId = e.TechnicianKeyID,
                                                         Zone = PemsEntities.MeterMaps.FirstOrDefault(mm => mm.MeterId == e.MeterId && mm.Customerid == e.CustomerID && mm.Areaid == e.AreaID).Zone.ZoneName,
                                                         TimeType1 = e.TimeType1,
                                                         TimeType2 = e.TimeType2,
                                                         TimeType3 = e.TimeType3,
                                                         TimeType4 = e.TimeType4,
                                                         TimeType5 = e.TimeType5,
                                                         Peak = e.TimeType4 != null
                                                     } ).FirstOrDefault();

            if ( eventDetails != null )
            {
               //update the area to be areaID2 of the metermap for this event
                eventDetails.DayOfWeek = eventDetails.EventDateTime.DayOfWeek.ToString();
                eventDetails.PartOfWeek = GetPartOfWeek( eventDetails.EventDateTime.DayOfWeek );
                AddTimeTypes( eventDetails );
            }
            return eventDetails;
        }

        /// <summary>
        /// Populates an event details with the valid typetypes
        /// </summary>
        /// <param name="eventDetails"></param>
        private void AddTimeTypes( EventDetails eventDetails )
        {
            eventDetails.TimeTypes = new List<TimeType>();

            if( eventDetails.TimeType1 != null )
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault( x => x.TimeTypeId == eventDetails.TimeType1 );
                if( tt != null )
                    eventDetails.TimeTypes.Add( new TimeType { Description = tt.TimeTypeDesc, Id = tt.TimeTypeId } );
            }
            if( eventDetails.TimeType2 != null )
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault( x => x.TimeTypeId == eventDetails.TimeType2 );
                if( tt != null )
                    eventDetails.TimeTypes.Add( new TimeType { Description = tt.TimeTypeDesc, Id = tt.TimeTypeId } );
            }
            if( eventDetails.TimeType3 != null )
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault( x => x.TimeTypeId == eventDetails.TimeType3 );
                if( tt != null )
                    eventDetails.TimeTypes.Add( new TimeType { Description = tt.TimeTypeDesc, Id = tt.TimeTypeId } );
            }
            if( eventDetails.TimeType4 != null )
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault( x => x.TimeTypeId == eventDetails.TimeType4 );
                if( tt != null )
                    eventDetails.TimeTypes.Add( new TimeType { Description = tt.TimeTypeDesc, Id = tt.TimeTypeId } );
            }
            if( eventDetails.TimeType5 != null )
            {
                var tt = PemsEntities.TimeTypes.FirstOrDefault( x => x.TimeTypeId == eventDetails.TimeType5 );
                if( tt != null )
                    eventDetails.TimeTypes.Add( new TimeType { Description = tt.TimeTypeDesc, Id = tt.TimeTypeId } );
            }
        }

        /// <summary>
        /// Determines weekday or weekend
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        private string GetPartOfWeek( DayOfWeek dayOfWeek )
        {
            switch( dayOfWeek.ToString() )
            {
                case "Saturday":
                case "Sunday":
                    return HttpContext.Current.GetLocaleResource( ResourceTypes.Label, "Weekend" ).ToString();
                    break;
                default:
                    return HttpContext.Current.GetLocaleResource(ResourceTypes.Label, "Weekday").ToString();
                    break;
            }
        }

        /// <summary>
        /// Gets a lsit of drop down items that represent time types in the system
        /// </summary>
        /// <returns></returns>
        public List<TimeTypeDDL> GetTimeTypesFilterItems()
        {
            // Create the queries
            var timeTypesQuery = PemsEntities.TimeTypes.Select(tt => new TimeTypeDDL
            {
                Id = tt.TimeTypeId,
                Text = tt.TimeTypeDesc
            });
            var timeTypes = timeTypesQuery.ToList();
            return timeTypes;
        }

        /// <summary>
        ///  Gets a lsit of drop down items that represent software versions in the system
        /// </summary>
        /// <param name="currentCityId"></param>
        /// <returns></returns>
        public List<string> GetSoftwareVersionFilterItems(int currentCityId)
        {

            IQueryable<string> softwareVersionsQuery = (from md in PemsEntities.MeterDiagnostics
                                                        from mdt in PemsEntities.MeterDiagnosticTypes
                                                        from mdtc in PemsEntities.MeterDiagnosticTypeCustomers
                                                        where md.DiagnosticType == mdt.ID && md.CustomerID == mdtc.CustomerId && md.CustomerID == currentCityId
                                                        where md.DiagnosticType == 215 // "MSM.FirmwareVer"
                                                        orderby md.DiagnosticValue
                                                        select md.DiagnosticValue).Distinct();
            return softwareVersionsQuery.ToList();
        }

        /// <summary>
        ///  Gets a lsit of drop down items that represent asset types in the system for a specific customer
        /// </summary>
        /// <param name="currentCityId"></param>
        /// <returns></returns>
        public List<string> GetAssetTypeFilterItems(int currentCityId)
        {
            IQueryable<string> assetTypesQuery = from at in PemsEntities.AssetTypes
                                                 where at.CustomerId == currentCityId && at.IsDisplay == true
                                                 select at.MeterGroupDesc;
            return assetTypesQuery.ToList();
        }

        /// <summary>
        ///  Gets a lsit of drop down items that represent event types in the system
        /// </summary>
        /// <returns></returns>
        public List<string> GetEventTypesFilterItems()
        {
            //IQueryable<string> eventClassesQuery = from et in PemsEntities.EventTypes
            //** Sairam added below code to filter 2003 and 2004 event from UI (DTPEMS-44)
            IQueryable<string> eventClassesQuery = from et in PemsEntities.EventTypes where et.EventTypeId!=23 && et.EventTypeId!=24
                                                   select et.EventTypeDesc;
            return eventClassesQuery.ToList();
        }

    }
}