using Duncan.PEMS.Business.AutoAlarm;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Entities.AutoAlarm;
using Duncan.PEMS.Framework.Controller;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Configuration;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Web.Areas.admin.Controllers
{
    public class AutoAlarmController : PemsController
    {
        //
        // GET: /admin/AutoAlarm/

        public ActionResult AutoAlarmGeneration()
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CurrentCity.Id);
             AutoAlarmModel model = (new AutoAlarmFactory(connStringName)).GetAutoAlarmModel();        
            return View(model);
        }

        public ActionResult AutoAlarmAdd()
        {           
            return View();
        }

        public ActionResult GetAutoAlarmCustomerList([DataSourceRequest]DataSourceRequest request)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CurrentCity.Id);
            var AutoAlarm = (new AutoAlarmFactory(connStringName)).GetAutoAlarmCustomerList();

            IQueryable<AutoAlarmModel> res = AutoAlarm.AsQueryable<AutoAlarmModel>();
            DataSourceResult res1 = res.ToDataSourceResult(request);
            return Json(res1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoAlarmDetails(string CustomerID, string LowBattAlarmCodeMinor, string LowBattAlarmCodeMajor, string LowBattEnabledMeters, string NoCommAlarmCode, string  NoCommEnabledMeters)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CurrentCity.Id);
            AutoAlarmModel model = new AutoAlarmModel();
            model.CustomerIDText = CustomerID;
            model.LowBattAlarmCodeMinor= LowBattAlarmCodeMinor;
            model.LowBattAlarmCodeMinorEvents = (new AutoAlarmFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLowBattAlarmCodeMinorEventCode(CustomerID, LowBattAlarmCodeMinor);
            model.LowBattAlarmCodeMajor=LowBattAlarmCodeMajor;
            model.LowBattAlarmCodeMajorEvents = (new AutoAlarmFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLowBattAlarmCodeMajorEventCode(CustomerID, LowBattAlarmCodeMajor);
            model.LowBattEnabledMeters=LowBattEnabledMeters;
            model.NoCommAlarmCode=NoCommAlarmCode;
            model.NoCommAlarmCodeEvents = (new AutoAlarmFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetNoCommAlarmCodeEventCode(CustomerID, NoCommAlarmCode);
            model.NoCommEnabledMeters = NoCommEnabledMeters;
            return View(model);            
        }


        public ActionResult CustomerIDList()
        {

            var items = (new AutoAlarmFactory(ConfigurationManager.AppSettings[Constants.Security.DefaultPemsConnectionStringName])).CustomerIDListWithoutRP();
           return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEventCode(int CustomerID)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerID);
            var Events = (new AutoAlarmFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEventCode(CustomerID);
            return Json(Events, JsonRequestBehavior.AllowGet);
        }
        public string AddAdminLevelAutoAlarm(string LowBattPollInterval, string LowBattAlarmClearVolt, string LowBattCritLowBattVolt,
                     string LowBattLowBattVolt, string LowBattSampleSinceHrs, string LowBattSsmVoltDiagType, string NoCommPollTime)
        {
           // var items = (new AutoAlarmFactory(ConfigurationManager.AppSettings[Constants.Security.DefaultPemsConnectionStringName])).CustomerIDList();
            var items = SettingsFactory.GetCustomerConnectionStringNames();
            foreach (var item in items)
            {
                var connStringName = item.Text;
                (new AutoAlarmFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).AddAdminLevelAutoAlarm(LowBattPollInterval, LowBattAlarmClearVolt, LowBattCritLowBattVolt,
                         LowBattLowBattVolt, LowBattSampleSinceHrs, LowBattSsmVoltDiagType, NoCommPollTime);
            }
            return "1";

        }

        public string AddAutoAlarm(int CustomerID, string LowBattAlarmCodeMinor,
                        string LowBattAlarmCodeMajor, string LowBattEnabledMeters,
                        string NoCommAlarmCode, string NoCommEnabledMeters)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerID);
            return (new AutoAlarmFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).AddAutoAlarm(CustomerID, LowBattAlarmCodeMinor, LowBattAlarmCodeMajor, LowBattEnabledMeters, NoCommAlarmCode, NoCommEnabledMeters);

        }
        public string UpdateAutoAlarm(int CustomerID, string LowBattAlarmCodeMinor,
                        string LowBattAlarmCodeMajor, string LowBattEnabledMeters,
                        string NoCommAlarmCode, string NoCommEnabledMeters)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerID);
            return (new AutoAlarmFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).UpdateAutoAlarm(CustomerID, LowBattAlarmCodeMinor, LowBattAlarmCodeMajor, LowBattEnabledMeters, NoCommAlarmCode, NoCommEnabledMeters);

        }
    }
}
