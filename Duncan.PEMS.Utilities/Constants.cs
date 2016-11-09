/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             __________________________________________________________________________________________________
 * 02/06/2014       Sergey Ostrerov                 JIRA: DPTXPEMS-213  TimePaid formatted 00:00:00
 * 02/20/2014       Sergey Ostrerov                 DPTXPEMS - 251 Added Payments Card Types.
 * *****************************************************************************************************************************************************/

namespace Duncan.PEMS.Utilities
{
    /// <summary>
    /// The <see cref="Duncan.PEMS.Utilities"/> namespace contains classes for general utilities and constants.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Class containing string constants used to define the types of Locale resources
    /// in <see cref="SqlResourceHelper"/>.  These categorize the resource strings into groups
    /// of like usage.
    /// </summary>
    public static class ResourceTypes
    {
        public const string GridColumn = "GridColumn";
        public const string PageTitle = "PageTitle";
        public const string Label = "Label";
        public const string StatusMessage = "StatusMessage";
        public const string ErrorMessage = "ErrorMessage";
        public const string Button = "Button";
        public const string Tooltip = "Tooltip";
        public const string Glossary = "Glossary";
        public const string DropDownDefault = "Select One";
        /// <summary>
        /// Report name and description resource types
        /// </summary>
        public const string ReportName = "ReportName";
        public const string ReportDesc = "ReportDesc";
        public const string ReportCat = "ReportCat";
        
    }


    #region PEMS Constants

    public static class Constants
    {
        public static class Audits
        {
            public const string RoleTableName = "Roles";
        }

        public static class Export
        {
            public const string EscapeChar = "\"";
            public const string DelimiterChar = ",";
            public const int PdfExportCount = 5000;
            public const int ExcelExportCount = 10000;
            public const int CsvExportCount = 100000;
        }

        public static class Menu
        {
            public const string MenuText = @"menutext";
            public const string MenuOrder = @"menuorder";
            public const string ToolTip = @"tooltip";
            public const string Icon = @"menuicon";
            public const string Url = @"url";
            public const string Operation = @"operation";
            public const string Application = @"application";
            public const string Target = @"target";
            public const string NewWindow = @"newwindow";
            public const string AuthText = @"authtext";
        }

        public static class PropertyGroups
        {
            public const string AlarmStatus = "Alarm Status";
        }

        public static class Routing
        {
            public const string LandingRouteName = "Landing";
            public const string AdminRouteName = "Admin Default";
            public const string CityRouteName = "City Default";
            public const string MaintRouteName = "Maint Default";
            public const string IzendaReportingName = "Izenda Reporting Default";
            public const string DefaultRouteName = "Default";
            public const string ErrorRouteName = "Shared Error";
        }

        public static class Security
        {

            public const string DefaultPemsConnectionStringName = "pems.database.default_cnx";
            public const string DefaultMaintConnectionStringName = "maint.database.default_cnx";
            public const string DefaultReportingConnectionStringName = "reporting.database.default_cnx";

            public const string PemsConnectionStringNameTemplate = "pems.database.cnx_pattern";
            public const string MaintConnectionStringNameTemplate = "maint.database.cnx_pattern";
            public const string ReportingConnectionStringNameTemplate = "reporting.database.cnx_pattern";

            public const string ConnectionStringSessionVariableName = "PemsConnectionStringName";
            public const string MaintenanceConnectionStringSessionVariableName = "MaintenanceConnectionStringName";
            public const string ReportingConnectionStringSessionVariableName = "ReportingConnectionStringName";
            public const string DefaultStoreGroupName = "_members";
            public const string DefaultMaintenanceGroupName = "_maintenance";
            public const string DefaultGroupNamePrefix = "_";
            public const string RbacConnectionStringName = "Duncan.Membership.Connector";
            public const string UserCityCookieName = "__userCity";
            public const string CityCookieExpirationAppSettingName = "cookies.city.expiration";
            public const string DefaultAzManPassword = "Password";
            public const string ActiveUserAzManColumnName = "IsActive";
            public const int NumPasswordFailuresBeforeLockout = 3;
            public const int NumSecondsToLockout = 3600;
            public const int DaysPasswordValidFor = 90;

            public const string IdleTimeout = "pems.security.timeout";
            public const string IdleTimeoutWarning = "pems.security.timeout.warning";
            public const string IdleTimeoutPolling = "pems.security.polling";

            public const string IzendaLicenseKey = "Izenda.LicKey";
            public const string IzendaReportLocation = "Izenda.ReportPath";
            public const string IzendaReportViewer = "Izenda.ReportViewer";


            public const string RequiredAuthorizationsTemplate = "pems.roles.required.";

        }

        public static class FieldMaintenance
        {
            public const int EventType = 29;
            public const int EventSourceId = 10;
            public const int AutomatedMaintenanceCodeId = 8;
            public const string AutomatedMaintenanceCodeDescription = "Closed from Meter";
            public const string OtherPartName = "000-0000-0000-0";
            public const string WebServiceCreateAlarmName = "pems.webservices.createalarm";
            public const string WebServiceCloseAlarmName = "pems.webservices.closealarm";

        }

        public static class User
        {
            public const int InvalidUserId = -1;
            public const string DefaultMiddleName = "X";
            public const string LastLoginTime = "LastLoginTime";
            public const string OrganizaitonNameField = "OrganizationName";
            public const string SecondaryIDType = "SecondaryIDType";
            public const string SecondaryIDValue = "SecondaryIDValue";
            public const string IsTechnician = "IsTechnician";
            public const string ValueNotSet = "Not set.";
        }

        public static class CollectionRoutes
        {
            public const int InactiveStatus = 0;
            public const int ActiveStatus = 1;
            public const int PendingStatus = 2;
        }

        public static class ViewData
        {
            public const string CurrentCity = "CurrentCity";
            public const string CurrentCityId = "CurrentCityId";
            public const string CurrentCityType = "CurrentCityType";
            public const string CurrentUser = "CurrentUser";
            public const string CurrentAction = "CurrentAction";
            public const string CurrentController = "CurrentController";
            public const string CurrentArea = "CurrentArea";
            public const string CurrentLocale = "CurrentLocale";
            public const string CurrentTimeZoneOffset = "CurrentTimeZoneOffset";
            public const string CurrentLocalTime = "CurrentLocalTime";
            public const string CurrentLocalTimeDisplay = "CurrentLocalTimeDisplay";
            public const string CurrentTimeFormatIs24 = "CurrentTimeFormatIs24";
            public const string CurrentRevision = "CurrentRevision";
            public const string ModelStateStatusKey = "StatusMessage";

            public const string IdleTimeout = "IdleTimeout";
            public const string IdleTimeoutWarning = "IdleTimeoutWarning";
            public const string IdleTimeoutPolling = "IdleTimeoutPolling";

        }

      //Changed By rajesh A
      public static class Reporting
        {
            public const string ReportingInitializedForSession = "_IzendaReportingInitialized_CustomerId";
            public const string VoidReasonSummary = "Complete Reports\\\\Void Reason Summary";
            public const string ViolationSummary = "Complete Reports\\\\Violation Summary";
            public const string ViolationSummaryByOfficer = "Complete Reports\\\\Violation Summary by Officer";
            public const string ViolationSummaryByArea = "Complete Reports\\\\Violation Summary by Area";
            public const string ActivityLogSummary = "Complete Reports\\\\Activity Log Summary";
            public const string DeviceUsageSummary = "Complete Reports\\\\Device Usage Summary";         
        }


        public static class DiscountScheme
        {
            public const int ApprovalEmailTemplateId = 4;
            public const int RejectionEmailTemplateId = 5;
        }

        public static class IdleTimeoutDefaults
        {
            public const string IdleTimeout = "3600";
            public const string IdleTimeoutWarning = "30";
            public const string IdleTimeoutPolling = "300";
        }

        public static class TimeFormats
        {
            public const string timeFormat_HH_MM_SS = "00:00:00";
            public const string timeFormat_HH_MM = "00:00";
            public const string timeFormat_HHH_MM = "000:00";
            public const string timeFormat_HHH_MM_SS = "000:00:00";
            public const string timeFormatToDisplay_HH_MM_SS = "{0:00}:{1:00}:{2:00}";
            public const string timeFormatToDisplay_HH_MM = "{0:00}:{1:00}";
            public const string timeFormatToDisplay_HHH_MM = "{0:000}:{1:00}";
            public const string timeFormatToDisplay_HHH_MM_SS = "{0:000}:{1:00}:{2:00}";
        }


        public static class HiddenFields
        {
            public const string FieldDemandArea = "FieldDemandArea";
            public const string FieldZone = "FieldZone";
            public const string FieldDiscountScheme = "DiscountScheme";
            public const string FieldCG1 = "FieldCG1";
            public const string FieldCG2 = "FieldCG2";
            public const string FieldCG3 = "FieldCG3";
            
        }

       

    }

   #endregion

   #region PEMS Enums
    public static class PEMSEnums
    {

        public enum PEMSEnumsTimeFormats
        {
            timeFormat_HH_MM_SS = 0,
            timeFormat_HH_MM = 1,
            timeFormat_HHH_MM = 2,
            timeFormat_HHH_MM_SS = 3
        };

        public enum PEMSEnumsPmntCardTypes
        {
           SmartCardTypeId = 20,
           PaymentTypeId = 10
        };

    }



   #endregion
}