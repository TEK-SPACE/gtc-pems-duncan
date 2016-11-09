using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Duncan.PEMS.SpaceStatus.UtilityClasses
{
    public enum AIExceptionCode
    {
        XMLServerNotAvailable = 1,
        DatabaseNotAvailable = 2,
        TableValuesUnAvailable = 3,
        MeterBayInfoUnavailable = 4,
        ParkingStatusUnAvailable = 5
    }

    public class AIException : System.Exception
    {
        public AIExceptionCode ErrorCode;

        public AIException(AIExceptionCode ErrorCode, string errMsg)
            : base(errMsg)
        {
            this.ErrorCode = ErrorCode;
        }
    }

    public class AIConfigurationException : System.Exception
    {
        public AIConfigurationException(string errMsg)
            : base(errMsg)
        {
        }
    }
}