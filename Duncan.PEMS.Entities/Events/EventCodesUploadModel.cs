using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Duncan.PEMS.Entities.Events
{
    public class UploadEventCodesModel
    {
        public int Id { get; set; }

        [CsvColumn(Name = "EventSource", CanBeNull = false)]    // [EventSources]
        public string EventSource { get; set; }

        [CsvColumn(Name = "EventCode", CanBeNull = false)]
        public int EventCode { get; set; } 

        [CsvColumn(Name = "AlarmTier", CanBeNull = false)]      // [AlarmTier]
        public string AlarmTier { get; set; }

        [CsvColumn(Name = "EventType", CanBeNull = false)]      // [EventType]
        public string EventType { get; set; }

        [CsvColumn(Name = "EventCategory", CanBeNull = false)]  // [EventCategory]
        public string EventCategory { get; set; }

        [CsvColumn(Name = "AssetType", CanBeNull = false)]      // [MeterGroup]/[AssetType]
        public string AssetType { get; set; }

        [CsvColumn(Name = "EventDescAbbrev", CanBeNull = true)]
        public string EventDescAbbrev { get; set; }

        [CsvColumn(Name = "EventDescVerbose", CanBeNull = true)]
        public string EventDescVerbose { get; set; }

        [CsvColumn(Name = "SLADuration", CanBeNull = true)]
        public int? SLADuration { get; set; } 
    }

}
