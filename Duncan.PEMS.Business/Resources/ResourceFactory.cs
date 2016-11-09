using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Utilities;
namespace Duncan.PEMS.Business.Resources
{
    public class ResourceFactory
    {
        /// <summary>
        /// GEts a localized title of an item. if no localized title is found, return the original title.
        /// </summary>
        public  string GetLocalizedTitle(string type, string title)
        {
            var globalResourceObject = HttpContext.Current.GetLocaleResource(type, title);
            return globalResourceObject != null ? globalResourceObject.ToString() : title;
        }

      
        /// <summary>
        ///     Gets a dictionary list of all items for a type and culture code.
        /// </summary>
        /// <param name="type">Type to get (Glossary, Config, etc)</param>
        /// <param name="cultureCode">Culture code for the resource items to get (en-US, es-EC, etc)</param>
        /// <param name="customerId">Customer Id </param>
        /// <returns></returns>
        public static IDictionary GetCustomerLocaleResources(string type,  string cultureCode, int customerId)
        {
            //get resourrce where type and culture match
            using (var pemsRbacContext = new PEMRBACEntities())
            {
                var result = (from ss in pemsRbacContext.LocaleResources
                              where ss.Type == type
                              where ss.CultureCode == cultureCode
                              select ss).Distinct().ToList();

                //return a dictionary list of items with the name and value
                if (result.Any())
                {
                            var customResources = (from ss in pemsRbacContext.LocaleResourcesCustoms
                                                   where ss.Type == type
                                                   where ss.CustomerId == customerId
                                                   select ss).Distinct().ToList();

                            //if there are custom resources for this customer
                            if (customResources.Any())
                            {
                                foreach (var resource in result)
                                {
                                    //check to see if there is a custom value
                                    var localeResourcesCustom =
                                        customResources.FirstOrDefault(x => x.Name == resource.Name);
                                    if (localeResourcesCustom != null)
                                        resource.Value = localeResourcesCustom.Value;
                                }
                            }

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
    }
}
