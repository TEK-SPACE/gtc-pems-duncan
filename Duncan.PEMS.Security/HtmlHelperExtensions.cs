using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Security
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Check if this User has rights to the requested city/controller/action and return
        /// a <see cref="MvcHtmlString"/> based upon rights.  If an <paramref name="altHtmlAttributes"/>
        /// was null and user had no rights then an empty string will be returned otherwise the 
        /// <paramref name="altHtmlAttributes"/> will be applied to the link.  The link will also be disabled
        /// depending upon whether "disabled" and "onClick" attributes are present.
        /// <para>If the <paramref name="altHtmlAttributes"/> does not contains the "disabled" attribute, it will be added and set to "disabled"</para>
        /// <para>If the <paramref name="altHtmlAttributes"/> does not contains the "onClick" attribute, it will be added and set to "return false;"</para>
        /// 
        /// </summary>
        /// <param name="htmlHelper">Instance of <see cref="HtmlHelper"/></param>
        /// <param name="linkText">String of text to display in link</param>
        /// <param name="actionName">Name of action to call</param>
        /// <param name="controllerName">Name of controller for <paramref name="actionName"/></param>
        /// <param name="routeValues">Optional route values.  May be null.</param>
        /// <param name="htmlAttributes">Optional HTML attributes.  May be Null.</param>
        /// <param name="altHtmlAttributes">Optional HTML attributes to use if user has no rights.  If null and user has no rights, an empty string will be returned.</param>
        /// <param name="city">City to check.</param>
        /// <param name="userName">User name to check.</param>
        /// <returns></returns>
        public static MvcHtmlString ActionLinkWithRights(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues,
                                                         object htmlAttributes, object altHtmlAttributes, string city, string userName)
        {
            MvcHtmlString mvcHtmlString = MvcHtmlString.Empty;
            AuthorizationManager authorizationManager = new AuthorizationManager(city);

            if ( authorizationManager.IsAuthorized( userName, actionName, controllerName ) )
            {
                mvcHtmlString = htmlHelper.ActionLink( linkText, actionName, controllerName, routeValues, htmlAttributes );
            }
            else
            {
                if(altHtmlAttributes != null)
                {
                    // Add disabled and onClick attributes.
                    Dictionary<string, object> altHtmlAttr = HelperMethods.TurnObjectIntoDictionary( altHtmlAttributes);
                    if (altHtmlAttr == null)
                    {
                        altHtmlAttr = new Dictionary<string, object>();
                    }
                    if (!altHtmlAttr.ContainsKey("disabled"))
                    {
                        altHtmlAttr.Add("disabled", "disabled");
                    }
                    if (!altHtmlAttr.ContainsKey("onClick"))
                    {
                        altHtmlAttr.Add("onClick", "return false;");
                    }


                    mvcHtmlString = htmlHelper.ActionLink(linkText, actionName, controllerName, new RouteValueDictionary(routeValues), altHtmlAttr);
                }
                
            }

            return mvcHtmlString;
        }
    }
}
