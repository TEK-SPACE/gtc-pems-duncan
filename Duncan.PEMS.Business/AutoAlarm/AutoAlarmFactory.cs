using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.AutoAlarm;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Business.AutoAlarm
{
    public class AutoAlarmFactory : BaseFactory
    {
        public AutoAlarmFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        public AutoAlarmModel GetAutoAlarmModel()
        {
            AutoAlarmModel model = new AutoAlarmModel();
            model.LowBattPollInterval = PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.PollInterval") == null ? "" : PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.PollInterval").ValueText;
            model.LowBattAlarmClearVolt = PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.AlarmClearVolt") == null ? "" : PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.AlarmClearVolt").ValueText;
            model.LowBattCritLowBattVolt = PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.CritLowBattVolt") == null ? "" : PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.CritLowBattVolt").ValueText;
            model.LowBattLowBattVolt = PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.LowBattVolt") == null ? "" : PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.LowBattVolt").ValueText;
            model.LowBattSampleSinceHrs = PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.SampleSinceHrs") == null ? "" : PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.SampleSinceHrs").ValueText;
            model.LowBattSsmVoltDiagType = PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.SsmVoltDiagType") == null ? "" : PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.LowBatt.SsmVoltDiagType").ValueText;

            model.NoCommPollTime = PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.NoComm.PollTime") == null ? "" : PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText == "Databus.AutoAlarm.NoComm.PollTime").ValueText;
            GetLowBattSsmVoltDiagTypes(model);
            return model;
        }
        protected void GetLowBattSsmVoltDiagTypes(AutoAlarmModel Model)
        {
            int value;
            int.TryParse(Model.LowBattSsmVoltDiagType, out value);

            List<int> types = new List<int> { 9, 10, 11, 12 };
            Model.LowBattSsmVoltDiagTypes = (from MDT in PemsEntities.MeterDiagnosticTypes
                                             where types.Contains(MDT.ID)
                                             select new SelectListItemWrapper()
                                             {
                                                 Selected = MDT.ID == value,
                                                 Text = MDT.DiagnosticDesc,
                                                 ValueInt = MDT.ID
                                             }).ToList();
        }


        public List<AutoAlarmModel> GetAutoAlarmCustomerList()
        {
            List<AutoAlarmModel> AutoAlarm = new List<AutoAlarmModel>();
            int value;
            var custlist = (from RP in PemsEntities.RipnetProperties
                            where RP.KeyText.Contains("Databus.AutoAlarm.LowBatt.AlarmCodeMinor")
                            select new AutoAlarmModel()
                            {
                                CustomerIDText = RP.KeyText
                            }).ToList();

            foreach (var cust in custlist)
            {
                AutoAlarmModel obj = new AutoAlarmModel();
                obj.CustomerIDText = cust.CustomerIDText.Substring(cust.CustomerIDText.LastIndexOf('.') + 1);// cust.CustomerID;
                if (PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText.Contains("Databus.AutoAlarm.LowBatt.AlarmCodeMinor." + obj.CustomerIDText)) == null)
                    obj.LowBattAlarmCodeMinor = "";
                else
                {
                    int.TryParse(PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText.Contains("Databus.AutoAlarm.LowBatt.AlarmCodeMinor." + obj.CustomerIDText)).ValueText, out value);
                    var cust1 = (from e in PemsEntities.EventCodes
                                where e.EventCode1 == value
                                select new AutoAlarmModel()
                                {
                                    CustomerIDText = e.EventDescVerbose + "(" + SqlFunctions.StringConvert((double)e.EventCode1) + ")",
                                }).FirstOrDefault();
                    obj.LowBattAlarmCodeMinor = cust1.CustomerIDText;// PemsEntities.EventCodes.FirstOrDefault(e => e.EventCode1 == value).EventDescVerbose;
                }


                if (PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText.Contains("Databus.AutoAlarm.LowBatt.AlarmCodeMajor." + obj.CustomerIDText)) == null)
                    obj.LowBattAlarmCodeMajor = "";
                else
                {
                    int.TryParse(PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText.Contains("Databus.AutoAlarm.LowBatt.AlarmCodeMajor." + obj.CustomerIDText)).ValueText, out value);
                    var cust1 = (from e in PemsEntities.EventCodes
                                 where e.EventCode1 == value
                                 select new AutoAlarmModel()
                                 {
                                     CustomerIDText = e.EventDescVerbose + "(" + SqlFunctions.StringConvert((double)e.EventCode1) + ")",
                                 }).FirstOrDefault();
                    obj.LowBattAlarmCodeMajor = cust1.CustomerIDText;//PemsEntities.EventCodes.FirstOrDefault(e => e.EventCode1 == value).EventDescVerbose;

                }

                obj.LowBattEnabledMeters = PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText.Contains("Databus.AutoAlarm.LowBatt.EnabledMeters." + obj.CustomerIDText)) == null ? "" : PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText.Contains("Databus.AutoAlarm.LowBatt.EnabledMeters." + obj.CustomerIDText)).ValueText;

                if (PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText.Contains("Databus.AutoAlarm.NoComm.AlarmCode." + obj.CustomerIDText)) == null)
                    obj.NoCommAlarmCode = "";
                else
                {
                    int.TryParse(PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText.Contains("Databus.AutoAlarm.NoComm.AlarmCode." + obj.CustomerIDText)).ValueText, out value);
                    var cust1 = (from e in PemsEntities.EventCodes
                                 where e.EventCode1 == value
                                 select new AutoAlarmModel()
                                 {
                                     CustomerIDText = e.EventDescVerbose + "(" + SqlFunctions.StringConvert((double)e.EventCode1) + ")",
                                 }).FirstOrDefault();
                    obj.NoCommAlarmCode = cust1.CustomerIDText;//PemsEntities.EventCodes.FirstOrDefault(e => e.EventCode1 == value).EventDescVerbose;

                }
                obj.NoCommEnabledMeters = PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText.Contains("Databus.AutoAlarm.NoComm.EnabledMeters." + obj.CustomerIDText)) == null ? "" : PemsEntities.RipnetProperties.FirstOrDefault(m => m.KeyText.Contains("Databus.AutoAlarm.NoComm.EnabledMeters." + obj.CustomerIDText)).ValueText;
                AutoAlarm.Add(obj);

            }
            return AutoAlarm;

        }
        public List<SelectListItemWrapper> GetLowBattAlarmCodeMinorEventCode(string CustId, string LowBattAlarmCodeMinor)
        {
            var events = new List<AutoAlarmModel>();
            int value;
            int.TryParse(CustId, out value);
            try
            {
              
                
                List<SelectListItemWrapper> list = new List<SelectListItemWrapper>();
               list.Add(new SelectListItemWrapper() { Selected = false, Text = "Select EventCode", ValueInt = -1 });

            
                List<SelectListItemWrapper> list1 = (from ec in PemsEntities.EventCodes
                                                     where ec.CustomerID == value && ec.IsAlarm == true
                                                     select new SelectListItemWrapper()
                                                     {
                                                         Selected = ec.EventDescVerbose + "(" + SqlFunctions.StringConvert((double)ec.EventCode1) + ")" == LowBattAlarmCodeMinor,
                                                         Text = ec.EventDescVerbose + "(" + SqlFunctions.StringConvert((double)ec.EventCode1) + ")",
                                                         ValueInt = ec.EventCode1
                                                     }).ToList();

                foreach (var item in list1)
                {
                    list.Add(item);
                }

                return list;
            }
            catch (Exception ex)
            {

            }
            return null;

        }

        public List<SelectListItemWrapper> GetLowBattAlarmCodeMajorEventCode(string CustId, string LowBattAlarmCodeMajor)
        {
            var events = new List<AutoAlarmModel>();
            int value;
            int.TryParse(CustId, out value);
            try
            {
                //LowBattAlarmCodeMajor = LowBattAlarmCodeMajor.Substring(0, LowBattAlarmCodeMajor.IndexOf('('));
                List<SelectListItemWrapper> list = new List<SelectListItemWrapper>();
                list.Add(new SelectListItemWrapper() { Selected = false, Text = "Select EventCode", ValueInt = -1 });

                List<SelectListItemWrapper> list1 = (from ec in PemsEntities.EventCodes
                                                     where ec.CustomerID == value && ec.IsAlarm == true
                                                     select new SelectListItemWrapper()
                                                     {
                                                         Selected = ec.EventDescVerbose + "(" + SqlFunctions.StringConvert((double)ec.EventCode1) + ")" == LowBattAlarmCodeMajor,
                                                         Text = ec.EventDescVerbose + "(" + SqlFunctions.StringConvert((double)ec.EventCode1) + ")",
                                                         ValueInt = ec.EventCode1
                                                     }).ToList();

                foreach (var item in list1)
                {
                    list.Add(item);
                }

                return list;
            }
            catch (Exception ex)
            {

            }
            return null;

        }

        public List<SelectListItemWrapper> GetNoCommAlarmCodeEventCode(string CustId, string NoCommAlarmCode)
        {
            var events = new List<AutoAlarmModel>();
            int value;
            int.TryParse(CustId, out value);
            try
            {
                //NoCommAlarmCode = NoCommAlarmCode.Substring(0, NoCommAlarmCode.IndexOf('('));
                List<SelectListItemWrapper> list = new List<SelectListItemWrapper>();
                list.Add(new SelectListItemWrapper() { Selected = false, Text = "Select EventCode", ValueInt = -1 });

                List<SelectListItemWrapper> list1 = (from ec in PemsEntities.EventCodes
                                                     where ec.CustomerID == value && ec.IsAlarm == true
                                                     select new SelectListItemWrapper()
                                                     {
                                                         Selected = ec.EventDescVerbose + "(" + SqlFunctions.StringConvert((double)ec.EventCode1) + ")" == NoCommAlarmCode,
                                                         Text = ec.EventDescVerbose + "(" + SqlFunctions.StringConvert((double)ec.EventCode1) + ")",
                                                         ValueInt = ec.EventCode1
                                                     }).ToList();

                foreach (var item in list1)
                {
                    list.Add(item);
                }

                return list;
            }
            catch (Exception ex)
            {

            }
            return null;

        }

        public List<AutoAlarmModel> CustomerIDList()
        {
            var list = new List<AutoAlarmModel>();
            var query = from customers in RbacEntities.CustomerProfiles
                        where customers.CustomerId > 0
                        select customers;

            foreach (var customer in query)
            {
                if (customer.CustomerTypeId == (int)CustomerProfileType.Customer)
                {

                    AutoAlarmModel obj = new AutoAlarmModel();
                    obj.CustomerID = customer.CustomerId;
                    list.Add(obj);

                }
            }
            return list;
        }


        public List<AutoAlarmModel> CustomerIDListWithoutRP()
        {
            var list = new List<AutoAlarmModel>();
            var query = from customers in RbacEntities.CustomerProfiles
                        where customers.CustomerId > 0
                        select customers;

            foreach (var customer in query)
            {
                if (customer.CustomerTypeId == (int)CustomerProfileType.Customer)
                {
                    var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customer.CustomerId);
                    var text = "Databus.AutoAlarm.LowBatt.AlarmCodeMinor." + customer.CustomerId;
                    var ripcust = (new PEMEntities(connStringName)).RipnetProperties.FirstOrDefault(c => c.KeyText.Contains(text));
                    if (ripcust == null)
                    {
                        AutoAlarmModel obj = new AutoAlarmModel();
                        obj.CustomerID = customer.CustomerId;
                        list.Add(obj);
                    }

                }
            }
            return list;
        }


        public List<SelectListItemWrapper> GetEventCode(int CustId)
        {
            var events = new List<AutoAlarmModel>();

            try
            {
                // List<SelectListItemWrapper> list = new List<SelectListItemWrapper>();

                List<SelectListItemWrapper> list = (from ec in PemsEntities.EventCodes
                                                    where ec.CustomerID == CustId && ec.IsAlarm == true
                                                    select new SelectListItemWrapper()
                                                    {
                                                        Selected = false,
                                                        Text = ec.EventDescVerbose + "(" + SqlFunctions.StringConvert((double)ec.EventCode1) + ")",
                                                        ValueInt = ec.EventCode1
                                                    }).ToList();


                //foreach (var item in list1)
                //{
                //    list.Add(item);
                //}

                return list;
            }
            catch (Exception ex)
            {

            }
            return null;

        }


        public void AddAdminLevelAutoAlarm(string LowBattPollInterval, string LowBattAlarmClearVolt, string LowBattCritLowBattVolt,
                       string LowBattLowBattVolt, string LowBattSampleSinceHrs, string LowBattSsmVoltDiagType, string NoCommPollTime)
        {
            try
            {

                var ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains("Databus.AutoAlarm.EventSource"));
                if (ripprop != null)
                {
                    ripprop.ValueText = "0"; //indicates meter
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = "Databus.AutoAlarm.EventSource";
                    DuncanProp.ValueText = "0"; //indicates meter
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }

                ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains("Databus.AutoAlarm.LowBatt.PollInterval"));
                if (ripprop != null)
                {
                    ripprop.ValueText = LowBattPollInterval; //indicates meter
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = "Databus.AutoAlarm.LowBatt.PollInterval";
                    DuncanProp.ValueText = LowBattPollInterval;
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }

                ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains("Databus.AutoAlarm.LowBatt.AlarmClearVolt"));
                if (ripprop != null)
                {
                    ripprop.ValueText = LowBattAlarmClearVolt;
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = "Databus.AutoAlarm.LowBatt.AlarmClearVolt";
                    DuncanProp.ValueText = LowBattAlarmClearVolt;
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }

                ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains("Databus.AutoAlarm.LowBatt.CritLowBattVolt"));
                if (ripprop != null)
                {
                    ripprop.ValueText = LowBattCritLowBattVolt;
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = "Databus.AutoAlarm.LowBatt.CritLowBattVolt";
                    DuncanProp.ValueText = LowBattCritLowBattVolt;
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }

                ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains("Databus.AutoAlarm.LowBatt.LowBattVolt"));
                if (ripprop != null)
                {
                    ripprop.ValueText = LowBattLowBattVolt;
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = "Databus.AutoAlarm.LowBatt.LowBattVolt";
                    DuncanProp.ValueText = LowBattLowBattVolt;
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }

                ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains("Databus.AutoAlarm.LowBatt.SampleSinceHrs"));
                if (ripprop != null)
                {
                    ripprop.ValueText = LowBattSampleSinceHrs;
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = "Databus.AutoAlarm.LowBatt.SampleSinceHrs";
                    DuncanProp.ValueText = LowBattSampleSinceHrs;
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }

                ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains("Databus.AutoAlarm.LowBatt.SsmVoltDiagType"));
                if (ripprop != null)
                {
                    ripprop.ValueText = LowBattSsmVoltDiagType;
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = "Databus.AutoAlarm.LowBatt.SsmVoltDiagType";
                    DuncanProp.ValueText = PemsEntities.MeterDiagnosticTypes.FirstOrDefault(e => e.DiagnosticDesc == LowBattSsmVoltDiagType).ID.ToString();
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }

                ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains("Databus.AutoAlarm.NoComm.PollTime"));
                if (ripprop != null)
                {
                    ripprop.ValueText = NoCommPollTime;
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = "Databus.AutoAlarm.NoComm.PollTime";
                    DuncanProp.ValueText = NoCommPollTime;
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }



            }
            catch (Exception ex)
            {

            }
        }

        public string AddAutoAlarm(int CustomerID, string LowBattAlarmCodeMinor,
                        string LowBattAlarmCodeMajor, string LowBattEnabledMeters,
                        string NoCommAlarmCode, string NoCommEnabledMeters)
        {
            try
            {
                LowBattAlarmCodeMinor = LowBattAlarmCodeMinor.Substring(0, LowBattAlarmCodeMinor.IndexOf('('));
                LowBattAlarmCodeMajor = LowBattAlarmCodeMajor.Substring(0, LowBattAlarmCodeMajor.IndexOf('('));
                NoCommAlarmCode = NoCommAlarmCode.Substring(0, NoCommAlarmCode.IndexOf('('));

                var text = "Databus.AutoAlarm.LowBatt.AlarmCodeMinor." + CustomerID;
                var ripcust = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains(text));
                if (ripcust != null)
                {
                    return "0";
                }

                RipnetProperty DuncanProp = new RipnetProperty();

                DuncanProp.KeyText = "Databus.AutoAlarm.LowBatt.AlarmCodeMinor." + CustomerID;
                DuncanProp.ValueText = PemsEntities.EventCodes.FirstOrDefault(e => e.EventDescVerbose == LowBattAlarmCodeMinor).EventCode1.ToString();
                PemsEntities.RipnetProperties.Add(DuncanProp);
                PemsEntities.SaveChanges();

                RipnetProperty DuncanProp1 = new RipnetProperty();
                DuncanProp1.KeyText = "Databus.AutoAlarm.LowBatt.AlarmCodeMajor." + CustomerID;
                DuncanProp1.ValueText = PemsEntities.EventCodes.FirstOrDefault(e => e.EventDescVerbose == LowBattAlarmCodeMajor).EventCode1.ToString();
                PemsEntities.RipnetProperties.Add(DuncanProp1);
                PemsEntities.SaveChanges();

                RipnetProperty DuncanProp2 = new RipnetProperty();
                DuncanProp2.KeyText = "Databus.AutoAlarm.LowBatt.EnabledMeters." + CustomerID;
                DuncanProp2.ValueText = LowBattEnabledMeters;
                PemsEntities.RipnetProperties.Add(DuncanProp2);
                PemsEntities.SaveChanges();

                RipnetProperty DuncanProp3 = new RipnetProperty();
                DuncanProp3.KeyText = "Databus.AutoAlarm.NoComm.AlarmCode." + CustomerID;
                DuncanProp3.ValueText = PemsEntities.EventCodes.FirstOrDefault(e => e.EventDescVerbose == NoCommAlarmCode).EventCode1.ToString();
                PemsEntities.RipnetProperties.Add(DuncanProp3);
                PemsEntities.SaveChanges();

                RipnetProperty DuncanProp4 = new RipnetProperty();
                DuncanProp4.KeyText = "Databus.AutoAlarm.NoComm.EnabledMeters." + CustomerID;
                DuncanProp4.ValueText = NoCommEnabledMeters;
                PemsEntities.RipnetProperties.Add(DuncanProp4);
                PemsEntities.SaveChanges();


                return "1";
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }


        public string UpdateAutoAlarm(int CustomerID, string LowBattAlarmCodeMinor,
                        string LowBattAlarmCodeMajor, string LowBattEnabledMeters,
                        string NoCommAlarmCode, string NoCommEnabledMeters)
        {
            try
            {
                LowBattAlarmCodeMinor = LowBattAlarmCodeMinor.Substring(0, LowBattAlarmCodeMinor.IndexOf('('));
                LowBattAlarmCodeMajor = LowBattAlarmCodeMajor.Substring(0, LowBattAlarmCodeMajor.IndexOf('('));
                NoCommAlarmCode = NoCommAlarmCode.Substring(0, NoCommAlarmCode.IndexOf('('));
                var text = "Databus.AutoAlarm.LowBatt.AlarmCodeMinor." + CustomerID;
                var ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains(text));

                if (ripprop != null)
                {
                    ripprop.ValueText = PemsEntities.EventCodes.FirstOrDefault(e => e.EventDescVerbose == LowBattAlarmCodeMinor).EventCode1.ToString();
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = text;
                    DuncanProp.ValueText = PemsEntities.EventCodes.FirstOrDefault(e => e.EventDescVerbose == LowBattAlarmCodeMinor).EventCode1.ToString();
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }

                text = "Databus.AutoAlarm.LowBatt.AlarmCodeMajor." + CustomerID;
                ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains(text));

                if (ripprop != null)
                {
                    ripprop.ValueText = PemsEntities.EventCodes.FirstOrDefault(e => e.EventDescVerbose == LowBattAlarmCodeMajor).EventCode1.ToString();
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = text;
                    DuncanProp.ValueText = PemsEntities.EventCodes.FirstOrDefault(e => e.EventDescVerbose == LowBattAlarmCodeMajor).EventCode1.ToString();
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }


                text = "Databus.AutoAlarm.LowBatt.EnabledMeters." + CustomerID;
                ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains(text));

                if (ripprop != null)
                {
                    ripprop.ValueText = LowBattEnabledMeters;
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = text;
                    DuncanProp.ValueText = LowBattEnabledMeters;
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }

                text = "Databus.AutoAlarm.NoComm.AlarmCode." + CustomerID;
                ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains(text));

                if (ripprop != null)
                {
                    ripprop.ValueText = PemsEntities.EventCodes.FirstOrDefault(e => e.EventDescVerbose == NoCommAlarmCode).EventCode1.ToString(); ;
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = text;
                    DuncanProp.ValueText = PemsEntities.EventCodes.FirstOrDefault(e => e.EventDescVerbose == NoCommAlarmCode).EventCode1.ToString();
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }

                text = "Databus.AutoAlarm.NoComm.EnabledMeters." + CustomerID;
                ripprop = PemsEntities.RipnetProperties.FirstOrDefault(c => c.KeyText.Contains(text));

                if (ripprop != null)
                {
                    ripprop.ValueText = NoCommEnabledMeters;
                    PemsEntities.SaveChanges();
                }
                else
                {
                    RipnetProperty DuncanProp = new RipnetProperty();
                    DuncanProp.KeyText = text;
                    DuncanProp.ValueText = NoCommEnabledMeters;
                    PemsEntities.RipnetProperties.Add(DuncanProp);
                    PemsEntities.SaveChanges();
                }
                return "1";


            }
            catch (Exception ex)
            {
                return "-1";
            }
        }

    }

}