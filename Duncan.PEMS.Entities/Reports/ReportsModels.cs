using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duncan.PEMS.Entities.Enumerations;

namespace Duncan.PEMS.Entities.Reports
{
    public class ReportsListViewModel
    {
        public int CustomerId { get; set; }
        public ReportCategory Category { get; set; }
        public string CategoryName { get; set; }
        public List<ReportViewModel> Reports { get; set; }
    }

    public class ReportViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
    }



    public class ReportLinkModel
    {
        public const string SessionActiveReport = "_ActiveReportForSession";
        public const string SessionActiveReportGroup = "_ActiveReportGroupForSession";
        public string Name { get; set; }
        public string Description { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string Parameters { get; set; }
        public string ReportName { get; set; }

        public string ReportViewer { get { return Host + Path + "?" + Parameters; } }
    }


    public class ReportErrorModel
    {
        public string ReturnController { get; set; }
        public string ReturnAction { get; set; }
        public string ErrorDescription { get; set; }
    }


    public class IzendaAdHocModel
    {
        public Dictionary<string, string> Settings { get; set; }
        public Dictionary<string, string> HiddenFilters { get; set; }

        public IzendaAdHocModel()
        {
            Settings = new Dictionary<string, string>();
            HiddenFilters = new Dictionary<string, string>();
        }
    }

}
