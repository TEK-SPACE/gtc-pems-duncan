using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Business.Globalization
{
    public class GlobalizationManager
    {
        /// <summary>
        /// Checks to see if a culture file exist on the file system for a specific culture
        /// </summary>
        private static bool KendoCultureExists(string cultureCode, out string src)
        {
            const string culturesDirectory = "/Scripts/Kendo/cultures/";
            string fileName = String.Format( "kendo.culture.{0}.min.js", cultureCode.Trim() );
            string relativePath = culturesDirectory + fileName;
            string absolutePath = HostingEnvironment.MapPath( relativePath );

            if ( File.Exists( absolutePath ) )
            {
                src = relativePath;
                return true;
            }
            else
            {
                src = String.Empty;
                return false;
            }
        }

        /// <summary>
        /// Sets a culture for kendo. This is set globally.
        /// </summary>
        public static HtmlString SetKendoCulture(string locale)
        {
            string javascript = "";

            if ( locale != "en-US" )
            {
                string filepath;
                if ( KendoCultureExists( locale, out filepath ) )
                {
                    javascript += String.Format( "<script type=\"text/javascript\" src=\"{0}\"></script>", filepath );
                    javascript += String.Format( "<script type=\"text/javascript\">kendo.culture(\"{0}\");</script>", locale );
                }
            }

            return new HtmlString( javascript );
        }


        /// <summary>
        /// Sets a 24 hour clock format, this is set globally.
        /// </summary>
        public static HtmlString Force24HourClock(bool currentTimeFormatIs24)
        {
            string javascript = "";

            if ( currentTimeFormatIs24 )
            {
                // force 24 hour clock
                javascript = "<script type='text/javascript'> set24HourClock() </script>";
            }

            return new HtmlString( javascript );
        }
    }
}