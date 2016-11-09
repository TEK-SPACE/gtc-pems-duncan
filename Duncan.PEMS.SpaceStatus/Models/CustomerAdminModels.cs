using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


using Duncan.PEMS.SpaceStatus.DataShapes;
using Duncan.PEMS.SpaceStatus.DataSuppliers;
using Duncan.PEMS.SpaceStatus.Helpers;
using Duncan.PEMS.SpaceStatus.UtilityClasses;

namespace Duncan.PEMS.SpaceStatus.Models
{
    // DEV NOTE: Refer to http://www.dotnetcurry.com/ShowArticle.aspx?ID=490 for good info on Linq to XML

    public class RegulatedHoursGroup
    {
        public int RegulatedHoursGroupID { get; set; }
        public string Description { get; set; }

        public bool ShouldSerializeCID() { return CID.HasValue; } // For XML Serialization purposes
        public int? CID { get; set; }

        public bool ShouldSerializeAID() { return AID.HasValue; }
        public int? AID { get; set; }

        public bool ShouldSerializeMID() { return MID.HasValue; }
        public int? MID { get; set; }
        
        //public int? BayID{get;set;}
        //public int ScopeType {get;set;}

        public RegulatedHoursGroup()
        {
            this.RegulatedHoursGroupID = -1;
            this.Description = "";
            this.CID = -1;
            this.AID = -1;
            this.MID = null;

            this.Details = new List<RegulatedHoursDetail>();
        }

        public RegulatedHoursGroup(int id, string description, int? cid, int? aid, int? mid)
        {
            this.RegulatedHoursGroupID = id;
            this.Description = description;
            this.CID = cid;
            this.AID = aid;
            this.MID = mid;

            this.Details = new List<RegulatedHoursDetail>();
        }

        public List<RegulatedHoursDetail> Details { get; set; }

        public string GetScopeDisplayValue()
        {
            string result = string.Empty;

            // First we must resolve the customer
            /*CustomerLogic customer = CustomerLogic.CustomerManager.GetCustomerLogicByCID(CID.ToString());*/
            CustomerConfig customerCfg = CustomerLogic.CustomerManager.GetDTOForCID(CID.ToString());

            if ((MID.HasValue) && ((int)MID != -1))
            {
                result = "Meter - " + MID.ToString();
            }
            else if ((AID.HasValue) && ((int)AID != -1))
            {
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerCfg))
                {
                    if (asset.AreaID == AID.Value)
                    {
                        if (!string.IsNullOrEmpty(asset.AreaName))
                            result = "Area - " + asset.AreaName;
                        else
                            result = "Area - " + asset.AreaID.ToString();
                        break;
                    }
                }
            }
            else
            {
                result = "Site-wide";
            }


            return result;
        }

        public string GetHTMLForDetails(DayOfWeek targetDayOfWeek)
        {
            StringBuilder sbResult = new StringBuilder();
        
            // Get a sorted list of each detail applicable to the passed day of the week
            List<RegulatedHoursDetail> detailsForDay = new List<RegulatedHoursDetail>();
            foreach (RegulatedHoursDetail nextDetail in this.Details)
            {
                if (nextDetail.DayOfWeek == (int)targetDayOfWeek)
                    detailsForDay.Add(nextDetail);
            }
            detailsForDay.Sort(RegulatedHoursDetailLogicalComparer.Default);

            BaseTagHelper tbTable = null;
            BaseTagHelper tbRow = null;
            BaseTagHelper tbCol = null;

            tbTable = new BaseTagHelper("table");
            tbRow = new BaseTagHelper("tr");
            tbTable.Children.Add(tbRow);
            tbRow.AddCssClass("yellow");

            tbCol = new BaseTagHelper("th");
            tbRow.Children.Add(tbCol);
            tbCol.SetInnerText("Start");

            tbCol = new BaseTagHelper("th");
            tbRow.Children.Add(tbCol);
            tbCol.SetInnerText("End");

            tbCol = new BaseTagHelper("th");
            tbRow.Children.Add(tbCol);
            tbCol.SetInnerText("Max Stay (Minutes)");

            tbCol = new BaseTagHelper("th");
            tbRow.Children.Add(tbCol);
            tbCol.SetInnerText("Type");

            // Build HTML for each detail
            foreach (RegulatedHoursDetail detail in detailsForDay)
            {
                tbRow = new BaseTagHelper("tr");
                tbTable.Children.Add(tbRow);

                tbCol = new BaseTagHelper("td");
                tbRow.Children.Add(tbCol);
                tbCol.SetInnerText(detail.StartTime.ToString("hh:mm:ss tt"));

                tbCol = new BaseTagHelper("td");
                tbRow.Children.Add(tbCol);
                tbCol.SetInnerText(detail.EndTime.ToString("hh:mm:ss tt"));

                tbCol = new BaseTagHelper("td");
                tbRow.Children.Add(tbCol);
                tbCol.SetInnerText(detail.MaxStayMinutes.ToString());

                tbCol = new BaseTagHelper("td");
                tbRow.Children.Add(tbCol);
                tbCol.SetInnerText(detail.Type.ToString());
            }

            sbResult.AppendLine(tbTable.ToString());
            return sbResult.ToString();
        }
    }

    public class RegulatedHoursGroupPredicate
    {
        public enum ParamDisambiguation {ParamIsGroupID};

        private int? _CID;
        private int? _AID;
        private int? _MID;
        private int _GroupID = -1;

        public RegulatedHoursGroupPredicate(int? cid, int? aid, int? mid)
        {
            _CID = cid;
            _AID = aid;
            _MID = mid;
        }

        public RegulatedHoursGroupPredicate(ParamDisambiguation disambiguation, int groupID)
        {
            // Added a ParamDisambiguation parameter, otherwise this constructor might conflict
            // with the other one that allows nullable ints for customer, area, and meter
            if (disambiguation == ParamDisambiguation.ParamIsGroupID)
                _GroupID = groupID;
        }

        public bool CompareScope(RegulatedHoursGroup pObject)
        {
            bool cidIsSame = false;
            bool aidIsSame = false;
            bool midIsSame = false;

            // Compare MeterID (Null and -1 will be treated as the same value)
            if ((pObject.MID == null) || (pObject.MID.HasValue == false) || (pObject.MID.Value == -1))
            {
                if ((this._MID == null) || (this._MID.HasValue == false) || (this._MID.Value == -1))
                    midIsSame = true;
            }
            else if (pObject.MID.HasValue)
            {
                if ((this._MID.HasValue) && (pObject.MID.Value == this._MID.Value))
                    midIsSame = true;
            }

            // Compare AreaID (Null and -1 will be treated as the same value)
            if ((pObject.AID == null) || (pObject.AID.HasValue == false) || (pObject.AID.Value == -1))
            {
                if ((this._AID == null) || (this._AID.HasValue == false) || (this._AID.Value == -1))
                    aidIsSame = true;
            }
            else if (pObject.AID.HasValue)
            {
                if ((this._AID.HasValue) && (pObject.AID.Value == this._AID.Value))
                    aidIsSame = true;
            }

            // Compare CustomerID (Null and -1 will be treated as the same value)
            if ((pObject.CID == null) || (pObject.CID.HasValue == false) || (pObject.CID.Value == -1))
            {
                if ((this._CID == null) || (this._CID.HasValue == false) || (this._CID.Value == -1))
                    cidIsSame = true;
            }
            else if (pObject.CID.HasValue)
            {
                if ((this._CID.HasValue) && (pObject.CID.Value == this._CID.Value))
                    cidIsSame = true;
            }

            if ((cidIsSame) && (aidIsSame) && (midIsSame))
                return true;
            else
                return false;
        }

        public bool CompareCustomerID(RegulatedHoursGroup pObject)
        {
            bool cidIsSame = false;

            // Compare CustomerID (Null and -1 will be treated as the same value)
            if ((pObject.CID == null) || (pObject.CID.HasValue == false) || (pObject.CID.Value == -1))
            {
                if ((this._CID == null) || (this._CID.HasValue == false) || (this._CID.Value == -1))
                    cidIsSame = true;
            }
            else if (pObject.CID.HasValue)
            {
                if ((this._CID.HasValue) && (pObject.CID.Value == this._CID.Value))
                    cidIsSame = true;
            }

            if (cidIsSame)
                return true;
            else
                return false;
        }

        public bool CompareGroupID(RegulatedHoursGroup pObject)
        {
            return this._GroupID == pObject.RegulatedHoursGroupID;
        }
    }

    public class RegulatedHoursDetail
    {
        public RegulatedHoursDetail()
        {
            this.RegulatedHoursDetailID = -1;
            this.RegulatedHoursGroupID = -1;

            this.DayOfWeek = 0;
            this.StartTime = DateTime.MinValue;
            this.EndTime = DateTime.MinValue;
            this.Type = "Regulated";
            this.MaxStayMinutes = 0;
        }

        public RegulatedHoursDetail(int regulatedHoursDetailID, int regulatedHoursGroupID, int dayOfWeek, DateTime startTime, DateTime endTime, string type, int maxStayMinutes)
        {
            this.RegulatedHoursDetailID = regulatedHoursDetailID;
            this.RegulatedHoursGroupID = regulatedHoursGroupID;
            this.DayOfWeek = dayOfWeek;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Type = type; 
            this.MaxStayMinutes = maxStayMinutes;
        }

        public int RegulatedHoursDetailID { get; set; }
        public int RegulatedHoursGroupID { get; set; }

        // DayOfWeek as integer.  Sunday = 0
        [Required(ErrorMessage = "Day of Week is required")]
        public int DayOfWeek { get; set; }

        [Required(ErrorMessage = "Start Time is required")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End Time is required")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Regulation type is required")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Max Stay Minutes is required")]
        public int MaxStayMinutes { get; set; }
    }

    public sealed class RegulatedHoursDetailLogicalComparer : System.Collections.Generic.IComparer<RegulatedHoursDetail>
    {
        private static readonly System.Collections.Generic.IComparer<RegulatedHoursDetail> _default = new RegulatedHoursDetailLogicalComparer();

        public RegulatedHoursDetailLogicalComparer()
        {
        }

        public static System.Collections.Generic.IComparer<RegulatedHoursDetail> Default
        {
            get { return _default; }
        }

        public int Compare(RegulatedHoursDetail s1, RegulatedHoursDetail s2)
        {
            // First level of comparison is the day of week
            int result = s1.DayOfWeek.CompareTo(s2.DayOfWeek);
            if (result != 0) 
                return result;

            // Seconday level of comparison is the start time
            result = string.CompareOrdinal(s1.StartTime.ToString("hh:mm:ss tt"), s2.StartTime.ToString("hh:mm:ss tt"));
            return result;
        }
    }



    public interface IRegulatedHoursGroupRepository
    {
        IEnumerable<RegulatedHoursGroup> GetGroups(int? cid);
        List<RegulatedHoursGroup> GetGroupsWithSameScope(int? cid, int? aid, int? mid);
        RegulatedHoursGroup GetBestGroupForMeter(int? cid, int? aid, int? mid);
        RegulatedHoursGroup GetGroupByID(int regulatedHoursGroupID);
        void InsertGroup(RegulatedHoursGroup group);
        void DeleteGroup(int regulatedHoursGroupID);
        void EditGroup(RegulatedHoursGroup group);
    }

    public class RegulatedHoursGroupRepository : IRegulatedHoursGroupRepository
    {
        static public RegulatedHoursGroupRepository Repository = null;

        // For thread-safety, use _Locker when accessing _AllGroups or _RegulationRuleData
        private ReaderWriterLockSlim _Locker = new ReaderWriterLockSlim();
        private List<RegulatedHoursGroup> _AllGroups = null;
        private XDocument _RegulationRuleData = null;

        private int? SafeGetNullableIntFromXElement(XElement element)
        {
            try
            {
                if (element == null)
                    return null;
                else if (string.IsNullOrEmpty(element.Value))
                    return null;
                else
                    return (int?)element;
            }
            catch (Exception ex)
            {
                // Attempt to log what failed to convert, but then return null response to caller
                try
                {
                    Logging.AddTextToGenericLog(Logging.LogLevel.Error,
                        "Failed to parse node as NullableInt. Node: " + element.Name.ToString() +
                        ", Type: " + element.NodeType.ToString() + ", Value: " + element.Value +
                        ", Exception: " + ex.ToString(),
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
                catch { }

                return null;
            }
        }

        private int SafeGetIntFromXElement(XElement element)
        {
            // Try to get an integer from node value.  If fails, we will return -1
            try
            {
                if (element == null)
                    return -1;
                else if (string.IsNullOrEmpty(element.Value))
                    return -1;
                else
                    return (int)element;
            }
            catch (Exception ex)
            {
                // Attempt to log what failed to convert, but then return null response to caller
                try
                {
                    Logging.AddTextToGenericLog(Logging.LogLevel.Error,
                        "Failed to parse node as Integer. Node: " + element.Name.ToString() +
                        ", Type: " + element.NodeType.ToString() + ", Value: " + element.Value +
                        ", Exception: " + ex.ToString(),
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
                catch { }

                return -1;
            }
        }

        private DateTime SafeGetDateTimeFromXElement(XElement element)
        {
            // Try to get a DateTime from node value.  If fails, we will return DateTime.MinValue
            try
            {
                if (element == null)
                    return DateTime.MinValue;
                else if (string.IsNullOrEmpty(element.Value))
                    return DateTime.MinValue;
                else
                    return (DateTime)element;
            }
            catch (Exception ex)
            {
                // Attempt to log what failed to convert, but then return DateTime.MinValue response to caller
                try
                {
                    Logging.AddTextToGenericLog(Logging.LogLevel.Error,
                        "Failed to parse node as DateTime. Node: " + element.Name.ToString() +
                        ", Type: " + element.NodeType.ToString() + ", Value: " + element.Value +
                        ", Exception: " + ex.ToString(),
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
                catch { }

                return DateTime.MinValue;
            }
        }

        private string SafeGetStringFromXElement(XElement element)
        {
            try
            {
                if (element == null)
                    return string.Empty;
                else
                    return element.Value;
            }
            catch (Exception ex)
            {
                // Attempt to log what failed, then return empty string response to caller
                try
                {
                    Logging.AddTextToGenericLog(Logging.LogLevel.Error,
                        "Failed to parse node as String. Node: " + element.Name.ToString() +
                        ", Type: " + element.NodeType.ToString() + ", Value: " + element.Value +
                        ", Exception: " + ex.ToString(),
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
                catch { }

                return string.Empty;
            }
        }

        private List<RegulatedHoursDetail> SafeGetDetailsListFromXElement(XElement element)
        {
            // Upon failure, write exception to log, and return empty list to caller
            try
            {
                if (element == null)
                    return new List<RegulatedHoursDetail>();

                List<RegulatedHoursDetail> result =
                    (from ste in element.Element("details").Elements("detail")
                     select new RegulatedHoursDetail
                     {
                         RegulatedHoursDetailID = SafeGetIntFromXElement(ste.Element("regulatedhoursdetailid")),
                         RegulatedHoursGroupID = SafeGetIntFromXElement(ste.Element("regulatedhoursgroupid")),
                         DayOfWeek = SafeGetIntFromXElement(ste.Element("dayofweek")),
                         StartTime = SafeGetDateTimeFromXElement(ste.Element("starttime")),
                         EndTime = SafeGetDateTimeFromXElement(ste.Element("endtime")),
                         MaxStayMinutes = SafeGetIntFromXElement(ste.Element("maxstayminutes")),
                         Type = SafeGetStringFromXElement(ste.Element("type"))
                     }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                // Attempt to log what failed, then return empty list to caller
                try
                {
                    Logging.AddTextToGenericLog(Logging.LogLevel.Error,
                        "Failed to parse node as List<RegulatedHoursDetail>. Node: " + element.Name.ToString() +
                        ", Type: " + element.NodeType.ToString() + ", Value: " + element.Value +
                        ", Exception: " + ex.ToString(),
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
                catch { }

                return new List<RegulatedHoursDetail>();
            }
        }

        public RegulatedHoursGroupRepository()
        {
            // Note avoid using "HttpContext.Current.Server.MapPath("~/App_Data/RegulationRules.xml")" because HttpContext isn't always available during app startup (i.e. IIS 7 in integrated mode)
            string rulesFilename = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data\\RegulationRules.xml");

            // We need an exclusive lock while loading the XML
            try
            {
                // Load the XML document and parse it into the _AllGroups object
                _Locker.EnterWriteLock();

                if (System.IO.File.Exists(rulesFilename))
                {
                    // Load the rules files
                    _RegulationRuleData = XDocument.Load(rulesFilename);
                }
                else
                {
                    Logging.AddTextToGenericLog(Logging.LogLevel.Warning,
                        "No 'RegulationRules.xml' file found, so a new one will be created",
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                    // Failed, so use a blank document as default
                    _RegulationRuleData = new XDocument();
                    // Add root node to XML
                    _RegulationRuleData.AddFirst(new XElement("Groups"));
                    // Persist the changes back to the XML
                    _RegulationRuleData.Save(rulesFilename);
                }
            }
            catch (Exception ex)
            {
                Logging.AddTextToGenericLog(Logging.LogLevel.Warning,
                    "Failed to load 'RegulationRules.xml', so a new one will be created. Exception: " + ex.ToString(),
                    MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                // Failed, so use a blank document as default
                _RegulationRuleData = new XDocument();
                // Add root node to XML
                _RegulationRuleData.AddFirst(new XElement("Groups"));
                // Persist the changes back to the XML
                _RegulationRuleData.Save(rulesFilename);
            }
            finally
            {
                // Release the exclusive lock
                if (_Locker.IsWriteLockHeld)
                    _Locker.ExitWriteLock();
            }

            // Now deserialize the XML -- Note this call is not inside of our lock, because it acquires its own lock, and nested locks are not allowed
            BuildMemCacheFromXDocument();
        }

        private void BuildMemCacheFromXDocument()
        {
            try
            {
                // We need an exclusive lock to modify our array
                _Locker.EnterWriteLock();

                // Deserialize the XML document into an array of RegulatedHoursGroup objects
                _AllGroups = new List<RegulatedHoursGroup>();
                try
                {
                    // Add root node to XML if not already present
                    if (_RegulationRuleData.Root == null)
                    {
                        _RegulationRuleData.AddFirst(new XElement("Groups"));
                    }


                    _AllGroups =
                        (from cntry in _RegulationRuleData.Element("Groups").Elements("Group")
                         select new RegulatedHoursGroup
                         {
                             RegulatedHoursGroupID = SafeGetIntFromXElement(cntry.Element("regulatedhoursgroupid")),
                             Description = SafeGetStringFromXElement(cntry.Element("description")),
                             CID = SafeGetNullableIntFromXElement(cntry.Element("cid")),
                             AID = SafeGetNullableIntFromXElement(cntry.Element("aid")),
                             MID = SafeGetNullableIntFromXElement(cntry.Element("mid")),
                             Details = SafeGetDetailsListFromXElement(cntry)
                         }).ToList();

                    // Sort the details of each list
                    foreach (RegulatedHoursGroup nextGroup in _AllGroups)
                    {
                        nextGroup.Details.Sort(RegulatedHoursDetailLogicalComparer.Default);
                    }
                }
                catch (Exception ex)
                {
                    // Log this error, then rethrow so caller does their own exception handling too
                    Logging.AddTextToGenericLog(Logging.LogLevel.Error,
                        "Failed to parse Regulation Rules from XML file. Exception: " + ex.ToString(),
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                    // Rethrow exception so caller can handle it too
                    _AllGroups = new List<RegulatedHoursGroup>();
                    throw;
                }
            }
            finally
            {
                // Release the exclusive lock
                if (_Locker.IsWriteLockHeld)
                    _Locker.ExitWriteLock();
            }
        }

        // Return a list of all RegulationRules for a given customer
        public IEnumerable<RegulatedHoursGroup> GetGroups(int? cid)
        {
            try
            {
                // Use a read lock for this operation (unless the thread has an active write lock -- in which case we consider the equivalent of read/write)
                if (_Locker.IsWriteLockHeld == false)
                    _Locker.EnterReadLock();

                // Find matches based on just the Customer ID
                RegulatedHoursGroupPredicate predicate = new RegulatedHoursGroupPredicate(cid, null, null);
                List<RegulatedHoursGroup> groupsForCustomerWithSameScope = _AllGroups.FindAll(predicate.CompareCustomerID);
                return groupsForCustomerWithSameScope;
            }
            finally
            {
                // Release the read lock (if it was acquired)           
                if (_Locker.IsReadLockHeld)
                    _Locker.ExitReadLock();
            }
        }

        public List<RegulatedHoursGroup> GetGroupsWithSameScope(int? cid, int? aid, int? mid)
        {
            try
            {
                // Use a read lock for this operation (unless the thread has an active write lock -- in which case we consider the equivalent of read/write)
                if (_Locker.IsWriteLockHeld == false)
                    _Locker.EnterReadLock();

                RegulatedHoursGroupPredicate predicate = new RegulatedHoursGroupPredicate(cid, aid, mid);
                List<RegulatedHoursGroup> groupsWithSameScope = _AllGroups.FindAll(predicate.CompareScope);
                return groupsWithSameScope;
            }
            finally
            {
                // Release the read lock (if it was acquired)           
                if (_Locker.IsReadLockHeld)
                    _Locker.ExitReadLock();
            }
        }

        public RegulatedHoursGroup GetBestGroupForMeter(int? cid, int? aid, int? mid)
        {
            try
            {
                // Use a read lock for this operation (unless the thread has an active write lock -- in which case we consider the equivalent of read/write)
                if (_Locker.IsWriteLockHeld == false)
                    _Locker.EnterReadLock();

                // Start by trying to find for all 3 scope elements together
                RegulatedHoursGroupPredicate predicate = new RegulatedHoursGroupPredicate(cid, aid, mid);
                List<RegulatedHoursGroup> groupsWithSameScope = _AllGroups.FindAll(predicate.CompareScope);

                // If no group found at meter-level, then we will try at the area-level
                if ((groupsWithSameScope == null) || (groupsWithSameScope.Count == 0))
                {
                    predicate = new RegulatedHoursGroupPredicate(cid, aid, null);
                    groupsWithSameScope = _AllGroups.FindAll(predicate.CompareScope);
                }

                // If no group found at area-level, then we will try at the customer-level
                if ((groupsWithSameScope == null) || (groupsWithSameScope.Count == 0))
                {
                    predicate = new RegulatedHoursGroupPredicate(cid, null, null);
                    groupsWithSameScope = _AllGroups.FindAll(predicate.CompareScope);
                }

                if ((groupsWithSameScope == null) || (groupsWithSameScope.Count == 0))
                    return null;
                else
                    return groupsWithSameScope[0];
            }
            finally
            {
                // Release the read lock (if it was acquired)           
                if (_Locker.IsReadLockHeld)
                    _Locker.ExitReadLock();
            }
        }

        public RegulatedHoursGroup GetGroupByID(int regulatedHoursGroupID)
        {
            try
            {
                // Use a read lock for this operation (unless the thread has an active write lock -- in which case we consider the equivalent of read/write)
                if (_Locker.IsWriteLockHeld == false)
                    _Locker.EnterReadLock();

                // DEBUG: We could either use a predicate, or for this simple comparison,
                // we can supply a lambda expression which makes for an in-line predicate...
                /*
                RegulatedHoursGroupPredicate predicate = new RegulatedHoursGroupPredicate( RegulatedHoursGroupPredicate.ParamDisambiguation.ParamIsGroupID, regulatedHoursGroupID);
                RegulatedHoursGroup result = _AllGroups.Find(predicate.CompareGroupID);
                return result;
                */

                return _AllGroups.Find(item => item.RegulatedHoursGroupID == regulatedHoursGroupID);
            }
            finally
            {
                // Release the read lock (if it was acquired)
                if (_Locker.IsReadLockHeld)
                    _Locker.ExitReadLock();
            }
        }

        // Insert Record
        public void InsertGroup(RegulatedHoursGroup group)
        {
            try
            {
                // We need an exclusive lock to modify our array
                _Locker.EnterWriteLock();

                try
                {
                    // Add root node to XML if not already present
                    if (_RegulationRuleData.Root == null)
                    {
                        _RegulationRuleData.AddFirst(new XElement("Groups"));
                    }

                    // Before inserting this group, we should see if there are any existing groups with the same scope. 
                    // If so, we will delete them since this should be a treated as a replacement of any existing rules
                    // that are for the same scope
                    RegulatedHoursGroupPredicate predicate = new RegulatedHoursGroupPredicate(group.CID, group.AID, group.MID);
                    List<RegulatedHoursGroup> groupsWithSameScope = _AllGroups.FindAll(predicate.CompareScope);
                    if (groupsWithSameScope.Count > 0)
                    {
                        foreach (RegulatedHoursGroup groupToRemove in groupsWithSameScope)
                        {
                            // Remove from the data store (XML in this case, but could also be a database)
                            _RegulationRuleData.Root.Elements("Group").Where(i => (int)i.Element("regulatedhoursgroupid") == groupToRemove.RegulatedHoursGroupID).Remove();

                            // Also remove from the in-memory cache
                            _AllGroups.Remove(groupToRemove);
                        }
                    }

                    // Assign a new ID to the group (AutoInc equivalent)
                    group.RegulatedHoursGroupID = (int)(from b in _RegulationRuleData.Descendants("Group") orderby (int)b.Element("regulatedhoursgroupid") descending select (int)b.Element("regulatedhoursgroupid")).FirstOrDefault() + 1;

                    // Create a new node for the group
                    XElement groupNode = null;
                    groupNode = new XElement("Group",
                            new XElement("regulatedhoursgroupid", group.RegulatedHoursGroupID),
                            new XElement("description", group.Description),
                            new XElement("cid", group.CID),
                            new XElement("aid", group.AID),
                            new XElement("mid", group.MID));
                    _RegulationRuleData.Root.Add(groupNode);

                    // Sort the children details
                    group.Details.Sort(RegulatedHoursDetailLogicalComparer.Default);

                    // Create a details node for the group, then add child nodes 
                    XElement detailsRoot = null;
                    detailsRoot = new XElement("details");
                    groupNode.Add(detailsRoot);
                    foreach (RegulatedHoursDetail detail in group.Details)
                    {
                        // Assign the correct parent ID to this child
                        detail.RegulatedHoursGroupID = group.RegulatedHoursGroupID;

                        // Assign a new ID to the child (AutoInc equivalent)
                        detail.RegulatedHoursDetailID = (int)(from b in _RegulationRuleData.Descendants("detail") orderby (int)b.Element("regulatedhoursdetailid") descending select (int)b.Element("regulatedhoursdetailid")).FirstOrDefault() + 1;

                        detailsRoot.Add(new XElement("detail", new XElement("regulatedhoursdetailid", detail.RegulatedHoursDetailID),
                            new XElement("regulatedhoursgroupid", detail.RegulatedHoursGroupID),
                            new XElement("dayofweek", detail.DayOfWeek),
                            new XElement("starttime", detail.StartTime.ToString("hh:mm:ss tt")),
                            new XElement("endtime", detail.EndTime.ToString("hh:mm:ss tt")),
                            new XElement("type", detail.Type),
                            new XElement("maxstayminutes", detail.MaxStayMinutes)));
                    }

                    // Persist the changes back to the XML
                    _RegulationRuleData.Save(HttpContext.Current.Server.MapPath("~/App_Data/RegulationRules.xml"));

                    // Also add the group to our in-memory cache
                    _AllGroups.Add(group);
                    group.Details.Sort(RegulatedHoursDetailLogicalComparer.Default);
                }
                catch (Exception ex)
                {
                    Logging.AddTextToGenericLog(Logging.LogLevel.Error, ex.ToString(),
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
            }
            finally
            {
                // Release the exclusive lock
                if (_Locker.IsWriteLockHeld)
                    _Locker.ExitWriteLock();
            }
        }

        // Delete Record
        public void DeleteGroup(int regulatedHoursGroupID)
        {
            try
            {
                // We need an exclusive lock to modify our array
                _Locker.EnterWriteLock();

                // Remove from the data store (XML in this case, but could also be a database)
                _RegulationRuleData.Root.Elements("Group").Where(i => (int)i.Element("regulatedhoursgroupid") == regulatedHoursGroupID).Remove();
                _RegulationRuleData.Save(HttpContext.Current.Server.MapPath("~/App_Data/RegulationRules.xml"));

                // Also remove from the in-memory cache
                RegulatedHoursGroup groupToRemove = this.GetGroupByID(regulatedHoursGroupID);
                if (groupToRemove != null)
                    _AllGroups.Remove(groupToRemove);
            }
            finally
            {
                // Release the exclusive lock
                if (_Locker.IsWriteLockHeld)
                    _Locker.ExitWriteLock();
            }
        }

        // Edit Record
        public void EditGroup(RegulatedHoursGroup group)
        {
            try
            {
                // We need an exclusive lock to modify our array
                _Locker.EnterWriteLock();

                // Before saving this group, we should see if there are any existing groups
                // with the same scope (but a different group ID, of course!). If so, we will delete them 
                // since this should be a treated as a replacement of existing rules.
                RegulatedHoursGroupPredicate predicate = new RegulatedHoursGroupPredicate(group.CID, group.AID, group.MID);
                List<RegulatedHoursGroup> groupsWithSameScope = _AllGroups.FindAll(predicate.CompareScope);
                if (groupsWithSameScope.Count > 0)
                {
                    foreach (RegulatedHoursGroup groupToRemove in groupsWithSameScope)
                    {
                        // Delete this group, but only if its NOT for the same GroupID that we are editing!
                        if (groupToRemove.RegulatedHoursGroupID != group.RegulatedHoursGroupID)
                        {
                            _RegulationRuleData.Root.Elements("Group").Where(i => (int)i.Element("regulatedhoursgroupid") == groupToRemove.RegulatedHoursGroupID).Remove();

                            // Also remove from the in-memory cache
                            _AllGroups.Remove(groupToRemove);
                        }
                    }
                }

                // Find the existing group node that we will be updating
                XElement groupNode = _RegulationRuleData.Root.Elements("Group").Where(i => (int)i.Element("regulatedhoursgroupid") == group.RegulatedHoursGroupID).FirstOrDefault();

                groupNode.SetElementValue("description", group.Description);
                groupNode.SetElementValue("cid", group.CID);
                groupNode.SetElementValue("aid", group.AID);
                groupNode.SetElementValue("mid", group.MID);

                // Sort the children details
                group.Details.Sort(RegulatedHoursDetailLogicalComparer.Default);

                // Try to get existing "details" child node
                XElement detailsRoot = groupNode.Element("details");
                if (detailsRoot != null)
                {
                    // If there is an existing details child node, we will delete it, because we will completely recreate it
                    detailsRoot.Remove();
                    detailsRoot = null;
                }

                // Create a new detail node, then populate with all children
                detailsRoot = new XElement("details");
                groupNode.Add(detailsRoot);

                foreach (RegulatedHoursDetail detail in group.Details)
                {
                    // We need to get a new ID for each detail, and also assign the correct parent ID
                    detail.RegulatedHoursGroupID = group.RegulatedHoursGroupID;
                    detail.RegulatedHoursDetailID = (int)(from b in _RegulationRuleData.Descendants("detail") orderby (int)b.Element("regulatedhoursdetailid") descending select (int)b.Element("regulatedhoursdetailid")).FirstOrDefault() + 1;

                    detailsRoot.Add(new XElement("detail", new XElement("regulatedhoursdetailid", detail.RegulatedHoursDetailID),
                        new XElement("regulatedhoursgroupid", detail.RegulatedHoursGroupID),
                        new XElement("dayofweek", detail.DayOfWeek),
                        new XElement("starttime", detail.StartTime.ToString("hh:mm:ss tt")),
                        new XElement("endtime", detail.EndTime.ToString("hh:mm:ss tt")),
                        new XElement("type", detail.Type),
                        new XElement("maxstayminutes", detail.MaxStayMinutes)));
                }

                // Persist changes back to the XML
                _RegulationRuleData.Save(HttpContext.Current.Server.MapPath("~/App_Data/RegulationRules.xml"));

                // Also update the in-memory cache
                RegulatedHoursGroup existingGroup = this.GetGroupByID(group.RegulatedHoursGroupID);
                if (existingGroup != null)
                    _AllGroups.Remove(existingGroup);
                _AllGroups.Add(group);
                group.Details.Sort(RegulatedHoursDetailLogicalComparer.Default);
            }
            finally
            {
                // Release the exclusive lock
                if (_Locker.IsWriteLockHeld)
                    _Locker.ExitWriteLock();
            }
        }
    }

    


    
    public class InactiveSpace
    {
        [DisplayName("Customer ID")]
        [DropDownList("CustomerChoices", "DataValue", "DataText" /*, "[Select Customer]"*/)]
        [RenderMode(RenderMode.EditModeOnly)]
        public int CustomerID { get; set; }

        [DisplayName("Customer")]
        [RenderMode(RenderMode.DisplayModeOnly)]
        public string CustomerIDAndName
        {
            get
            {
                return this.CustomerID.ToString() + "  (" + CustomerLogic.CustomerManager.GetCustomerNameForCID(this.CustomerID) + ")";
            }
        }

        [DisplayName("Meter ID")]
        public int MeterID { get; set; }

        [DisplayName("Bay")]
        public int Bay { get; set; }
        
        public InactiveSpace()
        {
            // Default the bay to 1
            this.Bay = 1;
        }
    }

    public class InactiveSpacePredicate
    {
        private int _CID;
        private int _MID;
        private int _BAY;
        private int _GroupID = -1;

        public InactiveSpacePredicate(int cid, int mid, int bay)
        {
            _CID = cid;
            _MID = mid;
            _BAY = bay;
        }

        public bool Compare(InactiveSpace pObject)
        {
            return (
                (this._CID == pObject.CustomerID) &&
                (this._MID == pObject.MeterID) &&
                (this._BAY == pObject.Bay)
                );
        }
    }

    public sealed class InactiveSpaceLogicalComparer : System.Collections.Generic.IComparer<InactiveSpace>
    {
        private static readonly System.Collections.Generic.IComparer<InactiveSpace> _default = new InactiveSpaceLogicalComparer();

        public InactiveSpaceLogicalComparer()
        {
        }

        public static System.Collections.Generic.IComparer<InactiveSpace> Default
        {
            get { return _default; }
        }

        public int Compare(InactiveSpace s1, InactiveSpace s2)
        {
            // First level of comparison is the customer
            int result = s1.CustomerID.CompareTo(s2.CustomerID);
            if (result != 0)
                return result;

            // Second level of comparison is the meter
            result = s1.MeterID.CompareTo(s2.MeterID);
            if (result != 0)
                return result;

            // Third level of comparison is the space
            result = s1.Bay.CompareTo(s2.Bay);
            return result;
        }
    }

    public interface IInactiveSpacesRepository
    {
        List<InactiveSpace> GetAll();
        InactiveSpace Get(int cid, int mid, int bay);
        void Insert(InactiveSpace inactiveSpaceObj);
        void Delete(InactiveSpace inactiveSpaceObj);
        
        // This repository only needs Insert and Delete support -- we will not support editing
        /*void Edit(InactiveSpace inactiveSpaceObj);*/
    }

    public class InactiveSpacesRepository : IInactiveSpacesRepository
    {
        static public InactiveSpacesRepository Repository = null;
        private const string cnstXMLRootNodeName = "Groups";
        private const string cnstXMLItemName = "Group";

        // For thread-safety, use _Locker when accessing _AllInactiveSpaces or _PersistXML
        private ReaderWriterLockSlim _Locker = new ReaderWriterLockSlim();
        private List<InactiveSpace> _AllInactiveSpaces = null;
        private XDocument _PersistXML = null;

        private string GetXMLFilename()
        {
            // Note avoid using "HttpContext.Current.Server.MapPath("~/App_Data/InactiveSpaces.xml")" because HttpContext isn't always available during app startup (i.e. IIS 7 in integrated mode)
            return System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data\\InactiveSpaces.xml");
        }

        private string GetXMLFilenameWithoutPath()
        {
            return System.IO.Path.GetFileName(GetXMLFilename());
        }

        private int SafeGetIntFromXElement(XElement element)
        {
            // Try to get an integer from node value.  If fails, we will return -1
            try
            {
                if (element == null)
                    return -1;
                else if (string.IsNullOrEmpty(element.Value))
                    return -1;
                else
                    return (int)element;
            }
            catch (Exception ex)
            {
                // Attempt to log what failed to convert, but then return null response to caller
                try
                {
                    Logging.AddTextToGenericLog(Logging.LogLevel.Error,
                        "Failed to parse node as Integer. Node: " + element.Name.ToString() +
                        ", Type: " + element.NodeType.ToString() + ", Value: " + element.Value +
                        ", Exception: " + ex.ToString(),
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
                catch { }

                return -1;
            }
        }

        private void BuildMemCacheFromXDocument()
        {
            try
            {
                // We need an exclusive lock to modify our array
                _Locker.EnterWriteLock();

                // Deserialize the XML document into an array of InactiveSpace objects
                _AllInactiveSpaces = new List<InactiveSpace>();
                try
                {
                    // Add root node to XML if not already present
                    if (_PersistXML.Root == null)
                    {
                        _PersistXML.AddFirst(new XElement(InactiveSpacesRepository.cnstXMLRootNodeName));
                    }


                    _AllInactiveSpaces =
                        (from cntry in _PersistXML.Element(InactiveSpacesRepository.cnstXMLRootNodeName).Elements(InactiveSpacesRepository.cnstXMLItemName)
                         select new InactiveSpace
                         {
                             CustomerID = SafeGetIntFromXElement(cntry.Element("cid")),
                             MeterID = SafeGetIntFromXElement(cntry.Element("mid")),
                             Bay = SafeGetIntFromXElement(cntry.Element("bay"))
                         }).ToList();

                    // Sort the list
                    _AllInactiveSpaces.Sort(InactiveSpaceLogicalComparer.Default);
                }
                catch (Exception ex)
                {
                    // Log this error, then rethrow so caller does their own exception handling too
                    Logging.AddTextToGenericLog(Logging.LogLevel.Error,
                        "Failed to parse inactive spaces from XML file. Exception: " + ex.ToString(),
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                    // Rethrow exception so caller can handle it too
                    _AllInactiveSpaces = new List<InactiveSpace>();
                    throw;
                }
            }
            finally
            {
                // Release the exclusive lock
                if (_Locker.IsWriteLockHeld)
                    _Locker.ExitWriteLock();
            }
        }

        public InactiveSpacesRepository()
        {
            string xmlFilename = GetXMLFilename();

            // We need an exclusive lock while loading the XML
            try
            {
                // Load the XML document and parse it into the _AllGroups object
                _Locker.EnterWriteLock();

                if (System.IO.File.Exists(xmlFilename))
                {
                    // Load the rules files
                    _PersistXML = XDocument.Load(xmlFilename);
                }
                else
                {
                    Logging.AddTextToGenericLog(Logging.LogLevel.Warning,
                        "No '" + GetXMLFilenameWithoutPath() + "' file found, so a new one will be created",
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                    // Failed, so use a blank document as default
                    _PersistXML = new XDocument();
                    // Add root node to XML
                    _PersistXML.AddFirst(new XElement(InactiveSpacesRepository.cnstXMLRootNodeName));
                    // Persist the changes back to the XML
                    _PersistXML.Save(xmlFilename);
                }
            }
            catch (Exception ex)
            {
                Logging.AddTextToGenericLog(Logging.LogLevel.Warning,
                    "Failed to load '" + GetXMLFilenameWithoutPath() + "', so a new one will be created. Exception: " + ex.ToString(),
                    MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                // Failed, so use a blank document as default
                _PersistXML = new XDocument();
                // Add root node to XML
                _PersistXML.AddFirst(new XElement(InactiveSpacesRepository.cnstXMLRootNodeName));
                // Persist the changes back to the XML
                _PersistXML.Save(xmlFilename);
            }
            finally
            {
                // Release the exclusive lock
                if (_Locker.IsWriteLockHeld)
                    _Locker.ExitWriteLock();
            }

            // Now deserialize the XML -- Note this call is not inside of our lock, because it acquires its own lock, and nested locks are not allowed
            BuildMemCacheFromXDocument();
        }

        // Return a list of all inactive spaces for entire website
        public List<InactiveSpace> GetAll()
        {
            try
            {
                // Use a read lock for this operation (unless the thread has an active write lock -- in which case we consider the equivalent of read/write)
                if (_Locker.IsWriteLockHeld == false)
                    _Locker.EnterReadLock();

                List<InactiveSpace> result = new List<InactiveSpace>();
                result.AddRange(_AllInactiveSpaces);
                return result;
            }
            finally
            {
                // Release the read lock (if it was acquired)           
                if (_Locker.IsReadLockHeld)
                    _Locker.ExitReadLock();
            }
        }

        // Get specific record
        public InactiveSpace Get(int cid, int mid, int bay)
        {
            try
            {
                // Use a read lock for this operation (unless the thread has an active write lock -- in which case we consider the equivalent of read/write)
                if (_Locker.IsWriteLockHeld == false)
                    _Locker.EnterReadLock();

                // DEBUG: We could either use a predicate, or for this simple comparison,
                // we can supply a lambda expression which makes for an in-line predicate...
                /*
                InactiveSpacePredicate predicate = new InactiveSpacePredicate(cid, mid, sid);
                InactiveSpace result = _AllInactiveSpaces.Find(predicate.Compare);
                return result;
                */

                return _AllInactiveSpaces.Find(item => ((item.CustomerID == cid) && (item.MeterID == mid) && (item.Bay == bay)) );
            }
            finally
            {
                // Release the read lock (if it was acquired)
                if (_Locker.IsReadLockHeld)
                    _Locker.ExitReadLock();
            }
        }

        // Insert Record
        public void Insert(InactiveSpace inactiveSpaceObj)
        {
            try
            {
                // We need an exclusive lock to modify our array
                _Locker.EnterWriteLock();

                try
                {
                    // Add root node to XML if not already present
                    if (_PersistXML.Root == null)
                    {
                        _PersistXML.AddFirst(new XElement(InactiveSpacesRepository.cnstXMLRootNodeName));
                    }

                    // Before inserting this object, we should see if there are any existing objects with the same scope. 
                    // If so, we will delete them since this should be a treated as a replacement of any existing objects
                    // that are for the same scope
                    InactiveSpacePredicate predicate = new InactiveSpacePredicate(inactiveSpaceObj.CustomerID, inactiveSpaceObj.MeterID, inactiveSpaceObj.Bay);
                    List<InactiveSpace> objectsWithSameScope = _AllInactiveSpaces.FindAll(predicate.Compare);
                    if (objectsWithSameScope.Count > 0)
                    {
                        foreach (InactiveSpace objectToRemove in objectsWithSameScope)
                        {
                            // Remove from the data store (XML in this case, but could also be a database)
                            _PersistXML.Root.Elements(InactiveSpacesRepository.cnstXMLItemName).Where(i => ( 
                                ((int)i.Element("cid") == objectToRemove.CustomerID) &&
                                ((int)i.Element("mid") == objectToRemove.MeterID) &&
                                ((int)i.Element("bay") == objectToRemove.Bay))).Remove();

                            // Also remove from the in-memory cache
                            _AllInactiveSpaces.Remove(objectToRemove);
                        }
                    }

                    // Create a new node for the object
                    XElement objectNode = null;
                    objectNode = new XElement(InactiveSpacesRepository.cnstXMLItemName,
                            new XElement("cid", inactiveSpaceObj.CustomerID),
                            new XElement("mid", inactiveSpaceObj.MeterID),
                            new XElement("bay", inactiveSpaceObj.Bay));
                    _PersistXML.Root.Add(objectNode);

                    // Persist the changes back to the XML
                    _PersistXML.Save(GetXMLFilename());

                    // Also add the object to our in-memory cache
                    _AllInactiveSpaces.Add(inactiveSpaceObj);

                    // And now since we have made a change, the customer's assets need to be reloaded so they get updated with correct "Inactive" status
                    CustomerConfig customerDto = CustomerLogic.CustomerManager.GetDTOForCID(inactiveSpaceObj.CustomerID);
                    if (customerDto != null)
                    {
                        CustomerLogic.CustomerManager.RefreshCustomerInventory(customerDto);
                    }
                }
                catch (Exception ex)
                {
                    Logging.AddTextToGenericLog(Logging.LogLevel.Error, ex.ToString(),
                        MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
            }
            finally
            {
                // Release the exclusive lock
                if (_Locker.IsWriteLockHeld)
                    _Locker.ExitWriteLock();
            }
        }

        // Delete Record
        public void Delete(InactiveSpace inactiveSpaceObj)
        {
            try
            {
                // We need an exclusive lock to modify our array
                _Locker.EnterWriteLock();

                // Remove from the data store (XML in this case, but could also be a database)
                _PersistXML.Root.Elements(InactiveSpacesRepository.cnstXMLItemName).Where(i => ( 
                    ((int)i.Element("cid") == inactiveSpaceObj.CustomerID) &&
                    ((int)i.Element("mid") == inactiveSpaceObj.MeterID) &&
                    ((int)i.Element("bay") == inactiveSpaceObj.Bay))).Remove();
                _PersistXML.Save(GetXMLFilename());

                // Also remove from the in-memory cache (But first find it, because passed DTO might not be the actual object in the list!)
                InactiveSpace dtoToRemove = _AllInactiveSpaces.Find(item => ((inactiveSpaceObj.CustomerID == inactiveSpaceObj.CustomerID) && 
                    (item.MeterID == inactiveSpaceObj.MeterID) && (item.Bay == inactiveSpaceObj.Bay)));
                if (dtoToRemove != null)
                    _AllInactiveSpaces.Remove(dtoToRemove);

                // And now since we have made a change, the customer's assets need to be reloaded so they get updated with correct "Inactive" status
                CustomerConfig customerDto = CustomerLogic.CustomerManager.GetDTOForCID(inactiveSpaceObj.CustomerID);
                if (customerDto != null)
                {
                    CustomerLogic.CustomerManager.RefreshCustomerInventory(customerDto);
                }
            }
            finally
            {
                // Release the exclusive lock
                if (_Locker.IsWriteLockHeld)
                    _Locker.ExitWriteLock();
            }
        }

        // This repository only needs Insert and Delete support -- we will not support editing
        /*
        // Edit Record
        public void Edit(InactiveSpace inactiveSpaceObj)
        {
            try
            {
                // We need an exclusive lock to modify our array
                _Locker.EnterWriteLock();

                // Find the existing object node that we will be updating
                XElement objectNode = _PersistXML.Root.Elements(InactiveSpacesRepository.cnstXMLItemName).Where(i => ( 
                    ((int)i.Element("cid") == inactiveSpaceObj.CustomerID) &&
                    ((int)i.Element("mid") == inactiveSpaceObj.MeterID) &&
                    ((int)i.Element("bay") == inactiveSpaceObj.Bay))).FirstOrDefault();

                if (objectNode != null)
                {
                    objectNode.SetElementValue("cid", inactiveSpaceObj.CustomerID);
                    objectNode.SetElementValue("mid", inactiveSpaceObj.MeterID);
                    objectNode.SetElementValue("bay", inactiveSpaceObj.Bay);

                    // Persist changes back to the XML
                    _PersistXML.Save(GetXMLFilename());
                }

                // Also update the in-memory cache
                InactiveSpace existingobject = this.Get(inactiveSpaceObj.CustomerID, inactiveSpaceObj.MeterID, inactiveSpaceObj.Bay);
                if (existingobject != null)
                    _AllInactiveSpaces.Remove(existingobject);
                _AllInactiveSpaces.Add(inactiveSpaceObj);
            }
            finally
            {
                // Release the exclusive lock
                if (_Locker.IsWriteLockHeld)
                    _Locker.ExitWriteLock();
            }
        }
        */

    }





    // DEBUG: RegulatedHours_ExperimentalDBSchemaV1Repository was partially implemented for RegulatedHours storage in ReinoComm DB,
    // but was never completed because the DB schema wasn't what we wanted.  For interim, we are using an XML file for repository
    // instead of the database. Therefore, RegulatedHours_ExperimentalDBSchemaV1Repository code is still a useful starting point
    // incase we end up moving to DB instead of XML, but it will need to be finalized and work with the current schema...
    public class RegulatedHours_ExperimentalDBSchemaV1DTO
    {
        public int ID_PrimaryKey { get; set; }
        public Int64 ParkingSpaceID { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int RegulatedStartTime_Minutes { get; set; }
        public int RegulatedEndTime_Minutes { get; set; }
        public int MaxStayMinute { get; set; }

        public int CID { get; set; }
        public int AID { get; set; }
        public int MID { get; set; }
        public int BayNumber { get; set; }
    }

    public class RegulatedHoursForSpace_ExperimentalDBSchemaV1DTO
    {
        public int CID { get; set; }
        public int AID { get; set; }
        public int MID { get; set; }
        public int BayNumber { get; set; }

        public List<RegulatedHours_ExperimentalDBSchemaV1DTO> Regulations { get; set; }

        public RegulatedHoursForSpace_ExperimentalDBSchemaV1DTO()
        {
            if (Regulations == null)
                Regulations = new List<RegulatedHours_ExperimentalDBSchemaV1DTO>();
        }
    }
}