using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;

namespace Duncan.PEMS.Entities.Tariffs
{
    #region Holiday Rate Models

    public class HolidayRateConfigurationModel : IValidatableObject
    {
        public int CustomerId { get; set; }

        public Int64 HolidayRateConfigurationId { get; set; }
        public string HolidayRateConfigurationIdDisplay { get { return HolidayRateConfigurationId.ToString(); } }

        public int HolidayRateCount { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedOnDisplay { get { return CreatedOn.HasValue ? CreatedOn.Value.ToString("g") : string.Empty; } }
        public int? CreatedBy { get; set; }

        public DateTime? ConfiguredOn { get; set; }
        public string ConfiguredOnDisplay { get { return ConfiguredOn.HasValue ? ConfiguredOn.Value.ToString("g") : string.Empty; } }
        public int? ConfiguredBy { get; set; }

        public TariffStateType State { get; set; }
        public string StateName { get { return State.ToString(); } }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            //// Name
            //if (string.IsNullOrWhiteSpace(Name))
            //{
            //    errors.Add(new ValidationResult("Configuration Name is required.", new[] { "Name" }));
            //}

            return errors;
        }

        /// <summary>
        /// Override of Equals so that Entity Framework will compare two instances of these 
        /// and be able to determine that they are the same record.
        /// </summary>
        /// <param name="obj">A generic object instance to compare</param>
        /// <returns>True if this and the compared object have the same HolidayRateConfigurationId</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is HolidayRateConfigurationModel)) return false;
            var hrcm = obj as HolidayRateConfigurationModel;
            return this.HolidayRateConfigurationId == hrcm.HolidayRateConfigurationId;
        }

    }

    public class HolidayRateModel : IValidatableObject, IComparable<HolidayRateModel>
    {
        ///// <summary>
        ///// Details the space associated with this holiday rate.  This may be null.
        ///// </summary>
        //public SpaceViewModel Space { get; set; }

        public int CustomerId { get; set; }

        public Int64 HolidayRateConfigurationId { get; set; }
        public string HolidayRateConfigurationIdDisplay { get { return HolidayRateConfigurationId.ToString(); } }

        public Int64 HolidayRateId { get; set; }
        public string HolidayRateIdDisplay { get { return HolidayRateId.ToString(); } }

        public string HolidayName { get; set; }

        public DateTime HolidayDateTime { get; set; }
        public string HolidayDateTimeDisplay { get { return HolidayDateTime.ToString("d"); } }

        public DateTime? CreatedOn { get; set; }
        public string CreatedOnDisplay { get { return CreatedOn.HasValue ? CreatedOn.Value.ToString("g") : string.Empty; } }
        public int? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string UpdatedOnDisplay { get { return UpdatedOn.HasValue ? UpdatedOn.Value.ToString("g") : string.Empty; } }
        public int? UpdatedBy { get; set; }

        // RateSchedule data.
        public Int64 RateScheduleConfigurationId { get; set; }
        public string RateScheduleConfigurationIdDisplay { get { return RateScheduleConfigurationId > 0 ? RateScheduleConfigurationId.ToString() : ""; } }

        public int DayOfWeek { get; set; }
        public string DayOfWeekName { get; set; }

        public int RateScheduleCount { get; set; }

//        public RateScheduleConfigurationViewModel RateSchedules { get; set; }

        public bool IsChanged { get; set; }

        public bool IsSaved { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // HolidayName
            if (string.IsNullOrWhiteSpace(HolidayName))
            {
                errors.Add(new ValidationResult("Holiday Name is required", new[] { "HolidayName" }));
            }

            // HolidayDateTime
            if (HolidayDateTime == DateTime.MinValue)
            {
                errors.Add(new ValidationResult("Holiday Date is required", new[] { "HolidayDateTime" }));
            }

            return errors;
        }

        /// <summary>
        /// Compare holiday dates to order lists of <see cref="HolidayRateModel"/>
        /// </summary>
        /// <param name="other"><see cref="HolidayRateModel"/> instance to compare to</param>
        /// <returns>1 if <see cref="other"/> is larger, 0 if equal, -1 if less</returns>
        public int CompareTo(HolidayRateModel other)
        {
            return HolidayDateTime.CompareTo(other.HolidayDateTime);
        }
    }

    public class HolidayRateScheduleModel : RateScheduleModel
    {
        public Int64 HolidayRateConfigurationId { get; set; }
        public string HolidayRateConfigurationIdDisplay { get { return HolidayRateConfigurationId.ToString(); } }

        public Int64 HolidayRateId { get; set; }
        public string HolidayRateIdDisplay { get { return HolidayRateId.ToString(); } }

        public Int64 RateScheduleConfigurationId { get; set; }
        public string RateScheduleConfigurationIdDisplay { get { return RateScheduleConfigurationId.ToString(); } }
    }

    
    //public class SpaceHolidayRateConfiguredViewModel : HolidayRateConfiguredViewModel
    //{
    //    public SpaceViewModel Space { get; set; }
    //}

    #endregion
}
