using Duncan.PEMS.Entities.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Entities.AutoAlarm
{
   public class AutoAlarmModel
    {
        public string AutoAlarmEventSource { get; set; }

        public int CustomerID { get; set; }
        public string CustomerIDText { get; set; }
        public string LowBattAlarmCodeMinor { get; set; }
        public List<SelectListItemWrapper> LowBattAlarmCodeMinorEvents { get; set; }
        public string LowBattAlarmCodeMajor { get; set; }
        public List<SelectListItemWrapper> LowBattAlarmCodeMajorEvents { get; set; }
        public string LowBattEnabledMeters { get; set; }
        public string LowBattPollInterval { get; set; }
        public string LowBattAlarmClearVolt { get; set; }
        public string LowBattCritLowBattVolt { get; set; }
        public string LowBattLowBattVolt { get; set; }

        public string LowBattSampleSinceHrs { get; set; }
        public List<SelectListItemWrapper> LowBattSsmVoltDiagTypes { get; set; }
        public string LowBattSsmVoltDiagType { get; set; }

        public string NoCommAlarmCode { get; set; }

        public List<SelectListItemWrapper> NoCommAlarmCodeEvents { get; set; }
        public string NoCommPollTime { get; set; }
        public string NoCommEnabledMeters { get; set; }
        public string EventsText { get; set; }

        public int EventsValue { get; set; }

       
    }

      
}
