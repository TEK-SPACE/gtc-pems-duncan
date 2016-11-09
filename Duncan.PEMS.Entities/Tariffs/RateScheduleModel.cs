using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Tariffs
{
    #region Rate Schedule Models

    public class RateScheduleConfigurationModel : IValidatableObject
    {
        public int CustomerId { get; set; }
        
        public Int64 RateScheduleConfigurationId { get; set; }
        public string RateScheduleConfigurationIdDisplay { get { return RateScheduleConfigurationId.ToString(); } }

        public int RateScheduleCount { get; set; }

        public int[] DayOfWeek { get; private set; }

        private const int AllDaysCoveredHash = 0x7F; // 127

        public bool AllDaysCovered
        {
            get { return DaysCoveredHash == AllDaysCoveredHash; }
        }

        public int DaysCoveredHash
        {
            get
            {
                int daysHash = 0;
                for (int i = 0; i < DayOfWeek.Length; i++)
                {
                    daysHash += ((int)Math.Pow(2, i)) * DayOfWeek[i];
                }
                return daysHash;
            }
        }
        
        
        public int DaysCovered 
        {
            get
            {
                int days = 0;
                for (int i = 0; i < DayOfWeek.Length; i++) days += DayOfWeek[i];
                return days;
            }
        }

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
        /// <returns>True if this and the compared object have the same RateScheduleConfigurationId</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is RateScheduleConfigurationModel)) return false;
            var rscm = obj as RateScheduleConfigurationModel;
            return this.RateScheduleConfigurationId == rscm.RateScheduleConfigurationId;
        }

        public RateScheduleConfigurationModel()
        {
            DayOfWeek = new int[7];
        }


        public void UpdateStats(List<RateScheduleModel> list)
        {
            for (int i = 0; i < DayOfWeek.Count(); i++)
            {
                DayOfWeek[i] = 0;
            }
            foreach (var rs in list)
            {
                DayOfWeek[rs.DayOfWeek] = 1;
            }

            RateScheduleCount = list.Count;
        }


    }

    //public class RateScheduleConfigurationForDayModel : IComparable<RateScheduleConfigurationForDayModel>
    //{
    //    public int CustomerId { get; set; }

    //    public Int64 RateScheduleConfigurationId { get; set; }
    //    public string RateScheduleConfigurationIdDisplay { get { return RateScheduleConfigurationId.ToString(); } }

    //    public int RateScheduleCount { get; set; }

    //    public int DayOfWeek { get; set; }
    //    public string DayOfWeekName { get; set; }

    //    /// <summary>
    //    /// Compare method - Compare day of week.
    //    /// </summary>
    //    /// <param name="other"><see cref="RateScheduleConfigurationForDayModel"/> to compare</param>
    //    /// <returns>1 if <see cref="other"/> is larger, 0 if equal, -1 if less</returns>
    //    public int CompareTo(RateScheduleConfigurationForDayModel other)
    //    {
    //        if (DayOfWeek < other.DayOfWeek) return -1;
    //        if (DayOfWeek > other.DayOfWeek) return 1;

    //        // this and other are effectively equal.
    //        return 0;
    //    }
    //}


    public class RateScheduleModel : IValidatableObject, IComparable<RateScheduleModel>
    {
        ///// <summary>
        ///// Details the space associated with this rate schedule.  This may be null.
        ///// </summary>
        //public SpaceViewModel Space { get; set; }

        public const string ChkBoxPrepend = "DOWCHK";
        public const char ChkBoxDivider = '_';


        public int CustomerId { get; set; }

        public Int64 RateScheduleConfigurationId { get; set; }
        public string RateScheduleConfigurationIdDisplay { get { return RateScheduleConfigurationId.ToString(); } }

        public Int64 RateScheduleId { get; set; }
        public string RateScheduleIdDisplay { get { return RateScheduleId.ToString(); } }

        public int ScheduleNumber { get { return 1; } }

        public int DayOfWeek { get; set; }
        public string DayOfWeekName { get; set; }

        public int StartTimeHour { get; set; }
        public int StartTimeMinute { get; set; }

        public string StartTime
        {
            get { return String.Format("{0,2:D2}:{1,2:D2}", StartTimeHour, StartTimeMinute); }
        }

        public int OperationMode { get; set; }
        public string OperationModeName { get; set; }

        //public int? MessageSequence { get; set; }
        //public string MessageSequenceName { get; set; }

        //public bool LockMaxTime { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedOnDisplay { get { return CreatedOn.HasValue ? CreatedOn.Value.ToString("g") : string.Empty; } }
        public int? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string UpdatedOnDisplay { get { return UpdatedOn.HasValue ? UpdatedOn.Value.ToString("g") : string.Empty; } }
        public int? UpdatedBy { get; set; }

        public Int64? TariffRateId { get; set; }
        public string TariffRateIdDisplay { get { return TariffRateId.HasValue ? TariffRateId.Value.ToString() : string.Empty; } }
        public string TariffRateName { get; set; }

        public bool IsChanged { get; set; }

        public bool IsSaved { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // OperationMode
            if (string.IsNullOrWhiteSpace( OperationModeName ))
            {
                errors.Add(new ValidationResult("Operation Mode is required.", new[] { "OperationMode" }));
            }

            return errors;
        }

        public RateScheduleModel()
        {
            OperationMode = -1;
            DayOfWeek = -1;
        }

        /// <summary>
        /// Compare method
        /// </summary>
        /// <param name="other"><see cref="RateScheduleModel"/> to compare</param>
        /// <returns>1 if <see cref="other"/> is larger, 0 if equal, -1 if less</returns>
        public int CompareTo(RateScheduleModel other)
        {
            if (DayOfWeek < other.DayOfWeek) return -1;
            if (DayOfWeek > other.DayOfWeek) return 1;
            if (StartTimeHour < other.StartTimeHour) return -1;
            if (StartTimeHour > other.StartTimeHour) return 1;
            if (StartTimeMinute < other.StartTimeMinute) return -1;
            if (StartTimeMinute > other.StartTimeMinute) return 1;
            if (OperationMode < other.OperationMode) return -1;
            if (OperationMode > other.OperationMode) return 1;

            // this and other are effectively equal.
            return 0;
        }
    }

    #endregion

}
