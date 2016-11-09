using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.WorkOrders;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.WebServices.FieldMaintenance;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Business.WebServices
{
    /// <summary>
    /// This factory will handle all of the web service interactions with Duncan.
    /// </summary>
    public class WebServiceFactory
    {
        #region Duncan Services to call
        

        #region Create Alarm
        /// <summary>
        /// Calls a duncan Web service that will determine if the alarm is already open and return the ID of that, or create a new event ID and return that one.
       /// </summary>
       /// <param name="request"></param>
       /// <returns></returns>
        public CreateAlarmResponse CreateAlarm( CreateAlarmRequest request)
        {
            //fix up the response with some default values
            var alarmResponse = new CreateAlarmResponse {IsValid = false};

            var dataRequest = GenerateAlarmRequest(request);

           //now that we have our data request, we need to send it to duncan and get the response.
            string strUrl = ConfigurationManager.AppSettings[Constants.FieldMaintenance.WebServiceCreateAlarmName];
            var serializedDataRequest = Serialize(dataRequest);

           if (string.IsNullOrEmpty(serializedDataRequest))
               return alarmResponse;

            byte[] dataByte = StringToByteArray(serializedDataRequest);
            var response = PostRequest(strUrl, dataByte);
            return GetAlarmResponse(response, alarmResponse);
        }

        /// <summary>
        /// Generates a Data request for the create alarm web service method from the Create Alarm Request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Data GenerateAlarmRequest(CreateAlarmRequest request)
        {
            //hook this up to the duncan web service to create the event
            //create the request
            var dataRequest = new Data {Request = new DataRequest {WorkOrder = new DataRequestWorkOrder[1]}};
            //add a single work order and a single alarm to that work order to pass the data to duncn so we can detemrine the eventUID
            dataRequest.Request.WorkOrder[0] = new DataRequestWorkOrder
                {
                    WorkOrderId = request.WorkOrderId.ToString(),
                    mid = request.AssetId.ToString(),
                    aid = request.AreaId.ToString(),
                    cid = request.CustomerId.ToString(),
                    ActiveAlarm = new DataRequestWorkOrderActiveAlarm[1]
                };

            //now add the alarm
            dataRequest.Request.WorkOrder[0].ActiveAlarm[0] = new DataRequestWorkOrderActiveAlarm
                {
                    EventCode = request.EventCode.ToString(),
                    EventSource = request.EventSource.ToString(),
                    Notes = request.Notes,
                    TimeOfNotification = request.LocalTime.AddMinutes(1),
                    TimeOfOccurrance = request.LocalTime
                };
            return dataRequest;
        }

        /// <summary>
        /// Gets the sla and eventuid values from the alarm response (Data) object and populate the Create Alarm Response object. 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="alarmResponse"></param>
        /// <returns></returns>
        private CreateAlarmResponse GetAlarmResponse(Data response, CreateAlarmResponse alarmResponse)
        {
            //now that we have it, we need to get the SLA value and the event UID for it.
            if (response != null)
            {
                if (response.Response != null)
                {
                    if (response.Response.WorkOrder != null)
                    {
                        if (response.Response.WorkOrder.Any())
                        {
                            var dataResponseWorkOrderActiveAlarms = response.Response.WorkOrder[0].ActiveAlarm;
                            if (dataResponseWorkOrderActiveAlarms != null)
                            {
                                if (dataResponseWorkOrderActiveAlarms.Any())
                                {
                                    alarmResponse.SlaDue = dataResponseWorkOrderActiveAlarms[0].SLADue;
                                    var eventID = -1;
                                    if (int.TryParse(dataResponseWorkOrderActiveAlarms[0].EventUID, out eventID))
                                    {
                                        alarmResponse.EventUID = eventID;
                                        alarmResponse.IsValid = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (response.Error != null)
                {
                    alarmResponse.IsValid = false;
                   alarmResponse.ErrorMessage = response.Error.Items[0];
                }
            }
            return alarmResponse;
        }

        #endregion

        #region Close Alarm
        /// <summary>
        /// Closes the alarm on Duncan sside and returns a response if it was successful or not
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CloseAlarmResponse CloseAlarm(CloseAlarmRequest request)
        {
            //fix up the response with some default values
            var alarmResponse = new CloseAlarmResponse {IsValid = false};

            var dataRequest = GenerateAlarmRequest(request);

            //now that we have our data request, we need to send it to duncan and get the response.
            string strUrl = ConfigurationManager.AppSettings[Constants.FieldMaintenance.WebServiceCloseAlarmName];
            var serializedDataRequest = Serialize(dataRequest);

            if (string.IsNullOrEmpty(serializedDataRequest))
                return alarmResponse;

            byte[] dataByte = StringToByteArray(serializedDataRequest);
            var response = PostRequest(strUrl, dataByte);
            return GetAlarmResponse(response, alarmResponse);
        }

        private Data GenerateAlarmRequest(CloseAlarmRequest request)
        {
            //hook this up to the duncan web service to create the event
            //create the request

            var dataRequest = new Data {Request = new DataRequest {ActiveAlarms = new DataRequestActiveAlarms[1]}};
            dataRequest.Request.ActiveAlarms[0] = new DataRequestActiveAlarms
                {
                    TimeOfNotification = request.LocalTime,
                    TimeOfNotificationSpecified = true,
                    ActiveAlarm = new DataRequestActiveAlarmsActiveAlarm[1]
                };

            dataRequest.Request.ActiveAlarms[0].ActiveAlarm[0] = new DataRequestActiveAlarmsActiveAlarm
                {
                    cid = request.CustomerId.ToString(),
                    aid = request.AreaId.ToString(),
                    mid = request.AssetId.ToString(),
                    EventUID = request.EventUID.ToString(),
                    EventCode = request.EventCode.ToString()
                };

            return dataRequest;
        }

        private CloseAlarmResponse GetAlarmResponse(Data response, CloseAlarmResponse alarmResponse)
        {
            if (response != null)
            {
                if (response.Response != null)
                {
                    if (response.Response.ActiveAlarms != null)
                    {
                        if (response.Response.ActiveAlarms.Any())
                        {
                            var dataResponseActiveAlarmsActiveAlarms = response.Response.ActiveAlarms[0].ActiveAlarm;
                            if (dataResponseActiveAlarmsActiveAlarms != null)
                            {
                                if (dataResponseActiveAlarmsActiveAlarms.Any())
                                {
                                    alarmResponse.Closed = dataResponseActiveAlarmsActiveAlarms[0].Closed == "1";
                                    alarmResponse.IsValid = true;
                                }
                            }
                        }
                    }
                }
                else if (response.Error != null)
                {
                    alarmResponse.IsValid = false;
                    alarmResponse.ErrorMessage = response.Error.Items[0];
                }
            }
            return alarmResponse;
        }

        #endregion
        #endregion

        #region Exposed Web Services

        #region Clear Alarms
        public Data ClearAlarms(Data wsRequest)
        {
            try
            {
                //get the request
                var request = wsRequest.Request;
                if (request == null)
                    return GenerateError("Invalid Request Object", "400");

                //get the active alarms for the request
                var requestHistoricAlarms = request.HistoricalAlarms;
                if (requestHistoricAlarms == null)
                    return GenerateError("Invalid Historic Alarms Object for the request", "400");

                //build a list of response alarms from the request alarms

                var responseHistoricAlarms = new DataResponseHistoricalAlarm[requestHistoricAlarms.Count()];
                for (int historicAlarmsIndex = 0;
                     historicAlarmsIndex < requestHistoricAlarms.Count();
                     historicAlarmsIndex++)
                {
                    var historicAlarmRequest = requestHistoricAlarms[historicAlarmsIndex];
                    var historicAlarmResponse = MakeClearAlarmsHistoricAlarmsResponse(historicAlarmRequest);
                    //then add it to our colleciton
                    responseHistoricAlarms[historicAlarmsIndex] = historicAlarmResponse;
                }

                var response = new Data {Response = new DataResponse {HistoricalAlarms = responseHistoricAlarms}};
                return response;
            }
            catch (Exception ex)
            {
                return GenerateError(ex.Message, "500");
            }
        }

        private DataResponseHistoricalAlarm MakeClearAlarmsHistoricAlarmsResponse(
            DataRequestHistoricalAlarm historicAlarmRequest)
        {
            //make a response from the request
            var historicAlarmResponse = new DataResponseHistoricalAlarm
                {
                    EventUID = historicAlarmRequest.EventUID,
                    statusCode = ClearAlarm(historicAlarmRequest)
                };
            //now create the work order event for this alarm
            return historicAlarmResponse;
        }

        private string ClearAlarm(DataRequestHistoricalAlarm historicAlarmRequest)
        {
            //clear the work order event

            const int defaultValue = -1;
            //first, parse the ids
            int customeId = int.TryParse(historicAlarmRequest.cid, out customeId) ? customeId : defaultValue;
            int meterId = int.TryParse(historicAlarmRequest.mid, out meterId) ? meterId : defaultValue;
            int areaId = int.TryParse(historicAlarmRequest.aid, out areaId) ? areaId : defaultValue;
            int eventSource = int.TryParse(historicAlarmRequest.EventSource, out eventSource)
                                  ? eventSource
                                  : defaultValue;
            int eventCode = int.TryParse(historicAlarmRequest.EventCode, out eventCode) ? eventCode : defaultValue;
            long eventUID = long.TryParse(historicAlarmRequest.EventUID, out eventUID) ? eventUID : defaultValue;
            int workOrderId = int.TryParse(historicAlarmRequest.WorkOrderId, out workOrderId)
                                  ? workOrderId
                                  : defaultValue;


            //check ot make sure all the ids were parsed correctly
            if (customeId > defaultValue && meterId > defaultValue && areaId > defaultValue
                && eventCode > defaultValue && eventSource > defaultValue && eventUID > defaultValue &&
                workOrderId > defaultValue)
            {
                var currentCustomer = new PemsCity(customeId.ToString());
                //go get the customer so we can get the correct connection string
                using (
                    var maintenanceEntities = new MaintenanceEntities(currentCustomer.MaintenanceConnectionStringName))
                {
                    //first we have to go to PEMS db to get the asset
                    //return nothing if the asset doesnt exist.
                    var assetFactory = (new AssetFactory(currentCustomer.PemsConnectionStringName));
                    var asset = assetFactory.GetAsset(customeId, areaId, meterId);
                    //fail state - work order or work order event will not be closed
                    if (asset == null)
                        return ((int) WorkOrderEventStatus.Open).ToString();

                    //close out the work order event
                    //go get the event
                    //check to see if the event exist first
                    var existingWorkOrderEvent =
                        maintenanceEntities.FMWorkOrderEvents.FirstOrDefault(
                            x => x.WorkOrderId == workOrderId && x.EventId == eventUID && x.EventCode == eventCode);
                    //if it doesnt exist, cant close it, just return closed
                    if (existingWorkOrderEvent == null)
                        return ((int) WorkOrderEventStatus.Closed).ToString();

                    //clear the event
                    existingWorkOrderEvent.Status = (int) WorkOrderEventStatus.Closed;
                    maintenanceEntities.SaveChanges();

                    //every time a event is created, resolved , etc, we have to update the sladue and highest severity, so lets do that now
                    var mobileWorkOrderFactory =
                        (new TechnicianWorkOrderFactory(currentCustomer.MaintenanceConnectionStringName,
                                                        currentCustomer.PemsConnectionStringName));
                    mobileWorkOrderFactory.UpdateEventAggregateInfo(workOrderId);

                    //then clear the work order (if its the last event that was open was just closed)
                    CloseWorkOrder(maintenanceEntities, workOrderId, currentCustomer.LocalTime);
                    return ((int) WorkOrderEventStatus.Closed).ToString();
                }
            }
            //cant find the itme the request was referencing, return closed.
            //return WorkOrderEventStatus enum representing if it were closed or not.
            return ((int) WorkOrderEventStatus.Closed).ToString();
        }

        /// <summary>
        /// Close the work order if it doesnt have any current open events associated with.
        /// </summary>
        /// <param name="maintenanceEntities"></param>
        /// <param name="workOrderId"></param>
        /// <param name="localTime"></param>
        /// <param name="vandalism"></param>
        public void CloseWorkOrder(MaintenanceEntities maintenanceEntities, long workOrderId, DateTime localTime,
                                   bool vandalism = false)
        {
            //get the work order
            var workOrder = maintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.WorkOrderId == workOrderId);
            if (workOrder != null)
            {
                //check to see if there are any open events for this work order
                var openEvents = workOrder.WorkOrderEvents.Any(x => x.Status == (int) WorkOrderEventStatus.Open);
                if (openEvents)
                    return;

                //for each event, set the status to resolved (completed).
                var fmworkorder = maintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.WorkOrderId == workOrderId);
                if (fmworkorder != null)
                {
                    fmworkorder.WorkOrderStatusId = (int) WorkOrderStatus.Closed;
                    fmworkorder.CompletedDate = localTime;
                    //we are using Maintnenance code of 8 - Closed from Meter. This should exist in the DB.
                    fmworkorder.ResolutionCode = Constants.FieldMaintenance.AutomatedMaintenanceCodeId;
                    fmworkorder.ResolutionDesc = Constants.FieldMaintenance.AutomatedMaintenanceCodeDescription;
                    maintenanceEntities.SaveChanges();
                }
                //if there arent any, close the work order
            }
        }

        #endregion

        #region Get Work Orders

        public Data GetWorkOrders(Data wsRequest)
        {
            try
            {
                //get the request
                var request = wsRequest.Request;
                if (request == null)
                    return GenerateError("Invalid Request Object", "400");

                //get the active alarms for the request
                var requestActiveAlarms = request.ActiveAlarms;
                if (requestActiveAlarms == null)
                    return GenerateError("Invalid Active Alarms Object for the request", "400");

                //build a list of response alarms from the request alarms
                var responseActiveAlarms = new DataResponseActiveAlarms[wsRequest.Request.ActiveAlarms.Count()];
                for (int activeAlarmsIndex = 0; activeAlarmsIndex < requestActiveAlarms.Count(); activeAlarmsIndex++)
                {
                    var responseActiveAlarm =
                        new DataResponseActiveAlarmsActiveAlarm[
                            requestActiveAlarms[activeAlarmsIndex].ActiveAlarm.Count()];
                    for (int activeAlarmIndex = 0;
                         activeAlarmIndex < requestActiveAlarms[activeAlarmsIndex].ActiveAlarm.Count();
                         activeAlarmIndex++)
                    {
                        var activeAlarmRequest = requestActiveAlarms[activeAlarmsIndex].ActiveAlarm[activeAlarmIndex];
                        var activeAlarmResponse = MakeGetWorkOrderActiveAlarmsResponse(activeAlarmRequest);
                        //now we have to create the work order event for this alarm

                        //then add it to our colleciton
                        responseActiveAlarm[activeAlarmIndex] = activeAlarmResponse;
                    }
                    var activeAlarmsResponse = new DataResponseActiveAlarms();
                    activeAlarmsResponse.ActiveAlarm = responseActiveAlarm;
                    responseActiveAlarms[activeAlarmsIndex] = activeAlarmsResponse;
                }
                var response = new Data {Response = new DataResponse {ActiveAlarms = responseActiveAlarms}};
                return response;
            }
            catch (Exception ex)
            {
                return GenerateError(ex.Message, "500");
            }
        }

        /// <summary>
        /// Gets a work order ID for this asset. IF one does not exist, it will create it instead.
        /// </summary>
        /// <param name="activeAlarmResponse"></param>
        /// <returns></returns>
        private string GetWorkOrderId(DataResponseActiveAlarmsActiveAlarm activeAlarmResponse)
        {
            const int defaultValue = -1;
            //first, parse the ids
            int customeId = int.TryParse(activeAlarmResponse.cid, out customeId) ? customeId : defaultValue;
            int meterId = int.TryParse(activeAlarmResponse.mid, out meterId) ? meterId : defaultValue;
            int areaId = int.TryParse(activeAlarmResponse.aid, out areaId) ? areaId : defaultValue;
            int eventSource = int.TryParse(activeAlarmResponse.EventSource, out eventSource)
                                  ? eventSource
                                  : defaultValue;
            int eventCode = int.TryParse(activeAlarmResponse.EventCode, out eventCode) ? eventCode : defaultValue;
            long eventUID = long.TryParse(activeAlarmResponse.EventUID, out eventUID) ? eventUID : defaultValue;
            long workOrderId = defaultValue;
            //check ot make sure all the ids were parsed correctly
            if (customeId > defaultValue && meterId > defaultValue && areaId > defaultValue
                && eventCode > defaultValue && eventSource > defaultValue && eventUID > defaultValue)
            {
                var currentCustomer = new PemsCity(customeId.ToString());
                //go get the customer so we can get the correct connection string
                using (
                    var maintenanceEntities = new MaintenanceEntities(currentCustomer.MaintenanceConnectionStringName))
                {
                    //first we have to go to PEMS db to get the asset
                    //return nothing if the asset doesnt exist.
                    var assetFactory = (new AssetFactory(currentCustomer.PemsConnectionStringName));
                    var asset = assetFactory.GetAsset(customeId, areaId, meterId);
                    //fail state - work order or work orde3r event will not be created
                    if (asset == null)
                        return string.Empty;

                    //now create the work order/ or get the exisitng one
                    var fmWorkOrder = CreateWorkOrder(activeAlarmResponse, areaId, currentCustomer, customeId, asset,
                                                      meterId, maintenanceEntities);
                    workOrderId = fmWorkOrder.WorkOrderId;

                    //now that we have our work order id (exisitng or new), we haev to create the event for this work order.
                    //first, we have to get the vlaues we need ot create an event (tier, etc)
                    //get event code

                    CreateWorkOrderEvent(activeAlarmResponse, currentCustomer, customeId, eventSource, eventCode,
                                         eventUID, workOrderId, maintenanceEntities);

                    //every time a event is created, resolved , etc, we have to update the sladue and highest severity, so lets do that now
                    var mobileWorkOrderFactory =
                        (new TechnicianWorkOrderFactory(currentCustomer.MaintenanceConnectionStringName,
                                                        currentCustomer.PemsConnectionStringName));
                    mobileWorkOrderFactory.UpdateEventAggregateInfo(workOrderId);
                    return workOrderId.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Creates a work order event if we need to.
        /// </summary>
        /// <param name="activeAlarmResponse"></param>
        /// <param name="currentCustomer"></param>
        /// <param name="customeId"></param>
        /// <param name="eventSource"></param>
        /// <param name="eventCode"></param>
        /// <param name="eventUID"></param>
        /// <param name="workOrderId"></param>
        /// <param name="maintenanceEntities"></param>
        private void CreateWorkOrderEvent(DataResponseActiveAlarmsActiveAlarm activeAlarmResponse,
                                          PemsCity currentCustomer,
                                          int customeId, int eventSource, int eventCode, long eventUID, long workOrderId,
                                          MaintenanceEntities maintenanceEntities)
        {
            //check to see if we need to crate the event first. the PK for it is workorderid, eventid, eventcode
            var existingWorkOrderEvent =
                maintenanceEntities.FMWorkOrderEvents.FirstOrDefault(
                    x =>
                    x.WorkOrderId == workOrderId && x.EventId == eventUID && x.EventCode == eventCode &&
                    x.Status == (int) WorkOrderEventStatus.Open);
            if (existingWorkOrderEvent != null)
                return;

            //if we got here, we have to creat the event.
            using (var pemsEntities = new PEMEntities(currentCustomer.PemsConnectionStringName))
            {
                var ec =
                    pemsEntities.EventCodes.FirstOrDefault(
                        x => x.CustomerID == customeId && x.EventSource == eventSource && x.EventCode1 == eventCode);
                if (ec != null)
                {
                    //we found the event code, continue. .
                    var fmWorkOrderEvent = new FMWorkOrderEvent();
                    fmWorkOrderEvent.AlarmTier = ec.AlarmTier; // alarm tier of the event code
                    fmWorkOrderEvent.Automated = true;
                    fmWorkOrderEvent.EventCode = eventCode;
                    fmWorkOrderEvent.EventDateTime = activeAlarmResponse.TimeOfOccurrance;
                    fmWorkOrderEvent.EventDesc = ec.EventDescVerbose;
                    fmWorkOrderEvent.EventId = eventUID;
                    fmWorkOrderEvent.Notes = activeAlarmResponse.Notes;
                    fmWorkOrderEvent.SLADue = activeAlarmResponse.SLADue;
                    fmWorkOrderEvent.Status = (int) WorkOrderEventStatus.Open;
                    fmWorkOrderEvent.Vandalism = false;
                    fmWorkOrderEvent.WorkOrderId = workOrderId;

                    maintenanceEntities.FMWorkOrderEvents.Add(fmWorkOrderEvent);
                    maintenanceEntities.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Creates a work order in the system
        /// </summary>
        /// <param name="activeAlarmResponse"></param>
        /// <param name="areaId"></param>
        /// <param name="currentCustomer"></param>
        /// <param name="customeId"></param>
        /// <param name="asset"></param>
        /// <param name="meterId"></param>
        /// <param name="maintenanceEntities"></param>
        /// <returns></returns>
        private static FMWorkOrder CreateWorkOrder(DataResponseActiveAlarmsActiveAlarm activeAlarmResponse, int areaId,
                                                   PemsCity currentCustomer, int customeId, Meter asset, int meterId,
                                                   MaintenanceEntities maintenanceEntities)
        {

            //we have to check to see if htey asset already has a work order. if it does, use that work order. This includes any open, incomplete or rejected work orders.
            //we add it to any open work order, even if it is assigned ot another technician (in practice this will almost never happen)
            var existingWorkOrder =
                maintenanceEntities.FMWorkOrders.FirstOrDefault(
                    x =>
                    x.MeterId == meterId && x.CustomerId == customeId && x.AreaId == areaId &&
                    (x.WorkOrderStatusId == (int) WorkOrderStatus.Open ||
                     x.WorkOrderStatusId == (int) WorkOrderStatus.Incomplete ||
                     x.WorkOrderStatusId == (int) WorkOrderStatus.Rejected));
            if (existingWorkOrder != null)
                return existingWorkOrder;


            //we need to crate a new work order
            //create the work order now. commented out properties we ignore upon creation.
            var fmWorkOrder = new FMWorkOrder
                {
                    AreaId = areaId,
                    CreateDateTime = currentCustomer.LocalTime,
                    CustomerId = customeId, //cid
                    HighestSeverity = 0,
                    Location = asset.Location,
                    Mechanism = asset.MeterType,
                    MeterGroup = asset.MeterGroup ?? 1,
                    MeterId = meterId,
                    Notes = string.Empty,
                    SLADue = activeAlarmResponse.SLADue, //this gets reset anyways, jsut set it to 
                    WorkOrderStatusId = (int) WorkOrderStatus.Open,
                    AssignmentState = (int) AssignmentState.Unassigned,
                    ZoneId =
                        asset.MeterMaps.FirstOrDefault() != null ? asset.MeterMaps.FirstOrDefault().ZoneId : (int?) null
                };
            maintenanceEntities.FMWorkOrders.Add(fmWorkOrder);
            maintenanceEntities.SaveChanges();

            //every time a event is created, resolved , etc, we have to update the sladue and highest severity, so lets do that now
            var mobileWorkOrderFactory =
                (new TechnicianWorkOrderFactory(currentCustomer.MaintenanceConnectionStringName,
                                                currentCustomer.PemsConnectionStringName));
            mobileWorkOrderFactory.UpdateEventAggregateInfo(fmWorkOrder.WorkOrderId);
            return fmWorkOrder;
        }

        private DataResponseActiveAlarmsActiveAlarm MakeGetWorkOrderActiveAlarmsResponse(
            DataRequestActiveAlarmsActiveAlarm activeAlarmRequest)
        {
            //make a response from the request
            var activeAlarmResponse = new DataResponseActiveAlarmsActiveAlarm
                {
                    cid = activeAlarmRequest.cid,
                    mid = activeAlarmRequest.mid,
                    aid = activeAlarmRequest.aid,
                    EventUID = activeAlarmRequest.EventUID,
                    EventCode = activeAlarmRequest.EventCode,
                    EventSource = activeAlarmRequest.EventSource,
                    TimeOfOccurrance = activeAlarmRequest.TimeOfOccurrance,
                   TimeOfOccurranceSpecified = true,
                    TimeOfNotification = activeAlarmRequest.TimeOfNotification,
                    TimeOfNotificationSpecified = true,
                    SLADue = activeAlarmRequest.SLADue,
                    Notes = activeAlarmRequest.Notes
                };

            //then get the WorkOrderId
            activeAlarmResponse.WorkOrderId = GetWorkOrderId(activeAlarmResponse);

            //now create the work order event for this alarm

            return activeAlarmResponse;
        }


        #endregion

        #endregion

        #region Helper Methods
        /// <summary>
        /// Post the request to the specified url and return the result
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="dataByte"></param>
        private Data PostRequest(string strUrl, byte[] dataByte)
        {
            var postRequest = (HttpWebRequest)WebRequest.Create(strUrl);
            //Method type
            postRequest.Method = "POST";
            // Data type - message body coming in xml
            postRequest.ContentType = "text/xml";
            postRequest.KeepAlive = false;
            //Content length of message body
            postRequest.ContentLength = dataByte.Length;

            //postRequest.SendChunked = true;

            // Get the request stream
            Stream posTstream = postRequest.GetRequestStream();
            // Write the data bytes in the request stream
            posTstream.Write(dataByte, 0, dataByte.Length);
            //posTstream.Close();

            
            //Get response from server
            try
            {
                var postResponse = (HttpWebResponse)postRequest.GetResponse();
                var reader = new StreamReader(postResponse.GetResponseStream(), Encoding.UTF8);
                var value = reader.ReadToEnd();

                //take the value and convert it to byte array
                var returnDataBytes = StringToByteArray(value);
                return Deserialize(returnDataBytes);
            }
            catch (WebException webEx)
            {
                return GenerateError(webEx.Message, "500");
            }
            catch (Exception ex)
            {
                return GenerateError(ex.Message, "500");
            }
        }

        /// <summary>
        /// Method - Deserialize Class XML
        /// </summary>
        /// <param name="xmlByteData"></param>
        /// <returns></returns>
        protected Data Deserialize(byte[] xmlByteData)
        {
            try
            {
                var ds = new XmlSerializer(typeof(Data));
                var memoryStream = new MemoryStream(xmlByteData);
                var data = (Data)ds.Deserialize(memoryStream);
                return data;
            }
            catch (Exception ex)
            {
                return GenerateError("Error Reading Response Object recieved for the request.", "400");
            }
        }

        public static string SerializeObject<T>(T toSerialize)
        {
            var xmlSerializer = new XmlSerializer(toSerialize.GetType());
            var textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }

        /// <summary>
        /// Method - Serialize Class to XML
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected  String Serialize(Data data)
        {
            try
            {
                String xmlizedString = null;
                var xs = new XmlSerializer(data.GetType());
                //create an instance of the MemoryStream class since we intend to keep the XML string 
                //in memory instead of saving it to a file.
                var memoryStream = new MemoryStream();
                //XmlTextWriter - fast, non-cached, forward-only way of generating streams or files 
                //containing XML data
                var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                //Serialize emp in the xmlTextWriter
                xs.Serialize(xmlTextWriter, data);
                //Get the BaseStream of the xmlTextWriter in the Memory Stream
                memoryStream = (MemoryStream) xmlTextWriter.BaseStream;
                //Convert to array
                xmlizedString = Utf8ByteArrayToString(memoryStream.ToArray());
                return xmlizedString;
            }
            catch (Exception ex)
            {
                //build the error and send it back
                //build the data object
                //build the error
                //serialize the error and send it back
                return null;
            }
        }

        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private String Utf8ByteArrayToString(Byte[] characters)
        {
            var encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            if (constructedString.StartsWith(_byteOrderMarkUtf8))
                constructedString = constructedString.Remove(0, _byteOrderMarkUtf8.Length);

            return (constructedString);
        }

        /// <summary>
        /// Convert the string to a byte array
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] StringToByteArray(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        private readonly string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());


        private static Data GenerateError(string errorMessage, string code)
        {
            var data = new Data
                {
                    Error = new DataError {ItemsElementName = new ItemsChoiceType[2], Items = new string[2]}
                };
            //add the message
            data.Error.ItemsElementName[0] = ItemsChoiceType.Message;
            data.Error.Items[0] = errorMessage;
            //add the code
            data.Error.ItemsElementName[1] = ItemsChoiceType.Code;
            data.Error.Items[1] = code;
            return data;
        }

        #endregion
    }
}
