
/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             __________________________________________________________________________________________________
 * 02/06/2014       Sergey Ostrerov                 JIRA: DPTXPEMS-213  TimePaid formatted 00:00:00
 * 
 * *****************************************************************************************************************************************************/

using System;
using System.Web;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Utilities
{
    public class FormatHelper
    {
        private const string nullChar = "";

        public static string FormatCurrency(int? amountInCents)
        {
            if ( amountInCents == null )
            {
                return nullChar;
            }

            decimal newAmount = (decimal) amountInCents / 100;
            string formattedAmount = String.Format( "{0:C}", newAmount );
            return formattedAmount;
        }

        public static string FormatCurrency( double? amountInDollars )
        {
            if( amountInDollars == null )
            {
                return nullChar;
            }

            string formattedAmount = String.Format( "{0:C}", amountInDollars );
            return formattedAmount;
        }

        public static string FormatTimeFromMinutes(int? minutes)
        {
            if (minutes == null)
                return nullChar;
            return FormatTimeFromMinutes((double?)minutes);
        }

        public static string FormatTimeFromMinutes(double? minutes)
        {
            if (minutes == null)
                return nullChar;
            double ? timeInSeconds = (int)60 * minutes;
            TimeSpan timeSpan = TimeSpan.FromMinutes((double)minutes);

            TimeSpan timeSpanSec = TimeSpan.FromSeconds((double)minutes);
            string formatted = "00:00";
            if (timeSpan.Hours > 0 || timeSpan.Minutes > 0)
            {
                //if the seconds are greater than 29, then add a minute. This will allow the display to effectivly round minutes up, since we dont display seconds.
                if (timeSpan.Seconds > 29)
                    formatted = String.Format("{0:00}:{1:00}", timeSpan.Hours, timeSpan.Minutes +1);
                else
                    formatted = String.Format("{0:00}:{1:00}", timeSpan.Hours, timeSpan.Minutes);

            }
            return formatted;
        }

        public static string FormatTimeFromMinutes(double? minutes, int timeFormatType)
        {
            string formatted = "999.99";

            if (minutes == null)
                return nullChar;
            var span = TimeSpan.FromMinutes((double)minutes);

            if(Math.Floor(span.TotalHours) <= 999){
                switch (timeFormatType)
                {
                    case (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HH_MM:
                        formatted = String.Format(Constants.TimeFormats.timeFormatToDisplay_HH_MM, Math.Floor(span.TotalHours), span.Minutes);
                        break;
                    case (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HH_MM_SS:
                        formatted = String.Format(Constants.TimeFormats.timeFormatToDisplay_HH_MM_SS, Math.Floor(span.TotalHours), span.Minutes, span.Seconds);
                        break;
                    case (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HHH_MM:
                        formatted = String.Format(Constants.TimeFormats.timeFormatToDisplay_HHH_MM, Math.Floor(span.TotalHours), span.Minutes);
                        break;
                    case (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HHH_MM_SS:
                        formatted = String.Format(Constants.TimeFormats.timeFormatToDisplay_HHH_MM_SS, Math.Floor(span.TotalHours), span.Minutes, span.Seconds);
                        break;
                }
            }

            return formatted;
        }

        public static string FormatTimeFromMinutesTwoDigit(double? minutes, int timeFormatType)
        {
            string formatted = "9999.99";

            if (minutes == null)
                return nullChar;
            var span = TimeSpan.FromMinutes((double)minutes);

            if (Math.Floor(span.TotalHours) <= 999)
            {
                switch (timeFormatType)
                {
                    case (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HH_MM:
                        formatted = String.Format(Constants.TimeFormats.timeFormatToDisplay_HH_MM, Math.Floor(span.TotalHours), span.Minutes);
                        break;
                    case (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HH_MM_SS:
                        formatted = String.Format(Constants.TimeFormats.timeFormatToDisplay_HH_MM_SS, Math.Floor(span.TotalHours), span.Minutes, span.Seconds);
                        break;
                    case (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HHH_MM:
                        formatted = String.Format(Constants.TimeFormats.timeFormatToDisplay_HHH_MM, Math.Floor(span.TotalHours), span.Minutes);
                        break;
                    case (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HHH_MM_SS:
                        formatted = String.Format(Constants.TimeFormats.timeFormatToDisplay_HHH_MM_SS, Math.Floor(span.TotalHours), span.Minutes, span.Seconds);
                        break;
                }
            }

            return formatted;
        }



        private static int GetOverflowsFormattedValue(double timeValHour, double timeValMinutes, string timeType)
        {
            int val = 0;
            if (timeType == "h")
            {
                if (timeValHour > 999)
                {
                    val = 999;
                }
                else
                {
                    val = (int)timeValHour;
                }
            }
            else if (timeType == "m")
            {
                if (timeValHour > 999)
                {
                    val = 99;
                }
                else
                {
                    val = (int)timeValMinutes;
                }
            }

            return val;
        }


        public static string FormatTimeFromSeconds(double? seconds)
        {
            if (seconds == null)
                return nullChar;
            TimeSpan span = new TimeSpan(0, 0, (int)seconds);

            return String.Format(Constants.TimeFormats.timeFormatToDisplay_HH_MM, span.Hours, span.Minutes);
        }

        public static string FormatTimeFromSecondsToHHMMSS(double? seconds)
        {
            if (seconds == null)
                return nullChar;
            TimeSpan span = new TimeSpan(0, 0, (int)seconds);
           

            return String.Format(Constants.TimeFormats.timeFormatToDisplay_HH_MM_SS, span.Hours, span.Minutes, span.Seconds);
        }

              
        
        public static string FormatDateTimeToShortDate(DateTime? dateTime)
        {
            string formatted = dateTime.HasValue
                                   ? dateTime.Value.ToShortDateString()
                                   : nullChar;
            return formatted;
        }

        public static string FormatDateTime(DateTime? dateTime, bool includeDayOfWeek = false, string formatSpecifier = "G")
        {
            string formatted;

            try
            {
                if ( includeDayOfWeek )
                {
                    formatted = dateTime.HasValue
                                    ? String.Format( "{0} {1}", dateTime.Value.DayOfWeek, dateTime.Value.ToString( formatSpecifier ) )
                                    : nullChar;
                }
                else
                {
                    formatted = dateTime.HasValue
                                    ? dateTime.Value.ToString( formatSpecifier )
                                    : nullChar;
                }
            }
            catch (Exception)
            {
                formatted = nullChar;
            }

            return formatted;
        }

        public static string FormatValue(object obj)
        {
            if ( obj == null )
            {
                return nullChar;
            }

            string strVal = obj.ToString();
            if ( String.IsNullOrWhiteSpace( strVal ) )
            {
                return nullChar;
            }

            return strVal;
        }

        public static string FormatBoolean(bool? value)
        {
            try
            {
                if( value == null )
                {
                    return nullChar;
                }

                if( value == true )
                {
                    return HttpContext.Current.GetLocaleResource( ResourceTypes.Glossary, "Yes" );
                }

                if( value == false )
                {
                    return HttpContext.Current.GetLocaleResource(ResourceTypes.Glossary, "No");
                }
            }
            catch (Exception ex)
            {
                return nullChar;
            }

            return nullChar;
        }
    }
}