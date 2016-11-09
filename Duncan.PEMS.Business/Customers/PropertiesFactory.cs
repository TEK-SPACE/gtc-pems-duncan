using System;
using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Customers;
using NLog;

namespace Duncan.PEMS.Business.Customers
{
    /// <summary>
    /// This class is used for properties of a customer.  These properties affect how a customer  
    /// function in the back office/server-side processes.
    /// 
    /// These properties are used only at the creation of a city and any subsequent administrative updates.
    /// </summary>
    public class PropertiesFactory : BaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Factory constructor taking a connection string name.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        public PropertiesFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }
        /// <summary>
        /// Gets a list of the present property(ies) for a customer based on property group name.
        /// </summary>
        /// <param name="customerId">Id of customer</param>
        /// <param name="propertyGroupName">String name of property group</param>
        /// <returns>List of <see cref="Property"/> representing value or empty list.</returns>
        public  List<Property> Get(int customerId, string propertyGroupName)
        {
            var list = new List<Property>();

            // Get the property group.
            var propertyGroup = GetCustomerPropertyGroup( customerId, propertyGroupName );
                
            list = (from cp in PemsEntities.CustomerProperties
                                      join cds in PemsEntities.CustomerDetails on cp.CustomerPropertyId equals cds.CustomerPropertyId
                                      where cp.CustomerPropertyGroupId == propertyGroup.CustomerPropertyGroupId && cds.CustomerID == customerId
                                      select new Property()                        {
                                        Id = cds.ID,
                                        Value = cp.PropertyDesc,
                                        AdditionalValue = cds.AdditionalValue,
                                        PropertyGroupName = propertyGroup.PropertyGroupDesc,
                                        ScreenName = cds.ScreenName,
                                        IsRequired = cds.IsRequired,
                                        IsDisplayed = cds.IsDisplay,
                                        SortOrder = cp.SortOrder
                                        }).ToList();
            list.Sort();
            return list;
        }


        /// <summary>
        /// Gets a single property for a customer based on property group name.
        /// </summary>
        /// <param name="propertyGroupName">String name of property group</param>
        /// <param name="customerId">Id of customer</param>
        /// <returns><see cref="Property"/> representing value or null.</returns>
        public  Property GetOne(int customerId, string propertyGroupName)
        {
            var list = Get(customerId, propertyGroupName);
            return list.Any() ? list.First() : null;
        }

        /// <summary>
        /// Gets a list of allowable values for a property group.
        /// </summary>
        /// <param name="propertyGroupName">String name of property group</param>
        /// <param name="customerId">Id of customer</param>
        /// <returns>Ordered list of allowable properties.  Empty list indicates no known properties.</returns>
        public  List<PropertyGroupItem> GetList(int customerId, string propertyGroupName)
        {
            var propertyGroup = GetCustomerPropertyGroup(customerId, propertyGroupName);

            var propertiesList = (from cp in PemsEntities.CustomerProperties
                              where (cp.CustomerPropertyGroupId == propertyGroup.CustomerPropertyGroupId && cp.CustomerID == customerId)
                              select new PropertyGroupItem()
                    {
                        Id = cp.CustomerPropertyId,
                        Value = cp.PropertyDesc,
                        SortOrder = cp.SortOrder
                    }).ToList();

            propertiesList.Sort();
            return propertiesList;
        }



        public  PropertyGroupItem GetPropertyGroupItem( int customerId, string propertyGroupName, string itemName)
        {
            var propertyGroup = GetCustomerPropertyGroup(customerId, propertyGroupName);
            var customerProperty = PemsEntities.CustomerProperties.FirstOrDefault( m => m.CustomerPropertyGroupId == propertyGroup.CustomerPropertyGroupId
                                                                                        && m.PropertyDesc.Equals( itemName ) );
            if ( customerProperty == null )
            {
                // Need to get max SortOrder of existing items.
                int nextSortOrder = 0;
                var propertySet = PemsEntities.CustomerProperties.Where( cp => propertyGroup.CustomerPropertyGroupId == cp.CustomerPropertyGroupId );
                if (propertySet.Any())
                {
                    nextSortOrder = propertySet.Max( m => m.SortOrder ) + 1;
                }

                customerProperty = new CustomerProperty()
                    {
                        CustomerPropertyGroupId = propertyGroup.CustomerPropertyGroupId,
                        CustomerID = customerId,
                        PropertyDesc = itemName,
                        SortOrder = nextSortOrder
                    };
                PemsEntities.CustomerProperties.Add( customerProperty );
                PemsEntities.SaveChanges();
            }

            return new PropertyGroupItem()
                {
                    Id = customerProperty.CustomerPropertyId,
                    Value = customerProperty.PropertyDesc,
                    SortOrder = customerProperty.SortOrder
                };
        }

        /// <summary>
        /// Gets value of a property detail item.
        /// </summary>
        /// <param name="propertyId">Integer id of the property detail.</param>
        /// <returns>Returns a <see cref="Property"/> instance if found.  Otherwise returns null.</returns>
        public  Property GetValue(int propertyId)
        {
            Property property = null;

                var propertyValue = PemsEntities.CustomerDetails.FirstOrDefault(x => x.ID == propertyId);

                if (propertyValue != null)
                {
                    var customerProperty = PemsEntities.CustomerProperties.FirstOrDefault(x => x.CustomerPropertyId == propertyValue.CustomerPropertyId);

                    if ( customerProperty != null )
                    {
                        property = new Property()
                        {
                            Id = propertyValue.ID,
                            Value = customerProperty.PropertyDesc,
                            AdditionalValue = propertyValue.AdditionalValue,
                            ScreenName = propertyValue.ScreenName,
                            IsRequired = propertyValue.IsRequired,
                            IsDisplayed = propertyValue.IsDisplay,
                            SortOrder = 1
                        };
                    }
                }


            return property;
        }

        /// <summary>
        /// Sets a property for a specific customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="propertyGroupName"></param>
        /// <param name="propertyValue"></param>
        public  void Set(int customerId, string propertyGroupName, bool propertyValue)
        {
            var propertyGroup = GetCustomerPropertyGroup(customerId, propertyGroupName);

            var customerProperty = PemsEntities.CustomerProperties.FirstOrDefault(m => m.CustomerPropertyGroupId == propertyGroup.CustomerPropertyGroupId
                                                                                                    && m.PropertyDesc.Equals(propertyGroupName)
                                                                                                    && m.CustomerID == customerId);
            if (customerProperty == null)
            {
                // Also create a CustomerProperty
                customerProperty = new CustomerProperty()
                    {
                        CustomerPropertyGroupId = propertyGroup.CustomerPropertyGroupId,
                        CustomerID = customerId,
                        PropertyDesc = propertyGroupName,
                        SortOrder = 0
                    };
                PemsEntities.CustomerProperties.Add( customerProperty );
                PemsEntities.SaveChanges();
            }

            // Get the CustomerDetails
            CustomerDetail customerDetail =
                PemsEntities.CustomerDetails.FirstOrDefault( m => m.CustomerID == customerId && m.CustomerPropertyId == customerProperty.CustomerPropertyId );
            if ( customerDetail == null )
            {
                customerDetail = new CustomerDetail()
                    {
                        CustomerID    = customerId,
                        CustomerPropertyId = customerProperty.CustomerPropertyId,
                        ScreenName = propertyGroupName,
                        IsDisplay = true,
                        IsRequired = true
                    };
                PemsEntities.CustomerDetails.Add( customerDetail );
            }
            customerDetail.AdditionalValue = propertyValue.ToString();

            PemsEntities.SaveChanges();
        }

        /// <summary>
        /// Sets a customer property
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="propertyGroupName"></param>
        /// <param name="propertyValue"></param>
        public  void Set(int customerId, string propertyGroupName, string propertyValue)
        {
            bool isListItem = true;

            var propertyGroup = GetCustomerPropertyGroup(customerId, propertyGroupName);

            // This propertyValue may be a list item.  That can be confirmed if
            // the CustomerProperty && CustomerPropertyGroup can be matched.  Otherwise this is 
            // probably a single-value proprty.


            CustomerProperty customerProperty = PemsEntities.CustomerProperties.FirstOrDefault(m => m.CustomerPropertyGroupId == propertyGroup.CustomerPropertyGroupId
                                                                                                    && m.PropertyDesc.Equals(propertyValue)
                                                                                                    && m.CustomerID == customerId);

            if (customerProperty == null)
            {
                // The customerProperty is a single-value type.  See if it already exists.

                customerProperty = PemsEntities.CustomerProperties.FirstOrDefault(m => m.CustomerPropertyGroupId == propertyGroup.CustomerPropertyGroupId
                                                                                        && m.PropertyDesc.Equals(propertyGroupName)
                                                                                        && m.CustomerID == customerId);
                // Create asingle-value  CustomerProperty
                if ( customerProperty == null )
                {

                    customerProperty = new CustomerProperty()
                        {
                            CustomerPropertyGroupId = propertyGroup.CustomerPropertyGroupId,
                            CustomerID = customerId,
                            PropertyDesc = propertyGroupName,
                            SortOrder = 0
                        };
                    PemsEntities.CustomerProperties.Add( customerProperty );
                    PemsEntities.SaveChanges();
                }
                isListItem = false;
            }

            // Get the CustomerDetails
            CustomerDetail customerDetail =
                PemsEntities.CustomerDetails.FirstOrDefault(m => m.CustomerID == customerId && m.CustomerPropertyId == customerProperty.CustomerPropertyId);
            if (customerDetail == null)
            {
                customerDetail = new CustomerDetail()
                {
                    CustomerID = customerId,
                    CustomerPropertyId = customerProperty.CustomerPropertyId,
                    AdditionalValue = (isListItem ? null : propertyValue),
                    ScreenName = propertyGroupName,
                    IsDisplay = true,
                    IsRequired = true
                };
                PemsEntities.CustomerDetails.Add(customerDetail);
            }
            else
            {
                customerDetail.CustomerPropertyId = customerProperty.CustomerPropertyId;
                customerDetail.AdditionalValue = ( isListItem ? null : propertyValue );
            }

            PemsEntities.SaveChanges();
        }

        /// <summary>
        /// Enable or disable a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="customerDetailId"></param>
        /// <param name="enabled"></param>
        public  void Enable(int customerId, int customerDetailId, bool enabled)
        {
            // Get the CustomerDetail
            var customerDetail = PemsEntities.CustomerDetails.FirstOrDefault( m => m.ID == customerDetailId );
            if ( customerDetail != null )
            {
                customerDetail.IsDisplay = enabled;
                PemsEntities.SaveChanges();
            }
        }

        /// <summary>
        /// Sets a customer property
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="propertyGroupName"></param>
        /// <param name="itemName"></param>
        /// <param name="enabled"></param>
        public  void Set(int customerId, string propertyGroupName, string itemName, bool enabled)
        {
            Set(customerId, GetPropertyGroupItem(customerId, propertyGroupName, itemName), enabled);
        }

        /// <summary>
        /// Sets a customer property
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="propertyGroupItem"></param>
        /// <param name="enabled"></param>
        public  void Set(int customerId, PropertyGroupItem propertyGroupItem, bool enabled)
        {

            // Get the CustomerDetails
            CustomerDetail customerDetail =
                PemsEntities.CustomerDetails.FirstOrDefault(m => m.CustomerID == customerId && m.CustomerPropertyId == propertyGroupItem.Id);
            if (customerDetail == null)
            {
                customerDetail = new CustomerDetail()
                {
                    CustomerID = customerId,
                    CustomerPropertyId = propertyGroupItem.Id,
                    AdditionalValue = null,
                    ScreenName = propertyGroupItem.Value,
                    IsRequired = true
                };
                PemsEntities.CustomerDetails.Add(customerDetail);
            }

            customerDetail.IsDisplay = enabled;

            PemsEntities.SaveChanges();
        }

        #region Utility Methods
        /// <summary>
        /// Gets the property group for a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="propertyGroupName"></param>
        /// <returns></returns>
        private  CustomerPropertyGroup GetCustomerPropertyGroup(int customerId, string propertyGroupName)
        {
            CustomerPropertyGroup propertyGroup = PemsEntities.CustomerPropertyGroups
                .FirstOrDefault(m => m.PropertyGroupDesc.Equals(propertyGroupName) && m.CustomerID == customerId);
            if ( propertyGroup == null )
            {
                propertyGroup = new CustomerPropertyGroup()
                    {
                        CustomerID = customerId,
                        PropertyGroupDesc = propertyGroupName
                    };
                PemsEntities.CustomerPropertyGroups.Add( propertyGroup );
                PemsEntities.SaveChanges();
            }

            return propertyGroup;
        }



        #endregion



        #region Conversion helper methods

        /// <summary>
        /// Convert a <see cref="Property"/> to a <see cref="DateTime"/>.  Return a true/false
        /// indication if conversion worked.  Returns <see cref="DateTime"/> in an output 
        /// parameter.  Assumes that <see cref="Property.AdditionalValue"/> contains date.
        /// </summary>
        /// <param name="property">And instance of a <see cref="Property"/>.  Can be null.</param>
        /// <param name="dateTime">An out parameter of type <see cref="DateTime"/></param>
        /// <returns>If true if conversion to <see cref="DateTime"/>, otherwise return false.</returns>
        public  bool ToDateTime(Property property, out DateTime dateTime)
        {
            bool conversionSuccessful = false;
            dateTime = DateTime.MinValue;
            if (property != null && property.AdditionalValue != null)
            {
                if ( DateTime.TryParse( property.AdditionalValue, out dateTime ) )
                    conversionSuccessful = true;
            }
            return conversionSuccessful;
        }

        /// <summary>
        /// Convert a <see cref="Property"/> to a true/false.  Return a true/false
        /// indication if conversion worked.  Returns <see cref="bool"/> in an output 
        /// parameter.  Assumes that <see cref="Property.AdditionalValue"/> contains true/false indication.
        /// </summary>
        /// <param name="property">And instance of a <see cref="Property"/>.  Can be null.</param>
        /// <param name="trueFalse">An out parameter of type <see cref="bool"/></param>
        /// <returns>If true if conversion to <see cref="bool"/>, otherwise return false.</returns>
        public  bool ToBool(Property property, out bool trueFalse)
        {
            bool conversionSuccessful = false;
            trueFalse = false;
            if (property != null && property.AdditionalValue != null && property.AdditionalValue.Length > 0)
            {
                switch (property.AdditionalValue[0])
                {
                    case 'T':
                    case 't':
                    case 'Y':
                    case 'y':
                    case '1':
                        trueFalse = true;
                        break;
                }
                conversionSuccessful = true;
            }
            return conversionSuccessful;
        }


        #endregion


    }
}
