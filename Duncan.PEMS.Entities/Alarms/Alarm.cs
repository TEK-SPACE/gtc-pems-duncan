using System;
using System.Collections.Generic;
using Duncan.PEMS.Entities.Enumerations;

namespace Duncan.PEMS.Entities.Alarms
{
    [Serializable]
    public class AlarmModel
    {
        public int CustomerId { get; set; }
        public int EventSource { get; set; }

        public long OccuranceTimeTicks
        {
            get { return TimeOccured.Ticks; }
        }

        public string ClosedByName { get; set; }

        #region Asset/Location information

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string AssetType { get; set; }
        public AssetClass AssetClass { get; set; }
        public int MeterGroupId { get; set; }
        public int AssetID { get; set; }
        public string AssetName { get; set; }
        public string Area { get; set; }
        public int? AreaId { get; set; }
        public string Zone { get; set; }
        public string Suburb { get; set; }
        public string Street { get; set; }
        public int BaysAffected { get; set; }

        #endregion

        #region Alarm Information

        public int AlarmID { get; set; }
        public int AlarmCode { get; set; }
        public string AlarmDesription { get; set; }
        public string AlarmDesriptionVerbose { get; set; }
        public DateTime ServiceTargetTime { get; set; }
        public string AlarmStatus { get; set; }
        public DateTime TimeNotified { get; set; }
        public double TimeRemainingTillTargetTime { get; set; }
        public string TimeRemainingTillTargetTimeDisplay { get; set; }
        public bool IsClosed { get; set; }

        public DateTime TimeOccured { get; set; }
        public List<TimeType> TimeTypes { get; set; }
        public int TimeType1 { get; set; }
        public int TimeType2 { get; set; }
        public int TimeType3 { get; set; }
        public int TimeType4 { get; set; }
        public int TimeType5 { get; set; }
        public string OperationalStatus { get; set; }
        public string AlarmSource { get; set; }
        public string AlarmSeverity { get; set; }
        public string EntryNotes { get; set; }

        #endregion

        #region Closure/Repair information

        // [Required]
        public int ResolutionCode { get; set; }
        public string ResolutionCodeDesc { get; set; }
        public int ClosedBy { get; set; }
        // [Required]
        public DateTime TimeCleared { get; set; }
        //  [Required]
        public DateTime ClosureNotification { get; set; }
        public string ServiceDesignation { get; set; }
        public int ServiceDesignationId { get; set; }
        //   [Required]
        public int TechnicianID { get; set; }
        public string TechnicianName { get; set; }
        public string TechnicianDisplay { get
        {
            if (TechnicianID > 0)
                return TechnicianID.ToString() + ": " + TechnicianName;
            return "";
        }}

        //  [Required]
        public string ClosureNotes { get; set; }

        #endregion

        #region DropDowns

        public List<AlarmDDLModel> ResolutionCodes { get; set; }
        public List<AlarmDDLModel> TechnicianIDs { get; set; }

        #endregion
    }

    public class TimeType
    {
        public string Description { get; set; }
        public int Id { get; set; }
    }
}