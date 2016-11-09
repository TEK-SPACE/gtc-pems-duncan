using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.PayByCell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using Kendo.Mvc.UI;
using Duncan.PEMS.Utilities;
using Duncan.PEMS.Business.Users;


namespace Duncan.PEMS.Business.PayByCell
{
    public class PayByCellFactory : BaseFactory
    {
        public PayByCellFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        /// <summary>
        /// Description:This will retreive vendor details from 'PayByCellVendors' for the logged in customer
        /// </summary>
        /// <param name="request"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<PayByCellModelAdmin> GetPayByCellSummary(DataSourceRequest request, out int total)
        {
            string paramValues = string.Empty;
            var spParams = GetSpParams(request, "TimeOfOccurrance desc", out paramValues);
            total = 0;
           
            string VendorID = spParams[3].Value.ToString();

            int myVendorID;

            if (VendorID.Trim().Length != 0)
            {
                myVendorID = Convert.ToInt32(VendorID);
            }
            else
            {
                myVendorID = -1;
            }

            try
            {

                var query = (from vendorMaster in PemsEntities.PayByCellVendors
                             join Cust in PemsEntities.Customers on vendorMaster.CreatedBy equals Cust.CustomerID
                             where (vendorMaster.VendorID == myVendorID || myVendorID == -1)
                             select new PayByCellModelAdmin
                             {
                                 VendorID = vendorMaster.VendorID,
                                 VendorName = vendorMaster.VendorName,
                                 CreatedOnDate = vendorMaster.CreatedOn,
                                 CreatedByID = vendorMaster.CreatedBy,
                                 CustomerID = Cust.CustomerID, //**  
                                 CreatedByName = Cust.Name,
                                 Deprecated=vendorMaster.DEPRECATE
                             });


                total = query.Count();
                query = query.ApplyFiltering(request.Filters);
                total = query.Count();
                query = query.ApplySorting(request.Groups, request.Sorts);
                query = query.ApplyPaging(request.Page, request.PageSize);
                return query.ToList();
            }

            catch (Exception ex)
            {
                var error = ex.Message;
            }
            return new List<PayByCellModelAdmin>();
        }

        /// <summary>
        /// Description:This method will fetch corresponding vendor name  from 'PayByCellVendors' for passed on vendor ID
        /// Modified :Prita()
        /// </summary>
        /// <param name="VID"></param>
        /// <returns></returns>
        public string GetVendorName(int VID)
        {

            return PemsEntities.PayByCellVendors.Where(a => a.VendorID == VID).Select(a => a.VendorName).FirstOrDefault();

        }

        /// <summary>
        /// Description:This method will fetch corresponding Vendor ID  from 'PayByCellVendors' for passed on vendor Name
        /// Modified :Prita()
        /// </summary>
        /// <param name="VendorName"></param>
        /// <returns></returns>
        public int GetVendorID(string VendorName)
        {

            return PemsEntities.PayByCellVendors.Where(a => a.VendorName == VendorName).Select(a => a.VendorID).FirstOrDefault();

        }

        /// <summary>
        /// Description:This will fetch all the vendorIDs and their names in a list from 'PayByCellVendors' table to populate autocomplete in index page
        /// Modified :Prita()
        /// </summary>
        /// <param name="CurrentCity"></param>
        /// <param name="typedtext"></param>
        /// <returns></returns>
        public List<PayByCellModelAdmin> GetAllVendors(int CurrentCity, string typedtext)
        {

            return (from FileTypes in PemsEntities.PayByCellVendors
                  
                    select new PayByCellModelAdmin
                    {
                        Text = FileTypes.VendorName,
                        Value = FileTypes.VendorID
                    }).ToList();
        }

        /// <summary>
        ///Description:This method will save the data from addnewvendor.cshtml into PayByCellVendor table as well as RipnetProperty table
        ///Modified:Prita()
        /// </summary>
        /// <param name="VID"></param>
        /// <param name="VName"></param>
        /// <param name="CustID"></param>
        /// <param name="DuncanGriddata"></param>
        /// <param name="CustGriddata"></param>
        public void SaveVendorDetails(int VID, string VName, int CustID, DuncanPropertyModel[] DuncanGriddata, CustomerPropertyModel[] CustGriddata)
        {

            using (PEMEntities context = new PEMEntities())
            {
               
                PayByCellVendor paybycell = new PayByCellVendor();
                paybycell.VendorID = VID;
                paybycell.VendorName = VName;
                paybycell.CustomerID = (CustID);
                paybycell.CreatedBy = CustID;
                paybycell.CreatedOn = System.DateTime.Now;
                paybycell.DEPRECATE = true;
                context.PayByCellVendors.Add(paybycell);
                context.SaveChanges();


                foreach (var item in DuncanGriddata)
                {
                    if (item.KeyText != null)
                    {
                        RipnetProperty DuncanProp = new RipnetProperty();
                        DuncanProp.KeyText = "paybycell.dp." + CustID + "." + VID + "." + item.KeyText;
                        DuncanProp.ValueText = item.ValueText;
                        context.RipnetProperties.Add(DuncanProp);
                        context.SaveChanges();
                    }
                }

                foreach (var item2 in CustGriddata)
                {
                    if (item2.KeyText != null)
                    {
                        RipnetProperty DuncanProp = new RipnetProperty();
                        DuncanProp.KeyText = "paybycell.cp." + CustID + "." + VID + "." + item2.KeyText;
                        DuncanProp.ValueText = item2.ValueText;
                        context.RipnetProperties.Add(DuncanProp);
                        context.SaveChanges();
                    }
                }

            }

        }

        /// <summary>
        ///Description:This method will set the seleted vendor as 'no more in use' by altering data in 'DEPRECATE' of PayByCellVendors table 
        ///Modified:Prita()
        /// </summary>
        /// <param name="VID"></param>
        public void DepricateVendorRecords(int VID)
        {
            {
                var stud = (from s in PemsEntities.PayByCellVendors
                            where (s.VendorID == VID) 
                            select s).FirstOrDefault();

                //stud.KeyText = KeyString + item.KeyText;
                stud.DEPRECATE = false;
                PemsEntities.SaveChanges();
            }
        }

        /// <summary>
        /// Description:This method will update the seleted vendor's customer and duncan property shown in editpaybycell by altering 'ValueText' of RipnetProperties table 
        /// Modified:Prita()
        /// </summary>
        /// <param name="VID"></param>
        /// <param name="VName"></param>
        /// <param name="CustID"></param>
        /// <param name="DuncanGriddata"></param>
        /// <param name="CustGriddata"></param>
        public void UpdateVendorDetails(int VID, string VName, int CustID, DuncanPropertyModel[] DuncanGriddata, CustomerPropertyModel[] CustGriddata)
        {
                foreach (var item in DuncanGriddata)
                {  
                    var KeyString = "paybycell.dp." + CustID + "." + VID + ".";       
                    if (item.KeyText!=item.KeyTextOld || item.ValueText!=item.ValueTextOld)
                    {                        
                          var stud = (from s in PemsEntities.RipnetProperties
                                      where (s.KeyText == KeyString+item.KeyTextOld) || (s.ValueText == item.ValueTextOld)
                                    select s).FirstOrDefault();

                       // stud.KeyText = KeyString + item.KeyText;
                        stud.ValueText = item.ValueText;
                        PemsEntities.SaveChanges();
                    }
                  
                }

                foreach (var item in CustGriddata)
                {
                    var KeyString = "paybycell.cp." + CustID + "." + VID + ".";
                    if (item.KeyText != item.KeyTextOld || item.ValueText != item.ValueTextOld)
                    {
                        var stud = (from s in PemsEntities.RipnetProperties
                                    where (s.KeyText == KeyString + item.KeyTextOld) || (s.ValueText == item.ValueTextOld)
                                    select s).FirstOrDefault();

                        //stud.KeyText = KeyString + item.KeyText;
                        stud.ValueText = item.ValueText;
                        PemsEntities.SaveChanges();
                    }

                }

            }

     
        /// <summary>
        ///Description:This method will fetch maximum vendorID in PayByCellVendors table and pass in to the controller after adding one to get a new vendorID for new vendor entry
        ///Modified:Prita()
        /// </summary>
        /// <returns></returns>
        //public int GetMaxVendorID()
        //{
        //    int MaxID = PemsEntities.PayByCellVendors.Max(x => x.VendorID);
        //    return MaxID + 1;
        //}

        public int GetMaxVendorID()
        {
            int MaxID = 0;
            int Count = PemsEntities.PayByCellVendors.Count();
            if (Count > 0)
            {
                MaxID = PemsEntities.PayByCellVendors.Max(x => x.VendorID);
            }
            return MaxID + 1;
        }

        public IQueryable<PayByCellModelAdmin> GetCustomerList(int CustomerID)
        {
            IQueryable<PayByCellModelAdmin> Result = Enumerable.Empty<PayByCellModelAdmin>().AsQueryable();

            Result = (from cust in PemsEntities.Customers
                      select new PayByCellModelAdmin
                      {
                          Text = cust.Name,
                          Value = cust.CustomerID
                      });
            return Result;
        }

        /// <summary>
        /// Description:This method will retrieve data for 'duncan property' grid in 'EditPayByCell' page From 'RipnetProperties' table
        /// Modified:Prita()
        /// </summary>
        /// <param name="custID"></param>
        /// <param name="VndrID"></param>
        /// <returns></returns>
        public List<DuncanPropertyModel>GetDuncanProp(int custID,int VndrID)
        {
            var KeyString="paybycell.dp." + custID +"."+VndrID+".";
            int indexOfdot = KeyString.LastIndexOf(".")+1;
             return (from Prop in PemsEntities.RipnetProperties
                     where (Prop.KeyText.Contains(KeyString))
                           select new DuncanPropertyModel
                           {
                               KeyText = Prop.KeyText.Substring(indexOfdot, 20),
                               ValueText = Prop.ValueText,
                               KeyTextOld = Prop.KeyText.Substring(indexOfdot, 20),
                               ValueTextOld = Prop.ValueText,
                           }).ToList();
               }

        public bool CheckIfExist(int vendorid,string vendorname,int CustID)
        {
            var VendorExists = (from s in PemsEntities.PayByCellVendors
                                    where s.VendorID == vendorid && s.VendorName == vendorname && s.CustomerID==CustID
                                    select s).FirstOrDefault();
            if (VendorExists != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Description:This method will retrieve data for 'customer property' grid in 'EditPayByCell' page From 'RipnetProperties' table
        /// Modified:Prita()
        /// </summary>
        /// <param name="custID"></param>
        /// <param name="VndrID"></param>
        /// <returns></returns>
        public List<CustomerPropertyModel> GetCustomerProp(int custID, int VndrID)
        {
            var KeyString = "paybycell.cp." + custID + "." + VndrID + ".";
            int indexOfdot = KeyString.LastIndexOf(".")+1;
            return (from Prop in PemsEntities.RipnetProperties
                    where (Prop.KeyText.Contains(KeyString))
                    select new CustomerPropertyModel
                    {
                        KeyText = Prop.KeyText.Substring(indexOfdot, 20),
                        ValueText = Prop.ValueText,
                        KeyTextOld = Prop.KeyText.Substring(indexOfdot, 20),
                        ValueTextOld = Prop.ValueText,
                    }).ToList();
        }

        /// <summary>
        /// This method will verify whether this vendor is in use or not on click of grid in index page
        /// </summary>
        /// <param name="VendorId"></param>
        /// <returns></returns>
        public bool CheckIfUsable(int VendorId)
        {
            return PemsEntities.PayByCellVendors.Where(a => a.VendorID == VendorId).Select(a => a.DEPRECATE).FirstOrDefault();
           
        }
        
        #region Client

        /// <summary>
        /// Description: (SpaceMgmt.cshtml)This method when called by will query and return vendorIDs having the specified customerID from PayByCell vendor's table in database 
        /// Modified:Prita(6-Aug-2014 to 12-Aug-2014)
        /// </summary>
        /// <param name="CustID"></param>
        /// <returns></returns>
        public IQueryable<PayByCellModel> GetVendors(int CustID)
        {

            var Files = (from Vendors in PemsEntities.PayByCellVendors
                         join CustVendorMappng in PemsEntities.vendorcustomerpaybycellmaps on Vendors.VendorID equals CustVendorMappng.vendorid
                         where (CustVendorMappng.customerid == CustID)
                         //where Vendors.CustomerID == CustID
                         select new PayByCellModel
                         {
                             VendorID = Vendors.VendorID,
                             VendorName = Vendors.VendorName,

                         });
            return Files;
        }

        /// <summary>
        /// Description:(SpaceMgmt.cshtml)Based on the type of Location/Dropdown to bind data with,this method will query data from respective table in database and return the result to controller
        /// Modified: Prita(6-Aug-2014 to 12-Aug-2014)
        /// </summary>
        /// <param name="locationType"></param>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
           public IEnumerable<PayByCellModel> GetLocationTypeId(string locationType, int CurrentCity)
           {
               IEnumerable<PayByCellModel> details = null;

               if (locationType == "Area")
               {
                   details = (from area in PemsEntities.Areas
                              where area.CustomerID == CurrentCity && area.AreaID != null
                              select new PayByCellModel
                              {
                                  Value = area.AreaID,
                                  Text = area.AreaName
                              });


               }
               else if (locationType == "Zone")
               {

                   details = (from zone in PemsEntities.Zones
                              where zone.customerID == CurrentCity && zone.ZoneId != null
                              select new PayByCellModel
                              {
                                  Value = zone.ZoneId,
                                  Text = zone.ZoneName
                              }).ToList();

               }
               else if (locationType == "Street")
               {
                   details = (from meter in PemsEntities.Meters
                              where meter.CustomerID == CurrentCity && meter.Location != null
                              orderby meter.Location ascending
                              select new PayByCellModel
                              {
                                  Text = meter.Location,
                              }).Distinct().ToList();
               }
               else if (locationType == "Suburb")
               {
                   details = (from customGroup in PemsEntities.CustomGroup1
                              where customGroup.CustomerId == CurrentCity && customGroup.DisplayName != null
                              select new PayByCellModel
                              {
                                  Value = customGroup.CustomGroupId,
                                  Text = customGroup.DisplayName
                              }).ToList();


               }

               return details;
           }

        /// <summary>
        /// Description:(SpaceMgmt.cshtml)Based on the Filter values passed,this method will query data from meter's table 
        /// joined with zreas zones,transactionMParks,Customers,CustomGroup1,PayByCellVendors table in database and return the result to controller
        /// Modified: Prita(6-Aug-2014 to 12-Aug-2014)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CurrentCity"></param>
        /// <param name="VendorID"></param>
        /// <param name="AreaID"></param>
        /// <param name="ZoneID"></param>
        /// <param name="SubUrbID"></param>
        /// <param name="Street"></param>
        /// <returns></returns>
           public List<PayByCellModel> GetSpaceManagementsummary(DataSourceRequest request, int CurrentCity, string VendorID, string AreaID, string ZoneID, string SubUrbID, string Street)
           {
               int VID = (!string.IsNullOrEmpty(VendorID)) ? Convert.ToInt32(VendorID) : -1;
               int AID = (!string.IsNullOrEmpty(AreaID)) ? Convert.ToInt32(AreaID) : -1;
               int ZID = (!string.IsNullOrEmpty(ZoneID)) ? Convert.ToInt32(ZoneID) : -1;
               int SUID = (!string.IsNullOrEmpty(SubUrbID)) ? Convert.ToInt32(SubUrbID) : -1;

               try
               {
                   var query = (from meter in PemsEntities.Meters

                                join customer in PemsEntities.Customers
                                on meter.CustomerID equals customer.CustomerID

                                //** Sairam has commented the below two lines
                                //join metermap in PemsEntities.MeterMaps
                                //on meter.MeterId equals metermap.MeterId

                                //**Sairam modified this code on oct 20th 2014 to do correct mapping
                                join mm in PemsEntities.MeterMaps on
                                    new { MeterId = meter.MeterId, AreaId = meter.AreaID, CustomerId = meter.CustomerID } equals
                                    new { MeterId = mm.MeterId, AreaId = (int)mm.AreaId2, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                                from l1 in metermap.DefaultIfEmpty()



                                join TransMparks in PemsEntities.TransactionsMParks
                                on new { meter.CustomerID, meter.AreaID, meter.MeterId }
                                equals new { TransMparks.CustomerID, TransMparks.AreaID, TransMparks.MeterId }

                                //** Sairam has commented the below two lines
                                //join area in PemsEntities.Areas
                                //on metermap.Areaid equals area.AreaID

                                //**Sairam modified this code on oct 20th 2014 to do correct mapping
                                join area in PemsEntities.Areas
                                     on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                                from A1 in mmArea.DefaultIfEmpty()

                                join zone in PemsEntities.Zones
                                    //on metermap.ZoneId equals zone.ZoneId
                                on l1.ZoneId equals zone.ZoneId  ////**Sairam modified this code on oct 20th 2014 to do correct mapping

                                join Suburb in PemsEntities.CustomGroup1
                                    //on metermap.CustomGroup1 equals Suburb.CustomGroupId
                                on l1.CustomGroup1 equals Suburb.CustomGroupId //**Sairam modified this code on oct 20th 2014 to do correct mapping

                                join PBCVendor in PemsEntities.PayByCellVendors
                               on TransMparks.VendorId equals PBCVendor.VendorID

                                // where customer.CustomerID == CurrentCity
                                // //&& (area.AreaID == AreaID || AreaID == -1)
                                // && (A1.AreaID == AreaID || AreaID == -1)
                                // && (zone.ZoneId == ZoneID || ZoneID == -1)
                                // && (meter.Location == Street || Street == string.Empty)
                                //// && (metermap.CustomGroup1 == SubUrbID || SubUrbID == -1)
                                // && (l1.CustomGroup1 == SubUrbID || SubUrbID == -1)
                                // && (TransMparks.VendorId == VendorID || VendorID == -1)

                                select new PayByCellModel
                                {
                                    CustomerID = customer.CustomerID,
                                    MeterID = meter.MeterId,
                                    // AreaID = area.AreaID,
                                    AreaID = A1.AreaID,
                                    AreaName = A1.AreaName,
                                    ZoneID = zone.ZoneId,
                                    ZoneName = zone.ZoneName,
                                    SubUrbID = Suburb.CustomGroupId,
                                    SubUrbName = Suburb.DisplayName,
                                    Street = meter.Location,
                                    VendorID = TransMparks.VendorId,
                                    VendorName = PBCVendor.VendorName
                                }).Distinct();

                   return query.ToList();
               }


               catch (Exception ex)
               {
               }
               return new List<PayByCellModel>();
           }
           
           public void UpdateFile_Active(PayByCellModel[] Griddata, int CustID)
           {
               try
               {
                   foreach (var item in Griddata)
                   {
                       if (item.MeterID == item.VendorID)
                       {
                           var stud = (from s in PemsEntities.TransactionsMParks
                                       where (s.CustomerID == CustID && s.MeterId == item.MeterID && s.AreaID == item.AreaID)
                                       select s).FirstOrDefault();

                           // stud.KeyText = KeyString + item.KeyText;
                           stud.VendorId = item.MeterID;
                           PemsEntities.SaveChanges();
                       }

                   }
               }
               catch (Exception ex) { }
           }
    #endregion
    }

    
}




       
