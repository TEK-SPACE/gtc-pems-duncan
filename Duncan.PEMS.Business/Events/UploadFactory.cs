using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Duncan.PEMS.Entities.Events;
using LINQtoCSV;
using NLog;


namespace Duncan.PEMS.Business.Events
{
    public class UploadFactory : EventCodesFactory
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
        public UploadFactory(string connectionStringName)
            : base(connectionStringName)
        {
        }

        private CsvContext _csvContext;
        private CsvFileDescription _csvFileDescription;


        #region Event Codes Upload Processing
        /// <summary>
        /// Process a event code file uplaoded for a specific customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public EventCodesUploadResultsModel Process(int customerId, string file)
        {
            var model = new EventCodesUploadResultsModel
                {
                    CustomerId = customerId,
                    UploadedFileName = file
                };

            // Prep the LINQtoCSV context.

            _csvContext = new CsvContext();
            _csvFileDescription = new CsvFileDescription
                {
                    MaximumNbrExceptions = 100,
                    SeparatorChar = ','
                };

            try
            {
                ProcessEventCodes(customerId, model);
            }
            catch (AggregatedException ae)
            {
                // Process all exceptions generated while processing the file
                var innerExceptionsList =
                    (List<Exception>)ae.Data["InnerExceptionsList"];

                foreach (Exception e in innerExceptionsList)
                {
                    model.Errors.Add(e.Message);
                }

            }
            catch (Exception ex)
            {
                model.Errors.Add("General exception");
                model.Errors.Add(ex.Message);
            }


            return model;
        }

        /// <summary>
        /// Uploads event codes for a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="model"></param>
        private void ProcessEventCodes(int customerId, EventCodesUploadResultsModel model)
        {
            IEnumerable<UploadEventCodesModel> eventCodes =
                _csvContext.Read<UploadEventCodesModel>(model.UploadedFileName, _csvFileDescription);

            foreach (var code in eventCodes)
            {
                bool canCreateEventCode = true;

                // First, validate that certain data, if present, resolves to the 
                // associated referential table.


                // "EventSource" == [EventSources]
                var eventSource = PemsEntities.EventSources.FirstOrDefault( m => m.EventSourceDesc.Equals( code.EventSource, StringComparison.CurrentCultureIgnoreCase ));
                if (eventSource == null)
                {
                    model.Errors.Add(string.Format("Record {0}, EventSource '{1}' is invalid.", code.Id, code.EventSource));
                    canCreateEventCode = false;
                }

                // "AlarmTier" == [AlarmTier]
                var alarmTier = PemsEntities.AlarmTiers.FirstOrDefault(m => m.TierDesc.Equals(code.AlarmTier, StringComparison.CurrentCultureIgnoreCase));
                if (alarmTier == null)
                {
                    model.Errors.Add(string.Format("Record {0}, AlarmTier '{1}' is invalid.", code.Id, code.AlarmTier));
                    canCreateEventCode = false;
                }

                // "EventType" == [EventType]
                var eventType = PemsEntities.EventTypes.FirstOrDefault(m => m.EventTypeDesc.Equals(code.EventType, StringComparison.CurrentCultureIgnoreCase));
                if (eventType == null)
                {
                    model.Errors.Add(string.Format("Record {0}, EventType '{1}' is invalid.", code.Id, code.EventType));
                    canCreateEventCode = false;
                }

                // "EventCategory" == [EventCategory]
                var eventCategory = PemsEntities.EventCategories.FirstOrDefault(m => m.EventCategoryDesc.Equals(code.EventCategory, StringComparison.CurrentCultureIgnoreCase));
                if (eventCategory == null)
                {
                    model.Errors.Add(string.Format("Record {0}, EventCategory '{1}' is invalid.", code.Id, code.EventCategory));
                    canCreateEventCode = false;
                }

                // "AssetType" == [MeterGroup]/[AssetType]
                var meterGroup =
                    (from at in PemsEntities.AssetTypes
                     join mg in PemsEntities.MeterGroups on at.MeterGroupId equals mg.MeterGroupId
                     where at.CustomerId == customerId && at.IsDisplay == true
                     where mg.MeterGroupDesc.Equals(code.AssetType, StringComparison.CurrentCultureIgnoreCase)
                     select mg).Distinct().FirstOrDefault();
                if (meterGroup == null)
                {
                    model.Errors.Add(string.Format("Record {0}, AssetType '{1}' is invalid.", code.Id, code.AssetType));
                    canCreateEventCode = false;
                }

                // Check if SLADuration was given and is greater than 0 and less than 1440.
                if ( code.SLADuration != null )
                {
                    if ( code.SLADuration <= 0 || code.SLADuration > 1440 )
                    {
                        model.Errors.Add(string.Format("Record {0}, SLADuration '{1}' is out of range. Range [1, 1440] or blank for no SLA.", code.Id, code.SLADuration));
                        canCreateEventCode = false;
                    }
                }

                // Check if EventCode is in range [0, int.MaxValue] or a -1.
                if ( code.EventCode < -1  )
                {
                    model.Errors.Add(string.Format("Record {0}, EventCode '{1}' is out of range. Range greater than 0 or -1 to generate an EventCode.", code.Id, code.EventCode));
                    canCreateEventCode = false;
                }


                // Can an Event Code be created?  If not skip to next record.
                if ( !canCreateEventCode )
                {
                    continue;
                }

                // Now should be able to create or update an EventCode

                // Does this EventCode already exist?
                var eventCodesFactory = new EventCodesFactory(ConnectionStringName);
                bool isNewEventCode = false;

                EventCodeEditModel eventCode = null;

                // Does this Event Code already exist?
                if ( code.EventCode >= 0 )
                {
                    eventCode = eventCodesFactory.GetEventCodeEditModel(customerId, eventSource.EventSourceCode, code.EventCode);
                }

                // Do I need to create a new event code?
                if ( eventCode == null )
                {
                    // A new event code needs to be created.
                    eventCode = eventCodesFactory.GetEventCodeEditModel(customerId);
                    // Record the event source id for the new event code.  This field is 
                    // part of the primary key.
                    eventCode.SourceId = eventSource.EventSourceCode;
                    eventCode.Id = code.EventCode;
                    isNewEventCode = true;
                }

                // Update the eventCode with the data from the CSV row.
                eventCode.AlarmTierId = alarmTier.Tier;
                eventCode.DescAbbrev = code.EventDescAbbrev.Trim().Substring(0, 16);
                eventCode.DescVerbose = code.EventDescVerbose.Trim().Substring(0, 50);
                eventCode.SLAMinutes = code.SLADuration;
                eventCode.ApplySLA = code.SLADuration != null && code.SLADuration > 0;
                eventCode.TypeId = eventType.EventTypeId;
                eventCode.IsAlarm = eventType.EventTypeId == eventCode.AlarmTypeId;
                eventCode.CategoryId = eventCategory.EventCategoryId;
                eventCode.AssetTypeId = meterGroup.MeterGroupId;

                // Wite the new/updated event code to [EventCodes]
                if ( isNewEventCode )
                {
                    int newId = eventCodesFactory.CreateEventCode(eventCode);
                    model.Results.Add(string.Format("Record {0}, EventCode with id of '{1}' created successfully.", code.Id, newId));
                }
                else
                {
                    eventCodesFactory.SetEventCodeEditModel(eventCode);
                    model.Results.Add(string.Format("Record {0}, EventCode '{1}' updated successfully.", code.Id, code.EventCode));
                }
            }
        }

        #endregion

        #region Constraints for Upload Files

        private const string ConstraintPad = "  ";
        private const string ConstraintEmpty = "[empty record]";
        private const string ConstraintReturn = "\n";
        private const string ConstraintSpacing = "\n\n";
        private const string FieldRequired = " - Required";
        private const string FieldOptional = " - Optional";


        /// <summary>
        /// Return a string that list the various constrained fields and the allowable data for each field.  This
        /// is used in conjunction with the following class - UploadEventCodesModel
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns></returns>
        public string ConstraintsList(int customerId)
        {
            return ConstraintsForEventCodes(customerId);
        }


        public string FieldsList(int customerId)
        {
            return FieldsForEventCodes(customerId);
        }

        /// <summary>
        /// Builds constraints for event codes for a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        private string ConstraintsForEventCodes(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            // "EventSource" == [EventSources]
            sb.Append("Field: EventSource").Append(ConstraintReturn);
            foreach (var row in PemsEntities.EventSources)
            {
                sb.Append(ConstraintPad).Append(row.EventSourceDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);
            
            // "AlarmTier" == [AlarmTier]
            sb.Append("Field: AlarmTier").Append(ConstraintReturn);
            foreach (var row in PemsEntities.AlarmTiers)
            {
                sb.Append(ConstraintPad).Append(row.TierDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // "EventType" == [EventType]
            sb.Append("Field: EventType").Append(ConstraintReturn);
            foreach (var row in PemsEntities.EventTypes)
            {
                sb.Append(ConstraintPad).Append(row.EventTypeDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // "EventCategory" == [EventCategory]
            sb.Append("Field: EventCategory").Append(ConstraintReturn);
            foreach (var row in PemsEntities.EventCategories)
            {
                sb.Append(ConstraintPad).Append(row.EventCategoryDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);
            
            // "AssetType" == [MeterGroup]/[AssetType]
            sb.Append("Field: AssetType").Append(ConstraintReturn);
            var allowableMeterGroups =
                (from at in PemsEntities.AssetTypes
                 join mg in PemsEntities.MeterGroups on at.MeterGroupId equals mg.MeterGroupId
                 where at.CustomerId == customerId && at.IsDisplay == true
                 select new { Id = mg.MeterGroupId, Desc = mg.MeterGroupDesc }).Distinct();
            foreach (var row in allowableMeterGroups)
            {
                sb.Append(ConstraintPad).Append(row.Desc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            return sb.ToString();
        }

        /// <summary>
        /// Builds fields for event codes
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        private string FieldsForEventCodes(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Field Notes:").Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("Id").Append(FieldRequired).Append(", Record row number used to report on results.").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("EventSource").Append(", See Field: EventSource for allowable values.").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("EventCode").Append(", Value of -1 will automatically generate an EventCode value.").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("AlarmTier").Append(", See Field: AlarmTier for allowable values.").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("EventType").Append(", See Field: EventType for allowable values.").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("EventCategory").Append(", See Field: EventCategory for allowable values.").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("AssetType").Append(", See Field: AssetType for allowable values.").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("EventDescAbbrev").Append(", Will be truncated to 16 characters.").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("EventDescVerbose").Append(", Will be truncated to 50 characters.").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("SLADuration").Append(", In minutes. Blank indicates no SLA.").Append(FieldOptional).Append(ConstraintReturn);

            return sb.ToString();
        }

        #endregion

    }
}
