using System;
using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Users;
//using Duncan.PEMS.DataAccess;
//using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.DataAccess.OracleDB; //** step 1
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using WebMatrix.WebData;
using Duncan.PEMS.Entities.Enforcement;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;

namespace Duncan.PEMS.Business.Enforcement
{
    public class EnforcementFactory : BaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private int citMaxLen;
        #endregion

        OracleDBEntities NewOrleansDataContext = new OracleDBEntities();


        public EnforcementFactory(string connectionStringName)
        {
            ConnectionStringName = "OracleDBEntities";  //** step 2
        }

        protected IQueryable<TSD> Tsds;
        private System.Web.HttpSessionStateBase Session;

        /// <summary>
        /// Description: It is used to popuplate the license plate of the vehicle.
        /// Modified By: Sairam on Aug 21st 2014.        
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<EnforcementModel> EnforcementLicenseType(int CurrentCity)
        {
            IQueryable<EnforcementModel> result = Enumerable.Empty<EnforcementModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                var raw_result = (from at in NewOrleansDataContext.PARKINGs
                                  where !String.IsNullOrEmpty(at.VEHLICTYPE)
                                  select at.VEHLICTYPE).Distinct().ToList();

                result = (from i in raw_result
                          select new EnforcementModel
                          {
                              LicenseType = i.ToString()
                          }).OrderBy(x => x.LicenseType).AsQueryable();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN EnforcementLicenseType factory method in AI Enquiry Summary page", ex);
            }
            return result;
        }

        /// <summary>
        /// Description: This method fetches the name of the agency the officer belongs to.
        /// Modified By Sairam on 21st 2014;
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<EnforcementModel> EnforcementAgency(int CurrentCity)
        {
            IQueryable<EnforcementModel> result = Enumerable.Empty<EnforcementModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                var raw_result = (from at in NewOrleansDataContext.PARKINGs
                                  where !String.IsNullOrEmpty(at.AGENCY)
                                  select at.AGENCY).Distinct().ToList();

                result = (from i in raw_result
                          select new EnforcementModel
                          {
                              Agency = i.ToString()
                          }).OrderBy(x => x.Agency).AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN EnforcementAgency factory method in AI Enquiry Summary page", ex);
            }
            return result;
        }

        /// <summary>
        /// Description: It is used to populate the state that the license plate is from.  This should default to the customer’s state.
        /// Modified By: Sairam on Aug 21st 2014.        /// </summary>
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<EnforcementModel> EnforcementLicenseState(int CurrentCity)
        {
            IQueryable<EnforcementModel> result = Enumerable.Empty<EnforcementModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                var results = (from at in NewOrleansDataContext.PARKINGs
                               where !String.IsNullOrEmpty(at.VEHLICSTATE)
                               select at.VEHLICSTATE).Distinct().ToList();

                result = (from i in results
                          select new EnforcementModel
                          {
                              VehLicState = i.ToString()
                          }).OrderBy(x => x.VehLicState).AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN EnforcementLicenseState factory method in AI Enquiry Summary page", ex);
            }
            return result;
        }

        /// <summary>
        ///Description: This method is used to fetch the beat the officer was working at the time of the citation.
        /// Modified By: Santhosh on May 16th 2014, Sairam on 20th Aug 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<EnforcementModel> EnforcementBeat(int CurrentCity)
        {
            IQueryable<EnforcementModel> ddlItems = Enumerable.Empty<EnforcementModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                var raw_result = (from at in NewOrleansDataContext.PARKINGs
                                  where !String.IsNullOrEmpty(at.BEAT) && !at.BEAT.Equals("00")
                                  select at.BEAT).Distinct().ToList();

                ddlItems = (from i in raw_result
                            select new EnforcementModel
                            {
                                Beat = i.ToString()
                            }).OrderBy(x => x.Beat).AsQueryable(); ;



            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN EnforcementBeat Factory Method in AI Enquiry (summmary page)", ex);
            }
            return ddlItems;
        }

        /// <summary>
        /// Description: It is used to populate the citation’s status.
        /// Modified By: Sairam on Aug 21st 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<EnforcementModel> EnforcementStatus(int CurrentCity)
        {
            IQueryable<EnforcementModel> result = Enumerable.Empty<EnforcementModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                var raw_result =
                         (from at in NewOrleansDataContext.PARKINGs
                          where !String.IsNullOrEmpty(at.VOIDSTATUS)
                          select new
                          {
                              Status = at.VOIDSTATUS
                          }).Distinct().ToList();

                result = (from i in raw_result
                          select new EnforcementModel
                          {
                              Status = i.Status
                          }).OrderBy(x => x.Status).AsQueryable();

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN EnforcementStatus factory method in AI Enquiry Summary page", ex);
            }
            return result;
        }

        /// <summary>
        /// Description: This method is fetching the ViolationClass for the AI Enquiry summary page
        /// Modified By: Sairam on 21st 2014.
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<EnforcementModel> EnforcementClass(int CurrentCity)
        {
            IQueryable<EnforcementModel> result = Enumerable.Empty<EnforcementModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;
            try
            {
                //result =
                //          (from at in NewOrleansDataContext.PARKINGs
                //           where  !String.IsNullOrEmpty(at.ViolationClass)
                //           select new EnforcementModel
                //           {
                //               Class = at.ViolationClass
                //           }).Distinct();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN EnforcementClass factory method in AI Enquiry Summary page", ex);
            }
            return result;
        }

        /// <summary>
        /// Description: It is the numeric value that indicates the violation which violation the vehicle is in.
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<EnforcementModel> EnforcementCode(int CurrentCity)
        {
            IQueryable<EnforcementModel> result = Enumerable.Empty<EnforcementModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                var raw_result =
                          (from at in NewOrleansDataContext.PARKING_VIOS
                           where !String.IsNullOrEmpty(at.VIOCODE)
                           select new
                           {
                               Code = at.VIOCODE
                           }).Distinct().ToList();

                result = (from i in raw_result
                          select new EnforcementModel
                          {
                              Code = i.Code
                          }).OrderBy(x => x.Code).AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN EnforcementCode factory method in AI Enquiry Summary page", ex);
            }
            return result;
        }

        /// <summary>
        /// Description: It is used to populate the description of the violation that caused the citation.
        /// Modified By: Sairam on Aug 21st 2014.        /// </summary>
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<EnforcementModel> EnforcementVioDesc(int CurrentCity)
        {
            IQueryable<EnforcementModel> result = Enumerable.Empty<EnforcementModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                var raw_result =
                           (from at in NewOrleansDataContext.PARKING_VIOS
                            where !String.IsNullOrEmpty(at.VIODESCRIPTION1)
                            select new
                            {
                                ViolationDescription = at.VIODESCRIPTION1
                            }).Distinct().ToList();

                result = (from i in raw_result
                          select new EnforcementModel
                          {
                              ViolationDescription = i.ViolationDescription
                          }).OrderBy(x => x.ViolationDescription).AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN EnforcementVioDesc factory method in AI Enquiry Summary page", ex);
            }
            return result;
        }

        /// <summary>
        /// Description: This is used to fetch the 'Type' of the Citation (AI Enquiry Summary)
        /// Modified By: Sairam on 21st 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<EnforcementModel> EnforcementType(int CurrentCity)
        {
            IQueryable<EnforcementModel> result = Enumerable.Empty<EnforcementModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                var raw_result =
                         (from at in NewOrleansDataContext.PARKINGs
                          where !String.IsNullOrEmpty(at.VEHTYPE)
                          select new
                          {
                              Type = at.VEHTYPE
                          }).Distinct().ToList();

                result = (from i in raw_result
                          select new EnforcementModel
                          {
                              Type = i.Type
                          }).OrderBy(x => x.Type).AsQueryable();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN EnforcementType factory method in AI Enquiry Summary page", ex);
            }
            return result;
        }

        /// <summary>
        /// Description: It is used to populate the issuing officer’s ID.
        /// Modified By: Sairam on Aug 21st 2014.       
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<EnforcementModel> EnforcementOfficerId(int CurrentCity)
        {
            IQueryable<EnforcementModel> result = Enumerable.Empty<EnforcementModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                var raw_result = (from at in NewOrleansDataContext.SECURITY_USER
                                  where at.OFFICERID != null && !at.OFFICERID.Equals("000")
                                  select at.OFFICERID).Distinct().ToList();

                result = (from i in raw_result
                          select new EnforcementModel
                          {
                              OfficerID = Convert.ToInt32(i)
                          }).OrderBy(x => x.OfficerID).AsQueryable();

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN EnforcementOfficerId factory method in AI Enquiry Summary page", ex);
            }
            return result;
        }

        /// <summary>
        /// Description:This method is used to populate the Street for enforcement page for the typed text in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="streetIs"></param>
        /// <returns></returns>
        //public List<EnforcementModel> EnforcementStreet(int CurrentCity, string streetIs)
        //{
        //    var ddlItems = new List<EnforcementModel>();
        //    try
        //    {
        //        var items = (from at in NewOrleansDataContext.PARKINGs
        //                    where  at.LocStreet.Contains(streetIs)
        //                    select new EnforcementModel
        //                    {
        //                        LocStreet = at.LocStreet,
        //                    }).Distinct();

        //        List<EnforcementModel> LicTypes = items.ToList();

        //        if (LicTypes.Any())
        //            ddlItems = LicTypes;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return ddlItems;
        //}

        /// <summary>
        /// Description: It is used to populate the name of the officer who issued the citation.
        /// Modified By: Sairam on Aug 21st 2014.
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<EnforcementModel> EnforcementOfficerNames(int CurrentCity)
        {
            IQueryable<EnforcementModel> result = Enumerable.Empty<EnforcementModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                var raw_result = (from at in NewOrleansDataContext.SECURITY_USER
                                  where !String.IsNullOrEmpty(at.OFFICERNAME)
                                  select at.OFFICERNAME).Distinct().ToList();

                result = (from i in raw_result
                          select new EnforcementModel
                          {
                              OfficerName = i.ToString()
                          }).OrderBy(x => x.OfficerName).AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN EnforcementOfficerNames factory method in AI Enquiry Summary page", ex);
            }
            return result;
        }

        /// <summary>
        /// Description:This method is used to populate the IssueNumberSuffix for the typed text in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="suffixIs"></param>
        /// <returns></returns>
        public List<EnforcementModel> EnforcementIssueNumberSuffix(int CurrentCity, string suffixIs)
        {
            var ddlItems = new List<EnforcementModel>();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                var raw_result = (from at in NewOrleansDataContext.PARKINGs
                                  where at.ISSUENOSFX.Contains(suffixIs)
                                  select at.ISSUENOSFX).Distinct().ToList();

                ddlItems = (from i in raw_result
                            select new EnforcementModel
                            {
                                IssueNoSfx = i.ToString()
                            }).ToList();

                //List<EnforcementModel> EnforcIssueNumberSuffix = items.ToList();

                //if (EnforcIssueNumberSuffix.Any())
                //    ddlItems = EnforcIssueNumberSuffix;
            }
            catch (Exception ex)
            {

            }
            return ddlItems;
        }

        /// <summary>
        /// Description:This method is used to populate the MeterIDs for enforcement page in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="meterIDIs"></param>
        /// <returns></returns>
        public List<EnforcementModel> EnforcementMeterId(int CurrentCity, string meterIDIs)
        {
            var ddlItems = new List<EnforcementModel>();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                var raw_result = (from at in NewOrleansDataContext.PARKINGs
                                  where at.METERNO.Contains(meterIDIs)
                                  select at.METERNO).Distinct().ToList();

                ddlItems = (from i in raw_result
                            select new EnforcementModel
                            {
                                MeterID = i.ToString()
                            }).ToList();

                //List<EnforcementModel> EnforcMeterId = items.ToList();

                //if (EnforcMeterId.Any())
                //    ddlItems = EnforcMeterId;
            }
            catch (Exception ex)
            {

            }
            return ddlItems;
        }

        /// <summary>
        /// Description:This method is used to populate the VehicleVIN for enforcement page for the typed text in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="vehVINIs"></param>
        /// <returns></returns>
        public List<EnforcementModel> EnforcementVehicleVIN(int CurrentCity, string vehVINIs)
        {
            var ddlItems = new List<EnforcementModel>();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                var raw_result = (from at in NewOrleansDataContext.PARKINGs
                                  where at.VEHVIN.Contains(vehVINIs)
                                  select at.VEHVIN).Distinct().ToList();

                ddlItems = (from i in raw_result
                            select new EnforcementModel
                            {
                                VehVIN = i.ToString()
                            }).ToList();
            }
            catch (Exception ex)
            {

            }
            return ddlItems;
        }

        /// <summary>
        /// Description:This method is used to populate the IssueNumber Prefix items for the typed prefix in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="prefixIs"></param>
        /// <returns></returns>
        public List<EnforcementModel> EnforcementIssueNumberPrefix(int CurrentCity, string prefixIs)
        {
            var ddlItems = new List<EnforcementModel>();
            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                var raw_result = (from at in NewOrleansDataContext.PARKINGs
                                  where at.ISSUENOPFX.Contains(prefixIs)
                                  select at.ISSUENOPFX).Distinct().ToList();

                ddlItems = (from i in raw_result
                            select new EnforcementModel
                            {
                                IssueNoPfx = i.ToString()
                            }).ToList();
            }
            catch (Exception ex)
            {

            }

            return ddlItems;
        }

        /// <summary>
        /// Description:This method is used to populate the IssueNumbers for the typed text in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="citIssueNumberIs"></param>
        /// <returns></returns>
        public List<EnforcementModel> EnforcementIssueNumber(int CurrentCity, string citIssueNumberIs)
        {
            var ddlItems = new List<EnforcementModel>();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                var raw_result = (from at in NewOrleansDataContext.PARKINGs
                                  select at.ISSUENO).Distinct().ToList();

                ddlItems = (from i in raw_result
                            select new EnforcementModel
                            {
                                IssueNo = Convert.ToInt64(i)
                            }).ToList();
            }
            catch (Exception ex)
            {

            }
            return ddlItems;
        }

        /// <summary>
        /// Description:This method is used to populate the LicenseNo for enforcement page for the typed text in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="vehLICNoIs"></param>
        /// <returns></returns>
        public List<EnforcementModel> EnforcementVehLicNo(int CurrentCity, string vehLICNoIs)
        {
            var ddlItems = new List<EnforcementModel>();
            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                var raw_result = (from at in NewOrleansDataContext.PARKINGs
                                  where at.VEHLICNO.Contains(vehLICNoIs)
                                  select at.VEHLICNO).Distinct().ToList();

                ddlItems = (from i in raw_result
                            select new EnforcementModel
                            {
                                VehLicNo = i.ToString()
                            }).ToList();
            }
            catch (Exception ex)
            {

            }
            return ddlItems;
        }

        public List<EnforcementModel> EnforcementCitationDetails(long citationID)
        {
            var CitationDetails = new List<EnforcementModel>();
            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                var enforcementCitation = (from ai_Park in NewOrleansDataContext.PARKINGs
                                           join offName in NewOrleansDataContext.SECURITY_USER
                                                    on ai_Park.OFFICERID equals offName.OFFICERID
                                           join offParking in NewOrleansDataContext.PARKING_VIOS
                                           on ai_Park.UNIQUEKEY equals offParking.MASTERKEY
                                           where (ai_Park.ISSUENO == citationID)
                                           select new
                                           {
                                               IssueNo = ai_Park.ISSUENO,
                                               IssueDate = ai_Park.ISSUEDATE,
                                               IssueTime = ai_Park.ISSUETIME,
                                               UnitSerial = ai_Park.UNITSERIAL,
                                               Agency = ai_Park.AGENCY,
                                               Beat = ai_Park.BEAT,
                                               OfficerID = ai_Park.OFFICERID,
                                               OfficerName = offName.OFFICERNAME,
                                               VehLicNo = ai_Park.VEHLICNO,
                                               VehLicState = ai_Park.VEHLICSTATE,
                                               VEHLICTYPE = ai_Park.VEHLICTYPE,
                                               VehMake = ai_Park.VEHMAKE,
                                               VehModel = ai_Park.VEHMODEL,
                                               VehBodyStyle = ai_Park.VEHBODYSTYLE,
                                               VEHCHECKDIGIT = ai_Park.ISSUENOCHKDGT,// ai_Park.VEHCHECKDIGIT,
                                               VehColor1 = ai_Park.VEHCOLOR1,
                                               VehColor2 = ai_Park.VEHCOLOR2,
                                               VehLicExpDate = ai_Park.VEHLICEXPDATE,
                                               VehVIN = ai_Park.VEHVIN,
                                               VehVIN4 = ai_Park.VEHVIN4,
                                               LocBlock = ai_Park.LOCBLOCK,
                                               LocCity = string.Empty,//ai_Park.LocCity,
                                               LocCrossStreet1 = ai_Park.LOCCROSSSTREET1,
                                               LocCrossStreet2 = ai_Park.LOCCROSSSTREET2,
                                               LocDescriptor = ai_Park.LOCDESCRIPTOR,
                                               LocDirection = ai_Park.LOCDIRECTION,
                                               LocLot = ai_Park.LOCLOT,
                                               LOCSIDEOFSTREET = string.Empty,//ai_Park.LOCSIDEOFSTREET,
                                               LocState = string.Empty,//ai_Park.LocState,
                                               LocStreet = ai_Park.LOCSTREET,
                                               LocSuburb = string.Empty,//ai_Park.LocSuburb,
                                               LOCZIP = string.Empty,//ai_Park.LOCZIP,
                                               MeterBayNo = ai_Park.ENF_METERBAYNO,
                                               MeterNo = ai_Park.METERNO,
                                               PermitNo = ai_Park.PERMITNO,
                                               Remark1 = ai_Park.REMARK1,
                                               Remark2 = ai_Park.REMARK2,
                                               Remark3 = ai_Park.REMARK3,
                                               VioDescription1 = offParking.VIODESCRIPTION1,
                                               VioCode = offParking.VIOCODE,
                                               VioFine = offParking.VIOFINE,
                                               VioLateFee1 = offParking.VIOLATEFEE1,
                                               VioLateFee2 = offParking.VIOLATEFEE2,
                                               ParkingId = offParking.MASTERKEY
                                           }).ToList().Select(x => new EnforcementModel
                                           {
                                               IssueNo = Convert.ToInt64(x.IssueNo),
                                               IssueDateTime = ConvertToDateTimeFormat(x.IssueDate, x.IssueTime),
                                               UnitSerial = x.UnitSerial,
                                               Agency = x.Agency,
                                               Beat = x.Beat,
                                               OfficerID = Convert.ToInt32(x.OfficerID),
                                               OfficerName = x.OfficerName,
                                               VehLicNo = x.VehLicNo,
                                               VehLicState = x.VehLicState,
                                               VEHLICTYPE = x.VEHLICTYPE,
                                               VehMake = x.VehMake,
                                               VehModel = x.VehModel,
                                               VehBodyStyle = x.VehBodyStyle,
                                               VEHCHECKDIGIT = x.VEHCHECKDIGIT,
                                               VehColor1 = x.VehColor1,
                                               VehColor2 = x.VehColor2,
                                               VehLicExpDate = x.VehLicExpDate,
                                               VehVIN = x.VehVIN,
                                               VehVIN4 = x.VehVIN4,
                                               LocBlock = x.LocBlock,
                                               LocCity = x.LocCity,
                                               LocCrossStreet1 = x.LocCrossStreet1,
                                               LocCrossStreet2 = x.LocCrossStreet2,
                                               LocDescriptor = x.LocDescriptor,
                                               LocDirection = x.LocDirection,
                                               LocLot = x.LocLot,
                                               LOCSIDEOFSTREET = x.LOCSIDEOFSTREET,
                                               LocState = x.LocState,
                                               LocStreet = x.LocStreet,
                                               //LocSuburb = Convert.ToInt32(x.LocSuburb),
                                               LocSuburb = x.LocSuburb,
                                               LOCZIP = x.LOCZIP,
                                               MeterBayNo = x.MeterBayNo,
                                               MeterNo = x.MeterNo,
                                               PermitNo = x.PermitNo,
                                               Remark1 = x.Remark1,
                                               Remark2 = x.Remark2,
                                               Remark3 = x.Remark3,
                                               VioDescription1 = x.VioDescription1,
                                               VioCode = x.VioCode,
                                               VioFine = "$" + Convert.ToString(x.VioFine),
                                               VioLateFee1 = "$" + Convert.ToString(x.VioLateFee1),
                                               VioLateFee2 = "$" + Convert.ToString(x.VioLateFee2),
                                               ParkingId = x.ParkingId
                                           }).ToList();

                List<EnforcementModel> EnforcCitationDetails = enforcementCitation.ToList();

                if (EnforcCitationDetails.Any())
                    CitationDetails = EnforcCitationDetails;
            }
            catch (Exception ex)
            {

            }
            return CitationDetails;
        }

        public List<EnforcementModel> GetEnforcementRecordHistory(int CitationID)
        {
            var EnforcementRecordHistory = new List<EnforcementModel>();
            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                //var AuditHistory_raw = (from ParkTranLink in NewOrleansDataContext.AI_PARKING_TRANSLINK
                //                        join ai_park in NewOrleansDataContext.PARKINGs
                //                                 on ParkTranLink.ParkingId equals ai_park.UNIQUEKEY
                //                        join offName in NewOrleansDataContext.SECURITY_USER
                //                                 on ParkTranLink.OFFICERID equals offName.OFFICERID
                //                        where (ai_park.ISSUENO == CitationID)
                //                        select new EnforcementModel
                //                        {
                //                            OfficerName = offName.OFFICERNAME,
                //                            StatusValue = ParkTranLink.StatusValue,
                //                            StatusDateTime = ParkTranLink.StatusDateTime,
                //                            StatusReason = ParkTranLink.StatusReason,
                //                            OfficerID = offName.OFFICERID,

                //                        });

                //List<EnforcementModel> EnforcRecordHistory = AuditHistory_raw.ToList();

                //if (EnforcRecordHistory.Any())
                //    EnforcementRecordHistory = EnforcRecordHistory;
            }
            catch (Exception ex)
            {

            }
            return EnforcementRecordHistory;
        }

        public List<EnforcementModel> GetEnforcementViolation(int CitationID)
        {
            var EnforcementViolation = new List<EnforcementModel>();
            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                var Violation_raw = (from Parkvio in NewOrleansDataContext.PARKING_VIOS
                                     join ai_park in NewOrleansDataContext.PARKINGs
                                     on Parkvio.MASTERKEY equals ai_park.UNIQUEKEY
                                     where (ai_park.ISSUENO == CitationID)
                                     select new
                                     {
                                         VioCode = Parkvio.VIOCODE,
                                         VioDescription1 = Parkvio.VIODESCRIPTION1,
                                         VioFine = Parkvio.VIOFINE,
                                         VioLateFee1 = Parkvio.VIOLATEFEE1,
                                         VioLateFee2 = Parkvio.VIOLATEFEE2
                                     }).ToList();

                var Violation_result = (from i in Violation_raw
                                        select new EnforcementModel
                                        {
                                            VioCode = i.VioCode,
                                            VioDescription1 = i.VioDescription1,
                                            //VioFine = (float)x.VioFine,
                                            //VioLateFee1 = (float)x.VioLateFee1,
                                            //VioLateFee2 = (float)x.VioLateFee2
                                            VioFine = "$" + Convert.ToString(i.VioFine),
                                            VioLateFee1 = "$" + Convert.ToString(i.VioLateFee1),
                                            VioLateFee2 = "$" + Convert.ToString(i.VioLateFee2)
                                        });

                List<EnforcementModel> EnforcRecordHistory = Violation_result.ToList();

                if (EnforcRecordHistory.Any())
                    EnforcementViolation = EnforcRecordHistory;
            }
            catch (Exception ex)
            {

            }
            return EnforcementViolation;
        }

        //public List<EnforcementModel> GetEnforcementExportHistory(int CitationID)
        //{
        //    var EnforcementExportHistory = new List<EnforcementModel>();
        //    try
        //    {
        //        var AuditHistory_raw = from ae in NewOrleansDataContext.AI_EXPORT
        //                               where
        //                                 ae.AI_PARKING.ISSUENO == CitationID
        //                               group ae by new EnforcementModel
        //                               {
        //                                   ExportDateTime = ae.ExportDateTime,
        //                                   ExportId = ae.ExportId,
        //                                   ExportType = ae.ExportType,
        //                                   Count = ae.ParkingId
        //                               } into g
        //                               select new EnforcementModel
        //                               {
        //                                   ExportDateTime = g.Key.ExportDateTime,
        //                                   ExportId = g.Key.ExportId,
        //                                   ExportType = g.Key.ExportType,
        //                                   Count = g.Count(p => p.ParkingId != null)
        //                               };
        //        List<EnforcementModel> EnforcExportHistory = AuditHistory_raw.ToList();

        //        if (EnforcExportHistory.Any())
        //            EnforcementExportHistory = EnforcExportHistory;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return EnforcementExportHistory;
        //}

        /// <summary>
        /// Description:This method is used to populate the Grid with the Enforcement Summary details.
        /// Modified By: Sairam on June 26th 2014
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="licensePlate"></param>
        /// <param name="licenseState"></param>
        /// <param name="licenseType"></param>
        /// <param name="vin"></param>
        /// <param name="classes"></param>
        /// <param name="prefix"></param>
        /// <param name="issueNo"></param>
        /// <param name="suffix"></param>
        /// <param name="code"></param>
        /// <param name="agency"></param>
        /// <param name="street"></param>
        /// <param name="beat"></param>
        /// <param name="meterNo"></param>
        /// <param name="officerID"></param>
        /// <param name="officerName"></param>
        /// <param name="vioDesc"></param>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public List<EnforcementModel> GetEnforcementsummary(string startDate, string endDate, string licensePlate, string licenseState, string licenseType, string vin, string classes, string prefix, string startCitationNo, string endCitationNo, string suffix, string code, string officerName, string agency, string beat, string meterNo, string officerID, string vioDesc, string parkingStatus, string parkingType, int CurrentCity)
        {
            List<EnforcementModel> Enforcementsummary = new List<EnforcementModel>();
            //** To avoid Timeout Error in EF 5.0, use the below command



            long startIssueNo;
            long endIssueNo;
            string myOfficerId;


            if (startCitationNo.Trim().Length != 0)
            {
                startIssueNo = Convert.ToInt64(startCitationNo);
            }
            else
            {
                //** Start Issue No filter text is empty
                startIssueNo = -1;
            }

            if (endCitationNo.Trim().Length != 0)
            {
                endIssueNo = Convert.ToInt64(endCitationNo);
            }
            else
            {
                //** End Issue No filter text is empty
                endIssueNo = -1;
            }

            if (officerID.Trim().Length != 0)
            {
                // myOfficerId = Convert.ToInt32(officerID);
                myOfficerId = officerID;
            }
            else
            {
                //myOfficerId = -1;
                myOfficerId = string.Empty;
            }
            DateTime startTimeOutput;
            DateTime startTimeEndOutput;
            DateTime startDateOnlyOutput;
            DateTime endTimeOutput;
            DateTime endTimeEndOutput;

            var split_1 = startDate.Split(' ');
            string startDateAlone = split_1[0].ToString() + " 00:00:00";
            string startTimeAlone = "12/30/1899 " + split_1[1].ToString() + " " + split_1[2].ToString();

            var split_2 = endDate.Split(' ');
            string endDateAlone = split_2[0].ToString() + " 00:00:00";
            string endTimeAlone = "12/30/1899 " + split_2[1].ToString() + " " + split_2[2].ToString();

            //** for date comparison
            if (DateTime.TryParse(startDateAlone, out startTimeOutput))
            {
                // ** Valid start date 
            }

            if (DateTime.TryParse(endDateAlone, out endTimeOutput))
            {
                //** Valid end date 
            }

            //** for time comparison
            if (DateTime.TryParse(startTimeAlone, out startTimeEndOutput))
            {
                // ** Valid start date 
            }

            if (DateTime.TryParse(endTimeAlone, out endTimeEndOutput))
            {
                //** Valid end date 
            }

            try
            {
                ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;


                Enforcementsummary = (from ai_PARKING in NewOrleansDataContext.PARKINGs

                                      //join customer in NewOrleansDataContext.Customers
                                      //on ai_PARKING.CustomerID equals customer.CustomerID

                                      join offName in NewOrleansDataContext.SECURITY_USER
                                      on ai_PARKING.OFFICERID equals offName.OFFICERID

                                      join parVios in NewOrleansDataContext.PARKING_VIOS  //** new join created based on oracle
                                      on ai_PARKING.UNIQUEKEY equals parVios.MASTERKEY

                                      where
                                      ((ai_PARKING.ISSUEDATE >= startTimeOutput && ai_PARKING.ISSUETIME >= startTimeEndOutput) &&
                                       (ai_PARKING.ISSUEDATE <= endTimeOutput && ai_PARKING.ISSUETIME <= endTimeEndOutput))
                                        && (ai_PARKING.VEHLICNO.Equals(licensePlate) || string.IsNullOrEmpty(licensePlate))
                                        && (ai_PARKING.VEHLICSTATE.Equals(licenseState) || string.IsNullOrEmpty(licenseState))
                                        && (ai_PARKING.VEHLICTYPE.Equals(licenseType) || string.IsNullOrEmpty(licenseType)) //** license type added on sep 14 2016
                                        && (ai_PARKING.VEHVIN.Equals(vin) || string.IsNullOrEmpty(vin))
                                        && ((ai_PARKING.ISSUENO >= startIssueNo || startIssueNo == -1) && (ai_PARKING.ISSUENO <= endIssueNo || endIssueNo == -1))
                                          //&& (ai_PARKING.ViolationClass == classes || classes == string.Empty)
                                       && (parVios.VIOCODE.Equals(code) || string.IsNullOrEmpty(code))
                                       && (parVios.VIODESCRIPTION1.Equals(vioDesc) || string.IsNullOrEmpty(vioDesc)) //** Violation Description added on sep 14 2016
                                       && (ai_PARKING.VOIDSTATUS.Equals(parkingStatus) || string.IsNullOrEmpty(parkingStatus))
                                       && (ai_PARKING.VEHTYPE.Equals(parkingType) || string.IsNullOrEmpty(parkingType))
                                        && (ai_PARKING.ISSUENOPFX.Equals(prefix) || string.IsNullOrEmpty(prefix))
                                        && (ai_PARKING.ISSUENOSFX.Equals(suffix) || string.IsNullOrEmpty(suffix))
                                        && (ai_PARKING.AGENCY.Equals(agency) || string.IsNullOrEmpty(agency))
                                        && (ai_PARKING.BEAT.Equals(beat) || string.IsNullOrEmpty(beat))
                                        && (ai_PARKING.METERNO.Equals(meterNo) || string.IsNullOrEmpty(meterNo))
                                        && (ai_PARKING.OFFICERID.Equals(myOfficerId) || string.IsNullOrEmpty(myOfficerId))
                                        && (ai_PARKING.OFFICERNAME.Equals(officerName) || string.IsNullOrEmpty(officerName))


                                      select new
                                      {
                                          OfficerID = ai_PARKING.OFFICERID,
                                          OfficerName = offName.OFFICERNAME,
                                          IssueNo = ai_PARKING.ISSUENO,
                                          IssueDate = ai_PARKING.ISSUEDATE,
                                          IssueTime = ai_PARKING.ISSUETIME,
                                          Status = ai_PARKING.VOIDSTATUS ?? "",
                                          IssueNoPfx = ai_PARKING.ISSUENOPFX == null ? "" : ai_PARKING.ISSUENOPFX,
                                          IssueNoSfx = ai_PARKING.ISSUENOSFX == null ? "" : ai_PARKING.ISSUENOSFX,
                                          VehLicNo = ai_PARKING.VEHLICNO,
                                          Beat = ai_PARKING.BEAT,
                                          MeterNo = ai_PARKING.METERNO == null ? "" : ai_PARKING.METERNO,
                                          VioCode = parVios.VIOCODE == null ? "" : parVios.VIOCODE,
                                          isAutoBindReqd = true
                                      }).ToList().Select(x => new EnforcementModel
                                      {
                                          OfficerID = Convert.ToInt32(x.OfficerID),
                                          OfficerName = x.OfficerName,
                                          IssueNo = Convert.ToInt64(x.IssueNo),
                                          IssueDateTime = ConvertToDateTimeFormat(x.IssueDate, x.IssueTime),
                                          IssueDateTime_final = ConvertToDateTimeFormat(x.IssueDate, x.IssueTime).ToString(),
                                          IssueNoWithPfxSfx = String.Concat(Convert.ToString(x.IssueNoPfx), GetIssueNoWithLeftPaddedZeroes(Convert.ToInt64(x.IssueNo)), Convert.ToString(x.IssueNoSfx)),

                                          Status = x.Status ?? "",
                                          IssueNoPfx = x.IssueNoPfx ?? "",
                                          IssueNoSfx = x.IssueNoSfx ?? "",
                                          VehLicNo = x.VehLicNo,
                                          Beat = x.Beat,
                                          MeterNo = x.MeterNo == null ? "" : x.MeterNo,
                                          VioCode = x.VioCode == null ? "" : x.VioCode,
                                          isAutoBindReqd = x.isAutoBindReqd
                                      }).ToList();

            }
            catch (Exception ex)
            {

            }
            return Enforcementsummary;
        }

        public string GetIssueNoWithLeftPaddedZeroes(long issueNo)
        {

            string issueNoIs = Convert.ToString(issueNo);
            return issueNoIs.PadLeft(citMaxLen, '0');
        }

        private DateTime ConvertToDateTimeFormat(DateTime? date_1, DateTime? date_2)
        {

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            string myDate_1 = date_1.ToString();
            string myDate_2 = date_2.ToString();

            DateTime derivedDateTime;

            var split_1 = myDate_1.Split(' ');
            string DateAlone = split_1[0].ToString();

            var split_2 = myDate_2.Split(' ');
            string TimeAlone = split_2[1].ToString() + " " + split_2[2].ToString();

            string concatStr = DateAlone + " " + TimeAlone;

            if (DateTime.TryParse(concatStr, out derivedDateTime))
            {
                // ** Valid start date 
            }

            return derivedDateTime;
        }

        public IQueryable<EnforcementModel> GetEnforcementsummary_Export([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string licensePlate, string licenseState, string licenseType, string vin, string classes, string prefix, string startCitationNo, string endCitationNo, string suffix, string code, string officerName, string agency, string beat, string meterNo, string officerID, string vioDesc, string parkingStatus, string parkingType, int CurrentCity, out int total)
        {
            List<EnforcementModel> Enforcementsummary = new List<EnforcementModel>();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;


            long startIssueNo;
            long endIssueNo;
            string myOfficerId;

            total = 0;

            if (startCitationNo.Trim().Length != 0)
            {
                startIssueNo = Convert.ToInt64(startCitationNo);
            }
            else
            {
                //** Start Issue No filter text is empty
                startIssueNo = -1;
            }

            if (endCitationNo.Trim().Length != 0)
            {
                endIssueNo = Convert.ToInt64(endCitationNo);
            }
            else
            {
                //** End Issue No filter text is empty
                endIssueNo = -1;
            }

            if (officerID.Trim().Length != 0)
            {
                myOfficerId = officerID;
            }
            else
            {
                myOfficerId = string.Empty;
            }
            DateTime startTimeOutput;
            DateTime startTimeEndOutput;
            DateTime startDateOnlyOutput;
            DateTime endTimeOutput;
            DateTime endTimeEndOutput;

            var split_1 = startDate.Split(' ');
            string startDateAlone = split_1[0].ToString() + " 00:00:00";
            string startTimeAlone = "12/30/1899 " + split_1[1].ToString() + " " + split_1[2].ToString();

            var split_2 = endDate.Split(' ');
            string endDateAlone = split_2[0].ToString() + " 00:00:00";
            string endTimeAlone = "12/30/1899 " + split_2[1].ToString() + " " + split_2[2].ToString();


            //** for date comparison
            if (DateTime.TryParse(startDateAlone, out startTimeOutput))
            {
                // ** Valid start date 
            }

            if (DateTime.TryParse(endDateAlone, out endTimeOutput))
            {
                //** Valid end date 
            }

            //** for time comparison
            if (DateTime.TryParse(startTimeAlone, out startTimeEndOutput))
            {
                // ** Valid start date 
            }

            if (DateTime.TryParse(endTimeAlone, out endTimeEndOutput))
            {
                //** Valid end date 
            }
            try
            {
                var items = (from ai_PARKING in NewOrleansDataContext.PARKINGs

                             //join customer in NewOrleansDataContext.Customers
                             //on ai_PARKING.CustomerID equals customer.CustomerID

                             join offName in NewOrleansDataContext.SECURITY_USER
                             on ai_PARKING.OFFICERID equals offName.OFFICERID

                             where
                              ((ai_PARKING.ISSUEDATE >= startTimeOutput && ai_PARKING.ISSUETIME >= startTimeEndOutput) &&
                              (ai_PARKING.ISSUEDATE <= endTimeOutput && ai_PARKING.ISSUETIME <= endTimeEndOutput))
                             && (ai_PARKING.VEHLICNO == licensePlate || licensePlate == string.Empty)
                             && (ai_PARKING.VEHLICSTATE == licenseState || licenseState == string.Empty)
                             && (ai_PARKING.VEHLICTYPE == licenseType || licenseType == string.Empty)
                             && (ai_PARKING.VEHVIN == vin || vin == string.Empty)
                                 //&& (ai_PARKING.ViolationClass == classes || classes == string.Empty)
                             && (ai_PARKING.VEHLICTYPECODE == code || code == string.Empty)
                                 //&& (ai_PARKING.ViolationDesc == vioDesc || vioDesc == string.Empty)
                             && (ai_PARKING.ISSUENOPFX == prefix || prefix == string.Empty)
                             && ((ai_PARKING.ISSUENO >= startIssueNo || startIssueNo == -1) && (ai_PARKING.ISSUENO <= endIssueNo || endIssueNo == -1))
                             && (ai_PARKING.ISSUENOSFX == suffix || suffix == string.Empty)
                             && (ai_PARKING.AGENCY == agency || agency == string.Empty)
                             && (ai_PARKING.BEAT == beat || beat == string.Empty)
                             && (ai_PARKING.METERNO == meterNo || meterNo == string.Empty)
                                 //&& (ai_PARKING.Status == parkingStatus || parkingStatus == string.Empty)
                                 //&& (ai_PARKING.Type == parkingType || parkingType == string.Empty)
                             && (ai_PARKING.OFFICERID == myOfficerId || myOfficerId == string.Empty)
                             && (offName.OFFICERNAME == officerName || officerName == string.Empty)

                             select new
                             {
                                 OfficerID = ai_PARKING.OFFICERID,
                                 OfficerName = offName.OFFICERNAME,
                                 IssueNo = ai_PARKING.ISSUENO,
                                 // IssueDateTime = ConvertToDateTimeFormat(ai_PARKING.ISSUEDATE,ai_PARKING.ISSUETIME),
                                 IssueDate = ai_PARKING.ISSUEDATE,
                                 IssueTime = ai_PARKING.ISSUETIME,
                                 Status = "",//ai_PARKING.Status == null ? "" : ai_PARKING.Status,
                                 IssueNoPfx = ai_PARKING.ISSUENOPFX == null ? "" : ai_PARKING.ISSUENOPFX,
                                 IssueNoSfx = ai_PARKING.ISSUENOSFX == null ? "" : ai_PARKING.ISSUENOSFX,
                                 VehLicNo = ai_PARKING.VEHLICNO,
                                 Beat = ai_PARKING.BEAT,
                                 MeterNo = ai_PARKING.METERNO == null ? "" : ai_PARKING.METERNO,
                                 VioCode = ai_PARKING.VEHLICTYPECODE == null ? "" : ai_PARKING.VEHLICTYPECODE,
                                 isAutoBindReqd = true
                             }).Select(x => new EnforcementModel
                             {
                                 OfficerID = Convert.ToInt32(x.OfficerID),
                                 OfficerName = x.OfficerName,
                                 IssueNo = Convert.ToInt64(x.IssueNo),
                                 IssueDateTime = ConvertToDateTimeFormat(x.IssueDate, x.IssueTime),
                                 Status = x.Status == null ? "" : x.Status,
                                 IssueNoPfx = x.IssueNoPfx == null ? "" : x.IssueNoPfx,
                                 IssueNoSfx = x.IssueNoSfx == null ? "" : x.IssueNoSfx,
                                 VehLicNo = x.VehLicNo,
                                 Beat = x.Beat,
                                 MeterNo = x.MeterNo == null ? "" : x.MeterNo,
                                 VioCode = x.VioCode == null ? "" : x.VioCode,
                                 isAutoBindReqd = x.isAutoBindReqd
                             });

                items = items.ApplyFiltering(request.Filters);
                total = items.Count();

                items = items.ApplySorting(request.Groups, request.Sorts);
                items = items.ApplyPaging(request.Page, request.PageSize);

                Enforcementsummary = items.ToList();

            }
            catch (Exception ex)
            {

            }
            return Enforcementsummary.AsQueryable();
        }

        /// <summary>
        ///  Description: This Method will retrive attachment file for particular CitationID
        ///   ModifiedBy: Santhosh  (19/Feb/2014 - 27/Feb/2014) 
        /// </summary>
        /// <param name="CitationID"></param>
        /// <returns></returns>
        public List<NoteModel> GetNotes(int CitationID)
        {
            var Notes = new List<NoteModel>();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                var enforcementNoteResult_raw = (from AI_PARK_NOTE in NewOrleansDataContext.PARKNOTEs
                                                 //join customer in NewOrleansDataContext.Customers
                                                 // on AI_PARK_NOTE.CustomerID equals customer.CustomerID
                                                 join ai_PARKING in NewOrleansDataContext.PARKINGs
                                                 on AI_PARK_NOTE.MASTERKEY equals ai_PARKING.UNIQUEKEY

                                                 where (ai_PARKING.ISSUENO == CitationID)
                                                 select new
                                                 {
                                                     ParkNoteId = AI_PARK_NOTE.UNIQUEKEY,
                                                     ParkingId = AI_PARK_NOTE.MASTERKEY,
                                                     // NoteDateTime = (DateTime)AI_PARK_NOTE.NoteDateTime,
                                                     // NoteDateTime = ConvertToDateTimeFormat(AI_PARK_NOTE.NOTEDATE, AI_PARK_NOTE.NOTETIME),
                                                     NoteDate = AI_PARK_NOTE.NOTEDATE,
                                                     NoteTime = AI_PARK_NOTE.NOTETIME,
                                                     NotesMemo = AI_PARK_NOTE.NOTESMEMO,
                                                     MultimediaNoteDataType = AI_PARK_NOTE.MULTIMEDIANOTEDATATYPE,
                                                     //CustomerId = AI_PARK_NOTE.CustomerID,
                                                     //CustomerName = customer.Name
                                                 }).ToList().Select(x => new NoteModel
                                                 {
                                                     ParkNoteId = x.ParkNoteId,
                                                     ParkingId = x.ParkingId,
                                                     NoteDateTime = ConvertToDateTimeFormat(x.NoteDate, x.NoteTime),
                                                     NotesMemo = x.NotesMemo,
                                                     MultimediaNoteDataType = x.MultimediaNoteDataType,
                                                     CustomerName = string.Empty
                                                 }).ToList();

                List<NoteModel> EnforcNotes = enforcementNoteResult_raw.ToList();

                if (EnforcNotes.Any())
                    Notes = EnforcNotes;
            }
            catch (Exception ex)
            {

            }
            return Notes;
        }

        /// <summary>
        ///  Description: This Method will retrive attachment file
        ///   ModifiedBy: Santhosh  (19/Feb/2014 - 27/Feb/2014) 
        /// </summary>
        /// <param name="ParkNoteId"></param>
        /// <returns></returns>
        public List<NoteModel> NoteAttachment(int ParkNoteId)
        {
            var NoteAttachmentdetails = new List<NoteModel>();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {
                var enforcementNoteResult_raw = (from AI_PARK_NOTE in NewOrleansDataContext.PARKNOTEs

                                                 where (AI_PARK_NOTE.UNIQUEKEY == ParkNoteId)
                                                 select new
                                                 {
                                                     ParkNoteId = AI_PARK_NOTE.UNIQUEKEY,
                                                     ParkingId = AI_PARK_NOTE.MASTERKEY,
                                                     //NoteDateTime = (DateTime)AI_PARK_NOTE.NoteDateTime,
                                                     NoteDate = AI_PARK_NOTE.NOTEDATE,
                                                     NoteTime = AI_PARK_NOTE.NOTETIME,
                                                     NotesMemo = AI_PARK_NOTE.NOTESMEMO,
                                                     MultimediaNoteDataType = AI_PARK_NOTE.MULTIMEDIANOTEDATATYPE,
                                                     MultimediaNoteData = AI_PARK_NOTE.MULTIMEDIANOTEDATA,
                                                     //CustomerId = AI_PARK_NOTE.CustomerID,

                                                 }).ToList();

                var enforcementNoteResult_result = (from x in enforcementNoteResult_raw
                                                    select new NoteModel
                                                    {
                                                        ParkNoteId = x.ParkNoteId,
                                                        ParkingId = x.ParkingId,
                                                        NoteDateTime = ConvertToDateTimeFormat(x.NoteDate, x.NoteTime),
                                                        NotesMemo = x.NotesMemo,
                                                        MultimediaNoteDataType = x.MultimediaNoteDataType,
                                                        MultimediaNoteData = x.MultimediaNoteData,
                                                    }).AsQueryable();

                List<NoteModel> EnforcNoteAttachment = enforcementNoteResult_result.ToList();

                if (EnforcNoteAttachment.Any())
                    NoteAttachmentdetails = EnforcNoteAttachment;
            }
            catch (Exception ex)
            {

            }
            return NoteAttachmentdetails;
        }

        /// <summary>
        ///  Description: This will add enforcement note to database
        ///   ModifiedBy: Santhosh  (25/AUG/2014)  
        /// </summary>
        /// <param name="CitationID"></param>
        /// <param name="filedata"></param>
        /// <param name="UploadBy"></param>
        /// <param name="MultimediaNoteDataType"></param>
        /// <param name="NoteMemo"></param>
        /// <returns></returns>
        public int UpdateFile(long CitationID, byte[] filedata, int UploadBy, string MultimediaNoteDataType, string NoteMemo)
        {

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            var Aipark = (from s in NewOrleansDataContext.PARKINGs
                          where s.ISSUENO == CitationID
                          select s).FirstOrDefault();


            PARKNOTE AI_PARK_NOTEobj = new PARKNOTE();
            //AI_PARK_NOTEobj.CustomerID = UploadBy;
            AI_PARK_NOTEobj.UNIQUEKEY = Aipark.UNIQUEKEY;
            AI_PARK_NOTEobj.OFFICERID = Aipark.OFFICERID;
            //AI_PARK_NOTEobj.NoteDateTime = DateTime.Now;
            AI_PARK_NOTEobj.NOTEDATE = DateTime.Now; //** Added for oracle
            AI_PARK_NOTEobj.NOTETIME = DateTime.Now; //** Added for oracle
            AI_PARK_NOTEobj.MULTIMEDIANOTEDATATYPE = MultimediaNoteDataType;
            AI_PARK_NOTEobj.MULTIMEDIANOTEDATA = filedata;
            AI_PARK_NOTEobj.NOTESMEMO = NoteMemo;
            NewOrleansDataContext.PARKNOTEs.Add(AI_PARK_NOTEobj);
            int save = NewOrleansDataContext.SaveChanges();
            return save;

        }

        /// <summary>
        ///  Description: This will change the status of a citation that has been previously been voided to active
        ///   ModifiedBy: Santhosh  (25/AUG/2014) 
        /// </summary>
        /// <param name="CitationID"></param>
        /// <returns></returns>
        public int SetREINSTATE(long CitationID)
        {
            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;
            int save = 0;

            try
            {
                var Aipark = (from s in NewOrleansDataContext.PARKINGs
                              where s.ISSUENO == CitationID
                              select s).FirstOrDefault();

                if (Aipark.VOIDSTATUS == null)
                {
                    return -1;
                }
                Aipark.VOIDSTATUS = null;
                Aipark.VOIDSTATUSDATE = null;
                save = NewOrleansDataContext.SaveChanges();
                return save;
            }
            catch (Exception e)
            {

            }

            return save;

        }

        /// <summary>
        ///  Description: This will change the status of an active citation to voided
        ///   ModifiedBy: Santhosh  (25/aUG/2014) 
        /// </summary>
        /// <param name="CitationID"></param>
        /// <returns></returns>
        public int SetVOID(long CitationID)
        {

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;
            int save = 0;
            try
            {
                var Aipark = (from s in NewOrleansDataContext.PARKINGs
                              where s.ISSUENO == CitationID
                              select s).ToList().FirstOrDefault();

                if (Aipark.VOIDSTATUS == "VO")
                {
                    return -1;
                }
                Aipark.VOIDSTATUS = "VO";
                Aipark.VOIDSTATUSDATE = DateTime.Now;
                save = NewOrleansDataContext.SaveChanges();
                return save;
            }
            catch (Exception e)
            {

            }
            return save;

        }
    }
}
