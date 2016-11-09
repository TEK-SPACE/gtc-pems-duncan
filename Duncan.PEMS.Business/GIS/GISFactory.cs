
/******************* CHANGELOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             __________________________________________________________________________________________________
 * 7/27/2014          Sairam                 Enhancement done in GIS - Meters page: Allow Demand zones for all three layers
 * 8/14/2014          Sairam                 Implemented new page (Citation) under GIS
 * *****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.OracleDB; //** step 1
using Duncan.PEMS.Entities.Events;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Utilities;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Duncan.PEMS.Entities.GIS;
using NLog;
using System.Data.Objects;
using Duncan.PEMS.Business.Exports;
using Duncan.PEMS.Entities.News;
using System.Data.Entity.Infrastructure;
using Duncan.PEMS.Entities;
using Oracle.ManagedDataAccess.Client;

namespace Duncan.PEMS.Business.GIS
{

    /// <summary>
    /// Description: The <see cref="Duncan.PEMS.Business.GIS"/> namespace contains classes for managing GIS related pages.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
        //
    }

    /// <summary>
    /// Description: Class encapsulating functionality for the GIS pages.    /// 
    /// Modified By: Sairam on May 27th 2014
    /// </summary>
    public class GISFactory : BaseFactory
    {

        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        OracleDbEntitiesAbstract NewOrleansDataContext = new OracleDbEntitiesAbstract("OracleDBEntities");

        /// <summary>
        /// Factory constructor taking a connection string name.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        public GISFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        /// <summary>
        /// Description: Get all the Asset Types 
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllData()
        {
            List<string> result = new List<string>();
            try
            {


                var assets = (from assetTypes in PemsEntities.MeterGroups
                              select assetTypes.MeterGroupDesc);
                result = assets.ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAllData", ex);
            }
            return result;
        }

        /// <summary>
        /// Description: It is used to populate the various types of (enabled) Assets available for a given customer based on the layer selected.
        /// Modified By: Sairam on Feb 25 2016       /// </summary>
        /// <param name="LayerID"></param>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public List<GISModel> GetAssetTypes(string LayerID, int customerID)
        {

            IQueryable<GISModel> assets = Enumerable.Empty<GISModel>().AsQueryable();
            List<GISModel> assetTypesData = new List<GISModel>();

            //** Show Meter Asset types (SSM / MSM) which are enabled for the customer
            try
            {

                if (LayerID == "2")
                {
                    //** Parking space
                    assets = (from assetTypes in PemsEntities.AssetTypes
                              where assetTypes.CustomerId == customerID && assetTypes.IsDisplay == true && assetTypes.MeterGroupId == 20
                              select new GISModel
                              {
                                  Text = assetTypes.MeterGroupDesc,
                                  Value = assetTypes.MeterGroupId,
                              }).Distinct();

                    assetTypesData = assets.ToList();
                }
                else
                {
                    assets = (from assetTypes in PemsEntities.AssetTypes
                              where assetTypes.CustomerId == customerID && assetTypes.IsDisplay == true
                              select new GISModel
                              {
                                  Text = assetTypes.MeterGroupDesc,
                                  Value = assetTypes.MeterGroupId,
                              }).Distinct();

                    //** First convert assets to list type
                    assetTypesData = assets.ToList();


                    //** Check Whether Smartcard is enabled or Not for the customer. If enabled, show Datakey and remove smartcard or vice versa.
                    bool isSmartCardEnabled = false;
                    for (var i = 0; i < assetTypesData.Count(); i++)
                    {
                        if (assetTypesData[i].Value == 12)
                        {
                            //** Smartcard is enabled, hence show datakey
                            isSmartCardEnabled = true;
                            break;
                        }
                        else
                        {
                            //** Smartcard is disabled, hence remove datakey
                            isSmartCardEnabled = false;

                        }
                    }

                    //*******************
                    //** Check Whether Smartcard is enabled or Not for the customer. If enabled, show Datakey and remove smartcard or vice versa.
                    bool isParkingSpaceEnabled = false;
                    for (var i = 0; i < assetTypesData.Count(); i++)
                    {
                        if (assetTypesData[i].Value == 20)
                        {
                            //** Parking Space is enabled, 
                            isParkingSpaceEnabled = true;
                            break;
                        }
                        else
                        {
                            //** Parking Space is disabled, 
                            isParkingSpaceEnabled = false;

                        }
                    }

                    //***************

                    if (isSmartCardEnabled == false)
                    {
                        //** Remove Datakey from the List;
                        for (var i = 0; i < assetTypesData.Count(); i++)
                        {
                            if (assetTypesData[i].Value == 32)
                            {
                                assetTypesData.RemoveAt(i);
                                break;
                            }

                        }

                    }
                    else
                    {
                        //** Remove SmartCard from the List;
                        for (var i = 0; i < assetTypesData.Count(); i++)
                        {
                            if (assetTypesData[i].Value == 12)
                            {
                                assetTypesData.RemoveAt(i);
                                break;
                            }

                        }
                    }

                    //**********
                    if (isParkingSpaceEnabled == false)
                    {
                        //** Remove Datakey from the List;
                        //for (var i = 0; i < assetTypesData.Count(); i++)
                        //{
                        //    if (assetTypesData[i].Value == 20)
                        //    {
                        //        assetTypesData.RemoveAt(i);
                        //        break;
                        //    }

                        //}

                    }
                    else
                    {
                        //** Remove parkingspace from the List;
                        for (var i = 0; i < assetTypesData.Count(); i++)
                        {
                            if (assetTypesData[i].Value == 20)
                            {
                                assetTypesData.RemoveAt(i);
                                break;
                            }

                        }
                    }
                    //**********

                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAssetTypes Method (GIS Reports)", ex);
            }
            return assetTypesData;

        }

        public List<GISModel> GetAssetTypes_HomePage(string LayerID, int customerID)
        {

            IQueryable<GISModel> assets = Enumerable.Empty<GISModel>().AsQueryable();
            List<GISModel> assetTypesData = new List<GISModel>();

            //** Show Meter Asset types (SSM / MSM) which are enabled for the customer
            try
            {

                if (LayerID == "2")
                {
                    //** Parking space
                    assets = (from assetTypes in PemsEntities.AssetTypes
                              where assetTypes.CustomerId == customerID && assetTypes.IsDisplay == true && assetTypes.MeterGroupId == 20
                              select new GISModel
                              {
                                  Text = assetTypes.MeterGroupDesc,
                                  Value = assetTypes.MeterGroupId,
                              }).Distinct();

                    assetTypesData = assets.ToList();
                }
                else
                {
                    assets = (from assetTypes in PemsEntities.AssetTypes
                              //where assetTypes.CustomerId == customerID && assetTypes.IsDisplay == true && (assetTypes.MeterGroupId != 12 && assetTypes.MeterGroupId != 13 && assetTypes.MeterGroupId != 20)
                              where assetTypes.CustomerId == customerID && assetTypes.IsDisplay == true
                              select new GISModel
                              {
                                  Text = assetTypes.MeterGroupDesc,
                                  Value = assetTypes.MeterGroupId,
                              }).Distinct();

                    //** First convert assets to list type
                    assetTypesData = assets.ToList();


                    //** Check Whether Smartcard is enabled or Not for the customer. If enabled, show Datakey and remove smartcard or vice versa.
                    bool isSmartCardEnabled = false;
                    for (var i = 0; i < assetTypesData.Count(); i++)
                    {
                        if (assetTypesData[i].Value == 12)
                        {
                            //** Smartcard is enabled, hence show datakey
                            isSmartCardEnabled = true;
                            break;
                        }
                        else
                        {
                            //** Smartcard is disabled, hence remove datakey
                            isSmartCardEnabled = false;

                        }
                    }

                    if (isSmartCardEnabled == false)
                    {
                        //** Remove Datakey from the List;
                        for (var i = 0; i < assetTypesData.Count(); i++)
                        {
                            if (assetTypesData[i].Value == 32)
                            {
                                assetTypesData.RemoveAt(i);
                                break;
                            }

                        }

                    }
                    else
                    {
                        //** Remove SmartCard from the List;
                        for (var i = 0; i < assetTypesData.Count(); i++)
                        {
                            if (assetTypesData[i].Value == 12)
                            {
                                assetTypesData.RemoveAt(i);
                                break;
                            }

                        }
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAssetTypes Method (GIS Reports)", ex);
            }
            return assetTypesData;

        }

        /// <summary>
        /// Description: This method is used
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public IQueryable<GISModel> GetLatLngOfCustomer(int customerID)
        {
            IQueryable<GISModel> Result = Enumerable.Empty<GISModel>().AsQueryable();

            try
            {
                Result = (from coord in PemsEntities.Customers
                          where coord.CustomerID == customerID
                          select new GISModel
                          {
                              Latitude = coord.Latitude,
                              Longitude = coord.Longitude
                          });
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetLatLngOfCustomer Method (GIS Reports - Index)", ex);
            }

            return Result;
        }

        /// <summary>
        /// Description: It is used to populate the various types of Assets available for financial revenue layer.
        /// Modified By: Sairam on4th july 2014        /// </summary>
        /// </summary>
        /// <param name="LayerID"></param>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public IQueryable<GISModel> GetAssetTypes_FinancialRevenueLayer(string LayerID, int customerID)
        {
            IQueryable<GISModel> assets = Enumerable.Empty<GISModel>().AsQueryable();

            try
            {

                //** Show Meter Asset types (SSM / MSM) which are enabled for the customer
                assets = (from assetTypes in PemsEntities.AssetTypes
                          where assetTypes.CustomerId == customerID && assetTypes.IsDisplay == true && (assetTypes.MeterGroupId == 0 || assetTypes.MeterGroupId == 1)
                          select new GISModel
                          {
                              Text = assetTypes.MeterGroupDesc,
                              Value = assetTypes.MeterGroupId,
                          }).Distinct();

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAssetTypes_FinancialRevenueLayer Method (GIS Reports - Revenue)", ex);
            }
            return assets;

        }


        /// <summary>
        /// Description:This method is used to compute the no. of citations issued by an officer for the selected date.
        /// Modified By: Sairam on July 25th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="startTimeOutput"></param>
        /// <param name="endTimeOutput"></param>
        /// <param name="officerID"></param>
        /// <returns></returns>
        //public IQueryable<GISModel> GetNoOfCitationIssued(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, int? officerID)
        //{
        //    IQueryable<GISModel> officers = Enumerable.Empty<GISModel>().AsQueryable();

        //    try
        //    {
        //        var officers_Res = (from A in PemsEntities.AI_PARKING
        //                            where A.OfficerID == officerID && A.CustomerID == CurrentCity && A.IssueDateTime >= startTimeOutput && A.IssueDateTime <= endTimeOutput
        //                            orderby
        //                              A.IssueDateTime
        //                            select new GISModel
        //                            {
        //                                LocLatitude = A.LocLatitude,
        //                                LocLongitude = A.LocLongitude,
        //                                activityDate = A.IssueDateTime,
        //                                LocationType = A.LocStreet,
        //                                Remark_1 = (A.Remark1 == null) ? "" : A.Remark1,
        //                                Remark_2 = (A.Remark2 == null) ? "" : A.Remark2,
        //                                Remark_3 = (A.Remark3 == null) ? "" : A.Remark3,
        //                                citationID = A.IssueNo
        //                            });

        //        officers = (from A in officers_Res
        //                    select new GISModel
        //                    {
        //                        LocLatitude = A.LocLatitude,
        //                        LocLongitude = A.LocLongitude,
        //                        activityDate = A.activityDate,
        //                        LocationType = A.LocationType,
        //                        Remark_1 = A.Remark_1,
        //                        Remark_2 = A.Remark_2,
        //                        Remark_3 = A.Remark_3,
        //                        citationID = A.citationID
        //                    });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("ERROR IN GetNoOfCitationIssued Method (GIS Reports - Officer Route)", ex);
        //    }

        //    return officers;
        //}

        public IQueryable<GISModel> GetNoOfCitationIssued(int customerId, string startDate, string endDate, string officerID)
        {
            IQueryable<GISModel> officers = Enumerable.Empty<GISModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                DateTime startTimeOutput;
                DateTime startTimeEndOutput;
                DateTime endTimeOutput;
                DateTime endTimeEndOutput;

                var T_index = startDate.IndexOf('T');
                var T_index_End = endDate.IndexOf('T');

                var Space_index = startDate.IndexOf(' ');
                var searchStr = ' ';
                string[] split_1 = new string[] { };
                string[] split_2 = new string[] { };
                string startDateAlone = string.Empty;
                string startTimeAlone = string.Empty;
                string endDateAlone = string.Empty;
                string endTimeAlone = string.Empty;

                if (T_index != -1)
                {
                    searchStr = 'T';
                    split_1 = startDate.Split(searchStr);
                    startDateAlone = split_1[0].ToString() + " 00:00:00";
                    startTimeAlone = "12/30/1899 " + split_1[1].ToString();

                }
                else
                {
                    searchStr = ' ';
                    split_1 = startDate.Split(searchStr);
                    startDateAlone = split_1[0].ToString() + " 00:00:00";
                    //  startTimeAlone = "12/30/1899 " + split_1[1].ToString() + " " + split_1[2].ToString();
                    startTimeAlone = "12/30/1899 " + "00:00:00";

                }

                if (T_index_End != -1)
                {
                    searchStr = 'T';
                    split_2 = endDate.Split(searchStr);
                    endDateAlone = split_2[0].ToString() + " 00:00:00";
                    endTimeAlone = "12/30/1899 " + split_2[1].ToString();
                    endTimeAlone = "12/30/1899 " + "23:59:59";

                }
                else
                {
                    searchStr = ' ';
                    split_2 = endDate.Split(searchStr);
                    endDateAlone = split_2[0].ToString() + " 00:00:00";
                    //endTimeAlone = "12/30/1899 " + split_2[1].ToString() + " " + split_2[2].ToString();
                    endTimeAlone = "12/30/1899 " + "11:59" + " " + "pm";
                }


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
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

                var officers_Res = (from A in NewOrleansDataContext.PARKINGs
                                    where (A.OFFICERID.Equals(officerID) &&
                                    (A.ISSUEDATE >= startTimeOutput && A.ISSUETIME >= startTimeEndOutput) &&
                                    (A.ISSUEDATE <= endTimeOutput && A.ISSUETIME <= endTimeEndOutput))
                                    orderby A.ISSUETIME
                                    select new
                                    {
                                        LocLatitude = A.LOCLATITUDE,
                                        LocLongitude = A.LOCLONGITUDE,
                                        activityDate = A.ISSUEDATE,
                                        activityTime = A.ISSUETIME,
                                        LocationType = A.LOCSTREET,
                                        Remark_1 = (A.REMARK1 == null) ? "" : A.REMARK1,
                                        Remark_2 = (A.REMARK2 == null) ? "" : A.REMARK2,
                                        Remark_3 = (A.REMARK3 == null) ? "" : A.REMARK3,
                                        citationID = A.ISSUENO
                                    }).ToList();

                officers = (from A in officers_Res
                            select new GISModel
                            {
                                LocLatitude = Convert.ToDouble(A.LocLatitude),
                                LocLongitude = Convert.ToDouble(A.LocLongitude),
                                activityDate = ConvertToDateTimeFormat(A.activityDate, A.activityTime),
                                LocationType = A.LocationType,
                                Remark_1 = A.Remark_1,
                                Remark_2 = A.Remark_2,
                                Remark_3 = A.Remark_3,
                                citationID = Convert.ToInt64(A.citationID)
                            }).AsQueryable();


               
            }
            catch (Exception ex)
            {
                // _logger.ErrorException("ERROR IN GetNoOfCitationIssued Method (GIS Reports - Officer Route)", ex);
            }

            return officers;
        }

        /// <summary>
        /// Description:This method is used to populate the Officer IDs for the selected customer
        /// Modified By: Sairam on June 12th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<GISModel> GetOfficerID(int CurrentCity)
        {
            IQueryable<GISModel> officers = Enumerable.Empty<GISModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                var raw_result = (from at in NewOrleansDataContext.SECURITY_USER
                                  //where off.CustomerID == CurrentCity
                                  orderby at.OFFICERID ascending
                                  where !String.IsNullOrEmpty(at.OFFICERID)
                                  select new
                                  {
                                      Text = at.OFFICERID,
                                      Value = at.OFFICERID
                                  }).Distinct().ToList();

                officers = (from off in raw_result
                            select new GISModel
                            {
                                Text = off.Text,
                                Value = Convert.ToInt64(off.Value),
                            }).AsQueryable().OrderBy(x => x.Value);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOfficerID Method (GIS Reports)", ex);
            }


            return officers;
        }

        /// <summary>
        /// Description:This method is used to populate the Officer Names for the selected customer
        /// Modified By: Sairam on June 11th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<GISModel> GetOfficerNames(int CurrentCity)
        {
            IQueryable<GISModel> officers = Enumerable.Empty<GISModel>().AsQueryable();

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            try
            {

                var raw_result = (from at in NewOrleansDataContext.SECURITY_USER
                                  //where off.CustomerID == CurrentCity
                                  orderby at.OFFICERNAME ascending
                                  where !String.IsNullOrEmpty(at.OFFICERNAME)
                                  select new
                                  {
                                      Text = at.OFFICERNAME,
                                      Value = at.OFFICERID
                                  }).Distinct().ToList();

                officers = (from off in raw_result
                            select new GISModel
                            {
                                Text = off.Text,
                                Value = Convert.ToInt64(off.Value),
                            }).AsQueryable().OrderBy(x => x.Text);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOfficerNames Method (GIS Reports)", ex);
            }

            return officers;
        }

        /// <summary>
        /// Description: This method is used to populate the enabled demand zones for the given customer.
        /// Modified By: Sairam on July 14th 2014.
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IQueryable<GISModel> GetDemandZones(int CurrentCity)
        {

            IQueryable<GISModel> assets = Enumerable.Empty<GISModel>().AsQueryable();

            try
            {


                //** Show only those demand zones which are enabled for the customer.
                assets = (from demandZones in PemsEntities.DemandZoneCustomers
                          join dZones in PemsEntities.DemandZones on
                          demandZones.DemandZoneId equals dZones.DemandZoneId
                          where demandZones.CustomerId == CurrentCity && demandZones.IsDisplay == true
                          select new GISModel
                          {
                              Text = dZones.DemandZoneDesc,
                              Value = dZones.DemandZoneId
                          });
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetDemandZones Method (GIS Reports)", ex);
            }

            return assets;
        }

        /// <summary>
        /// Description: It is used to populate the Asset Models for the selected Asset type in the financial revenue layer.
        /// Modified By: Sairam on 24th july 2014        /// </summary>
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public IQueryable<GISModel> GetAssetModels(int assetType, int CurrentCity)
        {
            IQueryable<GISModel> assets = Enumerable.Empty<GISModel>().AsQueryable();



            try
            {
                assets = (from assetModel in PemsEntities.MechanismMasterCustomers
                          join m in PemsEntities.Meters on new
                          {
                              customerid = assetModel.CustomerId,
                              mechid = assetModel.MechanismId
                          } equals new
                          {
                              customerid = m.CustomerID,
                              mechid = (int)m.MeterType
                          } into g
                          from A in g.DefaultIfEmpty()
                          where assetModel.CustomerId == CurrentCity && assetModel.IsDisplay == true && A.MeterGroup == assetType
                          group assetModel by assetModel.MechanismDesc into groupedResult

                          select new GISModel
                          {
                              Text = groupedResult.FirstOrDefault().MechanismDesc,//assetModel.MechanismDesc,
                              Value = groupedResult.FirstOrDefault().MechanismId//assetModel.MechanismId
                          });

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAssetModels Method (GIS Reports - Inventory / Revenue)", ex);
            }

            return assets;
        }


        /// <summary>
        /// Description:This method is used to populate the Status of Inventory
        /// Modified By: Sairam on May 21st 2014
        /// </summary>
        /// <returns></returns>
        public IQueryable<GISModel> GetAssetState()
        {
            IQueryable<GISModel> assets = Enumerable.Empty<GISModel>().AsQueryable();

            try
            {

                assets = (from assetTypes in PemsEntities.AssetStates

                          select new GISModel
                          {
                              Text = assetTypes.AssetStateDesc,
                              Value = assetTypes.AssetStateId
                          });
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAssetState Method (GIS Reports - Meters Inventory Layer)", ex);
            }

            return assets;
        }

        /// <summary>
        /// Description:This method is used to populate the Status of Occupancy
        /// Modified By: Sairam on May 21st 2014
        /// </summary>
        /// <returns></returns>
        public IQueryable<GISModel> GetOccupancyStatus()
        {
            IQueryable<GISModel> assets = Enumerable.Empty<GISModel>().AsQueryable();

            try
            {

                assets = (from assetTypes in PemsEntities.OccupancyStatus
                          where (assetTypes.StatusID == 1 || assetTypes.StatusID == 2)

                          select new GISModel
                          {
                              Text = assetTypes.StatusDesc,
                              Value = assetTypes.StatusID
                          });
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOccupancyStatus Method (GIS Reports - Meters - Demand layer)", ex);
            }

            return assets;
        }

        /// <summary>
        /// Description:This method is used to populate the Compliance Status for the Inventory.
        /// Modified By: Sairam on May 21st 2014
        /// </summary>
        /// <returns></returns>
        public List<GISModel> GetComplianceStatus()
        //  public IQueryable<GISModel> GetComplianceStatus()
        {
            IQueryable<GISModel> assets = Enumerable.Empty<GISModel>().AsQueryable();
            List<GISModel> finalList = new List<GISModel>();

            try
            {
                assets = (from assetTypes in PemsEntities.NonCompliantStatus

                          select new GISModel
                          {
                              Text = assetTypes.NonCompliantStatusDesc,
                              Value = assetTypes.NonCompliantStatusID
                          }).Distinct();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetComplianceStatus Method (GIS Reports - Meters - Demand Layer)", ex);
            }

            //** Sairam added code on 13th oct 2014
            finalList = assets.ToList();
            GISModel inst = new GISModel();
            inst.Text = "Compliant";
            inst.Value = 4;
            finalList.Insert(0, inst);
            return finalList;
        }


        /// <summary>
        /// Description:This method is used to populate the Operational Status for the Asset Operational Layer.
        /// Modified By: Sairam on May 21st 2014
        /// </summary>
        /// <returns></returns>
        public IQueryable<GISModel> GetOperationalStatus()
        {
            IQueryable<GISModel> assets = Enumerable.Empty<GISModel>().AsQueryable();

            try
            {
                assets = (from assetTypes in PemsEntities.OperationalStatus

                          select new GISModel
                          {
                              Text = assetTypes.OperationalStatusDesc,
                              Value = assetTypes.OperationalStatusId
                          });
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOperationalStatus Method (GIS Reports - Meters - Meter Operational layer)", ex);
            }

            return assets;
        }

        /// <summary>
        /// Description:This method is used to populate the Meter groups for the typed text.
        /// Modified By: Sairam on May 17th 2014
        /// </summary>
        /// <param name="meterGroupId"></param>
        /// <param name="searchby"></param>
        /// <param name="typedtext"></param>
        /// <returns></returns>
        public List<GISModel> GetAllMetersTypes(int meterGroupId, int currentCity, int searchby = 2, string typedtext = "")
        {
            List<GISModel> result = new List<GISModel>();

            try
            {

                if (searchby == 1)
                {
                    result = (from meter in PemsEntities.Meters
                              where (meter.CustomerID == currentCity && meter.MeterGroup == meterGroupId || meterGroupId == -1)
                                  && (meter.MeterName.Contains(typedtext))
                              select new GISModel

                              {
                                  Text = meter.MeterName,
                                  Value = meter.MeterId
                              }).Distinct().ToList();
                }
                else
                {
                    result = (from meter in PemsEntities.Meters
                              where (meter.CustomerID == currentCity && meter.MeterGroup == meterGroupId || meterGroupId == -1)
                                && SqlFunctions.StringConvert((double)meter.MeterId).Trim().Contains(typedtext)
                              select new GISModel

                              {
                                  Text = meter.MeterName,
                                  Value = meter.MeterId
                              }).Distinct().ToList();

                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAllMetersTypes Method (GIS Reports)", ex);
            }
            return result;
        }

        /// <summary>
        /// Description:This method is used to populate the Sensor Types for the typed text.
        /// Modified By: Sairam on May 17th 2014
        /// </summary>
        /// <param name="sensorType"></param>
        /// <param name="searchby"></param>
        /// <param name="typedtext"></param>
        /// <returns></returns>
        public List<GISModel> GetAllSensorsTypes(int sensorType, int searchby = 2, string typedtext = "")
        {
            List<GISModel> result = new List<GISModel>();

            try
            {

                if (searchby == 1)
                {
                    result = (from sensor in PemsEntities.Sensors

                              where (sensor.SensorType == sensorType || sensorType == -1)
                                && (sensor.SensorName.Contains(typedtext))
                              select new GISModel
                              {
                                  Text = sensor.SensorName,
                                  Value = sensor.SensorID
                              }).ToList();

                }
                else
                {
                    {
                        result = (from sensor in PemsEntities.Sensors
                                  where (sensor.SensorType == sensorType || sensorType == -1)
                                    && SqlFunctions.StringConvert((double)sensor.SensorID).Trim().Contains(typedtext)
                                  select new GISModel

                                  {
                                      Text = sensor.SensorName,
                                      Value = sensor.SensorID
                                  }).ToList();

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAllSensorsTypes Method (GIS Reports)", ex);
            }

            return result;

        }

        /// <summary>
        /// Description:This method is used to populate the Gateway Types for the typed text.
        /// Modified By: Sairam on May 17th 2014
        /// </summary>
        /// <param name="gateWayType"></param>
        /// <param name="searchby"></param>
        /// <param name="typedtext"></param>
        /// <returns></returns>
        public List<GISModel> GetAllGatewaysTypes(int gateWayType, int searchby = 2, string typedtext = "")
        {
            List<GISModel> result = new List<GISModel>();

            try
            {

                if (searchby == 1)
                {
                    result = (from gateway in PemsEntities.Gateways
                              where (gateway.GatewayType == gateWayType || gateWayType == -1)
                              && (gateway.Description.Contains(typedtext))
                              select new GISModel
                              {
                                  Text = gateway.Description,
                                  Value = gateway.GateWayID
                              }).ToList();

                }
                else
                {
                    result = (from gateway in PemsEntities.Gateways
                              where (gateway.GatewayType == gateWayType || gateWayType == -1)
                               && SqlFunctions.StringConvert((double)gateway.GateWayID).Trim().Contains(typedtext)
                              select new GISModel

                              {
                                  Text = gateway.Description,
                                  Value = gateway.GateWayID
                              }).ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAllGatewaysTypes Method (GIS Reports)", ex);
            }

            return result;

        }


        /// <summary>
        /// Description:This method is used to populate the Parking Space Types for the typed text.
        /// Modified By: Sairam on May 17th 2014
        /// </summary>
        /// <param name="SpaceType"></param>
        /// <param name="searchby"></param>
        /// <param name="typedtext"></param>
        /// <returns></returns>
        public List<GISModel> GetAllSpaceTypes(int SpaceType, int currentCity, int searchby = 2, string typedtext = "")
        {
            List<GISModel> result = new List<GISModel>();

            try
            {

                if (searchby == 1)
                {
                    var result_raw = (from spaces in PemsEntities.ParkingSpaces
                                      where (spaces.CustomerID == currentCity && spaces.ParkingSpaceType == SpaceType || SpaceType == -1)
                                        && (spaces.DisplaySpaceNum.Contains(typedtext))
                                      select new
                                      {
                                          Text = spaces.MeterId.ToString(),
                                          //Value = spaces.ParkingSpaceId
                                          Value = spaces.MeterId
                                      });

                    result = (from res in result_raw
                              select new GISModel
                              {
                                  Text = res.Text,
                                  Value = (long)res.Value
                              }).ToList();

                }
                else
                {
                    var result_raw = (from spaces in PemsEntities.ParkingSpaces
                                      where (spaces.CustomerID == currentCity && spaces.ParkingSpaceType == SpaceType || SpaceType == -1)
                                     && SqlFunctions.StringConvert((double)spaces.ParkingSpaceId, 64).Trim().Contains(typedtext)
                                      select new
                                      {
                                          Text = spaces.MeterId.ToString(),
                                          //Value = spaces.ParkingSpaceId
                                          Value = spaces.MeterId
                                      });

                    result = (from res in result_raw
                              select new GISModel
                              {
                                  Text = res.Text,
                                  Value = (long)res.Value
                              }).ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAllSpaceTypes Method (GIS Reports)", ex);
            }

            return result;

        }


        public string GetAssetName_ParkingSpaces(long outNum)
        {
            string result = string.Empty;

            try
            {
                result = PemsEntities.ParkingSpaces.Where(a => a.ParkingSpaceId == outNum).Select(a => a.DisplaySpaceNum).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAssetName_ParkingSpaces Method (GIS Reports)", ex);
            }

            return result;


        }

        public string GetAssetName_Meters(int outNum)
        {
            string result = string.Empty;

            try
            {
                result = PemsEntities.Meters.Where(a => a.MeterId == outNum).Select(a => a.MeterName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAssetName_Meters Method (GIS Reports)", ex);
            }

            return result;

        }

        public string GetAssetID_Meters(string meterName)
        {
            string result = string.Empty;

            try
            {
                result = PemsEntities.Meters.Where(a => a.MeterName == meterName).Select(a => a.MeterId).FirstOrDefault().ToString();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAssetName_Meters Method (GIS Reports)", ex);
            }

            return result;

        }


        /// <summary>
        /// Description:This method is used to populate the IDs for the given location types (Area/Zone/Street/Suburb/DemandArea).
        /// Modified By: Sairam on May 19th 2014
        /// </summary>
        /// <param name="locationType"></param>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IEnumerable<GISModel> GetLocationTypeId(string locationType, int CurrentCity)
        {
            IEnumerable<GISModel> details = Enumerable.Empty<GISModel>();

            try
            {

                if (locationType == "Area")
                {
                    details = (from area in PemsEntities.Areas
                               where area.CustomerID == CurrentCity && area.AreaID != null
                               where (area.AreaID != 99 && area.AreaID != 98 && area.AreaID != 97 && area.AreaID != 1)
                               select new GISModel
                               {
                                   Value = area.AreaID,
                                   Text = area.AreaName
                               }).Distinct(); //** Sairam added this line on Oct 7th 2014 to produce unique items


                }
                else if (locationType == "Zone")
                {

                    details = (from zone in PemsEntities.Zones
                               where zone.customerID == CurrentCity && zone.ZoneId != null
                               select new GISModel
                               {
                                   Value = zone.ZoneId,
                                   Text = zone.ZoneName
                               }).Distinct(); //** Sairam added this line on Oct 7th 2014 to produce unique items

                }
                else if (locationType == "Street")
                {
                    details = (from meter in PemsEntities.Meters
                               where meter.CustomerID == CurrentCity && meter.Location != null
                               select new GISModel
                               {
                                   Text = meter.Location,
                               }).Distinct(); //** Sairam added this line on Oct 7th 2014 to produce unique items
                }
                else if (locationType == "Suburb")
                {
                    details = (from customGroup in PemsEntities.CustomGroup1
                               where customGroup.CustomerId == CurrentCity && customGroup.DisplayName != null
                               select new GISModel
                               {
                                   Text = customGroup.DisplayName
                               }).Distinct(); //** Sairam added this line on Oct 7th 2014 to produce unique items


                }
                else if (locationType == "Demand Area")
                {
                    details = (from demandZone in PemsEntities.DemandZones
                               select new GISModel
                               {
                                   Text = demandZone.DemandZoneDesc
                               }).Distinct(); //** Sairam added this line on Oct 7th 2014 to produce unique items
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetLocationTypeId Method (GIS Reports)", ex);
            }


            return details;
        }





        //public IEnumerable<GISModel> GetLocationTypeId_latest(string locationType, string typedLoc, int CurrentCity)
        //{
        //    IEnumerable<GISModel> details = Enumerable.Empty<GISModel>();

        //    try
        //    {

        //        if (locationType == "Area")
        //        {
        //            details = (from area in PemsEntities.Areas
        //                       where area.CustomerID == CurrentCity && SqlFunctions.StringConvert((double)area.AreaID).Trim().Contains(typedLoc)
        //                       select new GISModel
        //                       {
        //                           Value = area.AreaID,
        //                       });

        //        }
        //        else if (locationType == "Zone")
        //        {

        //            details = (from zone in PemsEntities.Zones
        //                       where zone.customerID == CurrentCity && SqlFunctions.StringConvert((double)zone.ZoneId).Trim().Contains(typedLoc)
        //                       select new GISModel
        //                       {
        //                           Value = zone.ZoneId
        //                       });

        //        }
        //        else if (locationType == "Street")
        //        {
        //            details = (from meter in PemsEntities.Meters
        //                       where meter.CustomerID == CurrentCity && meter.Location.Contains(typedLoc)
        //                       select new GISModel
        //                       {
        //                           Text = meter.Location
        //                       });
        //        }
        //        else if (locationType == "Suburb")
        //        {
        //            details = (from customGroup in PemsEntities.CustomGroup1
        //                       where customGroup.CustomerId == CurrentCity && customGroup.DisplayName.Contains(typedLoc)
        //                       select new GISModel
        //                       {
        //                           Text = customGroup.DisplayName
        //                       });


        //        }
        //        else if (locationType == "Demand Area")
        //        {
        //            details = (from demandZone in PemsEntities.DemandZones
        //                       select new GISModel
        //                       {
        //                           Text = demandZone.DemandZoneDesc
        //                       });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("ERROR IN GetLocationTypeId_latest Method (GIS Reports)", ex);
        //    } 


        //    return details;
        //}

        /// <summary>
        /// Description:This method is used to populate the Officer Last reported locations for the 'Officer Current Position' GIS report when Other than 'All' option is chosen.
        /// Modified By: Sairam on July 11th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<GISOfficerLocationModel> GetOfficerCurrentLocationsOther([DataSourceRequest]DataSourceRequest request, int CurrentCity, List<string> ids, out int total)
        {
            IEnumerable<GISOfficerLocationModel> result = Enumerable.Empty<GISOfficerLocationModel>();
            total = 0;

            string paramValues = string.Empty;

            var spParams = GetSpParams(request, "officerID desc", out paramValues);
            string startDate = spParams[3].Value.ToString();
            //string endDate = spParams[4].Value.ToString();
            string endDate = spParams[3].Value.ToString();
            string officerID = spParams[5].Value.ToString();

            try
            {

                DateTime startTimeOutput;
                DateTime startTimeEndOutput;
                DateTime endTimeOutput;
                DateTime endTimeEndOutput;

                var T_index = startDate.IndexOf('T');
                var T_index_End = endDate.IndexOf('T');

                var Space_index = startDate.IndexOf(' ');
                var searchStr = ' ';
                string[] split_1 = new string[] { };
                string[] split_2 = new string[] { };
                string startDateAlone = string.Empty;
                string startTimeAlone = string.Empty;
                string endDateAlone = string.Empty;
                string endTimeAlone = string.Empty;

                if (T_index != -1)
                {
                    searchStr = 'T';
                    split_1 = startDate.Split(searchStr);
                    startDateAlone = split_1[0].ToString() + " 00:00:00";
                    startTimeAlone = "12/30/1899 " + split_1[1].ToString();

                }
                else
                {
                    searchStr = ' ';
                    split_1 = startDate.Split(searchStr);
                    startDateAlone = split_1[0].ToString() + " 00:00:00";
                    //startTimeAlone = "12/30/1899 " + split_1[1].ToString() + " " + split_1[2].ToString();
                    startTimeAlone = "12/30/1899 " + "00:00:00";

                }

                if (T_index_End != -1)
                {
                    searchStr = 'T';
                    split_2 = endDate.Split(searchStr);
                    endDateAlone = split_2[0].ToString() + " 00:00:00";
                    // endTimeAlone = "12/30/1899 " + split_2[1].ToString();
                    endTimeAlone = "12/30/1899 " + "23:59:59";

                }
                else
                {
                    searchStr = ' ';
                    split_2 = endDate.Split(searchStr);
                    endDateAlone = split_2[0].ToString() + " 00:00:00";
                    //endTimeAlone = "12/30/1899 " + split_2[1].ToString() + " " + split_2[2].ToString();
                    endTimeAlone = "12/30/1899 " + "11:59" + " " + "pm";
                }


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
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

                //** To avoid Timeout Error in EF 5.0, use the below command
                ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

                var items_raw = (from A in NewOrleansDataContext.ACTIVITYLOGs
                                 where
                                 ((A.ENDDATE >= startTimeOutput && A.ENDTIME >= startTimeEndOutput) &&
                                 (A.ENDDATE <= endTimeOutput && A.ENDTIME <= endTimeEndOutput)) &&
                                 ids.Contains(A.OFFICERID)
                                 group A by A.OFFICERID into g
                                 select new
                                 {
                                     officerId = g.Key,
                                     members = g
                                 }).OrderBy(x => x.officerId).ThenBy(p => p.members.Max(a => a.ENDDATE)).ThenBy(q => q.members.Max(b => b.ENDTIME)).ToList();

                result = (from O in items_raw
                          select new GISOfficerLocationModel
                          {
                              officerName = O.members.FirstOrDefault().OFFICERNAME,
                              officerID = Convert.ToInt32(O.officerId),
                              activityDateTime = ConvertToDateTimeFormat(O.members.Max(x => x.ENDDATE), O.members.Max(x => x.ENDTIME)),
                              officerActivity = O.members.FirstOrDefault().PRIMARYACTIVITYNAME,
                              activityDate = ConvertToDateTimeFormat(O.members.Max(x => x.ENDDATE), O.members.Max(x => x.ENDTIME)),
                              Latitude = Convert.ToDouble(O.members.FirstOrDefault().END_LOCLATITUDE),
                              Longitude = Convert.ToDouble(O.members.FirstOrDefault().END_LOCLONGITUDE)
                          }).OrderBy(x => x.officerID).ToList();



                IQueryable<GISOfficerLocationModel> items = result.AsQueryable<GISOfficerLocationModel>();
                total = items.Count();
                items = items.ApplySorting(request.Groups, request.Sorts);
                items = items.ApplyPaging(request.Page, request.PageSize);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOfficerCurrentLocationsALL Method (GIS Reports - Officer Current Location)", ex);
            }

            return result;

            //IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();

            ////** To avoid Timeout Error in EF 5.0, use the below command
            //((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            //total = 0;

            //string paramValues = string.Empty;

            //var spParams = GetSpParams(request, "officerID desc", out paramValues);
            //string startDate = spParams[3].Value.ToString();
            //string endDate = spParams[4].Value.ToString();
            //string officerID = spParams[5].Value.ToString();

            //try
            //{

            //    //IQueryable<GISModel> items = (from O in NewOrleansDataContext.ACTIVITYLOGs
            //    //                              join I in
            //    //                                  (
            //    //                                      (from I0 in NewOrleansDataContext.ACTIVITYLOGs
            //    //                                       //where I0.CustomerID == CurrentCity && I0.StartLocLatitude != 0
            //    //                                       where I0.START_LOCLATITUDE != 0
            //    //                                       group I0 by new
            //    //                                       {
            //    //                                           I0.OFFICERID
            //    //                                          // I0.CustomerID,
            //    //                                       } into g
            //    //                                       select new
            //    //                                       {
            //    //                                          // CustomerID = g.Key.CustomerID,
            //    //                                           OfficerId = g.Key.OFFICERID,
            //    //                                           enddate = (DateTime?)g.Max(p => p.ENDDATE)
            //    //                                       }))
            //    //                                  // on new { O.CustomerID, O.OfficerId, enddate = O.EndDateTime }
            //    //                                   on new { O.OFFICERID, enddate = O.ENDDATE }
            //    //                      //equals new { I.CustomerID, I.OfficerId, I.enddate }
            //    //                      equals new {I.OfficerId, I.enddate }
            //    //                              //where O.CustomerID == CurrentCity && ids.Contains(O.OfficerId) && O.StartLocLatitude != 0 //&& O.StartLocLongitude != 0
            //    //                              where ids.Contains(O.OFFICERID) && O.START_LOCLATITUDE != 0 //&& O.StartLocLongitude != 0
            //    //                              orderby
            //    //                                //O.EndDateTime descending
            //    //                                O.ENDDATE descending

            //    //                              select new GISModel
            //    //                              {
            //    //                                  //CustomerID = O.CustomerID,
            //    //                                 // officerName = O.AI_OFFICERS.OfficerName,
            //    //                                  officerName = O.OFFICERNAME,
            //    //                                  officerID = O.OFFICERID,
            //    //                                  //activityDate = O.StartDateTime,
            //    //                                  activityDate = O.STARTDATE,
            //    //                                  //officerActivity = O.PrimaryActivityName,
            //    //                                  officerActivity = O.PRIMARYACTIVITYNAME,
            //    //                                  Latitude = O.START_LOCLATITUDE,
            //    //                                  Longitude = O.START_LOCLONGITUDE,
            //    //                              }).Distinct().OrderByDescending(a => a.activityDate);
            //    //result = items.ToList();
            //    //items = result.AsQueryable();
            //    ////  items = items.ApplyFiltering(request.Filters);
            //    //total = items.Count();
            //    //items = items.ApplySorting(request.Groups, request.Sorts);
            //    //items = items.ApplyPaging(request.Page, request.PageSize);
            //    //result = items.ToList();

            //}
            //catch (Exception ex)
            //{
            //    _logger.ErrorException("ERROR IN GetOfficerCurrentLocationsOther Method (GIS Reports - Officer Current Location)", ex);
            //}

            //return result;

        }

        /// <summary>
        /// Description:This method is used to populate the Officer Last reported locations for the Officer Current Position GIS report when 'ALL' officers are selected..
        /// Modified By: Sairam on July 11th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IEnumerable<GISOfficerLocationModel> GetOfficerCurrentLocationsALL([DataSourceRequest]DataSourceRequest request, int CurrentCity, out int total)
        {
            List<GISOfficerLocationModel> result = new List<GISOfficerLocationModel>();
            total = 0;
            
            string paramValues = string.Empty;

            var spParams = GetSpParams(request, "officerID desc", out paramValues);
            string startDate = spParams[3].Value.ToString();
            //string endDate = spParams[4].Value.ToString();
            string endDate = spParams[3].Value.ToString();
            string officerID = spParams[5].Value.ToString();

            try
            {

                DateTime startTimeOutput;
                DateTime startTimeEndOutput;
                DateTime endTimeOutput;
                DateTime endTimeEndOutput;

                var T_index = startDate.IndexOf('T');
                var T_index_End = endDate.IndexOf('T');

                var Space_index = startDate.IndexOf(' ');
                var searchStr = ' ';
                string[] split_1 = new string[] { };
                string[] split_2 = new string[] { };
                string startDateAlone = string.Empty;
                string startTimeAlone = string.Empty;
                string endDateAlone = string.Empty;
                string endTimeAlone = string.Empty;

                if (T_index != -1)
                {
                    searchStr = 'T';
                    split_1 = startDate.Split(searchStr);
                    startDateAlone = split_1[0].ToString() + " 00:00:00";
                    startTimeAlone = "12/30/1899 " + split_1[1].ToString();

                }
                else
                {
                    searchStr = ' ';
                    split_1 = startDate.Split(searchStr);
                    startDateAlone = split_1[0].ToString() + " 00:00:00";
                    startTimeAlone = "12/30/1899 " + "00:00:00";

                }

                if (T_index_End != -1)
                {
                    searchStr = 'T';
                    split_2 = endDate.Split(searchStr);
                    endDateAlone = split_2[0].ToString() + " 00:00:00";
                    // endTimeAlone = "12/30/1899 " + split_2[1].ToString();
                    endTimeAlone = "12/30/1899 " + "23:59:59";

                }
                else
                {
                    searchStr = ' ';
                    split_2 = endDate.Split(searchStr);
                    endDateAlone = split_2[0].ToString() + " 00:00:00";
                    //endTimeAlone = "12/30/1899 " + split_2[1].ToString() + " " + split_2[2].ToString();
                    endTimeAlone = "12/30/1899 " + "11:59" + " " + "pm";
                }


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
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

                var user = HttpContext.Current.User.Identity.Name;
                string connectionString, reportQuery;
                string factory = "GIS";
                using (ReportingQueryEntities context = new ReportingQueryEntities())
                {
                    var entity = (from up in context.UserProfiles
                        join uca in context.UserCustomerAccesses on up.UserId equals uca.UserId
                        join cs in context.CustomerSettings on uca.CustomerId equals cs.CustomerId
                        join cp in context.CustomerProfiles on cs.CustomerId equals cp.CustomerId
                        where up.UserName == user && cp.CustomerId == CurrentCity
                        select new {cp.CustomerId, cp.ConnectionStringName, cp.DisplayName}
                    ).FirstOrDefault();


                    connectionString = entity?.ConnectionStringName;
                    reportQuery = (from rq in context.ReportQueries
                        join rt in context.ReportTypes on rq.ReportId equals rt.Id
                        where rt.Name == factory && rq.CustomerId == entity.CustomerId
                        select rq.Query).FirstOrDefault();
                }

                //** To avoid Timeout Error in EF 5.0, use the below command
                    ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

                //string connectionString = "DATA SOURCE=aipro2;PASSWORD=atOu!8%2n;PERSIST SECURITY INFO=True;USER ID=VGANESAN";
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    var oracleCommand =
                        new OracleCommand(reportQuery, connection);
                    oracleCommand.Parameters.Add(new OracleParameter("STARTTIMEOUTPUT", startTimeOutput.ToString("dd-MMM-yyyy")));
                    oracleCommand.Parameters.Add(new OracleParameter("STARTTIMEENDOUTPUT", startTimeEndOutput.ToString("dd-MMM-yyyy")));
                    oracleCommand.Parameters.Add(new OracleParameter("ENDTIMEOUTPUT", endTimeOutput.ToString("dd-MMM-yyyy")));
                    oracleCommand.Parameters.Add(new OracleParameter("ENDTIMEENDOUTPUT", endTimeEndOutput.ToString("dd-MMM-yyyy")));
                    connection.Open();

                    using (OracleDataReader row = oracleCommand.ExecuteReader())
                    {
                        while (row.Read())
                        {
                            result.Add(new GISOfficerLocationModel
                            {
                                officerName = row["OFFICERNAME"].ToString(),
                                officerID = Convert.ToInt32(row["officerId"]),
                                activityDateTime =
                                   Convert.ToDateTime(Convert.ToDateTime(row["ENDDATE"]).ToString("MM/dd/yyyy") + " " + Convert.ToDateTime(row["ENDTIME"]).ToString("hh:mm:ss")),
                                activityDate =
                                    Convert.ToDateTime(Convert.ToDateTime(row["ENDDATE"]).ToString("MM/dd/yyyy") + " " + Convert.ToDateTime(row["ENDTIME"]).ToString("hh:mm:ss")),
                                officerActivity = row["PRIMARYACTIVITYNAME"].ToString(),
                                Latitude =
                                    row["END_LOCLATITUDE"].ToString().Length > 0
                                        ? Convert.ToDouble(row["END_LOCLATITUDE"].ToString())
                                        : 0,
                                Longitude =
                                    row["END_LOCLONGITUDE"].ToString().Length > 0
                                        ? Convert.ToDouble(row["END_LOCLONGITUDE"].ToString())
                                        : 0
                            });
                        }
                    }
                    connection.Close();
                }

                //var query = (from A in NewOrleansDataContext.ACTIVITYLOGs
                //        where
                //        ((A.ENDDATE >= startTimeOutput && A.ENDTIME >= startTimeEndOutput) &&
                //         (A.ENDDATE <= endTimeOutput && A.ENDTIME <= endTimeEndOutput))
                //        group A by A.OFFICERID
                //        into g
                //        select new
                //        {
                //            officerId = g.Key,
                //            members = g
                //        }).OrderBy(x => x.officerId)
                //    .ThenBy(p => p.members.Max(a => a.ENDDATE))
                //    .ThenBy(q => q.members.Max(b => b.ENDTIME));

                //var items_raw = query.ToList();

                //result = (from O in items_raw
                //          select new GISOfficerLocationModel
                //          {
                //              officerName = O.members.FirstOrDefault().OFFICERNAME,
                //              officerID = Convert.ToInt32(O.officerId),
                //              activityDateTime = ConvertToDateTimeFormat(O.members.Max(x => x.ENDDATE), O.members.Max(x => x.ENDTIME)),
                //              activityDate = ConvertToDateTimeFormat(O.members.Max(x => x.ENDDATE), O.members.Max(x => x.ENDTIME)),
                //              officerActivity = O.members.FirstOrDefault().PRIMARYACTIVITYNAME,
                //              Latitude = Convert.ToDouble(O.members.FirstOrDefault().END_LOCLATITUDE),
                //              Longitude = Convert.ToDouble(O.members.FirstOrDefault().END_LOCLONGITUDE)
                //          }).OrderBy(x => x.officerID).ToList();



                IQueryable<GISOfficerLocationModel> items = result.AsQueryable<GISOfficerLocationModel>();
                total = items.Count();
                items = items.ApplySorting(request.Groups, request.Sorts);
                items = items.ApplyPaging(request.Page, request.PageSize);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOfficerCurrentLocationsALL Method (GIS Reports - Officer Current Location)", ex);
            }

            return result;

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

        /// <summary>
        /// Description:This method is used to populate the grid with the route information (such as lat/lng, activity) travelled by the officer for the selected date
        /// Modified By: Sairam on July 15th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="startTimeOutput"></param>
        /// <param name="endTimeOutput"></param>
        /// <param name="officerID"></param>
        /// <returns></returns>
        public List<GISOfficerRouteModel> GetOfficerRouteDetails(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, string officerID, [DataSourceRequest]DataSourceRequest request, out int total)
        {

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            List<GISOfficerRouteModel> result = new List<GISOfficerRouteModel>();
            total = 0;

            string paramValues = string.Empty;

            var spParams = GetSpParams(request, "officerID desc", out paramValues);
            string startDate = spParams[3].Value.ToString();
            string endDate = spParams[4].Value.ToString();

            try
            {

                //DateTime startTimeOutput;
                DateTime startTimeEndOutput;
                //DateTime endTimeOutput;
                DateTime endTimeEndOutput;

                var T_index = startDate.IndexOf('T');
                var T_index_End = endDate.IndexOf('T');

                var Space_index = startDate.IndexOf(' ');
                var searchStr = ' ';
                string[] split_1 = new string[] { };
                string[] split_2 = new string[] { };
                string startDateAlone = string.Empty;
                string startTimeAlone = string.Empty;
                string endDateAlone = string.Empty;
                string endTimeAlone = string.Empty;

                if (T_index != -1)
                {
                    searchStr = 'T';
                    split_1 = startDate.Split(searchStr);
                    startDateAlone = split_1[0].ToString() + " 00:00:00";
                    startTimeAlone = "12/30/1899 " + split_1[1].ToString();

                }
                else
                {
                    searchStr = ' ';
                    split_1 = startDate.Split(searchStr);
                    startDateAlone = split_1[0].ToString() + " 00:00:00";
                    startTimeAlone = "12/30/1899 " + split_1[1].ToString() + " " + split_1[2].ToString();
                    //startTimeAlone = "12/30/1899 " + "00:00:00";

                }

                if (T_index_End != -1)
                {
                    searchStr = 'T';
                    split_2 = endDate.Split(searchStr);
                    endDateAlone = split_2[0].ToString() + " 00:00:00";
                    endTimeAlone = "12/30/1899 " + split_2[1].ToString();
                    //endTimeAlone = "12/30/1899 " + "23:59:59";

                }
                else
                {
                    searchStr = ' ';
                    split_2 = endDate.Split(searchStr);
                    endDateAlone = split_2[0].ToString() + " 00:00:00";
                    endTimeAlone = "12/30/1899 " + split_2[1].ToString() + " " + split_2[2].ToString();
                    //endTimeAlone = "12/30/1899 " + "11:59" + " " + "pm";
                }


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
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


                var raw_result = (from A in NewOrleansDataContext.ACTIVITYLOGs
                                  where A.OFFICERID.Equals(officerID) &&
                                  ((A.STARTDATE >= startTimeOutput && A.STARTTIME >= startTimeEndOutput) &&
                                  (A.STARTDATE <= endTimeOutput && A.STARTTIME <= endTimeEndOutput))
                                  orderby A.STARTDATE, A.STARTTIME
                                  select new
                                  {
                                      //CustomerID = A.CustomerID,
                                      officerID = A.OFFICERID,
                                      activityStartDate = A.STARTDATE,
                                      activityStartTime = A.STARTTIME,
                                      activityEndDate = A.ENDDATE,
                                      activityEndTime = A.ENDTIME,
                                      startDateTime = A.STARTDATE,
                                      endDateTime = A.ENDDATE,
                                      //  endTime = SqlMethods.

                                      officerName = A.OFFICERNAME,
                                      officerActivity = A.PRIMARYACTIVITYNAME,
                                      Latitude = A.START_LOCLATITUDE,
                                      Longitude = A.START_LOCLONGITUDE
                                  }).ToList();


                result = (from r in raw_result
                          select new GISOfficerRouteModel
                          {
                              // CustomerID = r.CustomerID,
                              officerID = Convert.ToInt32(r.officerID),
                              dateOnly = Convert.ToString(r.startDateTime.Value.ToShortDateString()),
                              startTime = Convert.ToString(r.activityStartTime.Value.ToString("h:mm tt")),
                              endTime = (r.endDateTime.Value.AddDays(((r.activityEndTime.Value - r.activityStartTime.Value).Seconds) / 86400)).ToString("hh:mm:ss tt"),
                              officerName = r.officerName,
                              officerActivity = r.officerActivity,
                              Latitude = Convert.ToDouble(r.Latitude),
                              Longitude = Convert.ToDouble(r.Longitude)
                          }).ToList();


                var items = result.AsQueryable();
                total = items.Count();
                items = items.ApplySorting(request.Groups, request.Sorts);
                items = items.ApplyPaging(request.Page, request.PageSize);
                result = items.ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOfficerRouteDetails Method (GIS Reports - Officer Route)", ex);
            }

            return result;
        }

        /// <summary>
        /// Description: This method is used to populate and display citations for the selected Officers in the grid for Citations GIS Layer
        /// Modified By: Sairam on 14th Aug 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="startTimeOutput"></param>
        /// <param name="endTimeOutput"></param>
        /// <param name="officerID"></param>
        /// <returns></returns>
        public List<GISCitationModel> GetCitationForParkingOfficer(DataSourceRequest request, int CurrentCity, out int total)
        {
            string officerIDAll = "";
            List<GISCitationModel> result = new List<GISCitationModel>(); //** Query Results are sent to this item.

            //** To avoid Timeout Error in EF 5.0, use the below command
            ((IObjectContextAdapter)NewOrleansDataContext).ObjectContext.CommandTimeout = 3600;

            total = 0;
            string paramValues = string.Empty;

            var spParams = GetSpParams(request, "officerID desc", out paramValues);
            string startDate = spParams[3].Value.ToString();
            string endDate = spParams[4].Value.ToString();
            string officerID = spParams[5].Value.ToString();

            DateTime startTimeOutput;
            DateTime endTimeOutput;
            DateTime startTimeEndOutput;
            DateTime endTimeEndOutput;



            try
            {
                var T_index = startDate.IndexOf('T');
                var T_index_End = endDate.IndexOf('T');

                var Space_index = startDate.IndexOf(' ');
                var searchStr = ' ';
                string[] split_1 = new string[] { };
                string[] split_2 = new string[] { };
                string startDateAlone = string.Empty;
                string startTimeAlone = string.Empty;
                string endDateAlone = string.Empty;
                string endTimeAlone = string.Empty;

                if (T_index != -1)
                {
                    searchStr = 'T';
                    split_1 = startDate.Split(searchStr);
                    startDateAlone = split_1[0].ToString() + " 00:00:00";
                    startTimeAlone = "12/30/1899 " + split_1[1].ToString();

                }
                else
                {
                    searchStr = ' ';
                    split_1 = startDate.Split(searchStr);
                    startDateAlone = split_1[0].ToString() + " 00:00:00";
                    startTimeAlone = "12/30/1899 " + split_1[1].ToString() + " " + split_1[2].ToString();

                }

                if (T_index_End != -1)
                {
                    searchStr = 'T';
                    split_2 = endDate.Split(searchStr);
                    endDateAlone = split_2[0].ToString() + " 00:00:00";
                    endTimeAlone = "12/30/1899 " + split_2[1].ToString();

                }
                else
                {
                    searchStr = ' ';
                    split_2 = endDate.Split(searchStr);
                    endDateAlone = split_2[0].ToString() + " 00:00:00";
                    endTimeAlone = "12/30/1899 " + split_2[1].ToString() + " " + split_2[2].ToString();
                }


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
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

                //** Convert AssetType to int list type before invoking factory class
                string[] officerIDArr = officerID.Split(',');
                List<string> officerIDList = new List<string>();
                for (var i = 0; i < officerIDArr.Length; i++)
                {
                    if (i == 0 && officerIDArr[0] == "All")
                    {
                        officerIDList.Add("-1");
                    }
                    else
                    {
                        officerIDList.Add(officerIDArr[i].Trim());
                    }

                }

                if (officerIDList[0] == "-1")
                {
                    officerIDAll = string.Empty;
                }
                else
                {
                    officerIDAll = "-1";
                }

                string[] myOfficers = officerIDList.ToArray();

                var items_raw = (from A in NewOrleansDataContext.PARKINGs
                                 join officer in NewOrleansDataContext.SECURITY_USER on A.OFFICERID equals officer.OFFICERID
                                 //where (officerIDAll == "-1" || officerIDList.Contains(A.OFFICERID)) && A.CustomerID == CurrentCity && A.IssueDateTime >= startTimeOutput && A.IssueDateTime <= endTimeOutput && A.LocLatitude != 0 && A.LocLongitude != 0
                                 where (string.IsNullOrEmpty(officerIDAll) || myOfficers.Any(val => A.OFFICERID.Contains(val))) &&

                                 ((A.ISSUEDATE >= startTimeOutput && A.ISSUETIME >= startTimeEndOutput) &&
                                               (A.ISSUEDATE <= endTimeOutput && A.ISSUETIME <= endTimeEndOutput)) 
                                 //orderby
                                 //    A.OFFICERID
                                 select new
                                 {
                                     // CustomerID = (int)A.CustomerID,
                                     IssueNo = A.ISSUENO,
                                     officerID = A.OFFICERID,
                                     officerName = officer.OFFICERNAME,
                                     //startDateTime = A.IssueDateTime,
                                     startDateTime = A.ISSUEDATE,
                                     //endDateTime = A.IssueDateTime,
                                     endDateTime = A.ISSUETIME,
                                     Latitude = A.LOCLATITUDE,
                                     Longitude = A.LOCLONGITUDE
                                 }).Distinct().ToList();


                IQueryable<GISCitationModel> items = (from A in items_raw
                                                      select new GISCitationModel
                                                      {
                                                          // CustomerID = (int)A.CustomerID,
                                                          IssueNo = Convert.ToInt64(A.IssueNo),
                                                          officerID = Convert.ToInt32(A.officerID),
                                                          officerName = A.officerName,
                                                          //startDateTime = A.IssueDateTime,
                                                          startDateTime = A.startDateTime,
                                                          //endDateTime = A.IssueDateTime,
                                                          endDateTime = A.endDateTime,
                                                          dateOnly = Convert.ToString(A.startDateTime.Value.ToShortDateString()),
                                                          Latitude = Convert.ToDouble(A.Latitude),
                                                          Longitude = Convert.ToDouble(A.Longitude)
                                                      }).OrderBy(x => x.officerID).AsQueryable();



                result = items.ToList();
                items = result.AsQueryable();
                //  items = items.ApplyFiltering(request.Filters);
                total = items.Count();
                items = items.ApplySorting(request.Groups, request.Sorts);
                items = items.ApplyPaging(request.Page, request.PageSize);
                result = items.ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetCitationForParkingOfficer Method (GIS Reports Menu - Citation Link)", ex);
                result = new List<GISCitationModel>();
            }
            return result;
        }


        /// <summary>
        /// Description: This method is used to populate the grid with Financial Revenue details for the given search criteria.
        /// Modified By: Sairam on July 20th 2014.
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="tranStartDate"></param>
        /// <param name="tranEndDate"></param>
        /// <param name="assettype"></param>
        /// <param name="assetModelIDs"></param>
        /// <param name="assetID"></param>
        /// <param name="areaId"></param>
        /// <param name="zoneId"></param>
        /// <param name="street"></param>
        /// <param name="suburb"></param>
        /// <param name="revCategory"></param>
        /// <param name="revSource"></param>
        /// <param name="demZone"></param>
        /// <param name="highRev"></param>
        /// <param name="medRev"></param>
        /// <param name="lowRev"></param>
        /// <returns></returns>
        public List<GISModel> GetSearchMetersData_Revenue(int CurrentCity, DateTime tranStartDate, DateTime tranEndDate, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int> revCategory, List<int?> revSource, List<int?> demZone, double highRev, double medRev, double lowRev, [DataSourceRequest] DataSourceRequest request, out int total)
        {
            string checkAssetModel;
            int myAreaId = -1;
            int myZoneId = -1;

            total = 0;
            string paramValues = string.Empty;

            var spParams = GetSpParams(request, "longAssetID desc", out paramValues);

            List<GISModel> displayList = new List<GISModel>();

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }


                if (assetModelIDs.Count() == 0)
                {
                    checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    checkAssetModel = assetModelIDs[0].ToString();


                }

                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer


                var lstCurrent_res = from meter in PemsEntities.Meters
                                     join customer in PemsEntities.Customers
                                     on meter.CustomerID equals customer.CustomerID

                                     //join metermap in PemsEntities.MeterMaps
                                     //on meter.MeterId equals metermap.MeterId

                                     //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                                     join mm in PemsEntities.MeterMaps on
                                         new { MeterId = meter.MeterId, AreaId = meter.AreaID, CustomerId = meter.CustomerID } equals
                                         new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                                     from l1 in metermap.DefaultIfEmpty()

                                     //join area in PemsEntities.Areas
                                     //on new { meter.AreaID, meter.CustomerID } equals new { area.AreaID, area.CustomerID }

                                     join area in PemsEntities.Areas
                                     on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                                     from A1 in mmArea.DefaultIfEmpty()


                                     join zone in PemsEntities.Zones
                                         // on metermap.ZoneId equals zone.ZoneId
                                     on l1.ZoneId equals zone.ZoneId  //**Sairam modified this code on oct 3rd 2014 to do correct mapping

                                     //** The below lines are commented temporarily on 29th sep by sairam . It has to be enabled again
                                     //join demandZone in PemsEntities.DemandZones
                                     //on meter.DemandZone equals demandZone.DemandZoneId

                                     join materGroup in PemsEntities.MeterGroups
                                     on meter.MeterGroup equals materGroup.MeterGroupId


                                     join rSource in PemsEntities.Transactions on
                                       new { MeterId = meter.MeterId, AreaId = meter.AreaID, CustomerId = meter.CustomerID } equals
                                      new { MeterId = (int)rSource.MeterID, AreaId = (int)rSource.AreaID, CustomerId = (int)rSource.CustomerID }


                                     join revType in PemsEntities.TransactionTypes
                                     on rSource.TransactionType equals revType.TransactionTypeId

                                     join mech in PemsEntities.MechanismMasters  //**Correct Mapping
                                     on meter.MeterType equals mech.MechanismId

                                     where customer.CustomerID == CurrentCity
                                     && rSource.TransDateTime >= tranStartDate && rSource.TransDateTime <= tranEndDate
                                     && assettype.Contains(meter.MeterGroup)
                                     && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
                                     && (revSource.Contains(rSource.TransactionType))
                                     && (meter.MeterId == assetID || assetID == -1)
                                         //&& (area.AreaID == areaId || areaId == -1)
                                         //&& (zone.ZoneId == zoneId || zoneId == -1)
                                      && (A1.AreaID == myAreaId || myAreaId == -1)
                                      && (zone.ZoneId == myZoneId || myZoneId == -1)


                                     && (meter.Location == street || street == string.Empty)
                                     //** The below line is commented temporarily on 29th sep by sairam . It has to be enabled again once demandzone is ready
                                     // && (demZone.Contains(meter.DemandZone) || checkDemZone == "-1")

                                     let dt = rSource.TransDateTime //** This is required to truncate time part from transdatetime

                                     //** The below line is commented temporarily on 29th sep by sairam . It has to be enabled again once demandzone is ready
                                     //group new { meter, demandZone, zone, area, mech, materGroup, rSource } by new { meter.MeterId } into groupedResult
                                     //group new { meter,  zone, area, mech, materGroup, rSource } by new { meter.MeterId } into groupedResult
                                     group new { meter, zone, A1, mech, materGroup, rSource } by new { meter.MeterId } into groupedResult
                                     select new
                                     {
                                         GroupName = groupedResult.Key,
                                         Members = groupedResult,


                                     };

                //** After grouping, create an instance of GIS model and add it to list
                displayList = new List<GISModel>();

                //** Loop thru the grouped result to display the first record of every group in the grid
                foreach (var g in lstCurrent_res)
                {
                    GISModel inst = new GISModel();

                    inst.longAssetID = g.Members.Key.MeterId.ToString();
                    inst.MeterName = g.Members.FirstOrDefault().meter.MeterName;
                    inst.MeterGroup = g.Members.FirstOrDefault().materGroup.MeterGroupId;
                    inst.MeterGroupDesc = g.Members.FirstOrDefault().materGroup.MeterGroupDesc;
                    //** The below two lines are commented temporarily on 29th sep by sairam . It has to be enabled again once demandzone is ready
                    // inst.DemandZoneId = g.Members.FirstOrDefault().demandZone.DemandZoneId;
                    //inst.DemandZoneDesc = g.Members.FirstOrDefault().demandZone.DemandZoneDesc;
                    inst.assetModelDesc = g.Members.FirstOrDefault().mech.MechanismDesc;
                    inst.Location = g.Members.FirstOrDefault().meter.Location;
                    inst.ZoneName = g.Members.FirstOrDefault().zone.ZoneName;
                    inst.AreaName = g.Members.FirstOrDefault().A1.AreaName;
                    inst.Latitude = g.Members.FirstOrDefault().meter.Latitude;
                    inst.Longitude = g.Members.FirstOrDefault().meter.Longitude;

                    int? totCnt = 0;
                    double? totInDollars = 0;
                    int? cashTrans = 0;
                    int? creditCardTrans = 0;
                    int? smartCardTrans = 0;
                    int? phoneTrans = 0;

                    //** Find whether it is High/Low/Med revenue before inserting into list and also total revenue for each group
                    bool isAllRevFound = false;
                    bool isHighRevFound = false;
                    bool isMedRevFound = false;
                    bool isLowRevFound = false;

                    foreach (var member in g.Members)
                    {
                        //** Total Revenue
                        totCnt = totCnt + member.rSource.AmountInCents;
                        double dollarPart = Convert.ToDouble(totCnt / 100);
                        totInDollars = Convert.ToDouble(Math.Floor(dollarPart) + "." + totCnt % 100);

                        //** Check the values in the Revenue Category dropdown
                        for (var i = 0; i < revCategory.Count(); i++)
                        {
                            if (revCategory[i] == -1)
                            {
                                isAllRevFound = true;
                            }
                            if (revCategory[i] == 1)
                            {
                                isHighRevFound = true;
                            }
                            if (revCategory[i] == 2)
                            {
                                isMedRevFound = true;
                            }
                            if (revCategory[i] == 3)
                            {
                                isLowRevFound = true;
                            }
                        }

                        if (totInDollars >= highRev)
                        {
                            inst.revCategory = "High";
                        }
                        else if (totInDollars >= medRev && totInDollars < highRev)
                        {
                            inst.revCategory = "Medium";
                        }
                        else
                        {
                            inst.revCategory = "Low";
                        }

                        if (member.rSource.TransactionType == 3)
                        {
                            //** Cash Total
                            cashTrans = cashTrans + member.rSource.AmountInCents;
                        }
                        else if (member.rSource.TransactionType == 1)
                        {
                            //** CreditCard Total
                            creditCardTrans = creditCardTrans + member.rSource.AmountInCents;
                        }
                        else if (member.rSource.TransactionType == 2)
                        {
                            //** SmartCard Total
                            smartCardTrans = smartCardTrans + member.rSource.AmountInCents;
                        }
                        else if (member.rSource.TransactionType == 5)
                        {
                            //** Pay by Phone
                            phoneTrans = phoneTrans + member.rSource.AmountInCents;
                        }

                    }

                    inst.totalRevenue = "$" + (totCnt / 100).ToString() + "." + (totCnt % 100).ToString();
                    inst.cashAmount = cashTrans;
                    inst.cellAmount = phoneTrans;
                    inst.creditCardAmount = creditCardTrans;
                    inst.smartCardAmount = smartCardTrans;

                    //** Once assigned the values, add it to list
                    if (isHighRevFound == true && inst.revCategory == "High")
                    {
                        displayList.Add(inst);
                    }
                    else if (isMedRevFound == true && inst.revCategory == "Medium")
                    {
                        displayList.Add(inst);
                    }
                    else if (isLowRevFound == true && inst.revCategory == "Low")
                    {
                        displayList.Add(inst);
                    }
                    else if (isAllRevFound == true)
                    {
                        displayList.Add(inst);
                    }

                }

                //  items = items.ApplyFiltering(request.Filters);
                IQueryable<GISModel> items = displayList.AsQueryable();
                total = items.Count();
                items = items.ApplySorting(request.Groups, request.Sorts);
                items = items.ApplyPaging(request.Page, request.PageSize);
                displayList = items.ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Revenue Method (GIS Reports - Financial Report Type)", ex);
            }


            return displayList;
        }

        private int? TruncateTime(DateTime? nullable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Description:This method is used to populate the Grid in the UI for Inventory Layer .
        /// Modified By: Sairam on July 21st 2014

        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="assettype"></param>
        /// <param name="assetModelIDs"></param>
        /// <param name="assetID"></param>
        /// <param name="areaId"></param>
        /// <param name="zoneId"></param>
        /// <param name="street"></param>
        /// <param name="suburb"></param>
        /// <param name="demZone"></param>
        /// <param name="assetStatus"></param>
        /// <param name="Layer"></param>
        /// <returns></returns>
        public IEnumerable<GISModel> GetSearchMetersData_Inventory(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer, [DataSourceRequest] DataSourceRequest request, out int total)
        {

            string checkAssetModel;
            total = 0;
            int myAreaId = -1;
            int myZoneId = -1;

            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();

            try
            {
                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }


                if (assetModelIDs.Count() == 0)
                {
                    checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    checkAssetModel = assetModelIDs[0].ToString();
                }

                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer

                var tempResult = (from meter in PemsEntities.Meters
                                  join customer in PemsEntities.Customers
                                  on meter.CustomerID equals customer.CustomerID

                                  //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                                  join mm in PemsEntities.MeterMaps on
                                      new { MeterId = meter.MeterId, AreaId = meter.AreaID, CustomerId = meter.CustomerID } equals
                                      new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                                  // new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                                  from l1 in metermap.DefaultIfEmpty()

                                  join area in PemsEntities.Areas
                                  on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                                  from A1 in mmArea.DefaultIfEmpty()

                                  join zone in PemsEntities.Zones
                                  on l1.ZoneId equals zone.ZoneId //Sairam modified this code on oct 3rd 2014

                                  //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                  //join demandZone in PemsEntities.DemandZones
                                  //on meter.DemandZone equals demandZone.DemandZoneId

                                  join materGroup in PemsEntities.MeterGroups
                                  on meter.MeterGroup equals materGroup.MeterGroupId

                                  join assetState in PemsEntities.AssetStates
                                  on meter.MeterState equals assetState.AssetStateId

                                  join mech in PemsEntities.MechanismMasters  //**Correct Mapping
                                  on meter.MeterType equals mech.MechanismId

                                  where customer.CustomerID == CurrentCity
                                  && assettype.Contains(meter.MeterGroup)
                                  && assetStatus.Contains(meter.MeterState)
                                  && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
                                  && (meter.MeterId == assetID || assetID == -1)
                                      //&& (area.AreaID == areaId || areaId == -1)
                                      //&& (zone.ZoneId == zoneId || zoneId == -1)
                                  && (A1.AreaID == myAreaId || myAreaId == -1)
                                  && (zone.ZoneId == myZoneId || myZoneId == -1)

                                  && (meter.Location == street || street == string.Empty)
                                  && (demZone.Contains(meter.DemandZone) || checkDemZone == "-1")


                                  select new GISModel
                                  {
                                      CustomerID = customer.CustomerID,
                                      ZoneID = zone.ZoneId,
                                      ZoneName = zone.ZoneName,
                                      AreaID = A1.AreaID,
                                      //AreaName = area.AreaName,
                                      AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,
                                      AssetID = meter.MeterId,

                                      //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                                      DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                      DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                      assetModelDesc = mech.MechanismDesc,

                                      MeterName = meter.MeterName,
                                      AssetStateDesc = assetState.AssetStateDesc,
                                      Location = meter.Location,
                                      BayStart = meter.BayStart,
                                      BayEnd = meter.BayEnd,
                                      MeterGroupDesc = materGroup.MeterGroupDesc,
                                      Latitude = meter.Latitude,
                                      Longitude = meter.Longitude,
                                      MeterGroup = materGroup.MeterGroupId,
                                  }).ToList();

                result = from r in tempResult
                         select new GISModel
                         {
                             CustomerID = r.CustomerID,
                             ZoneID = r.ZoneID,
                             ZoneName = r.ZoneName,
                             AreaID = r.AreaID,
                             AreaName = r.AreaName,
                             AssetId = Convert.ToString(r.AssetID),

                             //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                             DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                             DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                             assetModelDesc = r.assetModelDesc,

                             MeterName = r.MeterName,
                             AssetStateDesc = r.AssetStateDesc,
                             Location = r.Location,
                             BayStart = r.BayStart,
                             BayEnd = r.BayEnd,
                             MeterGroupDesc = r.MeterGroupDesc,
                             Latitude = r.Latitude,
                             Longitude = r.Longitude,
                             MeterGroup = r.MeterGroup,
                         };
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return result;
        }

        public static List<GISModel> CastToDerived(IEnumerable<GISModel> originalItems, List<GISModel> newItems)
        {
            newItems.AddRange(from originalItem in originalItems let newItem = new GISModel() select ExportFactory.CopyFrom(newItem, originalItem));
            return newItems;
        }

        public IEnumerable<GISModel> GetSearchMetersData_Inventory_All(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer, [DataSourceRequest] DataSourceRequest request, out int total, string pageChosen)
        {

            IQueryable<GISModel> items = null;
            int? at = -1;
            total = 0;

            try
            {

                if (assettype.Count() == 1)
                {
                    at = assettype[0];
                }
                switch (at)
                {
                    case 0: //** Single Space Meter
                    case 1: //** Multi Space Meter
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 2: //** Sensor Only Location
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;

                    case 10: //** Sensor
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        //items = GetSearchMetersData_Inventory_Sensor(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 16: //** CSPark
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        //items = GetSearchMetersData_Inventory_CSPark(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 13: //** Gateway
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        // items = GetSearchMetersData_Inventory_Gateway(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 31: //** Mechanism
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        //items = GetSearchMetersData_Inventory_Mechanism(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 11: //** CashBox
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        //items = GetSearchMetersData_Inventory_CashBox(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 32: //** Datakey
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        //items = GetSearchMetersData_Inventory_Datakey(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    default:
                        //group them all, filter them first, then you can sort and page on all the items
                        var meters = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);

                        var finalItems = meters.ToList();

                        items = finalItems.AsQueryable();
                        total = items.Count();
                        if (pageChosen != "mapChosen")
                        {
                            items = items.ApplySorting(request.Groups, request.Sorts);
                            items = items.ApplyPaging(request.Page, request.PageSize);
                        }


                        return items.ToList();
                }
                if (items != null)
                {
                    //  items = items.ApplyFiltering(request.Filters);
                    var final_Items = items.ToList();
                    items = final_Items.AsQueryable();
                    total = items.Count();
                    if (pageChosen != "mapChosen")
                    {
                        items = items.ApplySorting(request.Groups, request.Sorts);
                        items = items.ApplyPaging(request.Page, request.PageSize);
                    }

                    return items.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return new List<GISModel>();
        }

        public IEnumerable<GISModel> GetSearchMetersData_Inventory_All_MapAlone(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer, string pageChosen)
        {

            IQueryable<GISModel> items = null;
            int? at = -1;
            // total = 0;

            try
            {

                if (assettype.Count() == 1)
                {
                    at = assettype[0];
                }
                switch (at)
                {
                    case 0: //** Single Space Meter
                    case 1: //** Multi Space Meter
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 2: //** Sensor Only Location
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;

                    case 10: //** Sensor
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        //items = GetSearchMetersData_Inventory_Sensor(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 16: //** CSPark
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        //items = GetSearchMetersData_Inventory_CSPark(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 31: //** Mechanism
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 13: //** Gateway
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 11: //** CashBox
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 32: //** Datakey
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    default:

                        //group them all, filter them first, then you can sort and page on all the items
                        var meters = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);


                        var finalItems = meters.ToList();

                        items = finalItems.AsQueryable();


                        return items.ToList();
                }
                if (items != null)
                {
                    //  items = items.ApplyFiltering(request.Filters);
                    var final_Items = items.ToList();
                    items = final_Items.AsQueryable();


                    return items.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return new List<GISModel>();
        }


        public IEnumerable<GISModel> GetSearchMetersData_Inventory_All_Home(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer)
        {

            IQueryable<GISModel> items = null;
            int? at = -1;
            //   total = 0;

            try
            {

                if (assettype.Count() == 1)
                {
                    at = assettype[0];
                }
                switch (at)
                {
                    case 0: //** Single Space Meter
                    case 1: //** Multi Space Meter
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 2: //** Sensor Only Location
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 16: //** CSPark
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 31: //** Mechanism
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 10: //** Sensor
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 13: //** Gateway
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 11: //** CashBox
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    case 32: //** Datakey
                        items = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);
                        break;
                    default:
                        //group them all, filter them first, then you can sort and page on all the items
                        var meters = GetSearchMetersData_Inventory_Meters(CurrentCity, assettype, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demZone, assetStatus, Layer);



                        var finalItems = meters.ToList();


                        return finalItems;

                        items = finalItems.AsQueryable();


                        return items.ToList();
                }
                if (items != null)
                {
                    //  items = items.ApplyFiltering(request.Filters);
                    var final_Items = items.ToList();
                    items = final_Items.AsQueryable();
                    return items.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return new List<GISModel>();
        }

        public IQueryable<GISModel> GetSearchMetersData_Inventory_Meters(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer)
        {

            string checkAssetModel;
            int myAreaId = -1;
            int myZoneId = -1;
            IQueryable<GISModel> result = null;

            try
            {
                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }


                if (assetModelIDs.Count() == 0)
                {
                    checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    checkAssetModel = assetModelIDs[0].ToString();
                }

                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer

                result = (from meter in PemsEntities.Meters
                          join customer in PemsEntities.Customers
                          on meter.CustomerID equals customer.CustomerID

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join mm in PemsEntities.MeterMaps on
                              new { MeterId = meter.MeterId, AreaId = meter.AreaID, CustomerId = meter.CustomerID } equals
                              new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                          // new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                          from l1 in metermap.DefaultIfEmpty()


                          //join area in PemsEntities.Areas
                          //on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                          //from A1 in mmArea.DefaultIfEmpty()

                          //join zone in PemsEntities.Zones
                          //on l1.ZoneId equals zone.ZoneId //Sairam modified this code on oct 3rd 2014

                          //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                          //join demandZone in PemsEntities.DemandZones
                          //on meter.DemandZone equals demandZone.DemandZoneId

                          join materGroup in PemsEntities.MeterGroups
                          on meter.MeterGroup equals materGroup.MeterGroupId

                          join assetState in PemsEntities.AssetStates
                          on meter.MeterState equals assetState.AssetStateId

                          join mech in PemsEntities.MechanismMasters  //**Correct Mapping
                          on meter.MeterType equals mech.MechanismId

                          where customer.CustomerID == CurrentCity
                          && assettype.Contains(meter.MeterGroup)
                          && assetStatus.Contains(meter.MeterState)
                          && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
                          && (meter.MeterId == assetID || assetID == -1)
                              //&& (A1.AreaID == myAreaId || myAreaId == -1)
                              //&& (zone.ZoneId == myZoneId || myZoneId == -1)
                          && (meter.Location == street || street == string.Empty)
                          && (demZone.Contains(meter.DemandZone) || checkDemZone == "-1")


                          select new GISModel
                          {
                              CustomerID = customer.CustomerID,
                              //ZoneID = zone.ZoneId,
                              //  ZoneID = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == CurrentCity) == null ? 1 : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == CurrentCity).ZoneId,
                              ZoneName = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == CurrentCity) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == CurrentCity).ZoneName,
                              //AreaID = A1.AreaID,
                              //  AreaID = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2) == null ? 1 : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaID,
                              // AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,
                              AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,
                              AssetID = meter.MeterId,

                              //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                              DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                              DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                              assetModelDesc = mech.MechanismDesc,

                              MeterName = meter.MeterName,
                              AssetStateDesc = assetState.AssetStateDesc,
                              Location = meter.Location,
                              BayStart = meter.BayStart == null ? 1 : meter.BayStart,
                              BayEnd = meter.BayEnd == null ? 1 : meter.BayEnd,
                              MeterGroupDesc = materGroup.MeterGroupDesc,
                              Latitude = meter.Latitude,
                              Longitude = meter.Longitude,
                              MeterGroup = materGroup.MeterGroupId,
                          });

                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return result;
        }

        public IQueryable<GISModel> GetSearchMetersData_Inventory_Meters_Specific(int CurrentCity, int assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer)
        {

            string checkAssetModel;
            int myAreaId = -1;
            int myZoneId = -1;
            IQueryable<GISModel> result = null;

            try
            {
                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }


                if (assetModelIDs.Count() == 0)
                {
                    checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    checkAssetModel = assetModelIDs[0].ToString();
                }

                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer

                result = (from meter in PemsEntities.Meters
                          join customer in PemsEntities.Customers
                          on meter.CustomerID equals customer.CustomerID

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join mm in PemsEntities.MeterMaps on
                              new { MeterId = meter.MeterId, AreaId = meter.AreaID, CustomerId = meter.CustomerID } equals
                              new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                          // new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                          from l1 in metermap.DefaultIfEmpty()

                          //** New extra condition added on 22nd feb 2016 to add cspark
                          //join hm in PemsEntities.HousingMasters on
                          //new { CustomerId = l1.Customerid, HousingId = l1.HousingId } equals
                          //new { CustomerId = hm.Customerid, HousingId = hm.HousingId } into housing
                          //from HS in housing.DefaultIfEmpty()

                          join area in PemsEntities.Areas
                          on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                          from A1 in mmArea.DefaultIfEmpty()

                          join zone in PemsEntities.Zones
                          on l1.ZoneId equals zone.ZoneId //Sairam modified this code on oct 3rd 2014

                          //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                          //join demandZone in PemsEntities.DemandZones
                          //on meter.DemandZone equals demandZone.DemandZoneId

                          join materGroup in PemsEntities.MeterGroups
                          on meter.MeterGroup equals materGroup.MeterGroupId

                          join assetState in PemsEntities.AssetStates
                          on meter.MeterState equals assetState.AssetStateId

                          join mech in PemsEntities.MechanismMasters  //**Correct Mapping
                          on meter.MeterType equals mech.MechanismId

                          where customer.CustomerID == CurrentCity
                          && assettype == meter.MeterGroup
                          && assetStatus.Contains(meter.MeterState)
                          && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
                          && (meter.MeterId == assetID || assetID == -1)
                          && (A1.AreaID == myAreaId || myAreaId == -1)
                          && (zone.ZoneId == myZoneId || myZoneId == -1)
                          && (meter.Location == street || street == string.Empty)
                          && (demZone.Contains(meter.DemandZone) || checkDemZone == "-1")


                          select new GISModel
                          {
                              CustomerID = customer.CustomerID,
                              ZoneID = zone.ZoneId,
                              ZoneName = zone.ZoneName,
                              AreaID = A1.AreaID,
                              AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,
                              AssetID = meter.MeterId,

                              //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                              DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                              DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                              assetModelDesc = mech.MechanismDesc,

                              MeterName = meter.MeterName,
                              AssetStateDesc = assetState.AssetStateDesc,
                              Location = meter.Location,
                              BayStart = meter.BayStart,
                              BayEnd = meter.BayEnd,
                              MeterGroupDesc = materGroup.MeterGroupDesc,
                              Latitude = meter.Latitude,
                              Longitude = meter.Longitude,
                              MeterGroup = materGroup.MeterGroupId,
                          });

                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return result;
        }

        //public IQueryable<GISModel> GetSearchMetersData_Inventory_Meters(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer)
        //{

        //    string checkAssetModel;
        //    int myAreaId = -1;
        //    int myZoneId = -1;
        //    IQueryable<GISModel> result = null;

        //    try
        //    {
        //        //** Sairam added on Oct 13th 2014 to filter by area or zone names
        //        if (areaNameIs == "")
        //        {
        //            myAreaId = -1;
        //        }
        //        else
        //        {
        //            myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
        //        }

        //        if (zoneNameIs == "")
        //        {
        //            myZoneId = -1;
        //        }
        //        else
        //        {
        //            myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
        //        }


        //        if (assetModelIDs.Count() == 0)
        //        {
        //            checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
        //        }
        //        else
        //        {
        //            checkAssetModel = assetModelIDs[0].ToString();
        //        }

        //        string checkDemZone;
        //        checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer

        //        result = (from meter in PemsEntities.Meters
        //                  join customer in PemsEntities.Customers
        //                  on meter.CustomerID equals customer.CustomerID

        //                  //**Sairam modified this code on oct 3rd 2014 to do correct mapping
        //                  join mm in PemsEntities.MeterMaps on
        //                      new { MeterId = meter.MeterId, AreaId = meter.AreaID, CustomerId = meter.CustomerID } equals
        //                      new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
        //                  // new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
        //                  from l1 in metermap.DefaultIfEmpty()

        //                  join area in PemsEntities.Areas
        //                  on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
        //                  from A1 in mmArea.DefaultIfEmpty()

        //                  join zone in PemsEntities.Zones
        //                  on l1.ZoneId equals zone.ZoneId //Sairam modified this code on oct 3rd 2014

        //                  //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
        //                  //join demandZone in PemsEntities.DemandZones
        //                  //on meter.DemandZone equals demandZone.DemandZoneId

        //                  join materGroup in PemsEntities.MeterGroups
        //                  on meter.MeterGroup equals materGroup.MeterGroupId

        //                  join assetState in PemsEntities.AssetStates
        //                  on meter.MeterState equals assetState.AssetStateId

        //                  join mech in PemsEntities.MechanismMasters  //**Correct Mapping
        //                  on meter.MeterType equals mech.MechanismId

        //                  where customer.CustomerID == CurrentCity
        //                  && assettype.Contains(meter.MeterGroup)
        //                  && assetStatus.Contains(meter.MeterState)
        //                  && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
        //                  && (meter.MeterId == assetID || assetID == -1)
        //                  && (A1.AreaID == myAreaId || myAreaId == -1)
        //                  && (zone.ZoneId == myZoneId || myZoneId == -1)
        //                  && (meter.Location == street || street == string.Empty)
        //                  && (demZone.Contains(meter.DemandZone) || checkDemZone == "-1")


        //                  select new GISModel
        //                  {
        //                      CustomerID = customer.CustomerID,
        //                      ZoneID = zone.ZoneId,
        //                      ZoneName = zone.ZoneName,
        //                      AreaID = A1.AreaID,
        //                      AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,
        //                      AssetID = meter.MeterId,

        //                      //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
        //                      DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
        //                      DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
        //                      assetModelDesc = mech.MechanismDesc,

        //                      MeterName = meter.MeterName,
        //                      AssetStateDesc = assetState.AssetStateDesc,
        //                      Location = meter.Location,
        //                      BayStart = meter.BayStart,
        //                      BayEnd = meter.BayEnd,
        //                      MeterGroupDesc = materGroup.MeterGroupDesc,
        //                      Latitude = meter.Latitude,
        //                      Longitude = meter.Longitude,
        //                      MeterGroup = materGroup.MeterGroupId,
        //                  });

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
        //    }

        //    return result;
        //}

        public IQueryable<GISModel> GetSearchMetersData_Inventory_CSPark(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer)
        {

            IQueryable<GISModel> result = null;

            try
            {
                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer



                result = from mechanism in PemsEntities.CSParks
                         where mechanism.Customerid == CurrentCity
                         join mm in PemsEntities.MeterMaps on
                           mechanism.CSParkId equals mm.CSParkId
                         where mm.Meter.MeterGroup == 16   // To get only CSPark in Asset Inquiry

                         select new GISModel
                         {
                             ZoneName = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == CurrentCity) == null ? ""
                                   : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == CurrentCity).ZoneName,

                             AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == mm.AreaId2) == null ? ""
                                   : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == mm.AreaId2).AreaName,
                             AssetID = mechanism.CSParkId,

                             //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                             DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                             DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                             assetModelDesc = "",// PemsEntities.MechanismMasterCustomers.FirstOrDefault(x => x.CustomerId == mm.Customerid && x.MechanismId == 31).MechanismDesc,

                             MeterName = mm.Meter.MeterName,
                             AssetStateDesc = mm.Meter.MeterState == null ? "isNull" : mm.Meter.AssetState.AssetStateDesc,
                             Location = mm.Meter.Location,
                             BayStart = mm.Meter.BayStart,
                             BayEnd = mm.Meter.BayEnd,
                             MeterGroupDesc = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == 16).MeterGroupDesc,
                             Latitude = mm.Meter.Latitude,
                             Longitude = mm.Meter.Longitude,
                             MeterGroup = mm.Meter.MeterGroup,
                         };

                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return result;
        }

        public IQueryable<GISModel> GetSearchMetersData_Inventory_Mechanism(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer)
        {

            IQueryable<GISModel> result = null;

            try
            {
                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer



                result = from mechanism in PemsEntities.MechMasters
                         where mechanism.Customerid == CurrentCity
                         join mm in PemsEntities.MeterMaps on
                            new { MechId = mechanism.MechIdNumber.Value, CustomerId = mechanism.Customerid.Value } equals
                            new { MechId = mm.MeterId, CustomerId = mm.Customerid } into l1
                         from metmap in l1.DefaultIfEmpty()
                         where metmap.Meter.MeterGroup == 31   // To get only one Mechanism in Asset Inquiry

                         select new GISModel
                         {
                             ZoneName = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metmap.ZoneId && x.customerID == CurrentCity) == null ? ""
                                   : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metmap.ZoneId && x.customerID == CurrentCity).ZoneName,

                             AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == metmap.AreaId2) == null ? ""
                                   : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == metmap.AreaId2).AreaName,
                             AssetID = mechanism.MechIdNumber ?? metmap.MechId.Value,

                             //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                             DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                             DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                             assetModelDesc = PemsEntities.MechanismMasterCustomers.FirstOrDefault(x => x.CustomerId == metmap.Customerid && x.MechanismId == 31).MechanismDesc,

                             MeterName = metmap.Meter.MeterName,
                             AssetStateDesc = metmap.Meter.MeterState == null ? "isNull" : metmap.Meter.AssetState.AssetStateDesc,
                             Location = metmap.Meter.Location,
                             BayStart = metmap.Meter.BayStart,
                             BayEnd = metmap.Meter.BayEnd,
                             MeterGroupDesc = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == 31).MeterGroupDesc,
                             Latitude = metmap.Meter.Latitude,
                             Longitude = metmap.Meter.Longitude,
                             MeterGroup = metmap.Meter.MeterGroup,
                         };

                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return result;
        }

        public IQueryable<GISModel> GetSearchMetersData_Inventory_Sensor(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer)
        {

            IQueryable<GISModel> result = null;

            try
            {

                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer


                result = from sensor in PemsEntities.Sensors
                         where sensor.CustomerID == CurrentCity
                         join mm in PemsEntities.MeterMaps on
                             new { SensorId = sensor.SensorID, CustomerId = sensor.CustomerID } equals
                             new { SensorId = mm.SensorID.Value, CustomerId = mm.Customerid } into l1
                         from metmap in l1.DefaultIfEmpty()
                         join vpn in PemsEntities.VersionProfileMeters on
                             new { SensorId = sensor.SensorID, CustomerId = sensor.CustomerID } equals
                             new { SensorId = vpn.SensorID.Value, CustomerId = vpn.CustomerId } into l2
                         from vpns in l2.DefaultIfEmpty()
                         join ps in PemsEntities.ParkingSpaces on sensor.ParkingSpaceId equals ps.ParkingSpaceId into l3
                         from parkingSpace in l2.DefaultIfEmpty()
                         let aa = metmap.Meter.ActiveAlarms.OrderByDescending(x => x.TimeOfOccurrance).FirstOrDefault()

                         //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                         //join demandZone in PemsEntities.DemandZones
                         //on meter.DemandZone equals demandZone.DemandZoneId

                         select new GISModel
                         {
                             ZoneName = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metmap.ZoneId && x.customerID == CurrentCity) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metmap.ZoneId && x.customerID == CurrentCity).ZoneName,
                             AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == metmap.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == metmap.AreaId2).AreaName,
                             AssetID = sensor.SensorID,

                             //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                             DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                             DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                             assetModelDesc = sensor.MechanismMaster == null ? " "
                                   : PemsEntities.MechanismMasterCustomers.FirstOrDefault(z => z.CustomerId == CurrentCity && z.MechanismId == sensor.MechanismMaster.MechanismId && z.IsDisplay) == null ? " "
                                   : PemsEntities.MechanismMasterCustomers.FirstOrDefault(z => z.CustomerId == CurrentCity && z.MechanismId == sensor.MechanismMaster.MechanismId && z.IsDisplay).MechanismDesc,

                             MeterName = sensor.SensorName ?? " ",
                             AssetStateDesc = sensor.SensorState == null ? "" : sensor.AssetState.AssetStateDesc,
                             Location = sensor.Location ?? " ",
                             MeterGroupDesc = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == 10).MeterGroupDesc,
                             Latitude = sensor.Latitude,
                             Longitude = sensor.Longitude,
                             //MeterGroup = sensor.MeterGroup,
                             MeterGroup = 10 //** sai added on 17th apr 2015 to fix gis - meters 
                         };

                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return result;
        }

        public IQueryable<GISModel> GetSearchMetersData_Inventory_Gateway(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer)
        {

            IQueryable<GISModel> result = null;

            try
            {

                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer



                result = from g in PemsEntities.Gateways
                         where g.CustomerID == CurrentCity
                         join meterMap in PemsEntities.MeterMaps on
                             new { SensorId = g.GateWayID, CustomerId = g.CustomerID } equals
                             new { SensorId = meterMap.GatewayID.Value, CustomerId = meterMap.Customerid } into l1
                         from metmap in l1.DefaultIfEmpty()
                         let aa = metmap.Meter.ActiveAlarms.OrderByDescending(x => x.TimeOfOccurrance).FirstOrDefault()

                         join profMeters in PemsEntities.VersionProfileMeters on
                            new { MeterId = metmap.MeterId, AreaId = metmap.Areaid, CustomerId = metmap.Customerid } equals
                          new { MeterId = profMeters.MeterId.Value, AreaId = profMeters.AreaId.Value, CustomerId = profMeters.CustomerId } into l4
                         from vpn in l4.DefaultIfEmpty()

                         //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                         //join demandZone in PemsEntities.DemandZones
                         //on meter.DemandZone equals demandZone.DemandZoneId

                         select new GISModel
                         {
                             ZoneName = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metmap.ZoneId && x.customerID == CurrentCity) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metmap.ZoneId && x.customerID == CurrentCity).ZoneName,
                             AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == metmap.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == metmap.AreaId2).AreaName,
                             AssetID = g.GateWayID,

                             //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                             DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                             DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                             assetModelDesc = g.MechanismMaster == null ? " "
                             : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == CurrentCity && m.MechanismId == g.MechanismMaster.MechanismId && m.IsDisplay) == null ? " "
                             : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == CurrentCity && m.MechanismId == g.MechanismMaster.MechanismId && m.IsDisplay).MechanismDesc,

                             MeterName = g.Description ?? " ",
                             AssetStateDesc = g.GatewayState == null ? "" : g.AssetState.AssetStateDesc,
                             Location = g.Location ?? " ",
                             MeterGroupDesc = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == 13).MeterGroupDesc,
                             Latitude = g.Latitude ?? 0.0,
                             Longitude = g.Longitude ?? 0.0,
                             MeterGroup = 13,
                         };

                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return result;
        }

        public IQueryable<GISModel> GetSearchMetersData_Inventory_CashBox(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer)
        {

            IQueryable<GISModel> result = null;

            try
            {

                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer



                result = PemsEntities.CashBoxes.Where(m => m.CustomerID == CurrentCity).Select(
                     item =>
                     new GISModel
                     {
                         ZoneName = "",// PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metmap.ZoneId && x.customerID == CurrentCity) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metmap.ZoneId && x.customerID == CurrentCity).ZoneName,
                         AreaName = "",//PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == metmap.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == metmap.AreaId2).AreaName,
                         AssetID = item.CashBoxID,

                         //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                         DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                         DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                         assetModelDesc = item.MechanismMaster == null ? " "
                        : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == CurrentCity && m.MechanismId == item.MechanismMaster.MechanismId && m.IsDisplay) == null ? " "
                        : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == CurrentCity && m.MechanismId == item.MechanismMaster.MechanismId && m.IsDisplay).MechanismDesc,

                         MeterName = item.CashBoxName,
                         AssetStateDesc = item.CashBoxState == null ? "" : item.AssetState.AssetStateDesc,
                         //Location = g.Location ?? " ",
                         MeterGroupDesc = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == 11).MeterGroupDesc,
                         // Latitude = g.Latitude ?? 0.0,
                         // Longitude = g.Longitude ?? 0.0,
                         MeterGroup = 11,
                     });

                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return result;
        }

        public IQueryable<GISModel> GetSearchMetersData_Inventory_Datakey(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> assetStatus, int Layer)
        {

            IQueryable<GISModel> result = null;

            try
            {

                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer



                result = from datakey in PemsEntities.DataKeys
                         where datakey.CustomerID == CurrentCity
                         join meterMap in PemsEntities.MeterMaps on
                             new { DataKeyId = datakey.DataKeyId, CustomerId = datakey.CustomerID } equals
                             new { DataKeyId = meterMap.DataKeyId.Value, CustomerId = meterMap.Customerid } into l1
                         from metMap in l1.DefaultIfEmpty()
                         let aa = metMap.Meter.ActiveAlarms.OrderByDescending(x => x.TimeOfOccurrance).FirstOrDefault()
                         join profMeters in PemsEntities.VersionProfileMeters on
                           new { MeterId = metMap.MeterId, AreaId = metMap.Areaid, CustomerId = metMap.Customerid } equals
                         new { MeterId = profMeters.MeterId.Value, AreaId = profMeters.AreaId.Value, CustomerId = profMeters.CustomerId } into l4
                         from vpn in l4.DefaultIfEmpty()

                         //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                         //join demandZone in PemsEntities.DemandZones
                         //on meter.DemandZone equals demandZone.DemandZoneId

                         select new GISModel
                         {
                             ZoneName = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metMap.ZoneId && x.customerID == CurrentCity) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == metMap.ZoneId && x.customerID == CurrentCity).ZoneName,
                             AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == metMap.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == metMap.AreaId2).AreaName,
                             AssetID = datakey.DataKeyIdNumber ?? metMap.DataKeyId.Value,

                             //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                             DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                             DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                             assetModelDesc = datakey.DataKeyDesc,
                             MeterName = datakey.DataKeyDesc,
                             AssetStateDesc = "",
                             Location = metMap.Meter.Location ?? " ",
                             MeterGroupDesc = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == 13).MeterGroupDesc,
                             Latitude = metMap.Meter.Latitude,
                             Longitude = metMap.Meter.Longitude,
                             MeterGroup = 32,
                         };

                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_Inventory Method (GIS Reports)", ex);
            }

            return result;
        }


        /// <summary>
        /// Description:This method is used to populate the Grid in the UI for High Demand Layer .
        /// Modified By: Sairam on July 26th 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="assettype"></param>
        /// <param name="assetModelIDs"></param>
        /// <param name="assetID"></param>
        /// <param name="areaId"></param>
        /// <param name="zoneId"></param>
        /// <param name="street"></param>
        /// <param name="suburb"></param>
        /// <param name="demZone"></param>
        /// <param name="occupancyIDs"></param>
        /// <param name="Layer"></param>
        /// <param name="NonComplianceStatusIDs"></param>
        /// <returns></returns>
        //public IEnumerable<GISModel> GetSearchMetersData_HighDemandBays(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> occupancyIDs, int Layer, List<int?> NonComplianceStatusIDs, [DataSourceRequest] DataSourceRequest request, out int total, string pageChosen)
        //{

        //    IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
        //    total = 0;
        //    string checkAssetModel;

        //    string isNoncompliantStatus = "-1";

        //    int myAreaId = -1;
        //    int myZoneId = -1;

        //    string paramValues = string.Empty;

        //    var spParams = GetSpParams(request, "AssetId desc", out paramValues);

        //    try
        //    {

        //        //** Sairam added on Oct 13th 2014 to filter by area or zone names
        //        if (areaNameIs == "")
        //        {
        //            myAreaId = -1;
        //        }
        //        else
        //        {
        //            myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
        //        }

        //        if (zoneNameIs == "")
        //        {
        //            myZoneId = -1;
        //        }
        //        else
        //        {
        //            myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
        //        }

        //        if (assetModelIDs.Count() == 0)
        //        {
        //            checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
        //        }
        //        else
        //        {
        //            checkAssetModel = assetModelIDs[0].ToString();
        //        }



        //        string checkDemZone;
        //        checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer


        //        result = (from parkingSpaces in PemsEntities.ParkingSpaces
        //                  join customer in PemsEntities.Customers
        //                  on parkingSpaces.CustomerID equals customer.CustomerID

        //                  //**Sairam modified this code on oct 3rd 2014 to do correct mapping
        //                  join meter in PemsEntities.Meters
        //                  on new { CustomerId = parkingSpaces.CustomerID, Areaid = parkingSpaces.AreaId, Meterid = parkingSpaces.MeterId } equals new { CustomerId = meter.CustomerID, Areaid = meter.AreaID, Meterid = meter.MeterId } into meterHolder
        //                  from m1 in meterHolder.DefaultIfEmpty()

        //                  //**Sairam modified this code on oct 3rd 2014 to do correct mapping
        //                  join mm in PemsEntities.MeterMaps on
        //                      new { MeterId = m1.MeterId, AreaId = m1.AreaID, CustomerId = m1.CustomerID } equals
        //                      new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
        //                  //new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
        //                  from l1 in metermap.DefaultIfEmpty()


        //                  join area in PemsEntities.Areas
        //                  on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
        //                  from A1 in mmArea.DefaultIfEmpty()


        //                  join zone in PemsEntities.Zones
        //                  on l1.ZoneId equals zone.ZoneId  //**Sairam modified this code on oct 3rd 2014

        //                  //** The below join ('demandzone') is commented temporarily on 29th sep 2014. It has to be enabled
        //                  //join demandZone in PemsEntities.DemandZones
        //                  //on parkingSpaces.DemandZoneId equals demandZone.DemandZoneId

        //                  //** The below join ('parkingSpaces.ParkingSpaceType') is uncommented temporarily on Oct 3rd 2014. It has to be enabled
        //                  //join materGroup in PemsEntities.MeterGroups
        //                  //on parkingSpaces.ParkingSpaceType equals materGroup.MeterGroupId

        //                  join mech in PemsEntities.MechanismMasters  //**Correct Mapping
        //                  on m1.MeterType equals mech.MechanismId

        //                  join pso in PemsEntities.ParkingSpaceOccupancies
        //                  on parkingSpaces.ParkingSpaceId equals pso.ParkingSpaceId

        //                  join occupancyStatus in PemsEntities.OccupancyStatus
        //                  on pso.LastStatus equals occupancyStatus.StatusID

        //                  //join sptxc in PemsEntities.SensorPaymentTransactionCurrents
        //                  //on parkingSpaces.ParkingSpaceId equals sptxc.ParkingSpaceId

        //                  //join ncs in PemsEntities.NonCompliantStatus
        //                  //on sptxc.NonCompliantStatus equals ncs.NonCompliantStatusID

        //                  where customer.CustomerID == CurrentCity
        //                      //** The below 'assettype.Contains(parkingSpaces.ParkingSpaceType)' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled 
        //                      // && assettype.Contains(parkingSpaces.ParkingSpaceType)
        //                      //** The below 'assettype.Contains(20)' is used temporarily on 29th sep 2014 by sairam. It has to be commented as it is hardcoded
        //                    && assettype.Contains(20)
        //                    && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
        //                    && (parkingSpaces.ParkingSpaceId == assetID || assetID == -1)
        //                    && (occupancyIDs.Contains(pso.LastStatus)) //** To check occupancy status from ParkingSpaceOccupancy table
        //                      // && (NonComplianceStatusIDs.Contains(sptxc.NonCompliantStatus)) || (string.IsNullOrEmpty(SqlFunctions.StringConvert((decimal) sptxc.NonCompliantStatus))) //** To check Non-Compliant status from SensorPaymentTransactionCurrent table
        //                      //&& (area.AreaID == areaId || areaId == -1)
        //                      //&& (zone.ZoneId == zoneId || zoneId == -1)
        //                      && (A1.AreaID == myAreaId || myAreaId == -1)
        //                      && (zone.ZoneId == myZoneId || myZoneId == -1)
        //                      && (m1.Location == street || street == string.Empty)

        //                  //** The below 'parkingSpaces.DemandZoneId' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled later
        //                  // && (demZone.Contains(parkingSpaces.DemandZoneId) || checkDemZone == "-1")


        //                  select new GISModel
        //                  {
        //                      CustomerID = customer.CustomerID,
        //                      AssetID = parkingSpaces.ParkingSpaceId,
        //                      MeterName = m1.MeterName,
        //                      ZoneID = zone.ZoneId,
        //                      ZoneName = zone.ZoneName,
        //                      AreaID = A1.AreaID,
        //                      //AreaName = area.AreaName,
        //                      AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,

        //                      Location = m1.Location,
        //                      //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once join with parkingspacetype is ready
        //                      //MeterGroup = materGroup.MeterGroupId,
        //                      //MeterGroupDesc = materGroup.MeterGroupDesc,
        //                      //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
        //                      MeterGroup = 20,
        //                      MeterGroupDesc = "Parking Spaces",
        //                      Latitude = m1.Latitude,
        //                      Longitude = m1.Longitude,
        //                      //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
        //                      //  DemandZoneId = demandZone.DemandZoneId,
        //                      //  DemandZoneDesc = demandZone.DemandZoneDesc,
        //                      DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
        //                      DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
        //                      assetModelDesc = mech.MechanismDesc,
        //                      OccupancyStatusID = pso.LastStatus,
        //                      OccupancyStatusDesc = occupancyStatus.StatusDesc,
        //                      NonCompliantStatus =0,// sptxc.NonCompliantStatus,  //***
        //                      //  NonCompliantStatusDesc = sptxc.NonCompliantStatus == null ?"Compliant": PemsEntities.NonCompliantStatus.FirstOrDefault(x=> x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014
        //                      //sptxc.NonCompliantStatus == null ? "Compliant"
        //                      NonCompliantStatusDesc =""// (occupancyStatus.StatusID == 2 ? "" : (sptxc.NonCompliantStatus == null ? "Compliant" : PemsEntities.NonCompliantStatus.FirstOrDefault(x => x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc))//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014

        //                  }).ToList();

        //        IQueryable<GISModel> tempResult = result.AsQueryable();
        //        // tempResult = tempResult.ApplyFiltering(request.Filters);
        //        total = tempResult.Count();
        //        if (pageChosen != "mapChosen")
        //        {
        //            tempResult = tempResult.ApplySorting(request.Groups, request.Sorts);
        //            tempResult = tempResult.ApplyPaging(request.Page, request.PageSize);
        //        }

        //        return tempResult.ToList();


        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("ERROR IN GetSearchMetersData_HighDemandBays Method (GIS Reports - High Demand bays)", ex);
        //    }

        //    return result;
        //}

        public IEnumerable<GISModel> GetSearchMetersData_HighDemandBays(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> occupancyIDs, int Layer, List<int?> NonComplianceStatusIDs, [DataSourceRequest] DataSourceRequest request, out int total, string pageChosen)
        {
            List<GISModel> final = new List<GISModel>();
            bool NonCommStatus = false;

            total = 0;

            int? myAreaId = null;
            int? myZoneId = null;
            int? myAssetID = null;

            string paramValues = string.Empty;

            var spParams = GetSpParams(request, "AssetId desc", out paramValues);

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = null;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = null;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }

                if (assetID == -1)
                {
                    myAssetID = null; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    myAssetID = (int?)assetID;
                }

                //*********************************************************************************************
                //** First convert from string to int;
                short? cityID = (short?)CurrentCity;
                short? areaId = (short?)myAreaId;
                short? zoneId = (short?)myZoneId;

                string spaceStatusType = string.Empty;

                //** Check for space status wise items
                if (occupancyIDs.Count() > 1)
                {
                    //** It indicates all are selected
                    spaceStatusType = "All";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 1)
                {
                    spaceStatusType = "Paid";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 2)
                {
                    spaceStatusType = "Paid and Occupied";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 3)
                {
                    spaceStatusType = "Occupied";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 4)
                {
                    spaceStatusType = "Vacant";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 5)
                {
                    spaceStatusType = "Expired";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 6)
                {
                    spaceStatusType = "Violated";
                }


                var result = PemsEntities.zzParkingInfoProc(cityID, areaId, zoneId, myAssetID).ToList<Duncan.PEMS.DataAccess.PEMS.zzzParkingSpace>();

                //** All status is required
                for (var i = 0; i < result.Count(); i++)
                {
                    GISModel inst = new GISModel();

                    inst.CustomerID = result[i].Customerid;
                    inst.AssetID = (long)result[i].ParkingSpaceID;
                    inst.MeterName = result[i].MeterName;
                    inst.ZoneID = (int)result[i].ZoneID;
                    inst.ZoneName = result[i].ZoneName;
                    inst.AreaID = (int)result[i].AreaId;
                    inst.AreaName = result[i].areaname;
                    inst.Location = result[i].Location;
                    inst.MeterGroupDesc = result[i].MeterType;
                    inst.MeterGroup = 20; //Parking space alone;
                    inst.Latitude = result[i].Latitude;
                    inst.Longitude = result[i].Longitude;
                    inst.DemandZoneId = 1;
                    inst.DemandZoneDesc = "High";
                    inst.assetModelDesc = "Parking Space";

                    inst.MeterID = result[i].MeterID.Value;
                    inst.BayID = result[i].ParkingSpaceID;
                    inst.HasSensor = result[i].HasSensor;
                    inst.PaymentType = result[i].LastPaymentSource;
                    inst.CurrentMeterTime = result[i].PresentMeterTime;
                    inst.NonSensorEventTime = Convert.ToDateTime(result[i].NonSensorEventTime);
                    inst.PaymentTime = Convert.ToString(inst.NonSensorEventTime);
                    inst.TimeZoneID = result[i].timezoneid;
                    inst.BayNumber = result[i].baynumber;
                    //** Logic to determine 'Expired' in case of no sensor for space

                    DateTime d1 = Convert.ToDateTime(result[i].ExpiryTime);
                    DateTime d2 = Convert.ToDateTime(result[i].PresentMeterTime);
                    DateTime d3 = Convert.ToDateTime(result[i].SensorEventTime); //** added on May 30th 2016 for finding NonCommSensor

                    int comp = DateTime.Compare(d1, d2);

                    if (result[i].HasSensor == false && comp < 0)
                    {
                        //** Greater 
                        inst.OccupancyStatusDesc = "Expired";
                    }
                    else if (result[i].HasSensor == false && comp < 0)
                    {
                        inst.OccupancyStatusDesc = "Paid";
                    }
                    else
                    {
                        inst.OccupancyStatusDesc = result[i].SpaceStatus;
                    }


                    //** Find non comm sensors - Logic to determine 'NonCommSensor' 

                    if (d3 != null)  //** Only for meters having sensors
                    {
                        TimeSpan diff = d2 - d3;
                        double hours = diff.TotalHours;

                        if (hours > 24)
                        {
                            NonCommStatus = true;
                        }
                        else
                        {
                            NonCommStatus = false;
                        }

                    }

                    inst.NonCommStatusDesc = NonCommStatus;

                    inst.ViolationType = result[i].ViolationType;
                    inst.amountInCent = result[i].AmountInCents;
                    inst.endDateTimeMob = Convert.ToString(result[i].ExpiryTime);

                    inst.startDateTimeMob = Convert.ToString(result[i].SensorEventTime);
                    inst.NonSensorEventTime = Convert.ToDateTime(result[i].NonSensorEventTime);
                    inst.PaymentTime = Convert.ToString(inst.NonSensorEventTime);

                    if (spaceStatusType == "All")
                    {
                        final.Add(inst);
                    }
                    else if (spaceStatusType.ToUpper() == inst.OccupancyStatusDesc.ToUpper())
                    {
                        final.Add(inst);
                    }
                }


                //**********************************************************************************************

                IQueryable<GISModel> tempResult = final.AsQueryable();
                // tempResult = tempResult.ApplyFiltering(request.Filters);
                total = tempResult.Count();
                if (pageChosen != "mapChosen")
                {
                    tempResult = tempResult.ApplySorting(request.Groups, request.Sorts);
                    tempResult = tempResult.ApplyPaging(request.Page, request.PageSize);
                }

                return tempResult.ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_HighDemandBays Method (GIS Reports - High Demand bays)", ex);
            }

            return final;
        }

        public IEnumerable<GISModel> GetSearchMetersData_HighDemandBays_MapAlone(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> occupancyIDs, int Layer, List<int?> NonComplianceStatusIDs, string pageChosen)
        {

            List<GISModel> final = new List<GISModel>();

            bool NonCommStatus = false;
            int? myAreaId = null;
            int? myZoneId = null;
            int? myAssetID = null;

            string paramValues = string.Empty;

            //var spParams = GetSpParams(request, "AssetId desc", out paramValues);

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = null;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = null;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }

                if (assetID == -1)
                {
                    myAssetID = null; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    myAssetID = (int?)assetID;
                }

                //*********************************************************************************************
                //** First convert from string to int;
                short? cityID = (short?)CurrentCity;
                short? areaId = (short?)myAreaId;
                short? zoneId = (short?)myZoneId;

                string spaceStatusType = string.Empty;

                //** Check for space status wise items
                if (occupancyIDs.Count() > 1)
                {
                    //** It indicates all are selected
                    spaceStatusType = "All";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 1)
                {
                    spaceStatusType = "Paid";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 2)
                {
                    spaceStatusType = "Paid & Occupied";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 3)
                {
                    spaceStatusType = "Occupied";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 4)
                {
                    spaceStatusType = "Vacant";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 5)
                {
                    spaceStatusType = "Expired";
                }
                else if (occupancyIDs.Count() == 1 && occupancyIDs[0] == 6)
                {
                    spaceStatusType = "Violated";
                }

                var result = PemsEntities.zzParkingInfoProc(cityID, areaId, zoneId, myAssetID).ToList<Duncan.PEMS.DataAccess.PEMS.zzzParkingSpace>();

                //** All status is required
                for (var i = 0; i < result.Count(); i++)
                {
                    GISModel inst = new GISModel();

                    inst.CustomerID = result[i].Customerid;
                    inst.AssetID = (long)result[i].ParkingSpaceID;
                    inst.MeterName = result[i].MeterName;
                    inst.ZoneID = (int)result[i].ZoneID;
                    inst.ZoneName = result[i].ZoneName;
                    inst.AreaID = (int)result[i].AreaId;
                    inst.AreaName = result[i].areaname;
                    inst.Location = result[i].Location;
                    inst.MeterGroupDesc = result[i].MeterType;
                    inst.MeterGroup = 20; //Parking space alone;
                    inst.Latitude = result[i].Latitude;
                    inst.Longitude = result[i].Longitude;
                    inst.DemandZoneId = 1;
                    inst.DemandZoneDesc = "High";
                    inst.assetModelDesc = "Parking Space";

                    inst.MeterID = result[i].MeterID.Value;
                    inst.BayID = result[i].ParkingSpaceID;
                    inst.HasSensor = result[i].HasSensor;
                    inst.PaymentType = result[i].LastPaymentSource;
                    inst.CurrentMeterTime = result[i].PresentMeterTime;
                    inst.NonSensorEventTime = Convert.ToDateTime(result[i].NonSensorEventTime);
                    inst.PaymentTime = Convert.ToString(inst.NonSensorEventTime);
                    inst.TimeZoneID = result[i].timezoneid;
                    inst.BayNumber = result[i].baynumber;
                    //** Logic to determine 'Expired' in case of no sensor for space

                    DateTime d1 = Convert.ToDateTime(result[i].ExpiryTime);
                    DateTime d2 = Convert.ToDateTime(result[i].PresentMeterTime);
                    DateTime d3 = Convert.ToDateTime(result[i].SensorEventTime); //** added on May 30th 2016 for finding NonCommSensor


                    int comp = DateTime.Compare(d1, d2);

                    if (result[i].HasSensor == false && comp < 0)
                    {
                        //** Greater 
                        inst.OccupancyStatusDesc = "Expired";
                    }
                    else if (result[i].HasSensor == false && comp < 0)
                    {
                        inst.OccupancyStatusDesc = "Paid";
                    }
                    else
                    {
                        inst.OccupancyStatusDesc = result[i].SpaceStatus;
                    }

                    //** Find non comm sensors - Logic to determine 'NonCommSensor' 

                    if (d3 != null)  //** Only for meters having sensors
                    {
                        TimeSpan diff = d2 - d3;
                        double hours = diff.TotalHours;

                        if (hours > 24)
                        {
                            NonCommStatus = true;
                        }
                        else
                        {
                            NonCommStatus = false;
                        }

                    }

                    inst.NonCommStatusDesc = NonCommStatus;

                    inst.ViolationType = result[i].ViolationType;
                    inst.amountInCent = result[i].AmountInCents;
                    inst.endDateTimeMob = Convert.ToString(result[i].ExpiryTime);

                    inst.startDateTimeMob = Convert.ToString(result[i].SensorEventTime);
                    inst.NonSensorEventTime = Convert.ToDateTime(result[i].NonSensorEventTime);
                    inst.PaymentTime = Convert.ToString(inst.NonSensorEventTime);

                    if (spaceStatusType == "All")
                    {
                        final.Add(inst);
                    }
                    else if (spaceStatusType.ToUpper() == inst.OccupancyStatusDesc.ToUpper())
                    {
                        final.Add(inst);
                    }

                }


                //**********************************************************************************************

                IQueryable<GISModel> tempResult = final.AsQueryable();
                // tempResult = tempResult.ApplyFiltering(request.Filters);
                // total = tempResult.Count();
                //if (pageChosen != "mapChosen")
                //{
                //    tempResult = tempResult.ApplySorting(request.Groups, request.Sorts);
                //    tempResult = tempResult.ApplyPaging(request.Page, request.PageSize);
                //}

                return tempResult.ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_HighDemandBays Method (GIS Reports - High Demand bays)", ex);
            }

            return final;
        }

        //public IEnumerable<GISModel> GetSearchMetersData_HighDemandBays_MapAlone(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> occupancyIDs, int Layer, List<int?> NonComplianceStatusIDs, string pageChosen)
        //{

        //    IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
        //    //   total = 0;
        //    string checkAssetModel;

        //    string isNoncompliantStatus = "-1";

        //    int myAreaId = -1;
        //    int myZoneId = -1;

        //    string paramValues = string.Empty;

        //    //   var spParams = GetSpParams(request, "AssetId desc", out paramValues);

        //    try
        //    {

        //        //** Sairam added on Oct 13th 2014 to filter by area or zone names
        //        if (areaNameIs == "")
        //        {
        //            myAreaId = -1;
        //        }
        //        else
        //        {
        //            myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
        //        }

        //        if (zoneNameIs == "")
        //        {
        //            myZoneId = -1;
        //        }
        //        else
        //        {
        //            myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
        //        }

        //        if (assetModelIDs.Count() == 0)
        //        {
        //            checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
        //        }
        //        else
        //        {
        //            checkAssetModel = assetModelIDs[0].ToString();
        //        }



        //        string checkDemZone;
        //        checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer


        //        result = (from parkingSpaces in PemsEntities.ParkingSpaces
        //                  join customer in PemsEntities.Customers
        //                  on parkingSpaces.CustomerID equals customer.CustomerID

        //                  //**Sairam modified this code on oct 3rd 2014 to do correct mapping
        //                  join meter in PemsEntities.Meters
        //                  on new { CustomerId = parkingSpaces.CustomerID, Areaid = parkingSpaces.AreaId, Meterid = parkingSpaces.MeterId } equals new { CustomerId = meter.CustomerID, Areaid = meter.AreaID, Meterid = meter.MeterId } into meterHolder
        //                  from m1 in meterHolder.DefaultIfEmpty()

        //                  //**Sairam modified this code on oct 3rd 2014 to do correct mapping
        //                  join mm in PemsEntities.MeterMaps on
        //                      new { MeterId = m1.MeterId, AreaId = m1.AreaID, CustomerId = m1.CustomerID } equals
        //                      new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
        //                  //new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
        //                  from l1 in metermap.DefaultIfEmpty()


        //                  join area in PemsEntities.Areas
        //                  on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
        //                  from A1 in mmArea.DefaultIfEmpty()


        //                  join zone in PemsEntities.Zones
        //                  on l1.ZoneId equals zone.ZoneId  //**Sairam modified this code on oct 3rd 2014

        //                  //** The below join ('demandzone') is commented temporarily on 29th sep 2014. It has to be enabled
        //                  //join demandZone in PemsEntities.DemandZones
        //                  //on parkingSpaces.DemandZoneId equals demandZone.DemandZoneId

        //                  //** The below join ('parkingSpaces.ParkingSpaceType') is uncommented temporarily on Oct 3rd 2014. It has to be enabled
        //                  //join materGroup in PemsEntities.MeterGroups
        //                  //on parkingSpaces.ParkingSpaceType equals materGroup.MeterGroupId

        //                  join mech in PemsEntities.MechanismMasters  //**Correct Mapping
        //                  on m1.MeterType equals mech.MechanismId

        //                  join pso in PemsEntities.ParkingSpaceOccupancies
        //                  on parkingSpaces.ParkingSpaceId equals pso.ParkingSpaceId

        //                  join occupancyStatus in PemsEntities.OccupancyStatus
        //                  on pso.LastStatus equals occupancyStatus.StatusID

        //                  //join sptxc in PemsEntities.SensorPaymentTransactionCurrents
        //                  //on parkingSpaces.ParkingSpaceId equals sptxc.ParkingSpaceId

        //                  //join ncs in PemsEntities.NonCompliantStatus
        //                  //on sptxc.NonCompliantStatus equals ncs.NonCompliantStatusID

        //                  where customer.CustomerID == CurrentCity
        //                      //** The below 'assettype.Contains(parkingSpaces.ParkingSpaceType)' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled 
        //                      // && assettype.Contains(parkingSpaces.ParkingSpaceType)
        //                      //** The below 'assettype.Contains(20)' is used temporarily on 29th sep 2014 by sairam. It has to be commented as it is hardcoded
        //                    && assettype.Contains(20)
        //                    && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
        //                    && (parkingSpaces.ParkingSpaceId == assetID || assetID == -1)
        //                    && (occupancyIDs.Contains(pso.LastStatus)) //** To check occupancy status from ParkingSpaceOccupancy table
        //                      // && (NonComplianceStatusIDs.Contains(sptxc.NonCompliantStatus)) || (string.IsNullOrEmpty(SqlFunctions.StringConvert((decimal) sptxc.NonCompliantStatus))) //** To check Non-Compliant status from SensorPaymentTransactionCurrent table
        //                      //&& (area.AreaID == areaId || areaId == -1)
        //                      //&& (zone.ZoneId == zoneId || zoneId == -1)
        //                      && (A1.AreaID == myAreaId || myAreaId == -1)
        //                      && (zone.ZoneId == myZoneId || myZoneId == -1)
        //                      && (m1.Location == street || street == string.Empty)

        //                  //** The below 'parkingSpaces.DemandZoneId' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled later
        //                  // && (demZone.Contains(parkingSpaces.DemandZoneId) || checkDemZone == "-1")


        //                  select new GISModel
        //                  {
        //                      CustomerID = customer.CustomerID,
        //                      AssetID = parkingSpaces.ParkingSpaceId,
        //                      MeterName = m1.MeterName,
        //                      ZoneID = zone.ZoneId,
        //                      ZoneName = zone.ZoneName,
        //                      AreaID = A1.AreaID,
        //                      //AreaName = area.AreaName,
        //                      AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,

        //                      Location = m1.Location,
        //                      //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once join with parkingspacetype is ready
        //                      //MeterGroup = materGroup.MeterGroupId,
        //                      //MeterGroupDesc = materGroup.MeterGroupDesc,
        //                      //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
        //                      MeterGroup = 20,
        //                      MeterGroupDesc = "Parking Spaces",
        //                      Latitude = m1.Latitude,
        //                      Longitude = m1.Longitude,
        //                      //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
        //                      //  DemandZoneId = demandZone.DemandZoneId,
        //                      //  DemandZoneDesc = demandZone.DemandZoneDesc,
        //                      DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
        //                      DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
        //                      assetModelDesc = mech.MechanismDesc,
        //                      OccupancyStatusID = pso.LastStatus,
        //                      OccupancyStatusDesc = occupancyStatus.StatusDesc,
        //                      NonCompliantStatus =0,// sptxc.NonCompliantStatus,  //***
        //                      //  NonCompliantStatusDesc = sptxc.NonCompliantStatus == null ?"Compliant": PemsEntities.NonCompliantStatus.FirstOrDefault(x=> x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014
        //                      //sptxc.NonCompliantStatus == null ? "Compliant"
        //                      NonCompliantStatusDesc = ""//(occupancyStatus.StatusID == 2 ? "" : (sptxc.NonCompliantStatus == null ? "Compliant" : PemsEntities.NonCompliantStatus.FirstOrDefault(x => x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc))//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014

        //                  }).ToList();

        //        IQueryable<GISModel> tempResult = result.AsQueryable();
        //        // tempResult = tempResult.ApplyFiltering(request.Filters);
        //        //total = tempResult.Count();
        //        //if (pageChosen != "mapChosen")
        //        //{
        //        //    tempResult = tempResult.ApplySorting(request.Groups, request.Sorts);
        //        //    tempResult = tempResult.ApplyPaging(request.Page, request.PageSize);
        //        //}

        //        return tempResult.ToList();


        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("ERROR IN GetSearchMetersData_HighDemandBays Method (GIS Reports - High Demand bays)", ex);
        //    }

        //    return result;
        //}



        public IEnumerable<GISModel> GetSearchMetersData_HighDemandBays_Home(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> occupancyIDs, int Layer, List<int?> NonComplianceStatusIDs)
        {

            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
            // total = 0;
            string checkAssetModel;

            string isNoncompliantStatus = "-1";

            int myAreaId = -1;
            int myZoneId = -1;

            string paramValues = string.Empty;

            // var spParams = GetSpParams(request, "AssetId desc", out paramValues);

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }

                if (assetModelIDs.Count() == 0)
                {
                    checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    checkAssetModel = assetModelIDs[0].ToString();
                }



                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer


                result = (from parkingSpaces in PemsEntities.ParkingSpaces
                          join customer in PemsEntities.Customers
                          on parkingSpaces.CustomerID equals customer.CustomerID

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join meter in PemsEntities.Meters
                          on new { CustomerId = parkingSpaces.CustomerID, Areaid = parkingSpaces.AreaId, Meterid = parkingSpaces.MeterId } equals new { CustomerId = meter.CustomerID, Areaid = meter.AreaID, Meterid = meter.MeterId } into meterHolder
                          from m1 in meterHolder.DefaultIfEmpty()

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join mm in PemsEntities.MeterMaps on
                              new { MeterId = m1.MeterId, AreaId = m1.AreaID, CustomerId = m1.CustomerID } equals
                              new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                          //new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                          from l1 in metermap.DefaultIfEmpty()


                          join area in PemsEntities.Areas
                          on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                          from A1 in mmArea.DefaultIfEmpty()


                          join zone in PemsEntities.Zones
                          on l1.ZoneId equals zone.ZoneId  //**Sairam modified this code on oct 3rd 2014

                          //** The below join ('demandzone') is commented temporarily on 29th sep 2014. It has to be enabled
                          //join demandZone in PemsEntities.DemandZones
                          //on parkingSpaces.DemandZoneId equals demandZone.DemandZoneId

                          //** The below join ('parkingSpaces.ParkingSpaceType') is uncommented temporarily on Oct 3rd 2014. It has to be enabled
                          //join materGroup in PemsEntities.MeterGroups
                          //on parkingSpaces.ParkingSpaceType equals materGroup.MeterGroupId

                          join mech in PemsEntities.MechanismMasters  //**Correct Mapping
                          on m1.MeterType equals mech.MechanismId

                          join pso in PemsEntities.ParkingSpaceOccupancies
                          on parkingSpaces.ParkingSpaceId equals pso.ParkingSpaceId

                          join occupancyStatus in PemsEntities.OccupancyStatus
                          on pso.LastStatus equals occupancyStatus.StatusID

                          //join sptxc in PemsEntities.SensorPaymentTransactionCurrents
                          //on parkingSpaces.ParkingSpaceId equals sptxc.ParkingSpaceId

                          //join ncs in PemsEntities.NonCompliantStatus
                          //on sptxc.NonCompliantStatus equals ncs.NonCompliantStatusID

                          where customer.CustomerID == CurrentCity
                              //** The below 'assettype.Contains(parkingSpaces.ParkingSpaceType)' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled 
                              // && assettype.Contains(parkingSpaces.ParkingSpaceType)
                              //** The below 'assettype.Contains(20)' is used temporarily on 29th sep 2014 by sairam. It has to be commented as it is hardcoded
                            && assettype.Contains(20)
                            && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
                            && (parkingSpaces.ParkingSpaceId == assetID || assetID == -1)
                            && (occupancyIDs.Contains(pso.LastStatus)) //** To check occupancy status from ParkingSpaceOccupancy table
                              // && (NonComplianceStatusIDs.Contains(sptxc.NonCompliantStatus)) || (string.IsNullOrEmpty(SqlFunctions.StringConvert((decimal) sptxc.NonCompliantStatus))) //** To check Non-Compliant status from SensorPaymentTransactionCurrent table
                              //&& (area.AreaID == areaId || areaId == -1)
                              //&& (zone.ZoneId == zoneId || zoneId == -1)
                              && (A1.AreaID == myAreaId || myAreaId == -1)
                              && (zone.ZoneId == myZoneId || myZoneId == -1)
                              && (m1.Location == street || street == string.Empty)

                          //** The below 'parkingSpaces.DemandZoneId' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled later
                          // && (demZone.Contains(parkingSpaces.DemandZoneId) || checkDemZone == "-1")


                          select new GISModel
                          {
                              CustomerID = customer.CustomerID,
                              AssetID = parkingSpaces.ParkingSpaceId,
                              MeterName = m1.MeterName,
                              ZoneID = zone.ZoneId,
                              ZoneName = zone.ZoneName,
                              AreaID = A1.AreaID,
                              //AreaName = area.AreaName,
                              AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,

                              Location = m1.Location,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once join with parkingspacetype is ready
                              //MeterGroup = materGroup.MeterGroupId,
                              //MeterGroupDesc = materGroup.MeterGroupDesc,
                              //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                              MeterGroup = 20,
                              MeterGroupDesc = "Parking Spaces",
                              Latitude = m1.Latitude,
                              Longitude = m1.Longitude,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                              //  DemandZoneId = demandZone.DemandZoneId,
                              //  DemandZoneDesc = demandZone.DemandZoneDesc,
                              DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                              DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                              assetModelDesc = mech.MechanismDesc,
                              OccupancyStatusID = pso.LastStatus,
                              OccupancyStatusDesc = occupancyStatus.StatusDesc,
                              NonCompliantStatus = 0,//sptxc.NonCompliantStatus,  //***
                              //  NonCompliantStatusDesc = sptxc.NonCompliantStatus == null ?"Compliant": PemsEntities.NonCompliantStatus.FirstOrDefault(x=> x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014
                              //sptxc.NonCompliantStatus == null ? "Compliant"
                              NonCompliantStatusDesc = ""//(occupancyStatus.StatusID == 2 ? "" : (sptxc.NonCompliantStatus == null ? "Compliant" : PemsEntities.NonCompliantStatus.FirstOrDefault(x => x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc))//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014

                          }).ToList();

                return result;

                //IQueryable<GISModel> tempResult = result.AsQueryable();
                //// tempResult = tempResult.ApplyFiltering(request.Filters);
                //total = tempResult.Count();
                //tempResult = tempResult.ApplySorting(request.Groups, request.Sorts);
                //tempResult = tempResult.ApplyPaging(request.Page, request.PageSize);
                //return tempResult.ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_HighDemandBays Method (GIS Reports - High Demand bays)", ex);
            }

            return result;
        }

        public IEnumerable<GISModel> GetSearchMetersData_HighDemandBays_OnlyCompliant(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> occupancyIDs, int Layer, List<int?> NonComplianceStatusIDs, [DataSourceRequest] DataSourceRequest request, out int total, string pageChosen)
        {

            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
            total = 0;
            string checkAssetModel;

            string isNoncompliantStatus = "-1";

            int myAreaId = -1;
            int myZoneId = -1;

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }

                if (assetModelIDs.Count() == 0)
                {
                    checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    checkAssetModel = assetModelIDs[0].ToString();
                }



                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer


                result = (from parkingSpaces in PemsEntities.ParkingSpaces
                          join customer in PemsEntities.Customers
                          on parkingSpaces.CustomerID equals customer.CustomerID

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join meter in PemsEntities.Meters
                          on new { CustomerId = parkingSpaces.CustomerID, Areaid = parkingSpaces.AreaId, Meterid = parkingSpaces.MeterId } equals new { CustomerId = meter.CustomerID, Areaid = meter.AreaID, Meterid = meter.MeterId } into meterHolder
                          from m1 in meterHolder.DefaultIfEmpty()

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join mm in PemsEntities.MeterMaps on
                              new { MeterId = m1.MeterId, AreaId = m1.AreaID, CustomerId = m1.CustomerID } equals
                              new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                          //new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                          from l1 in metermap.DefaultIfEmpty()


                          join area in PemsEntities.Areas
                          on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                          from A1 in mmArea.DefaultIfEmpty()


                          join zone in PemsEntities.Zones
                          on l1.ZoneId equals zone.ZoneId  //**Sairam modified this code on oct 3rd 2014

                          //** The below join ('demandzone') is commented temporarily on 29th sep 2014. It has to be enabled
                          //join demandZone in PemsEntities.DemandZones
                          //on parkingSpaces.DemandZoneId equals demandZone.DemandZoneId

                          //** The below join ('parkingSpaces.ParkingSpaceType') is uncommented temporarily on Oct 3rd 2014. It has to be enabled
                          //join materGroup in PemsEntities.MeterGroups
                          //on parkingSpaces.ParkingSpaceType equals materGroup.MeterGroupId

                          join mech in PemsEntities.MechanismMasters  //**Correct Mapping
                          on m1.MeterType equals mech.MechanismId

                          join pso in PemsEntities.ParkingSpaceOccupancies
                          on parkingSpaces.ParkingSpaceId equals pso.ParkingSpaceId

                          join occupancyStatus in PemsEntities.OccupancyStatus
                          on pso.LastStatus equals occupancyStatus.StatusID

                          //join sptxc in PemsEntities.SensorPaymentTransactionCurrents
                          //on parkingSpaces.ParkingSpaceId equals sptxc.ParkingSpaceId

                          //join ncs in PemsEntities.NonCompliantStatus
                          //on sptxc.NonCompliantStatus equals ncs.NonCompliantStatusID

                          where customer.CustomerID == CurrentCity
                              //** The below 'assettype.Contains(parkingSpaces.ParkingSpaceType)' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled 
                              // && assettype.Contains(parkingSpaces.ParkingSpaceType)
                              //** The below 'assettype.Contains(20)' is used temporarily on 29th sep 2014 by sairam. It has to be commented as it is hardcoded
                            && assettype.Contains(20)
                            && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
                            && (parkingSpaces.ParkingSpaceId == assetID || assetID == -1)
                            && (occupancyIDs.Contains(pso.LastStatus)) //** To check occupancy status from ParkingSpaceOccupancy table
                              //&& (sptxc.NonCompliantStatus == null)
                              // && (NonComplianceStatusIDs.Contains(sptxc.NonCompliantStatus)) || (string.IsNullOrEmpty(SqlFunctions.StringConvert((decimal) sptxc.NonCompliantStatus))) //** To check Non-Compliant status from SensorPaymentTransactionCurrent table
                              //&& (area.AreaID == areaId || areaId == -1)
                              //&& (zone.ZoneId == zoneId || zoneId == -1)
                              && (A1.AreaID == myAreaId || myAreaId == -1)
                              && (zone.ZoneId == myZoneId || myZoneId == -1)
                              && (m1.Location == street || street == string.Empty)

                          //** The below 'parkingSpaces.DemandZoneId' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled later
                          // && (demZone.Contains(parkingSpaces.DemandZoneId) || checkDemZone == "-1")


                          select new GISModel
                          {
                              CustomerID = customer.CustomerID,
                              AssetID = parkingSpaces.ParkingSpaceId,
                              MeterName = m1.MeterName,
                              ZoneID = zone.ZoneId,
                              ZoneName = zone.ZoneName,
                              AreaID = A1.AreaID,
                              //AreaName = area.AreaName,
                              AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,

                              Location = m1.Location,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once join with parkingspacetype is ready
                              //MeterGroup = materGroup.MeterGroupId,
                              //MeterGroupDesc = materGroup.MeterGroupDesc,
                              //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                              MeterGroup = 20,
                              MeterGroupDesc = "Parking Spaces",
                              Latitude = m1.Latitude,
                              Longitude = m1.Longitude,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                              //  DemandZoneId = demandZone.DemandZoneId,
                              //  DemandZoneDesc = demandZone.DemandZoneDesc,
                              DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                              DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                              assetModelDesc = mech.MechanismDesc,
                              OccupancyStatusID = pso.LastStatus,
                              OccupancyStatusDesc = occupancyStatus.StatusDesc,
                              NonCompliantStatus = 0,// sptxc.NonCompliantStatus,  //***
                              //  NonCompliantStatusDesc = sptxc.NonCompliantStatus == null ?"Compliant": PemsEntities.NonCompliantStatus.FirstOrDefault(x=> x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014
                              //sptxc.NonCompliantStatus == null ? "Compliant"
                              NonCompliantStatusDesc = ""//sptxc.NonCompliantStatus == null ? "Compliant" : PemsEntities.NonCompliantStatus.FirstOrDefault(x => x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014

                          }).ToList();
                var tempResult = result.AsQueryable();

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_HighDemandBays Method (GIS Reports - High Demand bays)", ex);
            }

            return result;
        }

        public IEnumerable<GISModel> GetSearchMetersData_HighDemandBays_OnlyCompliant_MapAlone(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> occupancyIDs, int Layer, List<int?> NonComplianceStatusIDs, string pageChosen)
        {

            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
            //total = 0;
            string checkAssetModel;

            string isNoncompliantStatus = "-1";

            int myAreaId = -1;
            int myZoneId = -1;

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }

                if (assetModelIDs.Count() == 0)
                {
                    checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    checkAssetModel = assetModelIDs[0].ToString();
                }



                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer


                result = (from parkingSpaces in PemsEntities.ParkingSpaces
                          join customer in PemsEntities.Customers
                          on parkingSpaces.CustomerID equals customer.CustomerID

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join meter in PemsEntities.Meters
                          on new { CustomerId = parkingSpaces.CustomerID, Areaid = parkingSpaces.AreaId, Meterid = parkingSpaces.MeterId } equals new { CustomerId = meter.CustomerID, Areaid = meter.AreaID, Meterid = meter.MeterId } into meterHolder
                          from m1 in meterHolder.DefaultIfEmpty()

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join mm in PemsEntities.MeterMaps on
                              new { MeterId = m1.MeterId, AreaId = m1.AreaID, CustomerId = m1.CustomerID } equals
                              new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                          //new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                          from l1 in metermap.DefaultIfEmpty()


                          join area in PemsEntities.Areas
                          on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                          from A1 in mmArea.DefaultIfEmpty()


                          join zone in PemsEntities.Zones
                          on l1.ZoneId equals zone.ZoneId  //**Sairam modified this code on oct 3rd 2014

                          //** The below join ('demandzone') is commented temporarily on 29th sep 2014. It has to be enabled
                          //join demandZone in PemsEntities.DemandZones
                          //on parkingSpaces.DemandZoneId equals demandZone.DemandZoneId

                          //** The below join ('parkingSpaces.ParkingSpaceType') is uncommented temporarily on Oct 3rd 2014. It has to be enabled
                          //join materGroup in PemsEntities.MeterGroups
                          //on parkingSpaces.ParkingSpaceType equals materGroup.MeterGroupId

                          join mech in PemsEntities.MechanismMasters  //**Correct Mapping
                          on m1.MeterType equals mech.MechanismId

                          join pso in PemsEntities.ParkingSpaceOccupancies
                          on parkingSpaces.ParkingSpaceId equals pso.ParkingSpaceId

                          join occupancyStatus in PemsEntities.OccupancyStatus
                          on pso.LastStatus equals occupancyStatus.StatusID

                          //join sptxc in PemsEntities.SensorPaymentTransactionCurrents
                          //on parkingSpaces.ParkingSpaceId equals sptxc.ParkingSpaceId

                          //join ncs in PemsEntities.NonCompliantStatus
                          //on sptxc.NonCompliantStatus equals ncs.NonCompliantStatusID

                          where customer.CustomerID == CurrentCity
                              //** The below 'assettype.Contains(parkingSpaces.ParkingSpaceType)' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled 
                              // && assettype.Contains(parkingSpaces.ParkingSpaceType)
                              //** The below 'assettype.Contains(20)' is used temporarily on 29th sep 2014 by sairam. It has to be commented as it is hardcoded
                            && assettype.Contains(20)
                            && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
                            && (parkingSpaces.ParkingSpaceId == assetID || assetID == -1)
                            && (occupancyIDs.Contains(pso.LastStatus)) //** To check occupancy status from ParkingSpaceOccupancy table
                              //&& (sptxc.NonCompliantStatus == null)
                              // && (NonComplianceStatusIDs.Contains(sptxc.NonCompliantStatus)) || (string.IsNullOrEmpty(SqlFunctions.StringConvert((decimal) sptxc.NonCompliantStatus))) //** To check Non-Compliant status from SensorPaymentTransactionCurrent table
                              //&& (area.AreaID == areaId || areaId == -1)
                              //&& (zone.ZoneId == zoneId || zoneId == -1)
                              && (A1.AreaID == myAreaId || myAreaId == -1)
                              && (zone.ZoneId == myZoneId || myZoneId == -1)
                              && (m1.Location == street || street == string.Empty)

                          //** The below 'parkingSpaces.DemandZoneId' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled later
                          // && (demZone.Contains(parkingSpaces.DemandZoneId) || checkDemZone == "-1")


                          select new GISModel
                          {
                              CustomerID = customer.CustomerID,
                              AssetID = parkingSpaces.ParkingSpaceId,
                              MeterName = m1.MeterName,
                              ZoneID = zone.ZoneId,
                              ZoneName = zone.ZoneName,
                              AreaID = A1.AreaID,
                              //AreaName = area.AreaName,
                              AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,

                              Location = m1.Location,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once join with parkingspacetype is ready
                              //MeterGroup = materGroup.MeterGroupId,
                              //MeterGroupDesc = materGroup.MeterGroupDesc,
                              //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                              MeterGroup = 20,
                              MeterGroupDesc = "Parking Spaces",
                              Latitude = m1.Latitude,
                              Longitude = m1.Longitude,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                              //  DemandZoneId = demandZone.DemandZoneId,
                              //  DemandZoneDesc = demandZone.DemandZoneDesc,
                              DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                              DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                              assetModelDesc = mech.MechanismDesc,
                              OccupancyStatusID = pso.LastStatus,
                              OccupancyStatusDesc = occupancyStatus.StatusDesc,
                              NonCompliantStatus = 0,// sptxc.NonCompliantStatus,  //***
                              //  NonCompliantStatusDesc = sptxc.NonCompliantStatus == null ?"Compliant": PemsEntities.NonCompliantStatus.FirstOrDefault(x=> x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014
                              //sptxc.NonCompliantStatus == null ? "Compliant"
                              NonCompliantStatusDesc = ""//sptxc.NonCompliantStatus == null ? "Compliant" : PemsEntities.NonCompliantStatus.FirstOrDefault(x => x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014

                          }).ToList();
                var tempResult = result.AsQueryable();

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_HighDemandBays Method (GIS Reports - High Demand bays)", ex);
            }

            return result;
        }


        /// <summary>
        /// Description: This method is especially for tracking Noncompliant status of the spaces
        /// Modified By: Sairam on 13th oct 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="assettype"></param>
        /// <param name="assetModelIDs"></param>
        /// <param name="assetID"></param>
        /// <param name="areaNameIs"></param>
        /// <param name="zoneNameIs"></param>
        /// <param name="street"></param>
        /// <param name="suburb"></param>
        /// <param name="demZone"></param>
        /// <param name="occupancyIDs"></param>
        /// <param name="Layer"></param>
        /// <param name="NonComplianceStatusIDs"></param>
        /// <returns></returns>
        public IEnumerable<GISModel> GetSearchMetersData_HighDemandBays_NonCompliant(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> occupancyIDs, int Layer, List<int?> NonComplianceStatusIDs, [DataSourceRequest] DataSourceRequest request, out int total, string pageChosen)
        {

            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
            total = 0;
            string checkAssetModel;

            string isNoncompliantStatus = "-1";

            int myAreaId = -1;
            int myZoneId = -1;

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }

                if (assetModelIDs.Count() == 0)
                {
                    checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    checkAssetModel = assetModelIDs[0].ToString();
                }



                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer


                result = (from parkingSpaces in PemsEntities.ParkingSpaces
                          join customer in PemsEntities.Customers
                          on parkingSpaces.CustomerID equals customer.CustomerID

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join meter in PemsEntities.Meters
                          on new { CustomerId = parkingSpaces.CustomerID, Areaid = parkingSpaces.AreaId, Meterid = parkingSpaces.MeterId } equals new { CustomerId = meter.CustomerID, Areaid = meter.AreaID, Meterid = meter.MeterId } into meterHolder
                          from m1 in meterHolder.DefaultIfEmpty()

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join mm in PemsEntities.MeterMaps on
                              new { MeterId = m1.MeterId, AreaId = m1.AreaID, CustomerId = m1.CustomerID } equals
                              new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                          //new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                          from l1 in metermap.DefaultIfEmpty()


                          join area in PemsEntities.Areas
                          on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                          from A1 in mmArea.DefaultIfEmpty()


                          join zone in PemsEntities.Zones
                          on l1.ZoneId equals zone.ZoneId  //**Sairam modified this code on oct 3rd 2014

                          //** The below join ('demandzone') is commented temporarily on 29th sep 2014. It has to be enabled
                          //join demandZone in PemsEntities.DemandZones
                          //on parkingSpaces.DemandZoneId equals demandZone.DemandZoneId

                          //** The below join ('parkingSpaces.ParkingSpaceType') is uncommented temporarily on Oct 3rd 2014. It has to be enabled
                          //join materGroup in PemsEntities.MeterGroups
                          //on parkingSpaces.ParkingSpaceType equals materGroup.MeterGroupId

                          join mech in PemsEntities.MechanismMasters  //**Correct Mapping
                          on m1.MeterType equals mech.MechanismId

                          join pso in PemsEntities.ParkingSpaceOccupancies
                          on parkingSpaces.ParkingSpaceId equals pso.ParkingSpaceId

                          join occupancyStatus in PemsEntities.OccupancyStatus
                          on pso.LastStatus equals occupancyStatus.StatusID

                          //join sptxc in PemsEntities.SensorPaymentTransactionCurrents
                          //on parkingSpaces.ParkingSpaceId equals sptxc.ParkingSpaceId

                          //join ncs in PemsEntities.NonCompliantStatus
                          //on sptxc.NonCompliantStatus equals ncs.NonCompliantStatusID

                          where customer.CustomerID == CurrentCity
                              //** The below 'assettype.Contains(parkingSpaces.ParkingSpaceType)' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled 
                              // && assettype.Contains(parkingSpaces.ParkingSpaceType)
                              //** The below 'assettype.Contains(20)' is used temporarily on 29th sep 2014 by sairam. It has to be commented as it is hardcoded
                            && assettype.Contains(20)
                            && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
                            && (parkingSpaces.ParkingSpaceId == assetID || assetID == -1)
                            && (occupancyIDs.Contains(pso.LastStatus)) //** To check occupancy status from ParkingSpaceOccupancy table
                              // && (NonComplianceStatusIDs.Contains(sptxc.NonCompliantStatus) && pso.LastStatus == 1)  //** To check Non-Compliant status from SensorPaymentTransactionCurrent table
                              //&& (area.AreaID == areaId || areaId == -1)
                              //&& (zone.ZoneId == zoneId || zoneId == -1)
                              && (A1.AreaID == myAreaId || myAreaId == -1)
                              && (zone.ZoneId == myZoneId || myZoneId == -1)
                              && (m1.Location == street || street == string.Empty)

                          //** The below 'parkingSpaces.DemandZoneId' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled later
                          // && (demZone.Contains(parkingSpaces.DemandZoneId) || checkDemZone == "-1")


                          select new GISModel
                          {
                              CustomerID = customer.CustomerID,
                              AssetID = parkingSpaces.ParkingSpaceId,
                              MeterName = m1.MeterName,
                              ZoneID = zone.ZoneId,
                              ZoneName = zone.ZoneName,
                              AreaID = A1.AreaID,
                              //AreaName = area.AreaName,
                              AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,

                              Location = m1.Location,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once join with parkingspacetype is ready
                              //MeterGroup = materGroup.MeterGroupId,
                              //MeterGroupDesc = materGroup.MeterGroupDesc,
                              //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                              MeterGroup = 20,
                              MeterGroupDesc = "Parking Spaces",
                              Latitude = m1.Latitude,
                              Longitude = m1.Longitude,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                              //  DemandZoneId = demandZone.DemandZoneId,
                              //  DemandZoneDesc = demandZone.DemandZoneDesc,
                              DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                              DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                              assetModelDesc = mech.MechanismDesc,
                              OccupancyStatusID = pso.LastStatus,
                              OccupancyStatusDesc = occupancyStatus.StatusDesc,
                              NonCompliantStatus = 0,//sptxc.NonCompliantStatus,  //***
                              // NonCompliantStatusDesc = sptxc.NonCompliantStatus == null ?"Compliant": PemsEntities.NonCompliantStatus.FirstOrDefault(x=> x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014
                              NonCompliantStatusDesc = ""//occupancyStatus.StatusID == 2 ? "" : PemsEntities.NonCompliantStatus.FirstOrDefault(x => x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014

                          }).ToList();
                var tempResult = result.AsQueryable();

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_HighDemandBays Method (GIS Reports - High Demand bays)", ex);
            }

            return result;
        }

        public IEnumerable<GISModel> GetSearchMetersData_HighDemandBays_NonCompliant_MapAlone(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> occupancyIDs, int Layer, List<int?> NonComplianceStatusIDs, string pageChosen)
        {

            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
            //total = 0;
            string checkAssetModel;

            string isNoncompliantStatus = "-1";

            int myAreaId = -1;
            int myZoneId = -1;

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }

                if (assetModelIDs.Count() == 0)
                {
                    checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    checkAssetModel = assetModelIDs[0].ToString();
                }



                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer


                result = (from parkingSpaces in PemsEntities.ParkingSpaces
                          join customer in PemsEntities.Customers
                          on parkingSpaces.CustomerID equals customer.CustomerID

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join meter in PemsEntities.Meters
                          on new { CustomerId = parkingSpaces.CustomerID, Areaid = parkingSpaces.AreaId, Meterid = parkingSpaces.MeterId } equals new { CustomerId = meter.CustomerID, Areaid = meter.AreaID, Meterid = meter.MeterId } into meterHolder
                          from m1 in meterHolder.DefaultIfEmpty()

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join mm in PemsEntities.MeterMaps on
                              new { MeterId = m1.MeterId, AreaId = m1.AreaID, CustomerId = m1.CustomerID } equals
                              new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                          //new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                          from l1 in metermap.DefaultIfEmpty()


                          join area in PemsEntities.Areas
                          on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                          from A1 in mmArea.DefaultIfEmpty()


                          join zone in PemsEntities.Zones
                          on l1.ZoneId equals zone.ZoneId  //**Sairam modified this code on oct 3rd 2014

                          //** The below join ('demandzone') is commented temporarily on 29th sep 2014. It has to be enabled
                          //join demandZone in PemsEntities.DemandZones
                          //on parkingSpaces.DemandZoneId equals demandZone.DemandZoneId

                          //** The below join ('parkingSpaces.ParkingSpaceType') is uncommented temporarily on Oct 3rd 2014. It has to be enabled
                          //join materGroup in PemsEntities.MeterGroups
                          //on parkingSpaces.ParkingSpaceType equals materGroup.MeterGroupId

                          join mech in PemsEntities.MechanismMasters  //**Correct Mapping
                          on m1.MeterType equals mech.MechanismId

                          join pso in PemsEntities.ParkingSpaceOccupancies
                          on parkingSpaces.ParkingSpaceId equals pso.ParkingSpaceId

                          join occupancyStatus in PemsEntities.OccupancyStatus
                          on pso.LastStatus equals occupancyStatus.StatusID

                          //join sptxc in PemsEntities.SensorPaymentTransactionCurrents
                          //on parkingSpaces.ParkingSpaceId equals sptxc.ParkingSpaceId

                          //join ncs in PemsEntities.NonCompliantStatus
                          //on sptxc.NonCompliantStatus equals ncs.NonCompliantStatusID

                          where customer.CustomerID == CurrentCity
                              //** The below 'assettype.Contains(parkingSpaces.ParkingSpaceType)' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled 
                              // && assettype.Contains(parkingSpaces.ParkingSpaceType)
                              //** The below 'assettype.Contains(20)' is used temporarily on 29th sep 2014 by sairam. It has to be commented as it is hardcoded
                            && assettype.Contains(20)
                            && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
                            && (parkingSpaces.ParkingSpaceId == assetID || assetID == -1)
                            && (occupancyIDs.Contains(pso.LastStatus)) //** To check occupancy status from ParkingSpaceOccupancy table
                              //&& (NonComplianceStatusIDs.Contains(sptxc.NonCompliantStatus) && pso.LastStatus == 1)  //** To check Non-Compliant status from SensorPaymentTransactionCurrent table
                              //&& (area.AreaID == areaId || areaId == -1)
                              //&& (zone.ZoneId == zoneId || zoneId == -1)
                              && (A1.AreaID == myAreaId || myAreaId == -1)
                              && (zone.ZoneId == myZoneId || myZoneId == -1)
                              && (m1.Location == street || street == string.Empty)

                          //** The below 'parkingSpaces.DemandZoneId' is commented temporarily on 29th sep 2014 by sairam. It has to be enabled later
                          // && (demZone.Contains(parkingSpaces.DemandZoneId) || checkDemZone == "-1")


                          select new GISModel
                          {
                              CustomerID = customer.CustomerID,
                              AssetID = parkingSpaces.ParkingSpaceId,
                              MeterName = m1.MeterName,
                              ZoneID = zone.ZoneId,
                              ZoneName = zone.ZoneName,
                              AreaID = A1.AreaID,
                              //AreaName = area.AreaName,
                              AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,

                              Location = m1.Location,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once join with parkingspacetype is ready
                              //MeterGroup = materGroup.MeterGroupId,
                              //MeterGroupDesc = materGroup.MeterGroupDesc,
                              //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                              MeterGroup = 20,
                              MeterGroupDesc = "Parking Spaces",
                              Latitude = m1.Latitude,
                              Longitude = m1.Longitude,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                              //  DemandZoneId = demandZone.DemandZoneId,
                              //  DemandZoneDesc = demandZone.DemandZoneDesc,
                              DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                              DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                              assetModelDesc = mech.MechanismDesc,
                              OccupancyStatusID = pso.LastStatus,
                              OccupancyStatusDesc = occupancyStatus.StatusDesc,
                              NonCompliantStatus = 0,// sptxc.NonCompliantStatus,  //***
                              // NonCompliantStatusDesc = sptxc.NonCompliantStatus == null ?"Compliant": PemsEntities.NonCompliantStatus.FirstOrDefault(x=> x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014
                              NonCompliantStatusDesc = ""//occupancyStatus.StatusID == 2 ? "" : PemsEntities.NonCompliantStatus.FirstOrDefault(x => x.NonCompliantStatusID == sptxc.NonCompliantStatus).NonCompliantStatusDesc//ncs.NonCompliantStatusDesc ?? "Compliant"//***  //**Sairam done on oct 7th 2014

                          }).ToList();
                var tempResult = result.AsQueryable();

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_HighDemandBays Method (GIS Reports - High Demand bays)", ex);
            }

            return result;
        }

        /// <summary>
        /// Description:This method is used to populate the Grid in the UI for Meter Operational Status Layer .
        /// Modified By: Sairam on July 21st 2014
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="assettype"></param>
        /// <param name="assetModelIDs"></param>
        /// <param name="assetID"></param>
        /// <param name="areaId"></param>
        /// <param name="zoneId"></param>
        /// <param name="street"></param>
        /// <param name="suburb"></param>
        /// <param name="demZone"></param>
        /// <param name="operationalIDs"></param>
        /// <param name="Layer"></param>
        /// <param name="NonComplianceStatusIDs"></param>
        /// <returns></returns>
        public IEnumerable<GISModel> GetSearchMetersData_ParkingMeterOperationStatus(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> operationalIDs, int Layer, List<int?> NonComplianceStatusIDs, [DataSourceRequest] DataSourceRequest request, out int total, string pageChosen)
        {

            string checkAssetModel;
            total = 0;
            string paramValues = string.Empty;
            // var spParams = GetSpParams(request, "AssetId desc", out paramValues);
            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
            int myAreaId = -1;
            int myZoneId = -1;

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }
                if (assetModelIDs.Count() == 0)
                {
                    checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    checkAssetModel = assetModelIDs[0].ToString();
                }

                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer


                result = (from meter in PemsEntities.Meters
                          join customer in PemsEntities.Customers
                          on meter.CustomerID equals customer.CustomerID

                          //**Sairam commented this below join on oct 3rd 2014 to do correct mapping
                          //join metermap in PemsEntities.MeterMaps
                          //    //on meter.MeterId equals metermap.MeterId
                          //on new { meter.CustomerID, meter.MeterId } equals new {metermap.Customerid, metermap.Areaid, metermap.MeterId}

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join mm in PemsEntities.MeterMaps on
                              new { MeterId = meter.MeterId, AreaId = meter.AreaID, CustomerId = meter.CustomerID } equals
                              new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                          //new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                          from l1 in metermap.DefaultIfEmpty()

                          //join area in PemsEntities.Areas
                          //on new { meter.AreaID, meter.CustomerID } equals new { area.AreaID, area.CustomerID }

                          //join area in PemsEntities.Areas
                          //on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                          //from A1 in mmArea.DefaultIfEmpty()


                          //join zone in PemsEntities.Zones
                          //    //on metermap.ZoneId equals zone.ZoneId
                          //on l1.ZoneId equals zone.ZoneId

                          //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                          //join demandZone in PemsEntities.DemandZones
                          //on meter.DemandZone equals demandZone.DemandZoneId

                          join materGroup in PemsEntities.MeterGroups
                          on meter.MeterGroup equals materGroup.MeterGroupId


                          join mech in PemsEntities.MechanismMasters  //**Correct Mapping
                          on meter.MeterType equals mech.MechanismId

                          join operationalStatus in PemsEntities.OperationalStatus
                          on meter.OperationalStatusID equals operationalStatus.OperationalStatusId

                          //** Sairam commented the below lines on Oct 10th 2014 to show meters (operational and inactive) and even if active alarms are zero
                          //join activeAlarms in PemsEntities.ActiveAlarms
                          //on new { meter.CustomerID, meter.AreaID, meter.MeterId } equals new { activeAlarms.CustomerID, activeAlarms.AreaID, activeAlarms.MeterId }

                          //** Sairam commented the below lines on Oct 10th 2014 to show meters even if active alarms are zero
                          //join eventCode in PemsEntities.EventCodeMasters
                          //on activeAlarms.EventCode equals eventCode.EventCode

                          where customer.CustomerID == CurrentCity
                          && assettype.Contains(meter.MeterGroup)
                          && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
                          && (meter.MeterId == assetID || assetID == -1)
                              //&& (area.AreaID == areaId || areaId == -1)
                              //&& (zone.ZoneId == zoneId || zoneId == -1)
                              //&& (A1.AreaID == myAreaId || myAreaId == -1)
                              //&& (zone.ZoneId == myZoneId || myZoneId == -1)

                          && (meter.Location == street || street == string.Empty)
                          && (demZone.Contains(meter.DemandZone) || checkDemZone == "-1")
                          && (operationalIDs.Contains(meter.OperationalStatusID))


                          select new GISModel
                          {
                              CustomerID = customer.CustomerID,
                              AssetID = meter.MeterId,
                              MeterName = meter.MeterName,
                              //ZoneID = zone.ZoneId,

                              ZoneName = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == CurrentCity) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == CurrentCity).ZoneName,
                              //AreaID = A1.AreaID,
                              //AreaName = area.AreaName,
                              AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,

                              Location = meter.Location,
                              MeterGroup = materGroup.MeterGroupId,
                              MeterGroupDesc = materGroup.MeterGroupDesc,
                              Latitude = meter.Latitude,
                              Longitude = meter.Longitude,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                              // DemandZoneId = demandZone.DemandZoneId,
                              // DemandZoneDesc = demandZone.DemandZoneDesc,
                              DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                              DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                              assetModelDesc = mech.MechanismDesc,
                              OperationalStatusId = operationalStatus.OperationalStatusId,
                              OperationalStatusDesc = operationalStatus.OperationalStatusDesc,
                              //EventCode = eventCode.EventCode,
                              EventCode = PemsEntities.ActiveAlarms.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == meter.AreaID && x.MeterId == meter.MeterId).EventCode == null ? 0 : PemsEntities.ActiveAlarms.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == meter.AreaID && x.MeterId == meter.MeterId).EventCode, //** Sairam added code on Oct 10th 2014 for showing operational and inactive items too
                              //EventDescVerbose = eventCode.EventDescVerbose
                              EventDescVerbose = PemsEntities.EventCodeMasters.FirstOrDefault(y => y.EventCode == PemsEntities.ActiveAlarms.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == meter.AreaID && x.MeterId == meter.MeterId).EventCode).EventDescVerbose == null ? "" : PemsEntities.EventCodeMasters.FirstOrDefault(y => y.EventCode == PemsEntities.ActiveAlarms.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == meter.AreaID && x.MeterId == meter.MeterId).EventCode).EventDescVerbose //** Sairam added code on Oct 10th 2014 for showing operational and inactive items too
                          }).ToList();

                IQueryable<GISModel> tempResult = result.AsQueryable();
                //  tempResult = tempResult.ApplyFiltering(request.Filters);
                total = tempResult.Count();
                if (pageChosen != "mapChosen")
                {
                    tempResult = tempResult.ApplySorting(request.Groups, request.Sorts);
                    tempResult = tempResult.ApplyPaging(request.Page, request.PageSize);
                }

                return tempResult.ToList();

                //result = from r in tempResult
                //         select new GISModel
                //         {
                //             CustomerID = r.CustomerID,
                //             AssetId = Convert.ToString(r.AssetID),
                //             MeterName = r.MeterName,
                //             ZoneID = r.ZoneID,
                //             ZoneName = r.ZoneName,
                //             AreaID = r.AreaID,
                //             AreaName = r.AreaName,
                //             Location = r.Location,
                //             MeterGroup = r.MeterGroup,
                //             MeterGroupDesc = r.MeterGroupDesc,
                //             Latitude = r.Latitude,
                //             Longitude = r.Longitude,
                //             //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                //             // DemandZoneId = demandZone.DemandZoneId,
                //             // DemandZoneDesc = demandZone.DemandZoneDesc,
                //             DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                //             DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                //             assetModelDesc = r.assetModelDesc,
                //             OperationalStatusId = r.OperationalStatusId,
                //             OperationalStatusDesc = r.OperationalStatusDesc,
                //             EventCode = r.EventCode,
                //             EventDescVerbose = r.EventDescVerbose
                //         };
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_ParkingMeterOperationStatus Method (GIS Reports)", ex);
            }

            return result;
        }

        public IEnumerable<GISModel> GetSearchMetersData_ParkingMeterOperationStatus_MapAlone(int CurrentCity, List<int?> assettype, List<int?> assetModelIDs, long assetID, string areaNameIs, string zoneNameIs, string street, string suburb, List<int?> demZone, List<int?> operationalIDs, int Layer, List<int?> NonComplianceStatusIDs, string pageChosen)
        {

            string checkAssetModel;
            //   total = 0;
            string paramValues = string.Empty;
            // var spParams = GetSpParams(request, "AssetId desc", out paramValues);
            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
            int myAreaId = -1;
            int myZoneId = -1;

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }
                if (assetModelIDs.Count() == 0)
                {
                    checkAssetModel = "-1"; //When Parking space Asset type is chosen, there is no asset models for it and hence -1.
                }
                else
                {
                    checkAssetModel = assetModelIDs[0].ToString();
                }

                string checkDemZone;
                checkDemZone = demZone[0].ToString(); //** If the demand zones are disabled for a customer


                result = (from meter in PemsEntities.Meters
                          join customer in PemsEntities.Customers
                          on meter.CustomerID equals customer.CustomerID

                          //**Sairam commented this below join on oct 3rd 2014 to do correct mapping
                          //join metermap in PemsEntities.MeterMaps
                          //    //on meter.MeterId equals metermap.MeterId
                          //on new { meter.CustomerID, meter.MeterId } equals new {metermap.Customerid, metermap.Areaid, metermap.MeterId}

                          //**Sairam modified this code on oct 3rd 2014 to do correct mapping
                          join mm in PemsEntities.MeterMaps on
                              new { MeterId = meter.MeterId, AreaId = meter.AreaID, CustomerId = meter.CustomerID } equals
                              new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                          //new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                          from l1 in metermap.DefaultIfEmpty()

                          //join area in PemsEntities.Areas
                          //on new { meter.AreaID, meter.CustomerID } equals new { area.AreaID, area.CustomerID }

                          //join area in PemsEntities.Areas
                          //on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                          //from A1 in mmArea.DefaultIfEmpty()


                          //join zone in PemsEntities.Zones
                          //    //on metermap.ZoneId equals zone.ZoneId
                          //on l1.ZoneId equals zone.ZoneId

                          //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                          //join demandZone in PemsEntities.DemandZones
                          //on meter.DemandZone equals demandZone.DemandZoneId

                          join materGroup in PemsEntities.MeterGroups
                          on meter.MeterGroup equals materGroup.MeterGroupId


                          join mech in PemsEntities.MechanismMasters  //**Correct Mapping
                          on meter.MeterType equals mech.MechanismId

                          join operationalStatus in PemsEntities.OperationalStatus
                          on meter.OperationalStatusID equals operationalStatus.OperationalStatusId

                          //** Sairam commented the below lines on Oct 10th 2014 to show meters (operational and inactive) and even if active alarms are zero
                          //join activeAlarms in PemsEntities.ActiveAlarms
                          //on new { meter.CustomerID, meter.AreaID, meter.MeterId } equals new { activeAlarms.CustomerID, activeAlarms.AreaID, activeAlarms.MeterId }

                          //** Sairam commented the below lines on Oct 10th 2014 to show meters even if active alarms are zero
                          //join eventCode in PemsEntities.EventCodeMasters
                          //on activeAlarms.EventCode equals eventCode.EventCode

                          where customer.CustomerID == CurrentCity
                          && assettype.Contains(meter.MeterGroup)
                          && (assetModelIDs.Contains(mech.MechanismId) || checkAssetModel == "-1")
                          && (meter.MeterId == assetID || assetID == -1)
                              //&& (area.AreaID == areaId || areaId == -1)
                              //&& (zone.ZoneId == zoneId || zoneId == -1)
                              //&& (A1.AreaID == myAreaId || myAreaId == -1)
                              //&& (zone.ZoneId == myZoneId || myZoneId == -1)

                          && (meter.Location == street || street == string.Empty)
                          && (demZone.Contains(meter.DemandZone) || checkDemZone == "-1")
                          && (operationalIDs.Contains(meter.OperationalStatusID))


                          select new GISModel
                          {
                              CustomerID = customer.CustomerID,
                              AssetID = meter.MeterId,
                              MeterName = meter.MeterName,
                              // ZoneID = zone.ZoneId,
                              ZoneName = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == CurrentCity) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == CurrentCity).ZoneName,
                              // AreaID = A1.AreaID,
                              //AreaName = area.AreaName,
                              AreaName = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == l1.AreaId2).AreaName,

                              Location = meter.Location,
                              MeterGroup = materGroup.MeterGroupId,
                              MeterGroupDesc = materGroup.MeterGroupDesc,
                              Latitude = meter.Latitude,
                              Longitude = meter.Longitude,
                              //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                              // DemandZoneId = demandZone.DemandZoneId,
                              // DemandZoneDesc = demandZone.DemandZoneDesc,
                              DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                              DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                              assetModelDesc = mech.MechanismDesc,
                              OperationalStatusId = operationalStatus.OperationalStatusId,
                              OperationalStatusDesc = operationalStatus.OperationalStatusDesc,
                              //EventCode = eventCode.EventCode,
                              EventCode = PemsEntities.ActiveAlarms.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == meter.AreaID && x.MeterId == meter.MeterId).EventCode == null ? 0 : PemsEntities.ActiveAlarms.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == meter.AreaID && x.MeterId == meter.MeterId).EventCode, //** Sairam added code on Oct 10th 2014 for showing operational and inactive items too
                              //EventDescVerbose = eventCode.EventDescVerbose
                              EventDescVerbose = PemsEntities.EventCodeMasters.FirstOrDefault(y => y.EventCode == PemsEntities.ActiveAlarms.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == meter.AreaID && x.MeterId == meter.MeterId).EventCode).EventDescVerbose == null ? "" : PemsEntities.EventCodeMasters.FirstOrDefault(y => y.EventCode == PemsEntities.ActiveAlarms.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaID == meter.AreaID && x.MeterId == meter.MeterId).EventCode).EventDescVerbose //** Sairam added code on Oct 10th 2014 for showing operational and inactive items too
                          }).ToList();

                IQueryable<GISModel> tempResult = result.AsQueryable();
                //  tempResult = tempResult.ApplyFiltering(request.Filters);
                //total = tempResult.Count();
                //if (pageChosen != "mapChosen")
                //{
                //    tempResult = tempResult.ApplySorting(request.Groups, request.Sorts);
                //    tempResult = tempResult.ApplyPaging(request.Page, request.PageSize);
                //}

                return tempResult.ToList();

                //result = from r in tempResult
                //         select new GISModel
                //         {
                //             CustomerID = r.CustomerID,
                //             AssetId = Convert.ToString(r.AssetID),
                //             MeterName = r.MeterName,
                //             ZoneID = r.ZoneID,
                //             ZoneName = r.ZoneName,
                //             AreaID = r.AreaID,
                //             AreaName = r.AreaName,
                //             Location = r.Location,
                //             MeterGroup = r.MeterGroup,
                //             MeterGroupDesc = r.MeterGroupDesc,
                //             Latitude = r.Latitude,
                //             Longitude = r.Longitude,
                //             //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                //             // DemandZoneId = demandZone.DemandZoneId,
                //             // DemandZoneDesc = demandZone.DemandZoneDesc,
                //             DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                //             DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                //             assetModelDesc = r.assetModelDesc,
                //             OperationalStatusId = r.OperationalStatusId,
                //             OperationalStatusDesc = r.OperationalStatusDesc,
                //             EventCode = r.EventCode,
                //             EventDescVerbose = r.EventDescVerbose
                //         };
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSearchMetersData_ParkingMeterOperationStatus Method (GIS Reports)", ex);
            }

            return result;
        }

        public IEnumerable<GISModel> GetMobileOccupancy(int CurrentCity)
        {
            //** First convert from string to int;
            short? cityID = (short?)CurrentCity;
            bool NonCommStatus = false;

            var result = PemsEntities.zzParkingInfoProc(cityID, null, null, null).ToList<Duncan.PEMS.DataAccess.PEMS.zzzParkingSpace>();
            //** Filter data based on space status

            List<GISModel> final = new List<GISModel>();

            //** All status is required
            for (var i = 0; i < result.Count(); i++)
            {
                GISModel inst = new GISModel();

                inst.MeterID = result[i].MeterID.Value;

                inst.MeterName = result[i].MeterName;
                inst.BayID = result[i].ParkingSpaceID;
                inst.HasSensor = result[i].HasSensor;
                inst.PaymentType = result[i].LastPaymentSource;
                inst.CurrentMeterTime = result[i].PresentMeterTime;
                inst.NonSensorEventTime = Convert.ToDateTime(result[i].NonSensorEventTime);
                inst.PaymentTime = Convert.ToString(inst.NonSensorEventTime);
                inst.TimeZoneID = result[i].timezoneid;
                inst.BayNumber = result[i].baynumber;
                //** Logic to determine 'Expired' in case of no sensor for space

                DateTime d1 = Convert.ToDateTime(result[i].ExpiryTime);
                DateTime d2 = Convert.ToDateTime(result[i].PresentMeterTime);
                DateTime d3 = Convert.ToDateTime(result[i].SensorEventTime); //** added on May 30th 2016 for finding NonCommSensor

                int comp = DateTime.Compare(d1, d2);

                if (result[i].HasSensor == false && comp < 0)
                {
                    //** Greater 
                    inst.OccupancyStatusDesc = "Expired";
                }
                else if (result[i].HasSensor == false && comp < 0)
                {
                    inst.OccupancyStatusDesc = "Paid";
                }
                else
                {
                    inst.OccupancyStatusDesc = result[i].SpaceStatus;
                }



                inst.ViolationType = result[i].ViolationType;
                inst.amountInCent = result[i].AmountInCents;
                inst.endDateTimeMob = Convert.ToString(result[i].ExpiryTime);

                inst.startDateTimeMob = Convert.ToString(result[i].SensorEventTime);

                //** Find non comm sensors - Logic to determine 'NonCommSensor' 

                if (d3 != null)  //** Only for meters having sensors
                {
                    TimeSpan diff = d2 - d3;
                    double hours = diff.TotalHours;

                    if (hours > 24)
                    {
                        NonCommStatus = true;
                    }
                    else
                    {
                        NonCommStatus = false;
                    }

                }

                inst.NonCommStatusDesc = NonCommStatus;

                inst.NonSensorEventTime = Convert.ToDateTime(result[i].NonSensorEventTime);
                inst.PaymentTime = Convert.ToString(inst.NonSensorEventTime);
                inst.Location = result[i].Location;
                inst.Latitude = result[i].Latitude;
                inst.Longitude = result[i].Longitude;
                inst.MeterType = result[i].MeterType;
                inst.AreaID = result[i].AreaId.Value;
                inst.AreaName = result[i].areaname;
                inst.ZoneID = result[i].ZoneID.Value;
                inst.ZoneName = result[i].ZoneName;

                final.Add(inst);
            }

            return final;
        }


        public int GetPayAccepted(int customerid)
        {
            var dt2 = DateTime.Now;
            var dt3 = DateTime.Today.AddDays(-1);
            var dt = dt2.Date;
            var dt1 = dt3.Date;
            int count = (from tcc in PemsEntities.TransactionsCreditCards
                         where tcc.CustomerId == customerid
                         && (from rowta in PemsEntities.TransactionsAudits
                             where rowta.Status == 103 &&
                             rowta.TransAuditDate < dt &&
                             rowta.TransAuditDate > dt1
                             select rowta.TransactionID).Contains(tcc.TransactionsCreditCardID) &&
                             tcc.TransDateTime < dt &&
                             tcc.TransDateTime > dt1
                         select tcc.MeterId).Count();
            return count;

        }
        public int GetPayPending(int customerid)
        {
            //DateTime dt, dt1;
            var dt2 = DateTime.Now;
            var dt3 = DateTime.Today.AddDays(-1);
            var dt = dt2.Date;
            var dt1 = dt3.Date;
            int count = (from tcc in PemsEntities.TransactionsCreditCards
                         where tcc.CustomerId == customerid
                         && (from rowta in PemsEntities.TransactionsAudits
                             where rowta.Status == 105 ||
                             rowta.Status == 106 &&
                             rowta.TransAuditDate < dt &&
                             rowta.TransAuditDate > dt1
                             select rowta.TransactionID).Contains(tcc.TransactionsCreditCardID) &&
                             tcc.TransDateTime < dt &&
                             tcc.TransDateTime > dt1
                         select tcc.MeterId).Count();
            return count;

        }
        public int GetPayRefund(int customerid)
        {
            //DateTime dt, dt1;
            var dt2 = DateTime.Now;
            var dt3 = DateTime.Today.AddDays(-1);
            var dt = dt2.Date;
            var dt1 = dt3.Date;
            int count = (from tcc in PemsEntities.TransactionsCreditCards
                         where tcc.CustomerId == customerid
                         && (from rowta in PemsEntities.TransactionsAudits
                             where rowta.Status == 108 &&
                             rowta.TransAuditDate < dt &&
                             rowta.TransAuditDate > dt1
                             select rowta.TransactionID).Contains(tcc.TransactionsCreditCardID) &&
                             tcc.TransDateTime < dt &&
                             tcc.TransDateTime > dt1
                         select tcc.MeterId).Count();
            return count;

        }
        public int GetCashTrans(int customerid)
        {
            //DateTime dt, dt1;
            var dt2 = DateTime.Now;
            var dt3 = DateTime.Today.AddDays(-1);
            var dt = dt2.Date;
            var dt1 = dt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 3 &&
                             //t.TransactionStatus!=104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt1
                         select t.MeterID).Count();
            return count;

        }
        public int GetCreditTrans(int customerid)
        {
            //DateTime dt, dt1;
            var dt2 = DateTime.Now;
            var dt3 = DateTime.Today.AddDays(-1);
            var dt = dt2.Date;
            var dt1 = dt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 1 &&
                         t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt1
                         select t.MeterID).Count();
            return count;

        }
        public int GetSmartTrans(int customerid)
        {
            //DateTime dt, dt1;
            var dt2 = DateTime.Now;
            var dt3 = DateTime.Today.AddDays(-1);
            var dt = dt2.Date;
            var dt1 = dt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 2 &&
                         t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt1
                         select t.MeterID).Count();
            return count;

        }
        public int GetCellTrans(int customerid)
        {
            //DateTime dt, dt1;
            var dt2 = DateTime.Now;
            var dt3 = DateTime.Today.AddDays(-1);
            var dt = dt2.Date;
            var dt1 = dt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 5 &&
                             //t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt1
                         select t.MeterID).Count();
            return count;

        }
        public int GetD5CashTrans(int customerid)
        {
            //DateTime dt, dt5;
            var dt2 = DateTime.Today.AddDays(-4);
            var dt3 = DateTime.Today.AddDays(-5);
            var dt = dt2.Date;
            var dt5 = dt3.Date;

            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 3 &&
                             //t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt5
                         select t.MeterID).Count();
            return count;

        }
        public int GetD5CreditTrans(int customerid)
        {
            //DateTime dt, dt5;
            var dt2 = DateTime.Today.AddDays(-4);
            var dt3 = DateTime.Today.AddDays(-5);
            var dt = dt2.Date;
            var dt5 = dt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 1 &&
                         t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt5
                         select t.MeterID).Count();
            return count;

        }
        public int GetD5SmartTrans(int customerid)
        {
            //DateTime dt, dt5;
            var dt2 = DateTime.Today.AddDays(-4);
            var dt3 = DateTime.Today.AddDays(-5);
            var dt = dt2.Date;
            var dt5 = dt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 2 &&
                         t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt5
                         select t.MeterID).Count();
            return count;

        }
        public int GetD5CellTrans(int customerid)
        {
            //DateTime dt, dt5;
            var dt2 = DateTime.Today.AddDays(-4);
            var dt3 = DateTime.Today.AddDays(-5);
            var dt = dt2.Date;
            var dt5 = dt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 5 &&
                             //t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt5
                         select t.MeterID).Count();
            return count;

        }
        public int GetD4CashTrans(int customerid)
        {
            //DateTime dt, dt4;
            var dt2 = DateTime.Today.AddDays(-3);
            var dt3 = DateTime.Today.AddDays(-4);
            var dt = dt2.Date;
            var dt4 = dt3.Date;

            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 3 &&
                             //t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt4
                         select t.MeterID).Count();
            return count;

        }
        public int GetD4CreditTrans(int customerid)
        {
            //DateTime dt, dt4;
            var dt2 = DateTime.Today.AddDays(-3);
            var dt3 = DateTime.Today.AddDays(-4);
            var dt = dt2.Date;
            var dt4 = dt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 1 &&
                         t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt4
                         select t.MeterID).Count();
            return count;

        }
        public int GetD4SmartTrans(int customerid)
        {
            //DateTime dt, dt4;
            var dt2 = DateTime.Today.AddDays(-3);
            var dt3 = DateTime.Today.AddDays(-4);
            var dt = dt2.Date;
            var dt4 = dt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 2 &&
                         t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt4
                         select t.MeterID).Count();
            return count;

        }
        public int GetD4CellTrans(int customerid)
        {
            //DateTime dt, dt4;
            var dt2 = DateTime.Today.AddDays(-3);
            var dt3 = DateTime.Today.AddDays(-4);
            var dt = dt2.Date;
            var dt4 = dt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 5 &&
                             //t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt4
                         select t.MeterID).Count();
            return count;

        }
        public int GetD3CashTrans(int customerid)
        {
            //DateTime dt, dt3;
            var dtt2 = DateTime.Today.AddDays(-2);
            var dtt3 = DateTime.Today.AddDays(-3);
            var dt = dtt2.Date;
            var dt3 = dtt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 3 &&
                             //t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt3
                         select t.MeterID).Count();
            return count;

        }
        public int GetD3CreditTrans(int customerid)
        {
            //DateTime dt, dt3;
            var dtt2 = DateTime.Today.AddDays(-2);
            var dtt3 = DateTime.Today.AddDays(-3);
            var dt = dtt2.Date;
            var dt3 = dtt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 1 &&
                         t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt3
                         select t.MeterID).Count();
            return count;

        }
        public int GetD3SmartTrans(int customerid)
        {
            //DateTime dt, dt3;
            var dtt2 = DateTime.Today.AddDays(-2);
            var dtt3 = DateTime.Today.AddDays(-3);
            var dt = dtt2.Date;
            var dt3 = dtt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 2 &&
                         t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt3
                         select t.MeterID).Count();
            return count;

        }
        public int GetD3CellTrans(int customerid)
        {
            //DateTime dt, dt3;
            var dtt2 = DateTime.Today.AddDays(-2);
            var dtt3 = DateTime.Today.AddDays(-3);
            var dt = dtt2.Date;
            var dt3 = dtt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 5 &&
                             //t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt3
                         select t.MeterID).Count();
            return count;

        }
        public int GetD2CashTrans(int customerid)
        {
            //DateTime dt, dt2;
            var dtt2 = DateTime.Today.AddDays(-1);
            var dtt3 = DateTime.Today.AddDays(-2);
            var dt = dtt2.Date;
            var dt2 = dtt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 3 &&
                             //t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt2
                         select t.MeterID).Count();
            return count;

        }
        public int GetD2CreditTrans(int customerid)
        {
            //DateTime dt, dt2;
            var dtt2 = DateTime.Today.AddDays(-1);
            var dtt3 = DateTime.Today.AddDays(-2);
            var dt = dtt2.Date;
            var dt2 = dtt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 1 &&
                         t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt2
                         select t.MeterID).Count();
            return count;

        }
        public int GetD2SmartTrans(int customerid)
        {
            //DateTime dt, dt2;
            var dtt2 = DateTime.Today.AddDays(-1);
            var dtt3 = DateTime.Today.AddDays(-2);
            var dt = dtt2.Date;
            var dt2 = dtt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 2 &&
                         t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt2
                         select t.MeterID).Count();
            return count;

        }
        public int GetD2CellTrans(int customerid)
        {
            //DateTime dt, dt2;
            var dtt2 = DateTime.Today.AddDays(-1);
            var dtt3 = DateTime.Today.AddDays(-2);
            var dt = dtt2.Date;
            var dt2 = dtt3.Date;
            int count = (from t in PemsEntities.Transactions
                         where t.CustomerID == customerid &&
                         t.TransactionType == 5 &&
                             //t.TransactionStatus != 104 &&
                         t.TransDateTime < dt &&
                         t.TransDateTime > dt2
                         select t.MeterID).Count();
            return count;

        }
        public int GetActSensor(int customerid)
        {
            int count = (from p in PemsEntities.ParkingSpaces
                         where p.CustomerID == customerid &&
                         p.HasSensor == true
                         select p.MeterId).Count();
            return count;
        }
        public int GetCommSensor(int customerid)
        {
            var dt2 = DateTime.Now;
            var dt3 = DateTime.Today.AddDays(-1);
            var dt = dt2.Date;
            var dt1 = dt3.Date;
            int count = (from rowmc in PemsEntities.MeterComms
                         join rowm in PemsEntities.Meters
                         on rowmc.MeterId equals rowm.MeterId
                         where rowmc.MeterTime < dt &&
                         rowmc.MeterTime > dt1 &&
                         rowmc.CustomerId == customerid &&
                         rowm.OperationalStatusID != 0
                         && (from p in PemsEntities.ParkingSpaces
                             where p.CustomerID == customerid &&
                             p.HasSensor == true
                             select p.MeterId).Contains(rowmc.MeterId)
                         select rowmc.MeterId).Distinct().Count();
            return count;
        }
        public int GetMinPurchased(int customerid)
        {
            var dt2 = DateTime.Now;
            var dt3 = DateTime.Today.AddDays(-1);
            var dt = dt2.Date;
            var dt1 = dt3.Date;
            int count = (from p in PemsEntities.OccupancyRateSummaryVs
                         where p.Customerid == customerid &&
                         p.ArrivalTime < dt &&
                         p.ArrivalTime > dt1
                         select (p.TotalTimePaidMinute)).Sum() ?? 0;
            return count / 60;
        }

        //MIN_ZEROD
        //MIN_RESOLD
        public int GetSpaceStatusSens(int customerid)
        {
            var dt2 = DateTime.Now;
            var dt3 = DateTime.Today.AddDays(-1);
            var dt = dt2.Date;
            var dt1 = dt3.Date;
            int count = (from p in PemsEntities.ParkingSpaces
                         where p.CustomerID == customerid &&
                         p.HasSensor == true
                         select (p.ParkingSpaceId)).Count();
            return count / 60;
        }
        public int GetSpaceStatusSensOccp(int customerid)
        {
            var dt2 = DateTime.Now;
            var dt3 = DateTime.Today.AddDays(-1);
            var dt = dt2.Date;
            var dt1 = dt3.Date;
            int count = (from pso in PemsEntities.ParkingSpaceOccupancies
                         where pso.GCustomerid == customerid &&
                         (from p in PemsEntities.ParkingSpaces
                          where p.CustomerID == customerid &&
                          p.HasSensor == true
                          select (p.ParkingSpaceId)).Contains(pso.ParkingSpaceId) &&
                         pso.LastStatus == 1
                         select pso.ParkingSpaceId).Count();
            return count;
        }

        public Dashboard GetDashBoradInventory(int customerId)
        {
            var currentDate = DateTime.Now;
            var dateMinusOne = DateTime.Today.AddDays(-1);
            var dateMinusTwo = DateTime.Today.AddDays(-2);
            var dateMinusThree = DateTime.Today.AddDays(-3);
            var dateMinusFour = DateTime.Today.AddDays(-4);
            var dateMinusFive = DateTime.Today.AddDays(-5);
            var dateMinusFifteen = DateTime.Today.AddDays(-15);

            //Date Part
            var dt = currentDate.Date;
            var dt1 = dateMinusOne.Date;
            var dt2 = dateMinusTwo.Date;
            var dt3 = dateMinusThree.Date;
            var dt4 = dateMinusFour.Date;
            var dt5 = dateMinusFive.Date;
            var dt15 = dateMinusFifteen.Date;

            var result = PemsEntities.sp_Landing(customerId, dt, dt1, dt2, dt3, dt4, dt5, dt15).FirstOrDefault();
            //var result = PemsEntities.sp_Landing(customerId, dt, dt1, dt2, dt3, dt4, dt5).FirstOrDefault();
            return new Dashboard()
            {
                TotalMeterCount = result.TotalMeterCount,
                ActMeterCount = result.ActMeterCount,
                ActiveMeterAlarmCount = result.ActAlarmCount,
                SevereAlarmCount = result.SevereAlarmCount,
                MajAlarmCount = result.MajAlarmCount,
                ActMeterCommCount = result.ActMeterCommCount,
                ChangeBatteryCount = result.ChangeBatteryCount,
                PlanChangeBatteryCount = result.PlanChangeBatteryCount,
                PaymentAccepted = result.AcceptedFinal,
                PaymentPending = result.Pending,
                PaymentRefunded = result.Refund,
                PaymentAcceptedPending = result.AcceptedPending,
                PaymentDeclained = result.Declined,
                PaymentNoTransaction = result.NoTransaction,
                PaymentAttempted = result.Attempted,
                PaymentDeclainedPending = result.DeclinedPending,
                CashTransaction = result.CashdateMinusOne,
                CreditTransaction = result.CreditdateMinusOne,
                SmartTransaction = result.SmartdateMinusOne,
                CellTransaction = result.CelldateMinusOne,
                D2CashTransaction = result.CashdateMinusTwo,
                D2CreditTransaction = result.CreditdateMinusTwo,
                D2SmartTransaction = result.SmartdateMinusTwo,
                D2CellTransaction = result.CelldateMinusTwo,
                D3CashTransaction = result.CashdateMinusThree,
                D3CreditTransaction = result.CreditdateMinusThree,
                D3SmartTransaction = result.SmartdateMinusThree,
                D3CellTransaction = result.CelldateMinusThree,
                D4CashTransaction = result.CashdateMinusFour,
                D4CreditTransaction = result.CreditdateMinusFour,
                D4SmartTransaction = result.SmartdateMinusFour,
                D4CellTransaction = result.CelldateMinusFour,
                D5CashTransaction = result.CashdateMinusFive,
                D5CreditTransaction = result.CreditdateMinusFive,
                D5SmartTransaction = result.SmartdateMinusFive,
                D5CellTransaction = result.CelldateMinusFive,
                ActWithSensor = result.ActWithSensor,
                TotalMinPurchased = result.TotalMinPurchased,
                MinZeroedOut = result.MinZeroedOut,
                MinResold = result.MinResold,
                SpaceStatusSensor = result.SpaceStatusSensor,
                SpacesWithPayment = result.SpaceWithPayment,
                EnforceableSpaces = result.EnforceableSpaces,
                HasSensor = result.HasSensor,
                SingleSpaceMeter = result.SingleSpaceMeter,
                MultiSpaceMeter = result.MultiSpaceMeter,
                Sensor = result.Sensor,
                Cashbox = result.Cashbox,
                Smartcard = result.Smartcard,
                Gateway = result.Gateway,
                CSPark = result.CSPark,
                ParkingSpaces = result.ParkingSpaces,
                Mechanism = result.Mechanism,
                DataKey = result.DataKey,
                GatewayComm = result.GatewayComm,
                TotalGatewayComm = result.TotalGatewayComm,
                CSPComm = result.CSPComm,
                TotalCSPComm = result.TotalCSPComm,
                ActCommSensor = result.ActCommSensor,
                TotalActCommSensor = result.TotalActCommSensor,
                LibertyComm = result.LibertyComm,
                TotalLibertyComm = result.TotalLibertyComm,
                CommSensor = result.CommSensor
            };
        }

        //public Dashboard GetDashBoradInventory(int customerId)
        //{
        //    var currentDate = DateTime.Now;
        //    var dateMinusOne = DateTime.Today.AddDays(-1);
        //    var dateMinusTwo = DateTime.Today.AddDays(-2);
        //    var dateMinusThree = DateTime.Today.AddDays(-3);
        //    var dateMinusFour = DateTime.Today.AddDays(-4);
        //    var dateMinusFive = DateTime.Today.AddDays(-5);
        //    var dateMinusFifteen = DateTime.Today.AddDays(-15);

        //    //Date Part
        //    var dt = currentDate.Date;
        //    var dt1 = dateMinusOne.Date;
        //    var dt2 = dateMinusTwo.Date;
        //    var dt3 = dateMinusThree.Date;
        //    var dt4 = dateMinusFour.Date;
        //    var dt5 = dateMinusFive.Date;
        //    var dt15 = dateMinusFifteen.Date;

        //    var result = PemsEntities.sp_Landing(customerId, dt, dt1, dt2, dt3, dt4, dt5, dt15).FirstOrDefault();
        //    //var result = PemsEntities.sp_Landing(customerId, dt, dt1, dt2, dt3, dt4, dt5).FirstOrDefault();
        //    return new Dashboard()
        //    {
        //        TotalMeterCount = result.TotalMeterCount,
        //        ActMeterCount = result.ActMeterCount,
        //        ActiveMeterAlarmCount = result.ActAlarmCount,
        //        SevereAlarmCount = result.SevereAlarmCount,
        //        MajAlarmCount = result.MajAlarmCount,
        //        ActMeterCommCount = result.ActMeterCommCount,
        //        ChangeBatteryCount = result.ChangeBatteryCount,
        //        PlanChangeBatteryCount = result.PlanChangeBatteryCount,
        //        PaymentAccepted = result.AcceptedFinal,
        //        PaymentPending = result.Pending,
        //        PaymentRefunded = result.Refund,
        //        CashTransaction = result.CashdateMinusOne,
        //        CreditTransaction = result.CreditdateMinusOne,
        //        SmartTransaction = result.SmartdateMinusOne,
        //        CellTransaction = result.CelldateMinusOne,
        //        D2CashTransaction = result.CashdateMinusTwo,
        //        D2CreditTransaction = result.CreditdateMinusTwo,
        //        D2SmartTransaction = result.SmartdateMinusTwo,
        //        D2CellTransaction = result.CelldateMinusTwo,
        //        D3CashTransaction = result.CashdateMinusThree,
        //        D3CreditTransaction = result.CreditdateMinusThree,
        //        D3SmartTransaction = result.SmartdateMinusThree,
        //        D3CellTransaction = result.CelldateMinusThree,
        //        D4CashTransaction = result.CashdateMinusFour,
        //        D4CreditTransaction = result.CreditdateMinusFour,
        //        D4SmartTransaction = result.SmartdateMinusFour,
        //        D4CellTransaction = result.CelldateMinusFour,
        //        D5CashTransaction = result.CashdateMinusFive,
        //        D5CreditTransaction = result.CreditdateMinusFive,
        //        D5SmartTransaction = result.SmartdateMinusFive,
        //        D5CellTransaction = result.CelldateMinusFive,
        //        ActWithSensor = result.ActWithSensor,
        //        TotalMinPurchased = result.TotalMinPurchased,
        //        MinZeroedOut = result.MinZeroedOut,
        //        MinResold = result.MinResold,
        //        SpaceStatusSensor = result.SpaceStatusSensor,
        //        SpacesWithPayment = result.SpaceWithPayment,
        //        EnforceableSpaces = result.EnforceableSpaces,
        //        HasSensor = result.HasSensor
        //    };

        //}

        public object GetBatteryReport(int customerId)
        {
            object result = PemsEntities.sp_LowBattery(customerId).FirstOrDefault();
            return result;
        }
        public object GetChangeBatteryReport(int customerId)
        {
            object result = PemsEntities.sp_changeBattery(customerId).FirstOrDefault();
            return result;
        }
        public object GetNoMeterCommReport(int customerId)
        {
            var currentDate = DateTime.Now;
            var dateMinusOne = DateTime.Today.AddDays(-1);
            var dt = currentDate.Date;
            var dt1 = dateMinusOne.Date;

            object result = PemsEntities.sp_NoMeterComm(customerId, dt, dt1).FirstOrDefault();
            return result;
        }
        public Double[] GetOccupancyDetails(int customerid, DateTime LocalTime)
        {
            //List<ChartData> objOcc = new List<ChartData>();
            //var src = LocalTime;
            Double[] key = new Double[int.Parse(LocalTime.ToString("HH"))];

            //var currentDate = DateTime.Now;
            var currentDate = LocalTime;
            var dt = currentDate.Date;

            var occupancyHist = (from pso in PemsEntities.ParkingOccupancyReports
                                 where pso.GCustomerid == customerid &&
                                  pso.Date == dt
                                 group pso by new
                                 {
                                     pso.Date,
                                     pso.Time,
                                     //pso.LastUpdatedTS,

                                 } into g
                                 select new
                                 {
                                     //Date = g.Key.Date,
                                     Time = g.Key.Time,
                                     LastStatus = g.Count()
                                 });

            foreach (var version in occupancyHist)
            {
                //ChartData objchar = new ChartData();
                //objchar.Key = version.LastStatus;
                if (key.Length > version.Time)
                    key[version.Time ?? 0] = version.LastStatus;
            }

            return key;
        }

        //Occupancy Status
        public int GetOccupancyStatus(int customerid)
        {
            int? CurrentCity = Convert.ToInt32(customerid);
            short? CID = (short?)CurrentCity;
            int res = 0;
            //Double[] result = PemsEntities.zzzParkingInfo(CID, null, null, null).FirstOrDefault();
            var result = PemsEntities.zzParkingInfoProc(CID, null, null, null).ToList<Duncan.PEMS.DataAccess.PEMS.zzzParkingSpace>();

            for (var i = 0; i < result.Count(); i++)
            {
                //if (result[i].OccupancyStatus == 1)
                if (result[i].HasSensor == true)
                {
                    if (result[i].OccupancyStatus == 2)
                        res++;
                }

            }

            return res;
        }

        //Payment Status
        public int GetPaymentStatus(int customerid)
        {
            int? CurrentCity = Convert.ToInt32(customerid);
            short? CID = (short?)CurrentCity;
            int res = 0;
            //Double[] result = PemsEntities.zzzParkingInfo(CID, null, null, null).FirstOrDefault();
            var result = PemsEntities.zzParkingInfoProc(CID, null, null, null).ToList<Duncan.PEMS.DataAccess.PEMS.zzzParkingSpace>();

            for (var i = 0; i < result.Count(); i++)
            {
                if (result[i].HasSensor == true)
                {
                    if (result[i].OccupancyStatus == 1 && (result[i].PresentMeterTime < result[i].ExpiryTime))
                        res++;
                }
            }

            return res;
        }

        //Enforceable
        public int GetEnforceStatus(int customerid)
        {
            int? CurrentCity = Convert.ToInt32(customerid);
            short? CID = (short?)CurrentCity;
            int res = 0;
            //Double[] result = PemsEntities.zzzParkingInfo(CID, null, null, null).FirstOrDefault();
            var result = PemsEntities.zzParkingInfoProc(CID, null, null, null).ToList<Duncan.PEMS.DataAccess.PEMS.zzzParkingSpace>();

            for (var i = 0; i < result.Count(); i++)
            {
                if (result[i].HasSensor == true)
                {
                    if (result[i].OccupancyStatus == 1 && (result[i].PresentMeterTime > result[i].ExpiryTime))
                        res++;
                }
            }

            return res;
        }

        public object GetSensorComm(int customerId)
        {
            var currentDate = DateTime.Now;
            var dateMinusOne = DateTime.Today.AddDays(-1);
            var dt = currentDate.Date;
            var dt1 = dateMinusOne.Date;
            object result = PemsEntities.sp_SensorComm(customerId, dt, dt1).FirstOrDefault();
            return result;
        }
        public object GetSpacestatusSensor(int customerId, DateTime LocalTime)
        {
            object result = PemsEntities.sp_LandingSpaceStatusSensorTest(customerId, LocalTime).FirstOrDefault();
            return result;
        }
        public object GetEnforceableSpace(int customerId, DateTime LocalTime)
        {
            object result = PemsEntities.sp_LandingEnforceableTest(customerId, LocalTime).FirstOrDefault();
            return result;
        }
    }

}