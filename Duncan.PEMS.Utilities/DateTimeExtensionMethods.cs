using System;

namespace Duncan.PEMS.Utilities
{
    public static class DateTimeExtensionMethods
    {
        public static string ToLocalizedString( this DateTime date )
        {
            return date.ToString( "G" ); // 6/15/2009 1:45:30 PM
        }
    }
}
