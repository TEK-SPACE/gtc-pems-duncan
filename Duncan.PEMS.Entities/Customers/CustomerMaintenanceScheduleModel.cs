/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             ___________________________________________________________________________________________________
 * 
 * 02/28/2014       Sergey Ostrerov                 DPTXPEMS - 240 Reopened - Maintenance Schedules for a given customer is created without schedule 
 *                                                                            start date and end date.
 * *****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerMaintenanceScheduleModel: CustomerBaseModel
    {
        public const string NameNoHoursPrefix = "dwNoHours";
        public const string NameStartTimePrefix = "dwStartTime";
        public const string NameEndTimePrefix = "dwEndTime";
        public char Separator = '_';
        public List<DayOfWeek> DaysOfWeek { get; set; }
        public CustomerMaintenanceScheduleModel()
        {
            DaysOfWeek = new List<DayOfWeek>();
        }
    }


    public class DayOfWeek
    {
        public int DayOfWeekId { get; set; }
        public string Name { get; set; }
        public int StartMinute { get; set; }
        public DateTime StartTime { get
        {
            //any datetime, we just care about hh:mm
            var dt = new DateTime(2012, 01, 01);
            var ts = TimeSpan.FromMinutes(StartMinute);
            dt = dt + ts;
            return dt;
        }
        }
        public int EndMinute { get; set; }
        public DateTime EndTime
        {
            get
            {
                //any datetime, we just care about hh:mm
                var dt = new DateTime(2012, 01, 01);
                var ts = TimeSpan.FromMinutes(EndMinute);
                dt = dt + ts;
                return dt;
            }
        }

        public DateTime ScheduleStartDate
        {
            get
            {
                return new DateTime(2010, 01, 01);;
            }
        }

        public DateTime ScheduleEndDate
        {
            get
            {
                return new DateTime(2050, 01, 01);
            }
        }

        
        public bool NoHours { get; set; }
    }
}
