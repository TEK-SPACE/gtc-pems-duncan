using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Enumerations;

namespace Duncan.PEMS.Entities.Customers
{
    public class PemsCity
    {
        private PEMRBACEntities _RBACContext = new PEMRBACEntities();

        /// <summary>
        ///     Constructor taking a string identifier for a customer.  This identifier is
        ///     intended to be gleaned from the parsing of the URL.  If the identifier can be
        ///     converted to an integer then the identifier is assumed to be the <see cref="Id" />
        ///     and the Customer object is built from it.
        ///     If the identifier is a string then it is assumed to be the <see cref="InternalName" />
        ///     and the customer is built from the name.
        /// </summary>
        /// <param name="identifier">Identifier of this customer as parsed from the URL</param>
        public PemsCity(string identifier)
        {
            int id = 0;
            netsqlazman_StoresTable storeCustomer;

            // identifier parameter represented a number
            if ( Int32.TryParse( identifier, out id ) )
                storeCustomer = _RBACContext.netsqlazman_StoresTable.SingleOrDefault( store => store.StoreId == id );
            else
                storeCustomer = _RBACContext.netsqlazman_StoresTable.SingleOrDefault( store => store.Name.Equals( identifier, StringComparison.CurrentCultureIgnoreCase ) );

            if ( storeCustomer != null )
            {
                InternalName = storeCustomer.Name;
                Id = storeCustomer.StoreId;
            }

            // Get the basic UTC offset of server.
            UTCOffset = (int)System.TimeZone.CurrentTimeZone.GetUtcOffset( DateTime.Now ).TotalHours;
            MaintenanceCustomers = new List<PemsCity>();
            CustomerType = CustomerProfileType.Customer;

            // If able to build basic customer from NetSqlAzMan
            if ( IsValid )
            {
                var customerProfile = _RBACContext.CustomerProfiles.SingleOrDefault( cp => cp.CustomerId == Id );

                if ( customerProfile != null )
                {

                    if (customerProfile.CustomerTypeId == (int)CustomerProfileType.Admin)
                        CustomerType = CustomerProfileType.Admin;

                    IsActive = customerProfile.Status == 1;
                    DisplayName = customerProfile.DisplayName;
                    Locale = customerProfile.DefaultLocale == null ? CultureInfo.CurrentCulture : new CultureInfo( customerProfile.DefaultLocale );
                    Is24HourFormat = customerProfile.Is24HrFormat ?? false;
                    PemsConnectionStringName = customerProfile.PEMSConnectionStringName;
                    MaintenanceConnectionStringName = customerProfile.MaintenanceConnectionStringName;
                    ReportingConnectionStringName = customerProfile.ReportingConnectionStringName;
                   
                    
                        var timeZone = _RBACContext.CustomerTimeZones.FirstOrDefault(m => m.TimeZoneID == customerProfile.TimeZoneID);
                        // LocalTimeUTCDifference is in minutes in the [TimeZones] table.  Convert it to hours.
                        if ( timeZone != null )
                        {
                            // If timeZone.DaylightSavingAdjustment != 0 add 1 hour to UTCOffset to handle Daylight Saving Time
                            UTCOffset = timeZone.LocalTimeUTCDifference/60 + (timeZone.DaylightSavingAdjustment != 0 ? 1 : 0);
                        }

                    if (customerProfile.CustomerTypeId == (int)CustomerProfileType.MaintenanceGroup)
                    {
                        CustomerType = CustomerProfileType.MaintenanceGroup;
                        //if the are not a customer,  we need to populate a list of customers thsi maint group is associated with
                        foreach (var mgCustomer in customerProfile.MaintenanceGroupCustomers1)
                        {
                            //have to check to make sure the pems city id is > than 0, otherwise, they dont exist in azman.
                            var newCust = new PemsCity(mgCustomer.CustomerId.ToString());
                            if (newCust.IsValid)
                                MaintenanceCustomers.Add(newCust);
                        }
                    }
                }
                else
                {
                    IsActive = false;
                    DisplayName = InternalName;
                    Locale = CultureInfo.CurrentCulture;
                    Is24HourFormat = false;
                }
            }

            // Get timezone corrected local customer time.
            LocalTime = DateTime.UtcNow + new TimeSpan(0, UTCOffset, 0, 0);
        }

        /// <summary>
        ///     True if this customer represents a customer in the
        ///     PEMS system.  Read-only property
        /// </summary>
        public bool IsValid
        {
            get { return Id > 0; }
        }

        public string PemsConnectionStringName { get; set; }
        public string MaintenanceConnectionStringName { get; set; }
        public string ReportingConnectionStringName { get; set; }

        /// <summary>
        ///     The numeric identifier of a customer.  Presently this is
        ///     the Id assigned by NetSqlAzMan
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        ///     Display string for customer name.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        ///     Internal string name for customer.  Presently, this is
        ///     analogous to a 'Store' in NetSqlAzMan
        /// </summary>
        public string InternalName { get; private set; }

        /// <summary>
        ///     This is the default locale for the customer.
        /// </summary>
        public CultureInfo Locale { get; private set; }

        /// <summary>
        ///     Indicates if the customer prefers 24-hour clock
        /// </summary>
        public bool Is24HourFormat { get; set; }


        /// <summary>
        /// Number of hours offset from UTC
        /// </summary>
        public int UTCOffset { get; set; }

        /// <summary>
        /// Timezone corrected local customer time.
        /// </summary>
        public DateTime LocalTime { get; set; }

        public bool IsActive { get; set; }

        public string GetSetting(string settingName)
        {
            return string.Empty;
        }

        public List<PemsCity> MaintenanceCustomers { get; set; }

        public CustomerProfileType CustomerType { get; set; }
    }
}