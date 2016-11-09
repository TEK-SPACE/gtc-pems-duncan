using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;

namespace Duncan.PEMS.Entities.Tariffs
{

    public class ConfigProfileModel : IValidatableObject
    {
        public int CustomerId { get; set; }

        public Int64 ConfigProfileId { get; set; }
        // This is used to overcome an issue  in Kendo where Kendo's ToString of an Int64 truncates trailing digits.
        public string ConfigProfileIdDisplay { get { return ConfigProfileId.ToString(); } }

        public string ConfigurationName { get; set; }
//        public string Version6 { get; set; }
        public string TariffPolicyName { get; set; }
        public bool Minute15FreeParking { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedOnDisplay { get { return CreatedOn.HasValue ? CreatedOn.Value.ToString("d") : string.Empty; } }
        public int? CreatedBy { get; set; }

        // Tariff Rates details.
        public Int64 TariffRateConfigurationId;
        public string TariffRateConfigurationIdDisplay { get { return TariffRateConfigurationId.ToString(); } }
        public string TariffRateConfigurationName { get; set; }
        public TariffStateType TariffRateConfigurationState { get; set; }
        public string TariffRateConfigurationStateName { get; set; }
        public int TariffRatesCount;

        // Rate Schedules details
        public Int64 RateScheduleConfigurationId;
        public string RateScheduleConfigurationIdDisplay { get { return RateScheduleConfigurationId.ToString(); } }
        public string RateScheduleConfigurationName { get; set; }
        public TariffStateType RateScheduleConfigurationState { get; set; }
        public string RateScheduleConfigurationStateName { get; set; }
        public int RateSchedulesCount;

        public int[] DayOfWeek { get; private set; }



        // Holiday Rates details
        public Int64 HolidayRateConfigurationId;
        public string HolidayRateConfigurationIdDisplay { get { return HolidayRateConfigurationId.ToString(); } }
        public string HolidayRateConfigurationName { get; set; }
        public TariffStateType HolidayRateConfigurationState { get; set; }
        public string HolidayRateConfigurationStateName { get; set; }
        public int HolidayRatesCount;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // ConfigurationName
            if (string.IsNullOrWhiteSpace(ConfigurationName))
            {
                errors.Add(new ValidationResult("Configuration Name is required.", new[] { "ConfigurationName" }));
            }

            // TariffPolicyName
            if (string.IsNullOrWhiteSpace(TariffPolicyName))
            {
                errors.Add(new ValidationResult("Policy Name is required.", new[] { "TariffPolicyName" }));
            }

            return errors;
        }


        public ConfigProfileModel()
        {
            DayOfWeek = new int[7];
        }


    }

    public class ConfigProfileSpaceViewModel : ConfigProfileModel
    {
        /// <summary>
        /// Details the space associated with this config profile space.  This may be null.
        /// </summary>
        public SpaceViewModel Space { get; set; }

        public Int64? ConfigProfileSpaceId { get; set; }
        // This is used to overcome an issue in Kendo where Kendo's ToString of an Int64 truncates trailing digits.
        public string ConfigProfileSpaceIdDisplay { get { return ConfigProfileSpaceId.HasValue ? ConfigProfileSpaceId.ToString() : string.Empty; } }

        public int ActiveSpacesCount { get; set; }

        public DateTime? ScheduledDate { get; set; }
        public string ScheduledDateDisplay { get { return ScheduledDate.HasValue ? ScheduledDate.Value.ToString("d") : string.Empty; } }

        public DateTime? ActivationDate { get; set; }
        public string ActivationDateDisplay { get { return ActivationDate.HasValue ? ActivationDate.Value.ToString("d") : string.Empty; } }

        public DateTime? CreationDate { get; set; }
        public string CreationDateDisplay { get { return CreationDate.HasValue ? CreationDate.Value.ToString("d") : string.Empty; } }

        public DateTime? EndDate { get; set; }
        public string EndDateDisplay { get { return EndDate.HasValue ? EndDate.Value.ToString("d") : string.Empty; } }

        public string Status { get; set; }

        public string UserName { get; set; }
    }
}