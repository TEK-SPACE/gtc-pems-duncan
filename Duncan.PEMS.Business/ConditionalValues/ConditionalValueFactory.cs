using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duncan.PEMS.Business.Customers;

namespace Duncan.PEMS.Business.ConditionalValues
{

    /// <summary>
    /// The <see cref="Duncan.PEMS.Business.Collections"/> namespace contains classes for managing collections.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// This class handles all of the conditional filters and field si the system. 
    /// Each method will be a single rule for a single field. 
    /// Each method should be a static method, accept a customer ID and return true or false indicating if hte customer has access to that field.
    /// Edit and creation pages will not be affected. The users must enter valid data to respect the DB required fields.
    /// </summary>
    public class ConditionalValueFactory : RbacBaseFactory
    {
        /// <summary>
        /// This method will check to see if the customer has access to the given field. if they do, we will return true, otherwise false.
        /// if the setting value does not exist, return true.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="fieldName">Constant value of hte name of hte hidden field to check. Constants.HiddenFields.DemandArea for example.</param>
        /// <returns></returns>
        public static bool DisplayField(int customerId, string fieldName)
        {
            try
            {
                var settingsFactory = new SettingsFactory();
                var setting = settingsFactory.GetValue(fieldName, customerId);
                //now try to convert it into a bool
                if (setting != null)
                    //has to be the opposite, since if the setting is set to true, we do not want to display it.
                    return !bool.Parse(setting);
            }
            catch (Exception)
            {
                throw;
            }
            //show it by default
            return true;
        }

        /// <summary>
        /// This method will return the class "hiddenFilter" that will visibly hide the element from the user while still displaying the field to the application.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetHiddenFieldClass(int customerId, string fieldName)
        {
            var displayField = DisplayField(customerId, fieldName);
            if (!displayField)
                return "hiddenFilter";
            return string.Empty;
        }

    }
}
