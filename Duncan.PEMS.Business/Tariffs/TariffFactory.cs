using System;
using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Entities.Tariffs;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Business.Tariffs
{
    public class TariffFactory : BaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// This is the common <see cref="DateTime"/> used for any configuration/tariff 
        /// creation and other customer-centric time uses.  By default it is set to DateTime.Now.  
        /// Alternative DateTimes can be passed via constructor.
        /// </summary>
        protected DateTime Now = DateTime.Now;

        /// <summary>
        /// This is the factory constructor for tariff business.  It takes a 
        /// a name of a connection string, <paramref name="connectionStringName"/>, so as to point to a specific instance of the PEMS database.  Also takes
        /// a <see cref="DateTime"/> <paramref name="customerNow"/> to reference the time at the customer's physical location.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        /// <param name="customerNow">
        /// <see cref="DateTime"/> indication of 'now' for the factory.  Generally used to ensure that
        /// the factory is set to the customer's timezone-adjusted 'now'.
        /// </param>
        public TariffFactory(string connectionStringName, DateTime customerNow)
        {
            Now = customerNow;
            ConnectionStringName = connectionStringName;

        }

        public TariffFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }


        #region Index

        public IQueryable<ConfigProfileModel> GetConfigProfileModels(long startDateTicks, long endDateTicks, int customerId)
        {
            DateTime startDate;
            DateTime endDate;


            var configProfileAggregateList = new List<ConfigProfile>();


            // Get [ConfigProfile] rows where there is a relationship to one or more of the following tables:
            //   [HolidayRateConfigurationProfile]
            //   [TariffRateConfigurationProfile]
            //   [RateScheduleConfigurationProfile]

            if ( startDateTicks > 0 && endDateTicks > 0 )
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();

                configProfileAggregateList = ( from cp in PemsEntities.ConfigProfiles
                                               join hrcp in PemsEntities.HolidayRateConfigurations on cp.ConfigProfileId equals hrcp.ConfigProfileId
                                               where hrcp.CustomerId == customerId
                                               where hrcp.ConfiguredOn >= startDate && hrcp.ConfiguredOn <= endDate
                                               where hrcp.State == (int)TariffStateType.Current
                                               select cp).ToList();

                configProfileAggregateList.AddRange( ( from cp in PemsEntities.ConfigProfiles
                                                       join trcp in PemsEntities.TariffRateConfigurations on cp.ConfigProfileId equals trcp.ConfigProfileId
                                                       where trcp.CustomerId == customerId
                                                       where trcp.ConfiguredOn >= startDate && trcp.ConfiguredOn <= endDate
                                                       where trcp.State == (int)TariffStateType.Current
                                                       select cp).ToList());

                configProfileAggregateList.AddRange( ( from cp in PemsEntities.ConfigProfiles
                                                       join rscp in PemsEntities.RateScheduleConfigurations on cp.ConfigProfileId equals rscp.ConfigProfileId
                                                       where rscp.CustomerId == customerId
                                                       where rscp.ConfiguredOn >= startDate && rscp.ConfiguredOn <= endDate
                                                       where rscp.State == (int)TariffStateType.Current
                                                       select cp).ToList());
            }
            else if ( startDateTicks > 0 )
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();

                configProfileAggregateList = (from cp in PemsEntities.ConfigProfiles
                                              join hrcp in PemsEntities.HolidayRateConfigurations on cp.ConfigProfileId equals hrcp.ConfigProfileId
                                              where hrcp.CustomerId == customerId
                                              where hrcp.ConfiguredOn >= startDate
                                              where hrcp.State == (int)TariffStateType.Current
                                              select cp).ToList();

                configProfileAggregateList.AddRange((from cp in PemsEntities.ConfigProfiles
                                                     join trcp in PemsEntities.TariffRateConfigurations on cp.ConfigProfileId equals trcp.ConfigProfileId
                                                     where trcp.CustomerId == customerId
                                                     where trcp.ConfiguredOn >= startDate
                                                     where trcp.State == (int)TariffStateType.Current
                                                     select cp).ToList());

                configProfileAggregateList.AddRange((from cp in PemsEntities.ConfigProfiles
                                                     join rscp in PemsEntities.RateScheduleConfigurations on cp.ConfigProfileId equals rscp.ConfigProfileId
                                                     where rscp.CustomerId == customerId
                                                     where rscp.ConfiguredOn >= startDate
                                                     where rscp.State == (int)TariffStateType.Current
                                                     select cp).ToList());
            }
            else if ( endDateTicks > 0 )
            {
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();

                configProfileAggregateList = (from cp in PemsEntities.ConfigProfiles
                                              join hrcp in PemsEntities.HolidayRateConfigurations on cp.ConfigProfileId equals hrcp.ConfigProfileId
                                              where hrcp.CustomerId == customerId
                                              where hrcp.ConfiguredOn <= endDate
                                              where hrcp.State == (int)TariffStateType.Current
                                              select cp).ToList();

                configProfileAggregateList.AddRange((from cp in PemsEntities.ConfigProfiles
                                                     join trcp in PemsEntities.TariffRateConfigurations on cp.ConfigProfileId equals trcp.ConfigProfileId
                                                     where trcp.CustomerId == customerId
                                                     where trcp.ConfiguredOn <= endDate
                                                     where trcp.State == (int)TariffStateType.Current
                                                     select cp).ToList());

                configProfileAggregateList.AddRange((from cp in PemsEntities.ConfigProfiles
                                                     join rscp in PemsEntities.RateScheduleConfigurations on cp.ConfigProfileId equals rscp.ConfigProfileId
                                                     where rscp.CustomerId == customerId
                                                     where rscp.ConfiguredOn <= endDate
                                                     where rscp.State == (int)TariffStateType.Current
                                                     select cp).ToList());
            }
            else
            {
                configProfileAggregateList = (from cp in PemsEntities.ConfigProfiles
                                              join hrcp in PemsEntities.HolidayRateConfigurations on cp.ConfigProfileId equals hrcp.ConfigProfileId
                                              where hrcp.CustomerId == customerId
                                              where hrcp.State == (int)TariffStateType.Current
                                              select cp).ToList();

                configProfileAggregateList.AddRange((from cp in PemsEntities.ConfigProfiles
                                                     join trcp in PemsEntities.TariffRateConfigurations on cp.ConfigProfileId equals trcp.ConfigProfileId
                                                     where trcp.CustomerId == customerId
                                                     where trcp.State == (int)TariffStateType.Current
                                                     select cp).ToList());

                configProfileAggregateList.AddRange((from cp in PemsEntities.ConfigProfiles
                                                     join rscp in PemsEntities.RateScheduleConfigurations on cp.ConfigProfileId equals rscp.ConfigProfileId
                                                     where rscp.CustomerId == customerId
                                                     where rscp.State == (int)TariffStateType.Current
                                                     select cp).ToList());
            }



            // Now have list of [ConfigProfile] objects which are related to the customer.
            // Get unique items.
            var configProfileList = configProfileAggregateList.Distinct();


            // For each [ConfigProfile] in the configProfileList need to make a ConfigProfileSpaceViewModel
            // and populate with the data from the following tables (if exists)
            //   [HolidayRateConfiguration]
            //   [TariffRateConfiguration]
            //   [RateScheduleConfiguration]
            //   [ConfigProfileSpace]

            List<ConfigProfileModel> cpvmList = new List<ConfigProfileModel>();

            foreach (var configProfile in configProfileList)
            {
                ConfigProfileModel cpvm = new ConfigProfileModel()
                    {
                        CustomerId = customerId,
                        ConfigProfileId = configProfile.ConfigProfileId,
                        ConfigurationName = configProfile.ConfigurationName,
                        //Version6 = configProfile.Version6 ?? "",
                        TariffPolicyName = configProfile.TariffPolicyName ?? "",
                        Minute15FreeParking = configProfile.Minunte15FreeParking ?? false,
                        CreatedOn = configProfile.CreatedOn,
                        CreatedBy = configProfile.CreatedBy
                    };


                // Are there [TariffRateConfiguration] assigned to this configProfile?
                var trcp = configProfile.TariffRateConfigurations.FirstOrDefault(m => m.CustomerId == customerId && m.ConfigProfileId == configProfile.ConfigProfileId);
                if (trcp != null)
                {
                    cpvm.TariffRatesCount = PemsEntities.TariffRateForConfigurations.Where(m => m.TariffRateConfigurationId == trcp.TariffRateConfigurationId).Distinct().Count();
                    cpvm.TariffRateConfigurationId = trcp.TariffRateConfigurationId;
                }

                // Are there [RateScheduleConfiguration] assigned to this configProfile?
                var rscp = configProfile.RateScheduleConfigurations.FirstOrDefault(m => m.CustomerId == customerId && m.ConfigProfileId == configProfile.ConfigProfileId);
                if (rscp != null)
                {
                    cpvm.RateSchedulesCount = PemsEntities.RateScheduleForConfigurations.Where(m => m.RateScheduleConfigurationId == rscp.RateScheduleConfigurationId).Distinct().Count();
                    cpvm.RateScheduleConfigurationId = rscp.RateScheduleConfigurationId;
                }

                // Are there [HolidayRateConfiguration] assigned to this configProfile?
                var hrcp = configProfile.HolidayRateConfigurations.FirstOrDefault(m => m.CustomerId == customerId && m.ConfigProfileId == configProfile.ConfigProfileId);
                if (hrcp != null)
                {
                    cpvm.HolidayRatesCount = PemsEntities.HolidayRateForConfigurations.Where(m => m.HolidayRateConfigurationId == hrcp.HolidayRateConfigurationId ).Distinct().Count();
                    cpvm.HolidayRateConfigurationId = hrcp.HolidayRateConfigurationId;
                }

                cpvmList.Add(cpvm);
            }

            return cpvmList.AsQueryable();

        }


        #endregion

        #region Tarif Config Id Generation

        public Int64 NextConfigId()
        {
            var id = new ConfigurationIDGen() { GenDate = Now };
            PemsEntities.ConfigurationIDGens.Add( id );
            PemsEntities.SaveChanges();
            return id.ConfigurationID;
        }

        #endregion


        #region Config Profile (Configured Tariffs)


        public void CreateConfigProfile(ConfigProfileModel model )
        {
            // Create a new ConfigProfile entry to get a ConfigProfileId
            var configProfile = new ConfigProfile()
                {
                    ConfigurationName = model.ConfigurationName,
                    //Version6 = model.Version6,
                    TariffPolicyName = model.TariffPolicyName,
                    Minunte15FreeParking = model.Minute15FreeParking,
                    CreatedBy = WebSecurity.CurrentUserId,
                    CreatedOn = Now
                };
            PemsEntities.ConfigProfiles.Add( configProfile );
            PemsEntities.SaveChanges();

            // Update the associated TariffRateConfiguration
            var trc = PemsEntities.TariffRateConfigurations.FirstOrDefault( m => m.CustomerId == model.CustomerId && m.TariffRateConfigurationId == model.TariffRateConfigurationId );
            trc.ConfigProfileId = configProfile.ConfigProfileId;

            // Update the associated RateScheduleConfiguration
            var rsc = PemsEntities.RateScheduleConfigurations.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.RateScheduleConfigurationId == model.RateScheduleConfigurationId);
            rsc.ConfigProfileId = configProfile.ConfigProfileId;

            // Optionally update the HolidayRateConfiguration if it was used.
            if ( model.HolidayRateConfigurationState == TariffStateType.Current )
            {
                var hrc = PemsEntities.HolidayRateConfigurations.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.HolidayRateConfigurationId == model.HolidayRateConfigurationId);
                hrc.ConfigProfileId = configProfile.ConfigProfileId;
            }

            // Now save the updates.
            PemsEntities.SaveChanges();
        }


        public ConfigProfileSpaceViewModel GetConfigProfileSpaceViewModel(int customerId, Int64 configProfileSpaceId, Int64 spaceId)
        {
            ConfigProfileSpaceViewModel viewModel = null;

            var configProfileSpace = PemsEntities.ConfigProfileSpaces.FirstOrDefault( m => m.ConfigProfileSpaceId == configProfileSpaceId );

            if ( configProfileSpace != null )
            {
                viewModel = GetConfigProfileSpaceViewModel( customerId, configProfileSpace.ConfigProfileId );

                // Get the space view model.
                viewModel.Space = (new SpaceFactory(ConnectionStringName, DateTime.Now)).GetViewModel(spaceId, null);

                // Get the remainder of ConfigProfileSpaceViewModel properties.

                viewModel.ScheduledDate = configProfileSpace.ScheduledDate;

                viewModel.ActivationDate = configProfileSpace.ActivationDate;

                viewModel.CreationDate = configProfileSpace.CreationDate;

                viewModel.EndDate = configProfileSpace.EndDate;

                viewModel.Status = configProfileSpace.ConfigStatu == null ? null : configProfileSpace.ConfigStatu.ConfigStatusDest;

                viewModel.UserName = configProfileSpace.UserId == null ? null 
                    : ( new UserFactory() ).GetUserById( (int)configProfileSpace.UserId ).FullName();

            }
            return viewModel;
        }

        public ConfigProfileSpaceViewModel GetConfigProfileSpaceViewModel(int customerId, Int64 configProfileId)
        {
            ConfigProfileSpaceViewModel model = null;

            var configProfile = PemsEntities.ConfigProfiles.FirstOrDefault( m => m.ConfigProfileId == configProfileId );

            if ( configProfile != null )
            {
                model = new ConfigProfileSpaceViewModel()
                    {
                        CustomerId = customerId,
                        ConfigProfileId = configProfileId,
                        ConfigurationName = configProfile.ConfigurationName,
                        //Version6 = configProfile.Version6,
                        TariffPolicyName = configProfile.TariffPolicyName,
                        Minute15FreeParking = configProfile.Minunte15FreeParking ?? false,
                        CreatedOn = configProfile.CreatedOn,
                        CreatedBy = configProfile.CreatedBy
                    };


                // Are there [TariffRateConfiguration] assigned to this configProfile?
                var trcp = configProfile.TariffRateConfigurations.FirstOrDefault(m => m.CustomerId == customerId && m.ConfigProfileId == configProfile.ConfigProfileId);
                if (trcp != null)
                {
                    model.TariffRatesCount = PemsEntities.TariffRateForConfigurations.Where(m => m.TariffRateConfigurationId == trcp.TariffRateConfigurationId).Distinct().Count();
                    model.TariffRateConfigurationId = trcp.TariffRateConfigurationId;
                }

                // Are there [RateScheduleConfiguration] assigned to this configProfile?
                var rscp = configProfile.RateScheduleConfigurations.FirstOrDefault(m => m.CustomerId == customerId && m.ConfigProfileId == configProfile.ConfigProfileId);
                if (rscp != null)
                {
                    model.RateSchedulesCount = PemsEntities.RateScheduleForConfigurations.Where(m => m.RateScheduleConfigurationId == rscp.RateScheduleConfigurationId).Distinct().Count();
                    model.RateScheduleConfigurationId = rscp.RateScheduleConfigurationId;
                }

                // Are there [HolidayRateConfiguration] assigned to this configProfile?
                var hrcp = configProfile.HolidayRateConfigurations.FirstOrDefault(m => m.CustomerId == customerId && m.ConfigProfileId == configProfile.ConfigProfileId);
                if (hrcp != null)
                {
                    model.HolidayRatesCount = PemsEntities.HolidayRateForConfigurations.Where(m => m.HolidayRateConfigurationId == hrcp.HolidayRateConfigurationId).Distinct().Count();
                    model.HolidayRateConfigurationId = hrcp.HolidayRateConfigurationId;
                }
            }

            return model;
        }

        #region Configured Tariff Rates

        public List<TariffRateModel> GetConfiguredTariffRates(int customerId, Int64 configProfileId, Int64 tariffRateConfigurationId)
        {

            List<TariffRateModel> list = (from trc in PemsEntities.TariffRateConfigurations
                                                                        join trfc in PemsEntities.TariffRateForConfigurations on trc.TariffRateConfigurationId equals trfc.TariffRateConfigurationId
                                                                        join tr in PemsEntities.TariffRates on trfc.TariffRateId equals tr.TariffRateId
                                                                where trc.CustomerId == customerId
                                                                where trc.ConfigProfileId == configProfileId
                                                                where trc.TariffRateConfigurationId == tariffRateConfigurationId
                                                                        select new TariffRateModel()
                                                              {
                                                                  //ConfigProfileId = configProfileId,
                                                                  //TariffRateConfigurationId = tariffRateConfigurationId,
                                                                  TariffRateId = tr.TariffRateId,
                                                                  CustomerId = customerId,
                                                                  RateName = tr.RateName,
                                                                  RateDescription = tr.RateDesc,
                                                                  RateInCents = tr.RateInCents,
                                                                  PerTimeValue = tr.PerTimeValue,
                                                                  PerTimeUnitId = tr.PerTimeUnit,
                                                                  PerTimeUnitName = tr.PerTimeValue.HasValue ? PemsEntities.TimeUnits.FirstOrDefault(m => m.TimeUnitId == tr.PerTimeUnit).TimeUnitDesc : null,
                                                                  MaxTimeValue = tr.MaxTimeValue,
                                                                  MaxTimeUnitId = tr.MaxTimeUnit,
                                                                  MaxTimeUnitName = tr.MaxTimeUnit.HasValue ? PemsEntities.TimeUnits.FirstOrDefault(m => m.TimeUnitId == tr.MaxTimeUnit).TimeUnitDesc : null,
                                                                  GracePeriodMinute = tr.GracePeriodMinute,
                                                                  LinkedTariffRateId = tr.LinkedRate,
                                                                  LinkedTariffRateName = tr.LinkedRate.HasValue ? PemsEntities.TariffRates.FirstOrDefault(m => m.TariffRateId == tr.LinkedRate).RateName : null,
                                                                  LockMaxTime = tr.LockMaxTime.HasValue && tr.LockMaxTime.Value,
                                                                  CreatedOn = tr.CreatedOn
                                                              }).ToList();

            return list;

        }



        #endregion

        #region Configured Rate Schedules


        public List<RateScheduleModel> GetConfiguredRateSchedules(int customerId, Int64 configProfileId, Int64 rateScheduleConfigurationId)
        {

            List<RateScheduleModel> list = (from rsc in PemsEntities.RateScheduleConfigurations
                                            join rsfc in PemsEntities.RateScheduleForConfigurations on rsc.RateScheduleConfigurationId equals rsfc.RateScheduleConfigurationId
                                            join rs in PemsEntities.RateSchedules on rsfc.RateScheduleId equals rs.RateScheduleId
                                                                where rsc.CustomerId == customerId
                                                                where rsc.ConfigProfileId == configProfileId
                                                                where rsc.RateScheduleConfigurationId == rateScheduleConfigurationId
                                            select new RateScheduleModel()
                                                                {
//                                                                    ConfigProfileId = configProfileId,
                                                                    RateScheduleConfigurationId = rateScheduleConfigurationId,
                                                                    RateScheduleId = rs.RateScheduleId,
                                                                    CustomerId = customerId,
                                                                    DayOfWeek = rs.DayOfWeek,
                                                                    DayOfWeekName = PemsEntities.DayOfWeeks.FirstOrDefault(m => m.DayOfWeekId == rs.DayOfWeek).DayOfWeekDesc,

                                                                    StartTimeHour = rs.StartTimeHour,
                                                                    StartTimeMinute = rs.StartTimeMinute,
                                                                    OperationMode = rs.OperationMode,
                                                                    OperationModeName = PemsEntities.OperationModes.FirstOrDefault(m => m.OperationModeId == rs.OperationMode).OperationModeDesc,

                                                                    //MessageSequence = rs.MessageSequence ?? 0,

                                                                    //LockMaxTime = rs.LockMaxTime.HasValue && rs.LockMaxTime.Value,
                                                                    CreatedOn = rs.CreatedOn,
                                                                    CreatedBy = rs.CreatedBy,

                                                                    TariffRateId = rs.TariffRateId,
                                                                    TariffRateName = rs.TariffRateId.HasValue ? PemsEntities.TariffRates.FirstOrDefault(m => m.TariffRateId == rs.TariffRateId).RateName : string.Empty

                                                                }).ToList();

            return list;

        }



        #endregion

        #region Configured Holiday Rates

        public List<HolidayRateModel> GetConfiguredHolidayRates(int customerId, Int64 configProfileId, Int64 holidayRateConfigurationId)
        {

            List<HolidayRateModel> list = (from hrc in PemsEntities.HolidayRateConfigurations
                                           join hrfc in PemsEntities.HolidayRateForConfigurations on hrc.HolidayRateConfigurationId equals hrfc.HolidayRateConfigurationId
                                           join hr in PemsEntities.HolidayRates on hrfc.HolidayRateId equals hr.HolidayRateId 
                                           where hrc.CustomerId == customerId
                                           where hrc.ConfigProfileId == configProfileId
                                           where hrc.HolidayRateConfigurationId == holidayRateConfigurationId
                                           select new HolidayRateModel()
                                                                          {
//                                                                              ConfigProfileId = configProfileId,
                                                                              HolidayRateConfigurationId = holidayRateConfigurationId,
                                                                              HolidayRateId = hr.HolidayRateId,
                                                                              CustomerId = customerId,
                                                                              HolidayName = hr.HolidayName,
                                                                              HolidayDateTime = hr.HolidayDateTime,
                                                                              RateScheduleConfigurationId = hr.RateScheduleConfigurationId,
                                                                              DayOfWeek = hr.DayOfWeek,
                                                                              DayOfWeekName = PemsEntities.DayOfWeeks.FirstOrDefault(m => m.DayOfWeekId == hr.DayOfWeek).DayOfWeekDesc,
                                                                              CreatedOn = hrc.CreatedOn,
                                                                              CreatedBy = hrc.CreatedBy
                                                                          }).ToList();


            foreach (var holidayRateModel in list)
            {
                var rateSchedules = from rsfc in PemsEntities.RateScheduleForConfigurations
                                      join rs in PemsEntities.RateSchedules on rsfc.RateScheduleId equals rs.RateScheduleId
                                      where rsfc.RateScheduleConfigurationId == holidayRateModel.RateScheduleConfigurationId
                                      where rs.DayOfWeek == holidayRateModel.DayOfWeek
                                      select rs;
                holidayRateModel.RateScheduleCount = rateSchedules == null ? 0 : rateSchedules.Count();
            }

            return list;

        }

        #endregion

        #endregion


        #region Tariff Rate Methods

        #region Tariff Rate Methods

        public List<TariffRateModel> GetTariffRates(int customerId, Int64 tariffRateConfigurationId)
        {

            List<TariffRateModel> list = (from trfc in PemsEntities.TariffRateForConfigurations
                                              join tr in PemsEntities.TariffRates on trfc.TariffRateId equals tr.TariffRateId
                                              where trfc.TariffRateConfigurationId == tariffRateConfigurationId
                                          select new TariffRateModel()
                                                {
                                                    TariffRateId = tr.TariffRateId,
                                                    CustomerId = customerId,
                                                    RateName = tr.RateName,
                                                    RateDescription = tr.RateDesc,
                                                    RateInCents = tr.RateInCents,
                                                    PerTimeValue = tr.PerTimeValue,
                                                    PerTimeUnitId = tr.PerTimeUnit,
                                                    PerTimeUnitName = tr.PerTimeValue.HasValue ? PemsEntities.TimeUnits.FirstOrDefault(m => m.TimeUnitId == tr.PerTimeUnit).TimeUnitDesc : null,
                                                    MaxTimeValue = tr.MaxTimeValue,
                                                    MaxTimeUnitId = tr.MaxTimeUnit,
                                                    MaxTimeUnitName = tr.MaxTimeUnit.HasValue ? PemsEntities.TimeUnits.FirstOrDefault(m => m.TimeUnitId == tr.MaxTimeUnit).TimeUnitDesc : null,
                                                    GracePeriodMinute = tr.GracePeriodMinute,
                                                    LinkedTariffRateId = tr.LinkedRate,
                                                    LinkedTariffRateName = tr.LinkedRate.HasValue ? PemsEntities.TariffRates.FirstOrDefault(m => m.TariffRateId == tr.LinkedRate).RateName : null,
                                                    LockMaxTime = tr.LockMaxTime.HasValue && tr.LockMaxTime.Value,
                                                    CreatedOn = tr.CreatedOn
                                                }).ToList();

            return TariffRateModelUtility.Sort(list);
        }

        /// <summary>
        /// Create a tariff rate in table [TariffRate]
        /// </summary>
        /// <param name="model">A <see cref="TariffRateModel"/> instance with new tariff rate data.</param>
        public TariffRateModel CreateTariffRate(TariffRateModel model)
        {
            var tariffRate = new TariffRate()
            {
                CustomerId = model.CustomerId,
                RateName = model.RateName,
                RateDesc = model.RateDescription,
                RateInCents = model.RateInCents
            };

            // Add PerTime information
            if (model.PerTimeValue > 0)
            {
                tariffRate.PerTimeValue = model.PerTimeValue;
                tariffRate.PerTimeUnit = model.PerTimeUnitId;
            }

            // Add MaxTime information
            if (model.MaxTimeValue > 0)
            {
                tariffRate.MaxTimeValue = model.MaxTimeValue;
                tariffRate.MaxTimeUnit = model.MaxTimeUnitId;
            }

            // Add Grace Period
            tariffRate.GracePeriodMinute = model.GracePeriodMinute > 0
                                               ? model.GracePeriodMinute
                                               : (int?)null;

            // Add Linked Tariff Rate
            if ( model.LinkedTariffRateId.HasValue && model.LinkedTariffRateId > 0 )
            {
                tariffRate.LinkedRate = model.LinkedTariffRateId;
            }

            // Lock Max Time
            tariffRate.LockMaxTime = model.LockMaxTime;

            // Set UpdateDateTime
            tariffRate.CreatedOn = Now;
            model.CreatedOn = Now;

            // Set created by
            tariffRate.CreatedBy = WebSecurity.CurrentUserId;
            model.CreatedBy = tariffRate.CreatedBy;

            // Now save the new tariff rate so an id is generated.
            PemsEntities.TariffRates.Add(tariffRate);
            PemsEntities.SaveChanges();

            // Copy back id to model.
            model.TariffRateId = tariffRate.TariffRateId;

            // Note that the model is no longer dirty
            model.IsChanged = false;

            return model;
        }


        /// <summary>
        /// Delete a tariff rate in table [TariffRate]
        /// </summary>
        /// <param name="model">A <see cref="TariffRateModel"/> instance with tariff rate data.</param>
        public void DeleteTariffRate(TariffRateModel model)
        {
            var tariffRate = PemsEntities.TariffRates.FirstOrDefault(m => m.TariffRateId == model.TariffRateId);

            if ( tariffRate != null )
            {
                // Is this tariff rate linked to other tariff rates?
                foreach (var linkedTariffRate in tariffRate.TariffRate1)
                {
                    // Remove the link.
                    linkedTariffRate.LinkedRate = null;
                }

                // Is this tariff rate involved in any [TariffRateForConfiguration] rows?
                foreach (var trfc in tariffRate.TariffRateForConfigurations)
                {
                    // Remove the configuration row.
                    PemsEntities.TariffRateForConfigurations.Remove( trfc );
                }
                PemsEntities.SaveChanges();

                // Now delete the tariff rate from the [TariffRates] table.
                PemsEntities.TariffRates.Remove( tariffRate );
                PemsEntities.SaveChanges();
            }
        }



        #endregion

        #region Tariff Rate Configuration Methods


        public TariffRateConfigurationModel GetTariffRateConfiguration(int customerId, Int64 tariffRateConfigurationId)
        {
            TariffRateConfigurationModel model = null;
            var trc = PemsEntities.TariffRateConfigurations.FirstOrDefault( m => m.CustomerId == customerId && m.TariffRateConfigurationId == tariffRateConfigurationId );

            if ( trc != null  )
            {
                model = new TariffRateConfigurationModel()
                    {
                        CustomerId = customerId,
                        TariffRateConfigurationId = tariffRateConfigurationId,
                        CreatedOn = trc.CreatedOn,
                        CreatedBy = trc.CreatedBy,
                        Name = trc.Name,
                        Description = trc.Desc,
                        TariffRateCount = PemsEntities.TariffRateForConfigurations.Count(m => m.TariffRateConfigurationId == trc.TariffRateConfigurationId)
                    };
            }

            return model;
        }


        public List<TariffRateConfigurationModel> GetTariffRateConfigurationsForImport(int customerId)
        {
            List<TariffRateConfigurationModel> list = (
                from trc in PemsEntities.TariffRateConfigurations
                where trc.CustomerId == customerId
                where trc.State == (int)TariffStateType.Current
                      select new TariffRateConfigurationModel()
                          {
                              CustomerId = customerId,
                              TariffRateConfigurationId = trc.TariffRateConfigurationId,
                              Name = trc.Name,
                              Description = trc.Desc,
                              CreatedOn = trc.CreatedOn,
                              CreatedBy = trc.CreatedBy,
                              ConfiguredOn = trc.ConfiguredOn,
                              ConfiguredBy = trc.ConfiguredBy,
                              TariffRateCount = PemsEntities.TariffRateForConfigurations.Count( m => m.TariffRateConfigurationId == trc.TariffRateConfigurationId )
                          } ).ToList();

            return list;
        }



        public TariffRateConfigurationModel SaveTariffRateConfiguration(int customerId, TariffRateConfigurationModel model, List<TariffRateModel> list)
        {
            // Does the model have a TariffRateConfigurationId?  If not then assign one and create an entry in [TariffRateConfiguration] table.
            if ( model.TariffRateConfigurationId == 0 )
            {
                model.TariffRateConfigurationId = NextConfigId();
                PemsEntities.TariffRateConfigurations.Add(
                new TariffRateConfiguration()
                    {
                        TariffRateConfigurationId = model.TariffRateConfigurationId,
                        Name = model.Name,
                        Desc = model.Description,
                        CustomerId = model.CustomerId,
                        CreatedOn = Now,
                        CreatedBy = WebSecurity.CurrentUserId,
                        State = (int)TariffStateType.New
                    });
                PemsEntities.SaveChanges();

                model.CreatedOn = Now;
                model.CreatedBy = WebSecurity.CurrentUserId;
                model.State = TariffStateType.New;
            }
            else
            {
                // TariffRateConfiguration already exists.  Save Name and Description as they may have changed.
                // Get the tariff rate configuration element from [TariffRateConfiguration]
                TariffRateConfiguration trc = PemsEntities.TariffRateConfigurations.FirstOrDefault(m => m.TariffRateConfigurationId == model.TariffRateConfigurationId);
                // Resave name and description.  This will catch 
                trc.Name = model.Name;
                trc.Desc = model.Description;
                trc.State = (int)TariffStateType.Pending;
                model.State = TariffStateType.Pending;
            }


            // Delete any existing rows in [TariffRateForConfigurations] to unlink existing TariffRates from this configuration.  New links will be established.
            foreach (var tr in PemsEntities.TariffRateForConfigurations.Where(w => w.TariffRateConfigurationId == model.TariffRateConfigurationId))
            {
                PemsEntities.TariffRateForConfigurations.Remove(tr);
            }
            model.TariffRateCount = 0;


            // Walk the list of TariffRateModel and update associated [TariffRate] rows as needed.
            foreach (var tariffRateModel in list)
            {
                // Do I need to update the [TariffRate] entry for this one?
                if ( tariffRateModel.IsChanged )
                {
                    var tariffRate = PemsEntities.TariffRates.FirstOrDefault( m => m.TariffRateId == tariffRateModel.TariffRateId );

                    tariffRate.RateName = tariffRateModel.RateName;

                    tariffRate.RateDesc = tariffRateModel.RateDescription;
                    tariffRate.RateInCents = tariffRateModel.RateInCents;

                    // Add PerTime information
                    if (tariffRateModel.PerTimeValue > 0)
                    {
                        tariffRate.PerTimeValue = tariffRateModel.PerTimeValue;
                        tariffRate.PerTimeUnit = tariffRateModel.PerTimeUnitId;
                    }
                    else
                    {
                        tariffRate.PerTimeValue = null;
                        tariffRate.PerTimeUnit = null;
                    }

                    // Add MaxTime information
                    if (tariffRateModel.MaxTimeValue > 0)
                    {
                        tariffRate.MaxTimeValue = tariffRateModel.MaxTimeValue;
                        tariffRate.MaxTimeUnit = tariffRateModel.MaxTimeUnitId;
                    }
                    else
                    {
                        tariffRate.MaxTimeValue = null;
                        tariffRate.MaxTimeUnit = null;
                    }

                    // Add Grace Period
                    tariffRate.GracePeriodMinute = tariffRateModel.GracePeriodMinute > 0
                                                       ? tariffRateModel.GracePeriodMinute
                                                       : (int?)null;

                    // Lock Max Time
                    tariffRate.LockMaxTime = tariffRateModel.LockMaxTime;


                    // Linked Rate
                    tariffRate.LinkedRate = tariffRateModel.LinkedTariffRateId > 0
                                                ? tariffRateModel.LinkedTariffRateId
                                                : (long?)null;

                    // Update DateTime
                    tariffRate.UpdatedOn = Now;
                    tariffRate.UpdatedBy = WebSecurity.CurrentUserId;

                    // Note that the model is no longer dirty
                    tariffRateModel.IsChanged = false;
                }

                // Create entry in [TariffRateForConfiguration] table.
                PemsEntities.TariffRateForConfigurations.Add(
                    new TariffRateForConfiguration()
                        {
                            TariffRateConfigurationId = model.TariffRateConfigurationId,
                            TariffRateId = tariffRateModel.TariffRateId
                        } );

                model.TariffRateCount++;

                // Mark this rate as saved
                tariffRateModel.IsSaved = true;
            }

            PemsEntities.SaveChanges();

            return model;
        }

        public TariffRateConfigurationModel ConfigureTariffRateConfiguration(int customerId, TariffRateConfigurationModel model)
        {
            // Set the state of the TariffRateConfiguration to TariffStateType.Current

            // TariffRateConfiguration already exists.
            // Get the tariff rate configuration element from [TariffRateConfiguration]
            TariffRateConfiguration trc = PemsEntities.TariffRateConfigurations.FirstOrDefault(m => m.TariffRateConfigurationId == model.TariffRateConfigurationId);
            
            trc.State = (int) TariffStateType.Current;
            trc.ConfiguredOn = Now;
            trc.ConfiguredBy = WebSecurity.CurrentUserId;

            PemsEntities.SaveChanges();

            model.State = TariffStateType.Current;
            model.ConfiguredOn = Now;
            model.ConfiguredBy = WebSecurity.CurrentUserId;

            return model;
        }


        public TariffRateConfigurationModel CloneTariffRateConfigurationList(int customerId, 
            Int64 tariffRateConfigurationId, List<TariffRateModel> list)
        {
            // Get a list of the existing tariff rates for this tariffRateConfigurationId
            List<TariffRateModel> sourceList = GetTariffRates( customerId, tariffRateConfigurationId );

            // Create a cross-reference list of original TariffRateIds and the new TariffRateIds as each tariff rate is
            // cloned in table [TariffRate]
            Dictionary<Int64, Int64> linkedRateDictionary = new Dictionary<long, long>();

            // Walk the list of tariff rates to clone and clone each one noting the new tariff rate id created.
            foreach (var tariffRate in sourceList)
            {
                // Save original tariff rate id.
                Int64 originalTariffRateId = tariffRate.TariffRateId;

                // Create clone.
                TariffRateModel newModel = CreateTariffRate( tariffRate );

                // Add dictionary entry to map original id to new id.
                linkedRateDictionary.Add(originalTariffRateId, newModel.TariffRateId);

                // Add new model to clone list.
                list.Add(newModel);
            }

            // Now have a list of tariff rate models that match source list.  Need to fix up any
            // LinkedRateIds that are still pointed to the old tariff rate ids.
            foreach (var tariffRate in list)
            {
                // Does this tariff rate have a linked rate?
                if ( tariffRate.LinkedTariffRateId.HasValue && tariffRate.LinkedTariffRateId.Value > 0 )
                {
                    // Update the LinkedTariffRateId in both the model and the associated row in [TariffRate] table.
                    tariffRate.LinkedTariffRateId = linkedRateDictionary[tariffRate.LinkedTariffRateId.Value];
                    var tr = PemsEntities.TariffRates.FirstOrDefault( m => m.TariffRateId == tariffRate.TariffRateId );
                    tr.LinkedRate = tariffRate.LinkedTariffRateId;
                    PemsEntities.SaveChanges();
                }
            }

            // Return an instance of the TariffRateConfiguration pointed to by tariffRateConfigurationId
            return GetTariffRateConfiguration(customerId, tariffRateConfigurationId);
        }

        public void DeleteTariffRateConfiguration(int customerId, Int64 tariffRateConfigurationId)
        {
            // The TariffRateConfiguration must be in a state of Pending or New
            var trc = PemsEntities.TariffRateConfigurations.FirstOrDefault( m => m.CustomerId == customerId && m.TariffRateConfigurationId == tariffRateConfigurationId );
            if ( trc == null || trc.State == (int) TariffStateType.Current || trc.State == (int) TariffStateType.Historic )
            {
                // Cannot delete TariffRateConfiguration.
                return;
            }

            // Clean up this TariffRateConfiguration and all of its associated tariffs.

            // Remove references in [TariffRateForConfiguration] table.  Save tariffRateIds while removing rows.
            List<Int64> tariffRateIds = new List<long>();

            foreach (var trfc in PemsEntities.TariffRateForConfigurations.Where(m => m.TariffRateConfigurationId == tariffRateConfigurationId))
            {
                tariffRateIds.Add(trfc.TariffRateId);
                PemsEntities.TariffRateForConfigurations.Remove( trfc );
            }

            // Now break any linked rates in the [TariffRate] rows associated with this TariffRateConfiguration
            // This is to ensure that no constraints will be violated when deleting the rates.  Simpler than determining
            // the order in which to delete the TariffRates. :)
            foreach (var tariffRateId in tariffRateIds)
            {
                var tr = PemsEntities.TariffRates.FirstOrDefault(m => m.TariffRateId == tariffRateId);
                tr.LinkedRate = null;
            }
            // Save changes.
            PemsEntities.SaveChanges();

            // Now remove the [TariffRate] rows associated with this TariffRateConfiguration
            foreach (var tariffRateId in tariffRateIds)
            {
                var tr = PemsEntities.TariffRates.FirstOrDefault( m => m.TariffRateId == tariffRateId );
                PemsEntities.TariffRates.Remove( tr );
            }

            // Now remove the [TariffRateConfiguration] row.
            PemsEntities.TariffRateConfigurations.Remove( trc );

            // Save changes.
            PemsEntities.SaveChanges();
        }



        #endregion

        #endregion


        #region Rate Schedules


        public List<RateScheduleModel> GetRateSchedules(int customerId, Int64 rateScheduleConfigurationId)
        {
            List<RateScheduleModel> list = (from rsc in PemsEntities.RateScheduleConfigurations
                                            join rsfc in PemsEntities.RateScheduleForConfigurations on rsc.RateScheduleConfigurationId equals rsfc.RateScheduleConfigurationId
                                            join rs in PemsEntities.RateSchedules on rsfc.RateScheduleId equals rs.RateScheduleId
                                            where rsc.RateScheduleConfigurationId == rateScheduleConfigurationId
                                            select new RateScheduleModel()
                                                                    {
                                                                        RateScheduleId = rs.RateScheduleId,
                                                                        CustomerId = customerId,
                                                                        DayOfWeek = rs.DayOfWeek,
                                                                        DayOfWeekName = PemsEntities.DayOfWeeks.FirstOrDefault(m => m.DayOfWeekId == rs.DayOfWeek).DayOfWeekDesc,

                                                                        StartTimeHour = rs.StartTimeHour,
                                                                        StartTimeMinute = rs.StartTimeMinute,
                                                                        OperationMode = rs.OperationMode,
                                                                        OperationModeName = PemsEntities.OperationModes.FirstOrDefault(m => m.OperationModeId == rs.OperationMode).OperationModeDesc,

                                                                        //MessageSequence = rs.MessageSequence,

                                                                        //LockMaxTime = rs.LockMaxTime.HasValue && rs.LockMaxTime.Value,
                                                                        CreatedOn = rs.CreatedOn,
                                                                        CreatedBy = rs.CreatedBy,

                                                                        TariffRateId = rs.TariffRateId

                                                                    }).ToList();

            return list;

        }



        /// <summary>
        /// Create a rate schedule in table [RateSchedule]
        /// </summary>
        /// <param name="model">A <see cref="RateScheduleModel"/> instance with new rate schedule data.</param>
        public RateScheduleModel CreateRateSchedule(RateScheduleModel model)
        {
            var rateSchedule = new RateSchedule()
            {
                CustomerId = model.CustomerId,
                ScheduleNumber = model.ScheduleNumber,
                DayOfWeek = model.DayOfWeek,
                StartTimeHour = model.StartTimeHour,
                StartTimeMinute = model.StartTimeMinute,
                OperationMode = model.OperationMode
            };

            // Add Message Sequence
            //rateSchedule.MessageSequence = model.MessageSequence > 0 ? model.MessageSequence : (int?)null;

            // Lock Max Time
            //rateSchedule.LockMaxTime = model.LockMaxTime;

            // Update DateTime
            rateSchedule.CreatedOn = Now;
            rateSchedule.CreatedBy = WebSecurity.CurrentUserId;

            // TariffRate
            rateSchedule.TariffRateId = model.TariffRateId;

            // Now save the new rate schedule so an id is generated.
            PemsEntities.RateSchedules.Add(rateSchedule);
            PemsEntities.SaveChanges();

            // Copy back id to model.
            model.RateScheduleId = rateSchedule.RateScheduleId;

            // Note that the model is no longer dirty
            model.IsChanged = false;

            // Get day-of-week name for model.
            model.DayOfWeekName = PemsEntities.DayOfWeeks.FirstOrDefault( m => m.DayOfWeekId == model.DayOfWeek ).DayOfWeekDesc;

            return model;
        }


        /// <summary>
        /// Delete a rate schedule in table [RateSchedule]
        /// </summary>
        /// <param name="model">A <see cref="RateScheduleModel"/> instance with rate schedule data.</param>
        public void DeleteRateSchedule(RateScheduleModel model)
        {
            var rateSchedule = PemsEntities.RateSchedules.FirstOrDefault( m => m.RateScheduleId == model.RateScheduleId );

            if ( rateSchedule != null )
            {
                // Remove any association with [RateScheduleForConfiguration]
                foreach (var rsfc in rateSchedule.RateScheduleForConfigurations)
                {
                    PemsEntities.RateScheduleForConfigurations.Remove( rsfc );
                }
                PemsEntities.SaveChanges();

                // Now remove this rate schedule from [RateSchedules]
                PemsEntities.RateSchedules.Remove( rateSchedule );
                PemsEntities.SaveChanges();
            }
        }

        public RateScheduleConfigurationModel GetRateScheduleConfiguration(int customerId, Int64 rateScheduleConfigurationId)
        {
            RateScheduleConfigurationModel model = null;

            var rateScheduleConfiguration =
                PemsEntities.RateScheduleConfigurations.FirstOrDefault( m => m.CustomerId == customerId && m.RateScheduleConfigurationId == rateScheduleConfigurationId );

            if (rateScheduleConfiguration != null)
            {
                model = new RateScheduleConfigurationModel()
                    {
                        CustomerId = customerId,
                        RateScheduleConfigurationId = rateScheduleConfiguration.RateScheduleConfigurationId,
                        Name = rateScheduleConfiguration.Name,
                        Description = rateScheduleConfiguration.Desc,
                        CreatedOn = rateScheduleConfiguration.CreatedOn,
                        CreatedBy = rateScheduleConfiguration.CreatedBy,
                        RateScheduleCount = PemsEntities.RateScheduleForConfigurations.Count(m => m.RateScheduleConfigurationId == rateScheduleConfigurationId)
                    };

                // Set DayOfWeek flags
                var rateSchedules = from rsfc in PemsEntities.RateScheduleForConfigurations
                                    join rs in PemsEntities.RateSchedules on rsfc.RateScheduleId equals rs.RateScheduleId
                                    where rsfc.RateScheduleConfigurationId == rateScheduleConfigurationId
                                    select rs;
                foreach (var rs in rateSchedules)
                {
                    model.DayOfWeek[rs.DayOfWeek] = 1;
                }

            }
            return model;
        }


        public List<RateScheduleConfigurationModel> GetRateScheduleConfigurationsForImport(int customerId)
        {
            List<RateScheduleConfigurationModel> list = (
                from rsc in PemsEntities.RateScheduleConfigurations
                where rsc.CustomerId == customerId
                where rsc.State == (int)TariffStateType.Current
                select new RateScheduleConfigurationModel()
                          {
                              CustomerId = customerId,
                              RateScheduleConfigurationId = rsc.RateScheduleConfigurationId,
                              Name = rsc.Name,
                              Description = rsc.Desc,
                              CreatedOn = rsc.CreatedOn,
                              CreatedBy = rsc.CreatedBy,
                              ConfiguredOn = rsc.ConfiguredOn,
                              ConfiguredBy = rsc.ConfiguredBy,
                              RateScheduleCount = PemsEntities.RateScheduleForConfigurations.Count(m => m.RateScheduleConfigurationId == rsc.RateScheduleConfigurationId)
                          }).ToList();

            foreach (var rateScheduleConfigurationModel in list)
            {
                // Set DayOfWeek flags
                var rateSchedules = from rsfc in PemsEntities.RateScheduleForConfigurations
                                    join rs in PemsEntities.RateSchedules on rsfc.RateScheduleId equals rs.RateScheduleId
                                    where rsfc.RateScheduleConfigurationId == rateScheduleConfigurationModel.RateScheduleConfigurationId
                                    select rs;
                foreach (var rs in rateSchedules)
                {
                    rateScheduleConfigurationModel.DayOfWeek[rs.DayOfWeek] = 1;
                }
            }

            return list;
        }


        public RateScheduleConfigurationModel SaveRateScheduleConfiguration(int customerId, RateScheduleConfigurationModel model, List<RateScheduleModel> list)
        {
            // Does the model have a RateScheduleConfigurationId?  If not then assign one and create an entry in [RateScheduleConfiguration] table.
            if (model.RateScheduleConfigurationId == 0)
            {
                model.RateScheduleConfigurationId = NextConfigId();
                PemsEntities.RateScheduleConfigurations.Add(
                new RateScheduleConfiguration()
                {
                    RateScheduleConfigurationId = model.RateScheduleConfigurationId,
                    Name = model.Name,
                    Desc = model.Description,
                    CustomerId = model.CustomerId,
                    CreatedOn = Now,
                    CreatedBy = WebSecurity.CurrentUserId,
                    State = (int)TariffStateType.New
                });
                PemsEntities.SaveChanges();

                model.CreatedOn = Now;
                model.CreatedBy = WebSecurity.CurrentUserId;
                model.State = TariffStateType.New;
            }
            else
            {
                // RateScheduleConfiguration already exists.  Save Name and Description as they may have changed.
                // Get the rate schedule configuration element from [RateScheduleConfiguration]
                RateScheduleConfiguration rsc = PemsEntities.RateScheduleConfigurations.FirstOrDefault(m => m.RateScheduleConfigurationId == model.RateScheduleConfigurationId);
                // Resave name and description.  This will catch 
                rsc.Name = model.Name;
                rsc.Desc = model.Description;
                rsc.State = (int) TariffStateType.Pending;
                model.State = TariffStateType.Pending;
            }


            // Remove existing rows from [RateScheduleConfiguration] table
            foreach (var rs in PemsEntities.RateScheduleForConfigurations.Where(w => w.RateScheduleConfigurationId == model.RateScheduleConfigurationId))
            {
                PemsEntities.RateScheduleForConfigurations.Remove(rs);
            }
            model.RateScheduleCount = 0;

            // Walk the list of RateScheduleEditModel and update associated [RateSchedule] rows as needed.
            foreach (var rateScheduleEditModel in list)
            {
                // Do I need to update the [TariffRate] entry for this one?
                if (rateScheduleEditModel.IsChanged)
                {
                    var rateSchedule = PemsEntities.RateSchedules.FirstOrDefault(m => m.RateScheduleId == rateScheduleEditModel.RateScheduleId);

                    rateSchedule.ScheduleNumber = rateScheduleEditModel.ScheduleNumber;
                    rateSchedule.DayOfWeek = rateScheduleEditModel.DayOfWeek;
                    rateSchedule.StartTimeHour = rateScheduleEditModel.StartTimeHour;
                    rateSchedule.StartTimeMinute = rateScheduleEditModel.StartTimeMinute;
                    rateSchedule.OperationMode = rateScheduleEditModel.OperationMode;

                    // Add Message Sequence
                    //rateSchedule.MessageSequence = rateScheduleEditModel.MessageSequence > 0 ? rateScheduleEditModel.MessageSequence : (int?)null;

                    // Lock Max Time
                    //rateSchedule.LockMaxTime = rateScheduleEditModel.LockMaxTime;

                    // TariffRate
                    rateSchedule.TariffRateId
                        = rateScheduleEditModel.TariffRateId > 0 ? rateScheduleEditModel.TariffRateId : (long?)null;

                    rateSchedule.UpdatedOn = Now;
                    rateSchedule.UpdatedBy = WebSecurity.CurrentUserId;

                    // Now save the updated rate schedule.
                    PemsEntities.SaveChanges();

                    // Note that the model is no longer dirty
                    rateScheduleEditModel.IsChanged = false;
                }

                // Create entry in [RateScheduleForConfiguration] table.
                PemsEntities.RateScheduleForConfigurations.Add(
                    new RateScheduleForConfiguration()
                    {
                        RateScheduleConfigurationId = model.RateScheduleConfigurationId,
                        RateScheduleId = rateScheduleEditModel.RateScheduleId
                    });

                model.DayOfWeek[rateScheduleEditModel.DayOfWeek] = 1;

                model.RateScheduleCount++;

                // Mark this rate schedule as saved
                rateScheduleEditModel.IsSaved = true;
            }

            PemsEntities.SaveChanges();

            model.CreatedOn = Now;
            model.CreatedBy = WebSecurity.CurrentUserId;

            return model;
        }


        public RateScheduleConfigurationModel ConfigureRateScheduleConfiguration(int customerId, RateScheduleConfigurationModel model)
        {
            // Set the state of the RateScheduleConfiguration to TariffStateType.Current

            // RateScheduleConfiguration already exists.
            // Get the rate schedule configuration element from [RateScheduleConfiguration]
            RateScheduleConfiguration rsc = PemsEntities.RateScheduleConfigurations.FirstOrDefault(m => m.RateScheduleConfigurationId == model.RateScheduleConfigurationId);

            rsc.State = (int)TariffStateType.Current;
            rsc.ConfiguredOn = Now;
            rsc.ConfiguredBy = WebSecurity.CurrentUserId;

            PemsEntities.SaveChanges();

            model.State = TariffStateType.Current;
            model.ConfiguredOn = Now;
            model.ConfiguredBy = WebSecurity.CurrentUserId;

            return model;
        }

        public RateScheduleConfigurationModel CloneRateScheduleConfigurationList(int customerId,
                Int64 rateScheduleConfigurationId, List<TariffRateModel> tariffRateList,
                List<RateScheduleModel> rateScheduleList)
        {
            // Find the tariff rate configuration id that is associated with one of the 
            // tariff rates used by one of the schedules.
            List<RateScheduleModel> sourceRateScheduleList = GetRateSchedules( customerId, rateScheduleConfigurationId );
            Int64 tariffRateId = (Int64)sourceRateScheduleList[0].TariffRateId;
            var tariffRateForConfiguration = PemsEntities.TariffRateForConfigurations.FirstOrDefault(m => m.TariffRateId == tariffRateId);
            Int64 tariffRateConfigurationId = tariffRateForConfiguration.TariffRateConfigurationId;

            // Get a list of the existing tariff rates for this tariffRateConfigurationId
            List<TariffRateModel> sourceTariffRateList = GetTariffRates(customerId, tariffRateConfigurationId);

            // Create a cross-reference list of original TariffRateIds and the new TariffRateIds as each tariff rate is
            // cloned in table [TariffRate]
            Dictionary<Int64, Int64> linkedRateDictionary = new Dictionary<long, long>();

            // Walk the list of tariff rates to clone and clone each one noting the new tariff rate id created.
            foreach (var tariffRate in sourceTariffRateList)
            {
                // Save original tariff rate id.
                Int64 originalTariffRateId = tariffRate.TariffRateId;

                // Create clone.
                TariffRateModel newTariffRateModel = CreateTariffRate(tariffRate);

                // Add dictionary entry to map original id to new id.
                linkedRateDictionary.Add(originalTariffRateId, newTariffRateModel.TariffRateId);

                // Add new model to clone list.
                tariffRateList.Add(newTariffRateModel);
            }

            // Now have a list of tariff rate models that match source list.  Need to fix up any
            // LinkedRateIds that are still pointed to the old tariff rate ids.
            foreach (var tariffRate in tariffRateList)
            {
                // Does this tariff rate have a linked rate?
                if (tariffRate.LinkedTariffRateId.HasValue && tariffRate.LinkedTariffRateId.Value > 0)
                {
                    // Update the LinkedTariffRateId in both the model and the associated row in [TariffRate] table.
                    tariffRate.LinkedTariffRateId = linkedRateDictionary[tariffRate.LinkedTariffRateId.Value];
                    var tr = PemsEntities.TariffRates.FirstOrDefault(m => m.TariffRateId == tariffRate.TariffRateId);
                    tr.LinkedRate = tariffRate.LinkedTariffRateId;
                    PemsEntities.SaveChanges();
                }
            }

            // Now clone the RateSchedules
            foreach (var rateSchedule in sourceRateScheduleList)
            {
                // Create clone.
                RateScheduleModel newRateScheduleModel = CreateRateSchedule(rateSchedule);

                // Update rateSchedule.TariffRateId
                rateSchedule.TariffRateId = linkedRateDictionary[(Int64)rateSchedule.TariffRateId];
                var rs = PemsEntities.RateSchedules.FirstOrDefault(m => m.RateScheduleId == rateSchedule.RateScheduleId);
                rs.TariffRateId = rateSchedule.TariffRateId;
                PemsEntities.SaveChanges();

                // Add new model to clone list.
                rateScheduleList.Add(newRateScheduleModel);
            }

            // Return an instance of the RateScheduleConfiguration
            return GetRateScheduleConfiguration( customerId, rateScheduleConfigurationId );
        }

        public void DeleteRateScheduleConfiguration(int customerId, Int64 rateScheduleConfigurationId)
        {
            // The RateScheduleConfiguration must be in a state of Pending or New
            var rsc = PemsEntities.RateScheduleConfigurations.FirstOrDefault(m => m.CustomerId == customerId && m.RateScheduleConfigurationId == rateScheduleConfigurationId);
            if (rsc == null || rsc.State == (int)TariffStateType.Current || rsc.State == (int)TariffStateType.Historic)
            {
                // Cannot delete RateScheduleConfiguration.
                return;
            }

            // Clean up this RateScheduleConfiguration and all of its associated rate schedules.

            // Remove references in [RateScheduleForConfiguration] table.  Save rateScheduleIds while removing rows.
            List<Int64> rateScheduleIds = new List<long>();

            foreach (var rsfc in PemsEntities.RateScheduleForConfigurations.Where(m => m.RateScheduleConfigurationId == rateScheduleConfigurationId))
            {
                rateScheduleIds.Add(rsfc.RateScheduleId);
                PemsEntities.RateScheduleForConfigurations.Remove(rsfc);
            }

            // Now remove the [RateSchedule] rows associated with this RateScheduleConfiguration
            foreach (var rateScheduleId in rateScheduleIds)
            {
                var rs = PemsEntities.RateSchedules.FirstOrDefault(m => m.RateScheduleId == rateScheduleId);
                PemsEntities.RateSchedules.Remove(rs);
            }

            // Now remove the [RateScheduleConfiguration] row.
            PemsEntities.RateScheduleConfigurations.Remove(rsc);

            // Save changes.
            PemsEntities.SaveChanges();

        }


        #endregion


        #region Holiday Rates

        public HolidayRateConfigurationModel GetHolidayRateConfiguration(int customerId, Int64 holidayRateConfigurationId)
        {
            HolidayRateConfigurationModel model = null;

            var hrc = PemsEntities.HolidayRateConfigurations.FirstOrDefault( m => m.CustomerId == customerId && m.HolidayRateConfigurationId == holidayRateConfigurationId );

            if ( hrc != null )
            {
                model = new HolidayRateConfigurationModel()
                {
                    CustomerId = customerId,
                    HolidayRateConfigurationId = hrc.HolidayRateConfigurationId,
                    Name = hrc.Name,
                    Description = hrc.Desc,
                    CreatedOn = hrc.CreatedOn,
                    CreatedBy = hrc.CreatedBy,
                    ConfiguredOn = hrc.ConfiguredOn,
                    ConfiguredBy = hrc.ConfiguredBy,
                    HolidayRateCount = PemsEntities.HolidayRateForConfigurations.Count(m => m.HolidayRateConfigurationId == hrc.HolidayRateConfigurationId)
                };
            }

            return model;
        }

        public List<HolidayRateModel> GetHolidayRates(int customerId, Int64 holidayRateConfigurationId)
        {
            List<HolidayRateModel> list = (from hrc in PemsEntities.HolidayRateConfigurations
                                           join hrfc in PemsEntities.HolidayRateForConfigurations on hrc.HolidayRateConfigurationId equals hrfc.HolidayRateConfigurationId
                                               join hr in PemsEntities.HolidayRates on hrfc.HolidayRateId equals hr.HolidayRateId
                                               where hrc.HolidayRateConfigurationId == holidayRateConfigurationId
                                                                 select new HolidayRateModel()
                                                                   {
                                                                       HolidayRateConfigurationId = holidayRateConfigurationId,
                                                                       HolidayRateId = hr.HolidayRateId,
                                                                       CustomerId = customerId,
                                                                       HolidayName = hr.HolidayName,
                                                                       HolidayDateTime = hr.HolidayDateTime,
                                                                       RateScheduleConfigurationId = hr.RateScheduleConfigurationId,
                                                                       RateScheduleCount =
                                                                           PemsEntities.RateScheduleConfigurations.Count(
                                                                               m => m.RateScheduleConfigurationId == hr.RateScheduleConfigurationId)
                                                                   }).ToList();
            return list;
        }


        /// <summary>
        /// Create a holiday rate in table [HolidayRate]
        /// </summary>
        /// <param name="model">A <see cref="HolidayRateModel"/> instance with new holiday rate data.</param>
        public HolidayRateModel CreateHolidayRate(HolidayRateModel model)
        {
            var holidayRate = new HolidayRate()
            {
                CustomerId = model.CustomerId,
                HolidayName = model.HolidayName,
                RateScheduleConfigurationId = model.RateScheduleConfigurationId,
                DayOfWeek = model.DayOfWeek,
                HolidayDateTime = model.HolidayDateTime,
                CreatedOn = Now,
                CreatedBy = WebSecurity.CurrentUserId,
            };

            // Now save the new holiday rate so an id is generated.
            PemsEntities.HolidayRates.Add(holidayRate);
            PemsEntities.SaveChanges();

            // Copy back id to model.
            model.HolidayRateId = holidayRate.HolidayRateId;

            // Note that the model is no longer dirty
            model.IsChanged = false;

            return model;
        }


        /// <summary>
        /// Delete a holiday rate in table [HolidayRate]
        /// </summary>
        /// <param name="model">A <see cref="HolidayRateModel"/> instance with holiday rate data.</param>
        public void DeleteHolidayRate(HolidayRateModel model)
        {
            var holidayRate = PemsEntities.HolidayRates.FirstOrDefault(m => m.HolidayRateId == model.HolidayRateId);

            if (holidayRate != null)
            {
                // Remove any association with [HolidayRateForConfiguration]
                foreach (var hrfc in holidayRate.HolidayRateForConfigurations)
                {
                    PemsEntities.HolidayRateForConfigurations.Remove(hrfc);
                }
                PemsEntities.SaveChanges();

                // Now remove this holiday rate from [HolidayRates]
                PemsEntities.HolidayRates.Remove(holidayRate);
                PemsEntities.SaveChanges();
            }
        }


        public HolidayRateConfigurationModel SaveHolidayRateConfiguration(int customerId, HolidayRateConfigurationModel model, List<HolidayRateModel> list)
        {
            // Does the model have a HolidayRateConfigurationId?  If not then assign one and create an entry in [HolidayRateConfiguration] table.
            if (model.HolidayRateConfigurationId == 0)
            {
                model.HolidayRateConfigurationId = NextConfigId();
                PemsEntities.HolidayRateConfigurations.Add(
                new HolidayRateConfiguration()
                {
                    HolidayRateConfigurationId = model.HolidayRateConfigurationId,
                    Name = model.Name,
                    Desc = model.Description,
                    CustomerId = model.CustomerId,
                    CreatedOn = Now,
                    CreatedBy = WebSecurity.CurrentUserId,
                    State = (int)TariffStateType.New
                });
                PemsEntities.SaveChanges();

                model.CreatedOn = Now;
                model.CreatedBy = WebSecurity.CurrentUserId;
                model.State = TariffStateType.New;
            }
            else
            {
                // HolidayRateConfiguration already exists.  Save Name and Description as they may have changed.
                // Get the holiday rate configuration element from [HolidayRateConfiguration]
                HolidayRateConfiguration hrc = PemsEntities.HolidayRateConfigurations.FirstOrDefault(m => m.HolidayRateConfigurationId == model.HolidayRateConfigurationId);
                // Resave name and description.  This will catch 
                hrc.Name = model.Name;
                hrc.Desc = model.Description;
                hrc.State = (int)TariffStateType.Pending;
                model.State = TariffStateType.Pending;
            }


            // Remove existing rows from [HolidayRateForConfiguration] table
            foreach (var rs in PemsEntities.HolidayRateForConfigurations.Where(w => w.HolidayRateConfigurationId == model.HolidayRateConfigurationId))
            {
                PemsEntities.HolidayRateForConfigurations.Remove(rs);
            }
            model.HolidayRateCount = 0;


            // Walk the list of HolidayRateEditModel and update associated [HolidayRate] rows as needed.
            foreach (var holidayRateEditModel in list)
            {
                // Do I need to update the [HolidayRate] entry for this one?
                if (holidayRateEditModel.IsChanged)
                {
                    var holidayRate = PemsEntities.HolidayRates.FirstOrDefault(m => m.HolidayRateId == holidayRateEditModel.HolidayRateId);

                    holidayRate.RateScheduleConfigurationId = holidayRateEditModel.RateScheduleConfigurationId;
                    holidayRate.HolidayDateTime = holidayRateEditModel.HolidayDateTime;

                    // Add Holiday Name
                    holidayRate.HolidayName = holidayRateEditModel.HolidayName;

                    holidayRate.RateScheduleConfigurationId = holidayRateEditModel.RateScheduleConfigurationId;
                    holidayRate.DayOfWeek = holidayRateEditModel.DayOfWeek;

                    holidayRate.UpdatedOn = Now;
                    holidayRate.UpdatedBy = WebSecurity.CurrentUserId;

                    // Now save the updated holiday rate.
                    PemsEntities.SaveChanges();

                    // Note that the model is no longer dirty
                    holidayRateEditModel.IsChanged = false;
                }

                // Create entry in [HolidayRateConfiguration] table.
                PemsEntities.HolidayRateForConfigurations.Add(
                    new HolidayRateForConfiguration()
                    {
                        HolidayRateConfigurationId = model.HolidayRateConfigurationId,
                        HolidayRateId = holidayRateEditModel.HolidayRateId
                    });

                model.HolidayRateCount++;

                // Mark this holiday rate as saved
                holidayRateEditModel.IsSaved = true;
            }

            PemsEntities.SaveChanges();

            model.CreatedOn = Now;
            model.CreatedBy = WebSecurity.CurrentUserId;

            return model;
        }


        public List<HolidayRateConfigurationModel> GetHolidayRateConfigurationsForImport(int customerId)
        {
            List<HolidayRateConfigurationModel> list = (
                from hrc in PemsEntities.HolidayRateConfigurations
                where hrc.CustomerId == customerId
                where hrc.State == (int)TariffStateType.Current
                select new HolidayRateConfigurationModel()
                {
                    CustomerId = customerId,
                    HolidayRateConfigurationId = hrc.HolidayRateConfigurationId,
                    Name = hrc.Name,
                    Description = hrc.Desc,
                    CreatedOn = hrc.CreatedOn,
                    CreatedBy = hrc.CreatedBy,
                    ConfiguredOn = hrc.ConfiguredOn,
                    ConfiguredBy = hrc.ConfiguredBy,
                    HolidayRateCount = PemsEntities.HolidayRateForConfigurations.Count(m => m.HolidayRateConfigurationId == hrc.HolidayRateConfigurationId)
                }).ToList();

            return list;
        }


        public HolidayRateConfigurationModel ConfigureHolidayRateConfiguration(int customerId, HolidayRateConfigurationModel model)
        {
            // Set the state of the HolidayRateConfigurationModel to TariffStateType.Current

            // HolidayRateConfigurationModel already exists.
            // Get the holiday rate configuration element from [HolidayRateConfiguration]
            HolidayRateConfiguration hrc = PemsEntities.HolidayRateConfigurations.FirstOrDefault(m => m.HolidayRateConfigurationId == model.HolidayRateConfigurationId);

            hrc.State = (int)TariffStateType.Current;
            hrc.ConfiguredOn = Now;
            hrc.ConfiguredBy = WebSecurity.CurrentUserId;

            PemsEntities.SaveChanges();

            model.State = TariffStateType.Current;
            model.ConfiguredOn = Now;
            model.ConfiguredBy = WebSecurity.CurrentUserId;

            return model;
        }

        public void DeleteHolidayRateConfiguration(int customerId, Int64 holidayRateConfigurationId)
        {
            // The HolidayRateConfiguration must be in a state of Pending or New
            var hrc = PemsEntities.HolidayRateConfigurations.FirstOrDefault(m => m.CustomerId == customerId && m.HolidayRateConfigurationId == holidayRateConfigurationId);
            if (hrc == null || hrc.State == (int)TariffStateType.Current || hrc.State == (int)TariffStateType.Historic)
            {
                // Cannot delete HolidayRateConfiguration.
                return;
            }

            // Clean up this HolidayRateConfiguration and all of its associated holiday rates.

            // Remove references in [HolidayRateConfiguration] table.  Save holidayRateIds while removing rows.
            List<Int64> holidayRateIds = new List<long>();

            foreach (var hrfc in PemsEntities.HolidayRateForConfigurations.Where(m => m.HolidayRateConfigurationId == holidayRateConfigurationId))
            {
                holidayRateIds.Add(hrfc.HolidayRateId);
                PemsEntities.HolidayRateForConfigurations.Remove(hrfc);
            }

            // Now remove the [HolidayRate] rows associated with this HolidayRateConfiguration
            foreach (var holidayRateId in holidayRateIds)
            {
                var hr = PemsEntities.HolidayRates.FirstOrDefault(m => m.HolidayRateId == holidayRateId);
                PemsEntities.HolidayRates.Remove(hr);
            }

            // Now remove the [HolidayRateConfiguration] row.
            PemsEntities.HolidayRateConfigurations.Remove(hrc);

            // Save changes.
            PemsEntities.SaveChanges();
        }



        #endregion


        #region Utilities

        public List<SelectListItemWrapper> TimeUnitsList(int customerId)
        {
            List<SelectListItemWrapper> list = (from tu in PemsEntities.TimeUnits
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = false,
                                                    Text = tu.TimeUnitDesc,
                                                    ValueInt = tu.TimeUnitId
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = true,
                Text = "",
                Value = "-1"
            });

            return list;
        }


        public List<SelectListItemWrapper> DaysOfWeekList(int customerId)
        {
            List<SelectListItemWrapper> list = (from dow in PemsEntities.DayOfWeeks
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = false,
                                                    Text = dow.DayOfWeekDesc,
                                                    ValueInt = dow.DayOfWeekId
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = true,
                Text = "",
                Value = "-1"
            });

            return list;
        }


        public int DayOfWeekId(string dayOfWeek)
        {
            int dayOfWeekId = -1;

            var dow = PemsEntities.DayOfWeeks.FirstOrDefault( m => m.DayOfWeekDesc.Equals( dayOfWeek, StringComparison.CurrentCultureIgnoreCase ) );

            if ( dow != null )
            {
                dayOfWeekId = dow.DayOfWeekId;
            }

            return dayOfWeekId;
        }



        public List<SelectListItemWrapper> OperationModesList(int customerId)
        {
            List<SelectListItemWrapper> list = (from om in PemsEntities.OperationModes
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = false,
                                                    Text = om.OperationModeDesc,
                                                    ValueInt = om.OperationModeId
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = true,
                Text = "",
                Value = "-1"
            });

            return list;
        }


        public List<SelectListItemWrapper> TariffMessageList(int customerId)
        {
            List<SelectListItemWrapper> list = new List<SelectListItemWrapper>();

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 1,
                    Text = "TOW AWAY ZONE NO PARKING",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 2,
                    Text = "BUS ZONE UNTIL: hh:mm",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 3,
                    Text = "CLEARWAY UNTIL: hh:mm",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 4,
                    Text = "NO STANDING UNTIL: hh:mm",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 5,
                    Text = "LOADING ZONE 15 MINUTES MAX",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 6,
                    Text = "NO PARKING UNTIL: hh:mm",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 7,
                    Text = "FREE PARKING 15 MINUTES MAX",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 8,
                    Text = "PERMIT PARKING ONLY",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 9,
                    Text = "REFER TO SIGNS UNTIL: hh:mm",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 10,
                    Text = "FREE PARKING UNTIL: hh:mm",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 11,
                    Text = "NO STOPPING FINES APPLY",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 12,
                    Text = "UNRESTRICTED PARKING UNTIL: hh:mm",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 13,
                    Text = "FREE PARKING NO PAYMENT REQUIRED",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 14,
                    Text = "TAXI STAND UNTIL: hh:mm",
                    Selected = false
                });

            list.Add(new SelectListItemWrapper()
                {
                    ValueInt = 15,
                    Text = "TAXI ZONE UNTIL: hh:mm",
                    Selected = false
                });


            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = true,
                Text = "",
                Value = "0"
            });

            return list;

        }




        #endregion


    }
}
