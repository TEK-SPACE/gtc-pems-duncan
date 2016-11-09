using System;

namespace Duncan.PEMS.Entities.Alarms
{
    [Serializable]
    public class AlarmListItemModel
    {
        public DateTime TimeOfComplaint { get; set; }

        public string AlarmCode { get; set; }

        public string AlarmDescription { get; set; }

        public string AlarmType { get; set; }

        public string AssetType { get; set; }

        public string AssetName { get; set; }

        public string Street { get; set; }

        public string AlarmSeverity { get; set; }

        public string AssetState { get; set; }

        public string AlarmStatus { get; set; }

        public string TimeRemainingUntilTarget { get; set; }

        public DateTime Time { get; set; }

        public DateTime TimeCleared { get; set; }

        public string Area { get; set; }

        public string Zone { get; set; }

        public string Suburb { get; set; }

        public string AlarmSource { get; set; }

        public string TechnicianID { get; set; }

        public int AlarmID { get; set; }
    }
}