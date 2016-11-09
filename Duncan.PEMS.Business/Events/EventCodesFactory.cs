/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                   DESCRIPTION
 * ___________      ___________________        _________________________________________________________________________________________________________
 * 
 * 02/10/2014       Sergey Ostrerov            DPTXPEMS-237 - Edit Screen for Event Code Crashes
 *                                                            Create/Update Event Code   
 * 
 * *****************************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Events;
using Duncan.PEMS.Entities.General;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Business.Events
{
    public class EventCodesFactory : BaseFactory
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
        public EventCodesFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }
        #region Index

        /// <summary>
        /// Get list of event code items for a specific customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IQueryable<EventCodeViewModel> GetSummaryModels(int customerId)
        {
            var ecm = from ec in PemsEntities.EventCodes
                      where ec.CustomerID == customerId
                      where ec.EventCode1 >= 0
                      join ecat in PemsEntities.EventCodeAssetTypes on
                            new { EventCodeId = ec.EventCode1, EventSourceId = ec.EventSource, CustomerId = ec.CustomerID } equals
                            new { EventCodeId = ecat.EventCode, EventSourceId = ecat.EventSource, CustomerId = ecat.CustomerID } into assetType
                      from at in assetType.DefaultIfEmpty()
                      select new EventCodeViewModel()
                      {
                          CustomerId = customerId,
                          SourceId = ec.EventSource,
                          Source = ec.EventSource1.EventSourceDesc,
                          AlarmTierId = ec.AlarmTier,
                          AlarmTier = ec.AlarmTier1.TierDesc,
                          TypeId = ec.EventType ?? -1,
                          Type = ec.EventType1 == null ? null : ec.EventType1.EventTypeDesc,
                          DescAbbrev = ec.EventDescAbbrev,
                          DescVerbose = ec.EventDescVerbose,
                          SLAMinutes = ec.SLAMinutes,
                          ApplySLA = ec.ApplySLA ?? false,
                          IsAlarm = ec.IsAlarm ?? false,
                          CategoryId = ec.EventCategory ?? -1,
                          Category = ec.EventCategory1 == null ? null : ec.EventCategory1.EventCategoryDesc,
                          Id = ec.EventCode1,
                          AssetTypeId = at == null ? -1 : at.MeterGroupId,
                          AssetType = at == null ? null : (at.MeterGroup == null ? null : at.MeterGroup.MeterGroupDesc)
                      };
            return ecm;

        }


        #endregion




        #region Customer Base
        /// <summary>
        /// Get the details ov an event code
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public EventCodesViewModel GetEventCodesViewModel(int customerId)
        {
            var model = new EventCodesViewModel {CustomerId = customerId};
            GetEventCodesCustomerModel(model);
            var codes = (new EventCodesFactory(ConnectionStringName)).GetSummaryModels(customerId);
            model.Codes = codes.ToList();
            model.Status = (new CustomerFactory(ConnectionStringName)).GetCustomerStatusModel(customerId);
            return model;
        }

        /// <summary>
        /// Populates the event code model with the customer display name
        /// </summary>
        /// <param name="model"></param>
        private void GetEventCodesCustomerModel(EventCodesViewModel model)
        {
            if (model == null) return;
            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == model.CustomerId);
            model.CustomerDisplayName = customerProfile != null ? customerProfile.DisplayName : "[Undefined]";
        }

        /// <summary>
        /// Gets an event code customer model
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public EventCodesCustomerModel GetEventCodesCustomerModel(int customerId)
        {
            EventCodesCustomerModel model = new EventCodesCustomerModel()
                {
                    CustomerId = customerId
                };
            
            GetEventCodesCustomerModel( model );

            return model;
        }

        /// <summary>
        /// Populates the event code model with the customer display name
        /// </summary>
        /// <param name="model"></param>
        private void GetEventCodesCustomerModel(EventCodesCustomerModel model)
        {
            if ( model != null )
            {

                var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault( m => m.CustomerId == model.CustomerId );
                if ( customerProfile != null )
                {
                    model.CustomerDisplayName = customerProfile.DisplayName;
                }
                else
                {
                    model.CustomerDisplayName = "[Undefined]";
                }
            }
        }

        #endregion


        #region View Model
        /// <summary>
        /// Gets an event code VIEW model
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="eventSourceId"></param>
        /// <param name="eventCodeId"></param>
        /// <returns></returns>
        public EventCodeViewModel GetEventCodeViewModel(int customerId, int eventSourceId, int eventCodeId)
        {
            var ecm = from ec in PemsEntities.EventCodes
                      where ec.CustomerID == customerId && ec.EventSource == eventSourceId
                            && ec.EventCode1 == eventCodeId
                      select new EventCodeViewModel()
                          {
                              CustomerId = customerId,
                              SourceId = ec.EventSource,
                              Source = ec.EventSource1.EventSourceDesc,
                              AlarmTierId = ec.AlarmTier,
                              AlarmTier = ec.AlarmTier1.TierDesc,
                              TypeId = ec.EventType ?? -1,
                              Type = ec.EventType1 == null ? null : ec.EventType1.EventTypeDesc,
                              DescAbbrev = ec.EventDescAbbrev,
                              DescVerbose = ec.EventDescVerbose,
                              SLAMinutes = ec.SLAMinutes ?? -1,
                              ApplySLA = ec.ApplySLA ?? false,
                              IsAlarm = ec.IsAlarm ?? false,
                              CategoryId = ec.EventCategory ?? -1,
                              Category = ec.EventCategory1 == null ? null : ec.EventCategory1.EventCategoryDesc,
                              Id = ec.EventCode1
                          };

            var model = ecm.FirstOrDefault();

            GetEventCodesCustomerModel( model );

            if ( model != null )
            {
                model.AssetTypeId = -1;

                var ecat = PemsEntities.EventCodeAssetTypes.FirstOrDefault(m => m.CustomerID == customerId && m.EventSource == model.SourceId && m.EventCode == model.Id);
                if(ecat != null && ecat.MeterGroup != null)
                {
                    model.AssetTypeId = ecat.MeterGroup.MeterGroupId;
                    model.AssetType = ecat.MeterGroup.MeterGroupDesc;
                }
            }



            return model;
        }

        #endregion

        #region Edit Model
        /// <summary>
        /// GEts an event code EDIT model
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="eventSourceId"></param>
        /// <param name="eventCodeId"></param>
        /// <returns></returns>
        public EventCodeEditModel GetEventCodeEditModel(int customerId, int eventSourceId = -1, int eventCodeId = -1)
        {
            EventCodeEditModel model = null;

            if ( eventCodeId > -1 )
            {
                // Get existing EventCode
                model = (from ec in PemsEntities.EventCodes
                          where ec.CustomerID == customerId && ec.EventSource == eventSourceId
                                && ec.EventCode1 == eventCodeId
                          select new EventCodeEditModel()
                              {
                                  CustomerId = customerId,
                                  SourceId = ec.EventSource,
                                  AlarmTierId = ec.AlarmTier,
                                  TypeId = ec.EventType ?? -1,
                                  DescAbbrev = ec.EventDescAbbrev,
                                  DescVerbose = ec.EventDescVerbose,
                                  SLAMinutes = ec.SLAMinutes,
                                  ApplySLA = ec.ApplySLA ?? false,
                                  IsAlarm = ec.IsAlarm ?? false,
                                  CategoryId = ec.EventCategory ?? -1,
                                  Id = ec.EventCode1,
                                  AssetTypeId = -1
                              }).FirstOrDefault() ;
            }
            else
            {
                // Create a model that will be used to creat a new EventCode
                model = new EventCodeEditModel()
                              {
                                  CustomerId = customerId,
                                  SourceId = -1,
                                  AlarmTierId = -1,
                                  TypeId = -1,
                                  CategoryId = -1,
                                  Id = -1,
                                  AssetTypeId = -1
                              };
            }

            GetEventCodesCustomerModel(model);

            if ( model != null )
            {
                model.Category = EventCategoryList(model.CategoryId);
                model.Type = EventTypeList(model.TypeId);

                // Determine the "Alarm Type" type Id
                model.AlarmTypeId = -1;
                foreach (var type in model.Type)
                {
                    if ( type.Text.Contains( "Alarm" ) || type.Text.Contains( "alarm" ) )
                    {
                        model.AlarmTypeId = int.Parse( type.Value );
                        break;
                    }
                }

                model.Source = EventSourceList(model.SourceId);

                // Get the present event code source
                foreach (var source in model.Source)
                {
                    if ( source.Selected )
                    {
                        model.SourceDisplay = source.Text;
                        break;
                    }
                    
                }

                model.AlarmTier = AlarmTierList(model.AlarmTierId);

                // Get Asset Type Id
                var ecat = PemsEntities.EventCodeAssetTypes.FirstOrDefault(m => m.CustomerID == customerId && m.EventSource == model.SourceId && m.EventCode == model.Id);
                if (ecat != null && ecat.MeterGroup != null)
                {
                    model.AssetTypeId = ecat.MeterGroupId;
                }
                model.AssetType = MeterGroupList( customerId, model.AssetTypeId );
            }

            return model;
        }

        /// <summary>
        /// Updates the event code edit model
        /// </summary>
        /// <param name="model"></param>
        public void RefreshEventCodeEditModel(EventCodeEditModel model)
        {
            GetEventCodesCustomerModel(model);

            if (model != null)
            {
                model.Category = EventCategoryList(model.CategoryId);
                model.Type = EventTypeList(model.TypeId);
                model.Source = EventSourceList(model.SourceId);
                model.AlarmTier = AlarmTierList(model.AlarmTierId);

                // Get Asset Type Id
                var ecat = PemsEntities.EventCodeAssetTypes.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.EventSource == model.SourceId && m.EventCode == model.Id);
                if (ecat != null && ecat.MeterGroup != null)
                {
                    model.AssetTypeId = ecat.MeterGroupId;
                }
                model.AssetType = MeterGroupList(model.CustomerId, model.AssetTypeId);
            }
        }

        /// <summary>
        /// sets the event code edit model with the event code data in the system
        /// </summary>
        /// <param name="model"></param>
        public void SetEventCodeEditModel(EventCodeEditModel model)
        {
            // Get the original EventCode
            var eventCode = PemsEntities.EventCodes.FirstOrDefault( m => m.CustomerID == model.CustomerId && m.EventSource == model.SourceId && m.EventCode1 == model.Id );

            if ( eventCode != null )
            {
                eventCode.AlarmTier = model.AlarmTierId;
                eventCode.EventDescAbbrev = model.DescAbbrev;
                eventCode.EventDescVerbose = model.DescVerbose;
                eventCode.SLAMinutes = model.ApplySLA ? model.SLAMinutes : null;
                eventCode.ApplySLA = model.ApplySLA;
                eventCode.IsAlarm = model.IsAlarm;
                eventCode.EventType = model.TypeId;
                eventCode.EventCategory = model.CategoryId;

                // Insert/update [EventCodeAssetType]
                var ecat = PemsEntities.EventCodeAssetTypes.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.EventSource == model.SourceId && m.EventCode == model.Id);
                if ( ecat == null )
                {
                    ecat = new EventCodeAssetType()
                        {
                            CustomerID = model.CustomerId,
                            EventSource = model.SourceId,
                            EventCode = model.Id,
                        };
                    PemsEntities.EventCodeAssetTypes.Add( ecat );
                }
                ecat.MeterGroupId = model.AssetTypeId;

                // Save changes
                PemsEntities.SaveChanges();

                // Add audit record.
                Audit(eventCode);
            }
        }

        #endregion

        #region Create Model

        /// <summary>
        /// Check to see if there is already an EventCode for this Customer with the following attributes:
        /// SourceId, TypeId, CategoryId and AssetTypeId
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CanCreateEventCode(EventCodeEditModel model)
        {
            // Check to see if there is already an EventCode for this Customer with the following attributes:
            // SourceId, TypeId, CategoryId and AssetTypeId
            EventCodeAssetType ecat = null;
            EventCode ecm = PemsEntities.EventCodes.FirstOrDefault( m =>
                                                              m.CustomerID == model.CustomerId
                                                              && m.EventSource == model.SourceId
                                                              && m.EventType == model.TypeId
                                                              && m.EventCategory == model.CategoryId );

            if ( ecm != null )
            {
                // Does this EventCode have the same asset type already assigned?
                ecat = PemsEntities.EventCodeAssetTypes.FirstOrDefault(m => 
                    m.CustomerID == ecm.CustomerID && m.EventSource == ecm.EventSource && m.EventCode == ecm.EventCode1);
            }

            return (ecm != null && ecat != null);
        }

        /// <summary>
        /// Created an event code int he system
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int CreateEventCode(EventCodeEditModel model)
        {
            EventCode eventCode = null;

            // Get next EventCode id for a given customer & EventSource.
            // If model.Id is non-zero then a particular event code id has been requested.

            int nextId = model.Id;
            eventCode = PemsEntities.EventCodes.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.EventSource == model.SourceId && m.EventCode1 == nextId);
            if (eventCode == null)
            {
                if (nextId < 0)
                {
                    nextId = 1;
                    eventCode = PemsEntities.EventCodes.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.EventSource == model.SourceId);
                    if (eventCode != null)
                    {
                        nextId = PemsEntities.EventCodes.Where(m => m.CustomerID == model.CustomerId && m.EventSource == model.SourceId).Max(m => m.EventCode1) + 1;
                    }
                }

                // Create the new EventCode
                eventCode = new EventCode()
                    {
                        CustomerID = model.CustomerId,
                        EventSource = model.SourceId,
                        EventCode1 = nextId,
                        AlarmTier = model.AlarmTierId,
                        EventDescAbbrev = model.DescAbbrev,
                        EventDescVerbose = model.DescVerbose,
                        SLAMinutes = model.ApplySLA ? model.SLAMinutes : null,
                        ApplySLA = model.ApplySLA,
                        IsAlarm = model.IsAlarm,
                        EventType = model.TypeId,
                        EventCategory = model.CategoryId
                    };
                PemsEntities.EventCodes.Add(eventCode);
                PemsEntities.SaveChanges();
            }
            else
            {
                eventCode.AlarmTier = model.AlarmTierId;
                eventCode.EventDescAbbrev = model.DescAbbrev;
                eventCode.EventDescVerbose = model.DescVerbose;
                eventCode.SLAMinutes = model.ApplySLA ? model.SLAMinutes : null;
                eventCode.ApplySLA = model.ApplySLA;
                eventCode.IsAlarm = model.IsAlarm;
                eventCode.EventType = model.TypeId;
                eventCode.EventCategory = model.CategoryId;
                PemsEntities.SaveChanges();

            }

            // Insert/update [EventCodeAssetType]
            var ecat = PemsEntities.EventCodeAssetTypes.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.EventSource == model.SourceId && m.EventCode == nextId);
            if (ecat == null)
            {
                ecat = new EventCodeAssetType()
                {
                    CustomerID = model.CustomerId,
                    EventSource = model.SourceId,
                    EventCode = nextId,
                };
                PemsEntities.EventCodeAssetTypes.Add(ecat);
            }
            ecat.MeterGroupId = model.AssetTypeId;

            // Save changes
            PemsEntities.SaveChanges();

            // Add audit record.
            Audit(eventCode);

            // Return new EventCode id
            return nextId;
        }


        #endregion


        #region List Helper Methods

        /// <summary>
        /// Gets a list of items that represent event categories and denotes the selected one that matches the selectedId passed in
        /// </summary>
        /// <param name="selectedId"></param>
        /// <returns></returns>
        private List<SelectListItemWrapper> EventCategoryList(int selectedId)
        {
            List<SelectListItemWrapper> list = (from ec in PemsEntities.EventCategories
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = ec.EventCategoryId == selectedId,
                                                    Text = ec.EventCategoryDesc,
                                                    ValueInt = ec.EventCategoryId
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }


        /// <summary>
        /// Gets a list of items that represent event types and denotes the selected one that matches the selectedId passed in
        /// </summary>
        /// <param name="selectedId"></param>
        /// <returns></returns>
        private List<SelectListItemWrapper> EventTypeList(int selectedId)
        {
            List<SelectListItemWrapper> list = (from et in PemsEntities.EventTypes
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = et.EventTypeId == selectedId,
                                                    Text = et.EventTypeDesc,
                                                    ValueInt = et.EventTypeId
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }

        /// <summary>
        /// Gets a list of items that represent event sources and denotes the selected one that matches the selectedId passed in
        /// </summary>
        /// <param name="selectedId"></param>
        /// <returns></returns>
        private List<SelectListItemWrapper> EventSourceList(int selectedId)
        {
            List<SelectListItemWrapper> list = (from es in PemsEntities.EventSources
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = es.EventSourceCode == selectedId,
                                                    Text = es.EventSourceDesc,
                                                    ValueInt = es.EventSourceCode
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }

        /// <summary>
        /// Gets a list of items that represent alarm tiers and denotes the selected one that matches the selectedId passed in
        /// </summary>
        /// <param name="selectedId"></param>
        /// <returns></returns>
        private List<SelectListItemWrapper> AlarmTierList(int selectedId)
        {
            List<SelectListItemWrapper> list = (from at in PemsEntities.AlarmTiers
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = at.Tier == selectedId,
                                                    Text = at.TierDesc,
                                                    ValueInt = at.Tier
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }

        /// <summary>
        /// Gets a list of items that represent asset types / meter groups and denotes the selected one that matches the selectedId passed in
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="selectedId"></param>
        /// <returns></returns>
        private List<SelectListItemWrapper> MeterGroupList(int customerId, int selectedId)
        {
            var allowableMeterGroups =
                ( from at in PemsEntities.AssetTypes
                  join mg in PemsEntities.MeterGroups on at.MeterGroupId equals mg.MeterGroupId
                  where at.CustomerId == customerId && at.IsDisplay == true
                  select new {Id = mg.MeterGroupId, Desc = mg.MeterGroupDesc} ).Distinct();

            List<SelectListItemWrapper> list = allowableMeterGroups.Select(m => 
                                                new SelectListItemWrapper()
                                                {
                                                    Selected = m.Id == selectedId,
                                                    Text = m.Desc,
                                                    ValueInt = m.Id
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }



        #endregion


        #region Asset SLAs

        /// <summary>
        /// Gets the default SLA for the asset type passed in (meter group)
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="meterGroupId"></param>
        /// <returns></returns>
        public int GetAssetTypeDefaultSLA(int customerId, int meterGroupId)
        {
                AssetType assetType = PemsEntities.AssetTypes.FirstOrDefault(m => m.CustomerId == customerId && m.MeterGroupId == meterGroupId && m.IsDisplay == true);
                return assetType == null ? -1 : assetType.SLAMinutes ?? 300;
        }

        #endregion


        #region Audits


        /// <summary>
        /// Adds an audit record to [EventCodesAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="EventCode"/> instance to add to audit table.</param>
        public void Audit(EventCode itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            EventCodesAudit audit = new EventCodesAudit();

            audit.CustomerID = itemToAudit.CustomerID;
            audit.EventSource = itemToAudit.EventSource;
            audit.EventCode = itemToAudit.EventCode1;
            audit.AlarmTier = itemToAudit.AlarmTier;
            audit.EventDescAbbrev = itemToAudit.EventDescAbbrev;
            audit.EventDescVerbose = itemToAudit.EventDescVerbose;
            audit.SLAMinutes = itemToAudit.SLAMinutes;
            audit.IsAlarm = itemToAudit.IsAlarm;
            audit.EventType = itemToAudit.EventType;
            audit.ApplySLA = itemToAudit.ApplySLA;
            audit.EventCategory = itemToAudit.EventCategory;

            audit.UserId = WebSecurity.CurrentUserId;
            audit.UpdatedDateTime = DateTime.Now;

            PemsEntities.EventCodesAudits.Add(audit);
            PemsEntities.SaveChanges();
        }



        #endregion


    }
}
