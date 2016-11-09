/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             ___________________________________________________________________________________________________
 * 
 * 01/15/2014       Sergey Ostrerov                 DPTXPEMS-8 - Can't create new TX meter through PEMS UI.
 * 02/10/2014       Sergey Ostrerov                 DPTXPEMS-237 - Edit Screen for Event Code Crashes
 *                                                                 Create/Update Event Code   
 * *****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Duncan.PEMS.Entities.Assets;
using System.ComponentModel.DataAnnotations;


namespace Duncan.PEMS.Utilities
{

    public static class CheckBoxListExtension
    {
        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, IEnumerable<SelectListItem> listOfValues, string cbListName)
        {
            var sb = new StringBuilder();

            if (listOfValues != null)
            {
                sb.Append("<ul>");
                foreach (var item in listOfValues)
                {
                    sb.Append("<li>");
                    sb.Append("<input name=\"" + cbListName + "\" " + (item.Selected ? "checked=\"checked\"" : "") +  " id=\"" + item.Value + "\" type=\"checkbox\" value=\"" + item.Value + "\" />");
                    sb.Append( "<label for=\"" + item.Value + "\">" + item.Text+ "</label>");
                    sb.Append("</li>");
                }
            }
            sb.Append("</ul>");
            return MvcHtmlString.Create(sb.ToString());
        }
    }

    public static class TooltipHelper
    {
        public static HtmlString Tooltip(this HtmlHelper htmlHelper, string key)
        {
            object globalResourceObject = HttpContext.Current.GetLocaleResource( ResourceTypes.Tooltip, key );
            if( globalResourceObject != null )
            {
                string tooltipText = globalResourceObject.ToString();
                var sb = new StringBuilder();
                sb.AppendFormat( "<span class=\"tooltip\" qtip-content=\"{0}\" >^</span>", tooltipText );
                var htmlString = new HtmlString( sb.ToString() );
                return htmlString;
            }
            
            return new HtmlString( "" );
        }
    }

    public static class ErrorDisplayExtension
    {
        public static MvcHtmlString ErrorOverview(this HtmlHelper htmlHelper, ModelStateDictionary modelState)
        {
            var sb = new StringBuilder();
            string errMsg = String.Empty;

            var modelStateErrors = modelState.Values.SelectMany(m => m.Errors);

            foreach(var e in modelStateErrors)
            {
                errMsg = e.ErrorMessage;
            }


            if (!modelState.IsValid)
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                if (String.IsNullOrEmpty(errMsg))
                {
                    sb.Append("Please fix errors below.");
                }
                else
                {
                    sb.Append(errMsg);
                }
                sb.Append("</span>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }


        public static MvcHtmlString AssetCreatedStatusMsg(this HtmlHelper htmlHelper, ModelStateDictionary modelState, MeterEditModel model, string currCity, string assetIdStatus)
        {
            var sb = new StringBuilder();

            if (model.Saved)
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                if (assetIdStatus == "New")
                {
                    sb.Append("New Meter " + model.AssetId + " for City " + currCity + " Successfully Created.");
                }
                else
                {
                    sb.Append("Meter " + model.AssetId + " for City " + currCity + " Successfully Updated.");
                }
                sb.Append("</span>");
            }
            else
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                sb.Append("");
                sb.Append("</span>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString AssetSpaceCreatedStatusMsg(this HtmlHelper htmlHelper, ModelStateDictionary modelState, SpaceEditModel model, string currCity, string assetIdStatus)
        {
            var sb = new StringBuilder();

            if (model.Saved)
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                if (assetIdStatus == "New")
                {
                    sb.Append("New Space " + model.AssetId + " for City " + currCity + " Successfully Created.");
                }
                else
                {
                    sb.Append("Space " + model.AssetId + " for City " + currCity + " Successfully Updated.");
                }
                sb.Append("</span>");
            }
            else
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                sb.Append("");
                sb.Append("</span>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString AssetGatewayCreatedStatusMsg(this HtmlHelper htmlHelper, ModelStateDictionary modelState, GatewayEditModel model, string currCity, string assetIdStatus)
        {
            var sb = new StringBuilder();

            if (model.Saved)
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                if (assetIdStatus == "New")
                {
                    sb.Append("New Gateway " + model.AssetId + " for City " + currCity + " Successfully Created.");
                }
                else
                {
                    sb.Append("Gateway " + model.AssetId + " for City " + currCity + " Successfully Updated.");
                }
                sb.Append("</span>");
            }
            else
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                sb.Append("");
                sb.Append("</span>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString AssetMechanismCreatedStatusMsg(this HtmlHelper htmlHelper, ModelStateDictionary modelState, MechanismEditModel model, string currCity, string assetIdStatus)
        {
            var sb = new StringBuilder();

            if (model.Saved)
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                if (assetIdStatus == "New")
                {
                    sb.Append("New Mechanism " + model.AssetId + " for City " + currCity + " Successfully Created.");
                }
                else
                {
                    sb.Append("Mechanism " + model.AssetId + " for City " + currCity + " Successfully Updated.");
                }
                sb.Append("</span>");
            }
            else
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                sb.Append("");
                sb.Append("</span>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString AssetDatakeyCreatedStatusMsg(this HtmlHelper htmlHelper, ModelStateDictionary modelState, DataKeyEditModel model, string currCity, string assetIdStatus)
        {
            var sb = new StringBuilder();

            if (model.Saved)
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                if (assetIdStatus == "New")
                {
                    sb.Append("New DataKey " + model.AssetId + " for City " + currCity + " Successfully Created.");
                }
                else
                {
                    sb.Append("DataKey " + model.AssetId + " for City " + currCity + " Successfully Updated.");
                }
                sb.Append("</span>");
            }
            else
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                sb.Append("");
                sb.Append("</span>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString AssetSensorCreatedStatusMsg(this HtmlHelper htmlHelper, ModelStateDictionary modelState, SensorEditModel model, string currCity, string assetIdStatus)
        {
            var sb = new StringBuilder();

            if (model.Saved)
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                if (assetIdStatus == "New")
                {
                    sb.Append("New Sensor " + model.AssetId + " for City " + currCity + " Successfully Created.");
                }
                else
                {
                    sb.Append("Sensor " + model.AssetId + " for City " + currCity + " Successfully Updated.");
                }
                sb.Append("</span>");
            }
            else
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                sb.Append("");
                sb.Append("</span>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString AssetCashBoxCreatedStatusMsg(this HtmlHelper htmlHelper, ModelStateDictionary modelState, CashboxEditModel model, string currCity, string assetIdStatus)
        {
            var sb = new StringBuilder();

            if (model.Saved)
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                if (assetIdStatus == "New")
                {
                    sb.Append("New CashBox " + model.AssetId + " for City " + currCity + " Successfully Created.");
                }
                else
                {
                    sb.Append("CashBox " + model.AssetId + " for City " + currCity + " Successfully Updated.");
                }
                sb.Append("</span>");
            }
            else
            {
                sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                sb.Append("");
                sb.Append("</span>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }


        public static MvcHtmlString RequiredMarkFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            if ( ModelMetadata.FromLambdaExpression( expression, htmlHelper.ViewData ).IsRequired )
            {
                RouteValueDictionary attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);   
                var sb = new StringBuilder();
                sb.Append( "<span" );

                if(attributes.ContainsKey("class"))
                    sb.Append(" class=\"").Append(attributes["class"]).Append("\"");
                sb.Append( ">*</span>" );

                return MvcHtmlString.Create(sb.ToString());
            }
                return MvcHtmlString.Empty;
        }

        public static MvcHtmlString ShowAllErrors(this HtmlHelper helper, String key)
        {
            var sb = new StringBuilder();
            if (helper.ViewData.ModelState[key] != null)
            {
                foreach (var e in helper.ViewData.ModelState[key].Errors)
                {
                    if (key != Constants.ViewData.ModelStateStatusKey)
                    {
                        sb.Append("<span class=\"").Append("field-validation-error").Append("\">");
                        sb.Append(e.ErrorMessage); 
                        sb.Append( "</span>");
                    }
                }
                return MvcHtmlString.Create(sb.ToString());
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString ShowStatusMessage(this HtmlHelper helper)
        {
            var sb = new StringBuilder();
            if (helper.ViewData.ModelState[Constants.ViewData.ModelStateStatusKey] != null)
            {
                foreach (var e in helper.ViewData.ModelState[Constants.ViewData.ModelStateStatusKey].Errors)
                {
                    sb.Append("<span ");
                    sb.Append(" class=\"").Append("page-validation-status").Append("\"");
                    sb.Append(">" + e.ErrorMessage + "</span>");
                }
                return MvcHtmlString.Create(sb.ToString());
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString HeaderMessage(this HtmlHelper helper)
        {
            var sb = new StringBuilder();

            //only add if the model state is invalid

            if (!helper.ViewData.ModelState.IsValid)
            {
                //if the model has errors that are NOT status messages, add the generic message as well.
                if (helper.ViewData.ModelState.Any(x => x.Key != Constants.ViewData.ModelStateStatusKey && x.Value.Errors.Count > 0))
                {
                    sb.Append("<span class=\"").Append("page-validation-error").Append("\">");
                    sb.Append("Please fix errors below.");
                    sb.Append("</span>");
                }
            }
            //if the model statse has a message for the user, display it - Status Message
            if (helper.ViewData.ModelState[Constants.ViewData.ModelStateStatusKey] != null)
            {
                foreach (var e in helper.ViewData.ModelState[Constants.ViewData.ModelStateStatusKey].Errors)
                {
                    sb.Append("<span ");
                    sb.Append(" class=\"").Append("page-validation-status").Append("\"");
                    sb.Append(">" + e.ErrorMessage + "</span>");
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString DisplayForLatLong<TModel>(this HtmlHelper<TModel> htmlHelper, double? latOrLong, object htmlAttributes)
        {
            // Get the value of the lat/long
            var value = latOrLong == null ? "" : latOrLong.Value.ToString("N5");

            RouteValueDictionary attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var sb = new StringBuilder();
            sb.Append("<span");

            if (attributes.ContainsKey("class"))
                sb.Append(" class=\"").Append(attributes["class"]).Append("\"");
            sb.Append(">").Append(value).Append("</span>");

            return MvcHtmlString.Create(sb.ToString());
        }


        public static MvcHtmlString DisplayForLatLong<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            // Get the value of the lat/long
            var model = htmlHelper.ViewData.Model;
            var value = "";
            var body = expression.Body as MemberExpression;
            var propertyName = body.Member.Name;

            if ( model != null )
            {
                var modelType = typeof (TModel);
                var propertyInfo = modelType.GetProperty( propertyName );

                var propertyValue = propertyInfo.GetValue( model );

                // Is this a double or float?
                if ( propertyValue != null && (propertyValue is double || propertyValue is float ))
                {
                    value = ( (double) propertyValue ).ToString( "N5" );
                }
            }
            
            RouteValueDictionary attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var sb = new StringBuilder();
            sb.Append("<span");

            if (attributes.ContainsKey("class"))
                sb.Append(" class=\"").Append(attributes["class"]).Append("\"");
            sb.Append( ">" ).Append(value).Append("</span>");

            return MvcHtmlString.Create(sb.ToString());
        }



    }

}

