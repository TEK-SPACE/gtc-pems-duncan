using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Compilation;
using Duncan.PEMS.DataAccess.RBAC;

namespace Duncan.PEMS.Utilities
{

    public  static class HttpContextExtenstions
    {
        public static string GetLocaleResource(this HttpContext context, string classKey, string resourceKey)
        {
            var cityId = (string)context.Session[Constants.ViewData.CurrentCityId];
            int cityID;
            if (int.TryParse(cityId, out cityID) && cityID > 0)
            {
                //var cid = (int)context.Items["CustomerId"];
                var customerCache = SqlResourceHelper.GetCustomerResourceCache(cityID);
                var value = customerCache[resourceKey];
                if (value != null)
                    return value.ToString();
            }

            //This has to call the original GetGlocalResourceObject method call to get it out of the regular langauge cache
            var globalResourceObject = HttpContext.GetGlobalResourceObject(classKey, resourceKey);
            if (globalResourceObject != null)
                return globalResourceObject.ToString();
            return resourceKey;
        }
    }

    public sealed class SqlResourceProviderFactory : ResourceProviderFactory
    {
        public override IResourceProvider CreateGlobalResourceProvider(string classKey)
        {
            return new SqlResourceProvider( classKey );
        }

        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            virtualPath = Path.GetFileName( virtualPath );
            return new SqlResourceProvider( virtualPath );
        }

        private sealed class SqlResourceProvider : IResourceProvider
        {
            private readonly string _type;
            private IDictionary _resourceCache;
            public SqlResourceProvider(string type)
            {
                _type = type;
            }

            /// <summary>
            ///     Gets a resource item that matches a specified culture and Type
            /// </summary>
            /// <param name="resourceKey">Type </param>
            /// <param name="culture">Culture to use (en-US, es-EC, etc)</param>
            /// <returns></returns>
            object IResourceProvider.GetObject(string resourceKey, CultureInfo culture)
            {
                string cultureName = culture != null ? culture.Name : CultureInfo.CurrentUICulture.Name;

                //Get the resource from the cache. if it is not in cache, grab it from the db
                object value = GetResourceCache( cultureName )[resourceKey];

                //if it doesnt exist in either location, create it.
                if ( value == null )
                {
                    // resource is missing for current culture, create it
                    SqlResourceHelper.AddLocaleResource( resourceKey, _type, cultureName );

                    //we just added to the resource, so we need to update the cache to include the new item
                    //it should exist now, re-get it for this culture. have to reset the cache we pull from
                    value = GetResourceCache( cultureName, true )[resourceKey];
                }

                return value;
            }

            IResourceReader IResourceProvider.ResourceReader
            {
                get { return new SqlResourceReader( GetResourceCache( null, true ) ); }
            }

            /// <summary>
            ///     Gets a cache of the culture. if refresh is passed in as true, then ignore the cache and get it directly from the DB
            ///     If nothing in the cache exist, also get it from the DB
            /// </summary>
            /// <param name="cultureName">Culture to look for (en-US, es-EC)</param>
            /// <param name="refresh">Bool to determine if we should ignore the cache and get the resources directly from the db</param>
            /// <returns></returns>
            private IDictionary GetResourceCache(string cultureName, bool refresh = false)
            {
                //if the cache is null, set it to an empty list
                if ( _resourceCache == null )
                    _resourceCache = new ListDictionary();

                //try to get the resource dictionary from the cache
                var resourceDict = _resourceCache[cultureName] as IDictionary;

                //if there was nothing in the cache or we need to force a refresh
                //if it is a custom field or grid resource, we have to force a refresh
                if ( resourceDict == null || refresh )
                {
                    //get the resource dictionary for the culture and type and add it to the cache.
                    resourceDict = SqlResourceHelper.GetResources( _type, cultureName );
                    _resourceCache[cultureName] = resourceDict;
                }

                //return the dictionary
                return resourceDict;
            }
        }

        private sealed class SqlResourceReader : IResourceReader
        {
            private readonly IDictionary _resources;

            public SqlResourceReader(IDictionary resources)
            {
                _resources = resources;
            }

            IDictionaryEnumerator IResourceReader.GetEnumerator()
            {
                return _resources.GetEnumerator();
            }

            void IResourceReader.Close()
            {
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _resources.GetEnumerator();
            }

            void IDisposable.Dispose()
            {
            }
        }
    }

    public static class SqlResourceHelper
    {
        //public static int CustomerId { get; set; }

        /// <summary>
        ///     Gets a dictionary list of all items for a type and culture code.
        /// </summary>
        /// <param name="type">Type to get (Glossary, Config, etc)</param>
        /// <param name="cultureCode">Culture code for the resource items to get (en-US, es-EC, etc)</param>
        /// <returns></returns>
        public static IDictionary GetResources(string type, string cultureCode)
        {
            //get resourrce where type and culture match
            using (var pemsRbacContext = new PEMRBACEntities())
            {
                var result = ( from ss in pemsRbacContext.LocaleResources
                               where ss.Type == type
                               where ss.CultureCode == cultureCode
                               select ss ).Distinct().ToList();

                //return a dictionary list of items with the name and value
                if (result.Any())
                {
                    var dictionary = new Dictionary<string, string>();
                    //DO NOT convert this to lambda
                    foreach (var item in result)
                    {
                        if (!dictionary.ContainsKey(item.Name))
                            dictionary.Add(item.Name, item.Value);
                    }

                    return dictionary;
                }
            }
            return new ListDictionary();
        }

        /// <summary>
        ///     Adds a Locale Resource to the system for the specified culture and type. Defaults the value to the name of the item if the value isnt passed in
        /// </summary>
        /// <param name="name">Name of hte resource item</param>
        /// <param name="type">Type of the item (Glossary, config, etc)</param>
        /// <param name="cultureCode">Culture code for the resource (en-US, etc)</param>
        /// <param name="value">(Optional) Value for the resource</param>
        public static void AddLocaleResource(string name, string type, string cultureCode, string value = null)
        {
            using (var pemsRbacContext = new PEMRBACEntities())
            {
                //create the item and add it to the system
                var item = new LocaleResource
                               {
                                   CultureCode = cultureCode,
                                   Name = name,
                                   Value = value ?? name,
                                   Type = type
                               };
                pemsRbacContext.LocaleResources.Add( item );
                pemsRbacContext.SaveChanges();
            }
        }

        /// <summary>
        ///     Updates the value of a resource inthe system for the type, name, and culture code passed in.
        /// </summary>
        /// <param name="name">Name if the Locale Resource item</param>
        /// <param name="type">Type of the resourse (Glossary, Config, etc)</param>
        /// <param name="cultureCode">Culture code for the item (en-US, es-EC, etc)</param>
        /// <param name="value">Value of the resource for that paricular culture</param>
        public static void UpdateLocaleResource(string name, string type, string cultureCode, string value)
        {
            using (var pemsRbacContext = new PEMRBACEntities())
            {
                //get the item
                var result = ( from ss in pemsRbacContext.LocaleResources
                               where ss.Type == type
                               where ss.CultureCode == cultureCode
                               where ss.Name == name
                               select ss ).FirstOrDefault();

                //update the item
                if ( result != null )
                {
                    result.Value = value;
                    pemsRbacContext.SaveChanges();
                }
                    //if it wasnt found, add the item
                else
                    AddLocaleResource( name, type, cultureCode, value );
            }
        }

        #region Customer Custom Resources
        private static IDictionary _customerResourceCache;

        /// <summary>
        ///     Gets a dictionary list of all items for a type and culture code.
        /// </summary>
        /// <param name="customerId">ID of the customer to get the data for</param>
        /// <returns></returns>
        public static IDictionary GetCustomResources(int customerId)
        {
            //make sure the customerid is valid
                //get resourrce where type and culture match
                using (var pemsRbacContext = new PEMRBACEntities())
                {
                    var customResources = (from ss in pemsRbacContext.LocaleResourcesCustoms
                                           where ss.CustomerId == customerId
                                           select ss).Distinct().ToList();
                    //return a dictionary list of items with the name and value
                    //if there are custom resources for this customerTime of Complaint
                    if (customResources.Any())
                    {
                        var dictionary = new Dictionary<string, string>();
                        //DO NOT convert this to lambda
                        foreach (var item in customResources)
                        {
                            if (!dictionary.ContainsKey(item.Name))
                                dictionary.Add(item.Name, item.Value);
                        }
                        return dictionary;
                    }
                }
            return new ListDictionary();
        }

        public static IDictionary GetCustomerResourceCache(int customerId)
        {
            //if the cache is null, set it to an empty list
            if (_customerResourceCache == null)
                _customerResourceCache = new ListDictionary();

            //try to get the resource dictionary from the cache
            var resourceDict = _customerResourceCache[customerId] as IDictionary;
            if (resourceDict == null)
            {
                //get the resource dictionary for the culture and type and add it to the cache.
                resourceDict = GetCustomResources(customerId);
                _customerResourceCache[customerId] = resourceDict;
            }

            //return the dictionary
            return resourceDict;
        }

        /// <summary>
        /// Resets the customer resource cache. This is done when the custom labels are done via the administration tool so the changes display immediately
        /// </summary>
        /// <param name="customerId"></param>
        public static void ResetCustomerResourceCache(int customerId)
        {
            //reset the cache for this customer
            _customerResourceCache[customerId] = null;

            //then call the get to re-populate for this customer
            GetCustomerResourceCache(customerId);
        }
        #endregion

    }
}