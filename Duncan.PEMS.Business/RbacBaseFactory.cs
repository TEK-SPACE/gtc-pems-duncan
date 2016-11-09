using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using NLog;

namespace Duncan.PEMS.Business
{
    /// <summary>
    /// This is the base business factory class.  This class encapsulates the Entity Framework context 
    /// that is used to access the PEMS RBAC database.  This class would be inherited from for factories
    /// that only accessed the PEMS RBAC database.
    /// </summary>
    public abstract class RbacBaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Instance of <see cref="PEMRBACEntities"/> for factory use.
        /// </summary>
        private PEMRBACEntities _rbacEntities;
        /// <summary>
        /// Protected property to get/create instance of <see cref="PEMRBACEntities"/>
        /// </summary>
        protected PEMRBACEntities RbacEntities
        {
            get { return _rbacEntities ?? (_rbacEntities = new PEMRBACEntities()); }
        }

        public List<FilterDescriptor> GetFilterDescriptors(IList<IFilterDescriptor> filterDescriptors, List<FilterDescriptor> filters)
        {
            foreach (IFilterDescriptor filter in filterDescriptors)
            {
                if (filter is CompositeFilterDescriptor)
                    filters = GetFilterDescriptors(((CompositeFilterDescriptor)filter).FilterDescriptors, filters);
                else
                    filters.Add((FilterDescriptor)filter);
            }
            return filters;
        }

        /// <summary>
        /// Gets a list of parameters form a data source request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultOrderBy"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public SqlParameter[] GetSpParams(DataSourceRequest request, string defaultOrderBy, out string paramValues)
        {
            //get the filters
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            var parameters = new List<SqlParameter>();

            //ORDER BY
            //now lets add the order by clause - generate the string required
            string orderBystring = string.Empty;
            foreach (SortDescriptor sortDescriptor in request.Sorts)
            {
                if (string.IsNullOrEmpty(orderBystring))
                    orderBystring = sortDescriptor.Member;
                else orderBystring += ", " + sortDescriptor.Member;
                if (sortDescriptor.SortDirection == ListSortDirection.Descending)
                    orderBystring += " desc";
            }
            if (string.IsNullOrEmpty(orderBystring))
                orderBystring = defaultOrderBy;
            parameters.Add(new SqlParameter("orderBy", orderBystring));

            //now the REQUIRED param values here (that dont come from request.filters
            paramValues = "@orderBy, @PageNumber, @PageSize";
            parameters.Add(new SqlParameter("PageNumber", request.Page));
            parameters.Add(new SqlParameter("PageSize", request.PageSize));

            foreach (var filter in filters)
            {
                //add the param to the list

                //sterilize the input toprevent sql injection attacks
                //only have to sanitize if the string is not empty
                if (!string.IsNullOrEmpty(filter.Value.ToString()))
                {
                    var safeValue = GetSafeSqlLiteral(filter.Value, 2);
                    parameters.Add(new SqlParameter(filter.Member, safeValue));
                }
                else
                    parameters.Add(new SqlParameter(filter.Member, filter.Value));
                //update the string
                paramValues += ",  @" + filter.Member;
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// Cleans up a string to make it safe from sql injection attacks
        /// </summary>
        /// <param name="theValue"></param>
        /// <param name="intLevel"></param>
        /// <returns></returns>
        public string GetSafeSqlLiteral(object theValue, int intLevel)
        {
            // Written by user CWA, CoolWebAwards.com Forums. 2 February 2010
            // http://forum.coolwebawards.com/threads/12-Preventing-SQL-injection-attacks-using-C-NET

            // intLevel represent how thorough the value will be checked for dangerous code
            // intLevel (1) - Do just the basic. This level will already counter most of the SQL injection attacks
            // intLevel (2) -   (non breaking space) will be added to most words used in SQL queries to prevent unauthorized access to the database. Safe to be printed back into HTML code. Don't use for usernames or passwords

            string strValue;
            if (theValue is DateTime)
            {
                // .ToString() may yield a datetime culture variant that SQL
                // can't parse into a datetime. So call culture-invariant method
                CultureInfo InvC = new CultureInfo(""); // Creates a CultureInfo set to InvariantCulture.
                DateTime dateTime = DateTime.Parse(theValue.ToString());
                strValue = dateTime.ToString("s", InvC); //Sortable date/time pattern.
            }
            else
            {
                strValue = theValue.ToString();
            }

            if (intLevel > 0)
            {
                strValue = strValue.Replace("'", "''"); // Most important one! This line alone can prevent most injection attacks
                strValue = strValue.Replace("--", "");
                strValue = strValue.Replace("[", "[[]");
                strValue = strValue.Replace("%", "[%]");
                strValue = strValue.Replace(" OR ", "");
                strValue = strValue.Replace(" or ", "");
                strValue = strValue.Replace(" oR ", "");
                strValue = strValue.Replace(" Or ", "");
                strValue = strValue.Replace(" AND ", "");
                strValue = strValue.Replace(" ANd ", "");
                strValue = strValue.Replace(" And ", "");
                strValue = strValue.Replace(" and ", "");
                strValue = strValue.Replace(" aNd ", "");
                strValue = strValue.Replace(" aND ", "");
                strValue = strValue.Replace(" anD ", "");
                strValue = strValue.Replace("({", "");
                strValue = strValue.Replace("/*", "");
            }
            if (intLevel > 1)
            {
                var myArray = new[] { "xp_ ", "update ", "insert ", "select ", "drop ", "alter ", "create ", "rename ", "delete ", "replace " };
                foreach (string vals in myArray)
                {
                    string strWord = vals;
                    var rx = new Regex(strWord, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    MatchCollection matches = rx.Matches(strValue);
                    int i2 = 0;
                    foreach (Match match in matches)
                    {
                        GroupCollection groups = match.Groups;
                        int intLenghtLeft = groups[0].Index + vals.Length + i2;
                        strValue = strValue.Substring(0, intLenghtLeft - 1) + "&nbsp;" + strValue.Substring(strValue.Length - (strValue.Length - intLenghtLeft), strValue.Length - intLenghtLeft);
                        i2 += 5;
                    }
                }
            }
            return strValue;
        }
    }
}
