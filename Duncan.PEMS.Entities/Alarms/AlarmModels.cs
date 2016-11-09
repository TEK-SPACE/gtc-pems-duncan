/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             ___________________________________________________________________________________________________
 * 
 * 01/20/2014       Sergey Ostrerov                 DPTXPEMS-75 - AssetId isn't available in grid page although it's available in filter
 * 
 * *****************************************************************************************************************************************************/

using System;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Alarms
{
    public class SpAlarmModel
    {
        public long RowNumber { get; set; }
        public int Count { get; set; }
        public int CustomerID { get; set; }
        public DateTime TimeOfOccurrance { get; set; }
        [OriginalGridPosition(Position = 0)]
        public string TimeOfOccurranceDisplay { get { return TimeOfOccurrance == DateTime.MinValue ? string.Empty : TimeOfOccurrance.ToString(); } }
        [OriginalGridPosition(Position = 1)]
        public int? AlarmUID { get; set; }
        [OriginalGridPosition(Position = 2)]
        public int AlarmCode { get; set; }
        [OriginalGridPosition(Position = 3)]
        public string AlarmCodeDesc { get; set; }
        [OriginalGridPosition(Position = 4)]
        public string AssetType { get; set; }
        [OriginalGridPosition(Position = 5)]
        public int MeterId { get; set; }
        [OriginalGridPosition(Position = 6)]
        public string AssetName { get; set; }
        [OriginalGridPosition(Position = 7)]
        public string Location { get; set; }
        [OriginalGridPosition(Position = 8)]
        public string AlarmSeverity { get; set; }
        [OriginalGridPosition(Position = 9)]
        public string AssetState { get; set; }
        [OriginalGridPosition(Position =10)]
        public string AlarmStatus { get; set; }

        public int EventState { get; set; }

        [OriginalGridPosition(Position = 11)]
        public string EventStateDisplay { get { return EventState == 0 ? "Raised" : "Cleared"; } }

        [OriginalGridPosition(Position = 12)]
        public string TimeOfClearanceDisplay { get { return TimeOfClearance.HasValue ? TimeOfClearance.Value.ToString() : string.Empty; } }
        [OriginalGridPosition(Position = 13)]
        public string TotalMinutesDisplay { get; set; }
        public int? TotalMinutes { get; set; }
        [OriginalGridPosition(Position = 14)]
        public string Area { get; set; }
        public int AreaID { get; set; }
        public int? AreaId2 { get; set; }
        [OriginalGridPosition(Position = 15)]
        public string Zone { get; set; }
        [OriginalGridPosition(Position =16)]
        public string Suburb { get; set; }
        [OriginalGridPosition(Position = 17)]
        public string AlarmSourceDesc { get; set; }
        public int? AlarmSourceCode { get; set; }
        [OriginalGridPosition(Position = 18)]
        public int? TechnicianId { get; set; }
        public int? TimeType1 { get; set; }
        public int? TimeType2 { get; set; }
        public int? TimeType3 { get; set; }
        public int? TimeType4 { get; set; }
        public int? TimeType5 { get; set; }
        //public int MeterId { get; set; }
        public string TargetService { get; set; }
        public DateTime? TimeOfClearance { get; set; }
        //[OriginalGridPosition(Position =11)]
        //public string TimeOfClearanceDisplay { get { return TimeOfClearance.HasValue ? TimeOfClearance.Value.ToString() : string.Empty; } }
        public string DemandArea { get; set; }

        public bool IsClosed
        {
            get { return AlarmStatus != "Open"; }
        }

        public long OccuranceTimeTicks
        {
            get { return TimeOfOccurrance.Ticks; }
        }

        public Nullable<int> TimeRemainingUntilTarget { get; set; }
    }

    public class ListAlarmModel
    {
        [Display(Name = "Time of Complaint")]
        public DateTime TimeOfComplaint { get; set; }

        [Display(Name = "Alarm Id")]
        public int AlarmId { get; set; }

        [Display(Name = "Alarm Code")]
        public int AlarmCode { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Asset Type")]
        public string AssetType { get; set; }

        [Display(Name = "Asset Id")]
        public int MeterId { get; set; }

        [Display(Name = "Asset Name")]
        public string AssetName { get; set; }

        [Display(Name = "Street")]
        public string Street { get; set; }

        [Display(Name = "Alarm Severity")]
        public string AlarmSeverity { get; set; }

        [Display(Name = "Asset State")]
        public string AssetState { get; set; }

        [Display(Name = "Asset Status")]
        public string AlarmStatus { get; set; }

        [Display(Name = "Event State")]
        public int EventState { get; set; }

        [Display(Name = "Time Remaining Until Target Time")]
        public int TimeRemainingUntilTargetTime { get; set; }

        [Display(Name = "Time Cleared")]
        public DateTime TimeCleared { get; set; }

        [Display(Name = "Area")]
        public string Area { get; set; }

        [Display(Name = "Area Id")]
        public int AreaId { get; set; }

        [Display(Name = "Zone")]
        public string Zone { get; set; }

        [Display(Name = "Suburb")]
        public string Suburb { get; set; }

        [Display(Name = "Alarm Source")]
        public string AlarmSource { get; set; }

        [Display(Name = "Technician Id")]
        public int TechnicianId { get; set; }

        [Display(Name = "Target Service")]
        public string TargetService { get; set; }

        public string TargetServiceId { get; set; }
        public int AlarmSourceId { get; set; }

        public long OccuranceTimeTicks
        {
            get { return TimeOfComplaint.Ticks; }
        }

        //public int MeterId { get; set; }
        public bool IsClosed { get; set; }
        public int TimeType1 { get; set; }
        public int TimeType2 { get; set; }
        public int TimeType3 { get; set; }
        public int TimeType4 { get; set; }
        public int TimeType5 { get; set; }
    }
}