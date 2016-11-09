/******************* CHANGE LOG *************************************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             _________________________________________________________________________________________________________
 * 
 * 12/19/2013       Sergey Ostrerov                 Issue: DPTXPEMS-129. Lattitude and Longitude filter shows incorrect data - Filer is not working.
 *                                                         Updated: private static Expression GenerateLambda<T>(.
 *                                                         Added logic StartsWith for Latitude/Longitude filters.
 *                                                         
 * ******************************************************************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Objects;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Duncan.PEMS.Entities.Roles;
using Duncan.PEMS.Entities.Users;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.Infrastructure;

namespace Duncan.PEMS.Utilities
{
    public static class AjaxCustomBindingExtensions
    {
        private const ListSortDirection DefaultSortDirection = ListSortDirection.Ascending;

        private static object GetPropertyValue(object obj, string property)
        {
            if (obj == null)
                return null;

            if (string.IsNullOrEmpty(property))
                property = obj.GetType().GetProperties()[0].Name;

            PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }

        #region "Paging"

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> data, int page, int pageSize)
        {
            if (pageSize > 0 && page > 0)
                data = data.Skip((page - 1)*pageSize);
            data = data.Take(pageSize);
            return data;
        }

        #endregion

        #region "Sorting"

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> data,
                                                    IList<GroupDescriptor> groupDescriptors,
                                                    IList<SortDescriptor> sortDescriptors)
        {
            if (groupDescriptors != null && groupDescriptors.Any())
                if (data != null)
                    data = groupDescriptors.Reverse()
                                           .Aggregate(data,
                                                      (current, groupDescriptor) =>
                                                      AddSortExpression(current, groupDescriptor.SortDirection,
                                                                        groupDescriptor.Member));

            if (sortDescriptors != null && sortDescriptors.Any())
                data = sortDescriptors.Aggregate(data,
                                                 (current, sortDescriptor) =>
                                                 AddSortExpression(current, sortDescriptor.SortDirection,
                                                                   sortDescriptor.Member));
                //if there is no sort selected, we need to default it to something - it will grab the first property of the class
                //TODO - need to make sure the first property isnt a class, if it is, then skip and find the first non-class property
            else
            {
                Type type = typeof (T);
                PropertyInfo[] properties = type.GetProperties();
                if ( properties.Any() )
                {
                    // If contains a 'DateTime' property, use that. Else, use the first property
                    string orderBy = ( from property in properties where property.Name == "DateTime"  select property.Name ).FirstOrDefault();
                    if ( String.IsNullOrEmpty( orderBy ) )
                    {
                        orderBy = properties[0].Name;
                    }
                     
                    data = AddSortExpression( data, DefaultSortDirection, orderBy );
                }
            }

            return data;
        }

        private static IQueryable<T> AddSortExpression<T>(IQueryable<T> data, ListSortDirection
                                                                                  sortDirection, string memberName)
        {
            if (sortDirection == ListSortDirection.Ascending)
                data = data.OrderBy(memberName + " ASC");
            else
                data = data.OrderBy(memberName + " DESC");
            return data;
        }

        #endregion

        #region "Filtering"

        public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> data, IList<IFilterDescriptor> filterDescriptors)
        {
            if ( filterDescriptors != null && filterDescriptors.Any() )
            {
                //create the object parameter (x => x.SomeProperty ... ) with type passed in
                ParameterExpression parameter = Expression.Parameter( typeof (T), "x" );

                Expression dynamiclambda = null;
                dynamiclambda = GenerateLambda<T>( filterDescriptors, parameter, dynamiclambda );

                if ( null != dynamiclambda )
                {
                    Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>( dynamiclambda, parameter );
                    data = data.Where( predicate.Compile() ).AsQueryable();
                }
            }
            return data;
        }

        private static Expression GenerateLambda<T>(IList<IFilterDescriptor> filterDescriptors,
                                                    ParameterExpression parameter,
                                                    Expression dynamiclambda)
        {
            foreach (IFilterDescriptor filter in filterDescriptors)
            {
                if (filter is CompositeFilterDescriptor)
                    dynamiclambda = GenerateLambda<T>(((CompositeFilterDescriptor) filter).FilterDescriptors, parameter,
                                                      dynamiclambda);
                else
                {
                    FilterDescriptor filterDescriptor = (FilterDescriptor) filter;
                    PropertyInfo propertyInfo = typeof (T).GetProperty(filterDescriptor.Member);
                    if (null != propertyInfo)
                    {
                        // Determine type, and do this for strings, that for ints, etc.

                        // Get a reference to the object property (e.g. x.AssetName)
                        MemberExpression property = Expression.MakeMemberAccess( parameter, propertyInfo );
                        Expression expression = null; // default to empty expression
                        FilterOperator filterOperator = filterDescriptor.Operator;
                        MethodInfo toLowermethod = null;
                        MethodCallExpression tolowerCall = null;
                        // Handle string type
                        if (propertyInfo.PropertyType == typeof (string))
                        {
                            string filterValue = filterDescriptor.Value.ToString().ToLower().Trim();

                            switch (filterOperator)
                            {
                                case FilterOperator.IsEqualTo:
                                    expression = Expression.Call( typeof (string),
                                                                  "Equals",
                                                                  null,
                                                                  property,
                                                                  Expression.Constant( filterValue ),
                                                                  Expression.Constant( StringComparison.OrdinalIgnoreCase ) );
                                    break;
                                case FilterOperator.StartsWith:
                                    MethodInfo startsWithMethod = typeof(String).GetMethod("StartsWith", new[] { typeof(String) });
                                    toLowermethod = typeof( string ).GetMethod( "ToLower", Type.EmptyTypes );
                                    tolowerCall = Expression.Call( property, toLowermethod );
                                    expression = Expression.Call(tolowerCall, startsWithMethod, Expression.Constant(filterValue));
                                    break;

                                default:
                                    // Default to "Contains"
                                    MethodInfo containsMethod = typeof( String ).GetMethod( "Contains", new[] { typeof( String ) } );
                                    toLowermethod = typeof( string ).GetMethod( "ToLower", Type.EmptyTypes );
                                    tolowerCall = Expression.Call( property, toLowermethod );
                                    expression = Expression.Call( tolowerCall, containsMethod, Expression.Constant( filterValue ) );
                                    break;
                            }
                        }

                        // Handle int type
                        if (propertyInfo.PropertyType == typeof (int))
                        {
                            int val;
                            bool parsed = int.TryParse( filterDescriptor.Value.ToString(), out val );
                            if( parsed )
                            {
                                switch (filterOperator)
                                {
                                    case FilterOperator.IsGreaterThanOrEqualTo:
                                        expression = Expression.GreaterThanOrEqual( property, Expression.Constant( val ) );
                                        break;
                                    case FilterOperator.IsLessThanOrEqualTo:
                                        expression = Expression.LessThanOrEqual( property, Expression.Constant( val ) );
                                        break;
                                    default:
                                        // Default to "Equals"
                                        expression = Expression.Equal( property, Expression.Constant( val ) );
                                        break;
                                }
                            }
                        }


                        // Handle long type
                        if (propertyInfo.PropertyType == typeof(long))
                        {
                            long val;
                            bool parsed = long.TryParse(filterDescriptor.Value.ToString(), out val);
                            if (parsed)
                            {
                                switch (filterOperator)
                                {
                                    case FilterOperator.IsGreaterThanOrEqualTo:
                                        expression = Expression.GreaterThanOrEqual(property, Expression.Constant(val));
                                        break;
                                    case FilterOperator.IsLessThanOrEqualTo:
                                        expression = Expression.LessThanOrEqual(property, Expression.Constant(val));
                                        break;
                                    default:
                                        // Default to "Equals"
                                        expression = Expression.Equal(property, Expression.Constant(val));
                                        break;
                                }
                            }
                        }


                        // Handle nullable long type
                        if (propertyInfo.PropertyType == typeof(long?))
                        {
                            long val;
                            bool parsed = long.TryParse(filterDescriptor.Value.ToString(), out val);
                            if (parsed)
                            {
                                switch (filterOperator)
                                {
                                    case FilterOperator.IsGreaterThanOrEqualTo:
                                        expression = Expression.GreaterThanOrEqual(property, Expression.Constant(val, typeof(long?)));
                                        break;
                                    case FilterOperator.IsLessThanOrEqualTo:
                                        expression = Expression.LessThanOrEqual(property, Expression.Constant(val, typeof(long?)));
                                        break;
                                    default:
                                        // Default to "Equals"
                                        expression = Expression.Equal(property, Expression.Constant(val, typeof(long?)));
                                        break;
                                }
                            }
                        }



                        // Handle nullable int type
                        if( propertyInfo.PropertyType == typeof( int? ) )
                        {
                            int val;
                            bool parsed = int.TryParse( filterDescriptor.Value.ToString(), out val );
                            if( parsed )
                            {
                                switch( filterOperator )
                                {
                                    case FilterOperator.IsGreaterThanOrEqualTo:
                                        expression = Expression.GreaterThanOrEqual( property, Expression.Constant( val, typeof( int? ) ) );
                                        break;
                                    case FilterOperator.IsLessThanOrEqualTo:
                                        expression = Expression.LessThanOrEqual( property, Expression.Constant( val, typeof( int? ) ) );
                                        break;
                                    default:
                                        // Default to "Equals"
                                        expression = Expression.Equal( property, Expression.Constant( val, typeof( int? ) ) );
                                        break;
                                }
                            }
                        }

                        //double
                        // Handle DateTime type
                        if (propertyInfo.PropertyType == typeof(double))
                        {
                            double value;
                            bool parsed = double.TryParse(filterDescriptor.Value.ToString(), out value);
                            if (parsed)
                            {
                                switch (filterOperator)
                                {
                                    case FilterOperator.IsGreaterThanOrEqualTo:
                                        expression = Expression.GreaterThanOrEqual(property, Expression.Constant(value));
                                        break;
                                    case FilterOperator.IsLessThanOrEqualTo:
                                        expression = Expression.LessThanOrEqual(property, Expression.Constant(value));
                                        break;
                                    default:
                                        // Default to "Equals"
                                        expression = Expression.Equal(property, Expression.Constant(value));
                                        break;
                                }
                            }
                        }
                        // Handle DateTime? type
                        if (propertyInfo.PropertyType == typeof(double?))
                        {
                            double value;
                            bool parsed = double.TryParse(filterDescriptor.Value.ToString(), out value);
                            if (parsed)
                            {
                                switch (filterOperator)
                                {
                                    case FilterOperator.IsGreaterThanOrEqualTo:
                                        expression = Expression.GreaterThanOrEqual(property, Expression.Constant(value, typeof(double?)));
                                        break;
                                    case FilterOperator.IsLessThanOrEqualTo:
                                        expression = Expression.LessThanOrEqual(property, Expression.Constant(value, typeof(double?)));
                                        break;
                                    default:
                                        // Default to "Equals"
                                        expression = Expression.Equal(property, Expression.Constant(value, typeof(double?)));
                                        break;
                                }
                            }
                        }



                        // Handle DateTime type
                        if( propertyInfo.PropertyType == typeof( DateTime ) )
                        {
                            DateTime value;
                            bool parsed = DateTime.TryParse( filterDescriptor.Value.ToString(), out value );
                            if ( parsed )
                            {
                                switch( filterOperator )
                                {
                                    case FilterOperator.IsGreaterThanOrEqualTo:
                                        expression = Expression.GreaterThanOrEqual(property, Expression.Constant(value));
                                        break;
                                    case FilterOperator.IsLessThanOrEqualTo:
                                        expression = Expression.LessThanOrEqual(property, Expression.Constant(value));
                                        break;
                                    default:
                                        // Default to "Equals"
                                        expression = Expression.Equal(property, Expression.Constant(value));
                                        break;
                                }
                            }
                        }
                        // Handle DateTime? type
                        if (propertyInfo.PropertyType == typeof(DateTime?))
                        {
                            DateTime value;
                            bool parsed = DateTime.TryParse(filterDescriptor.Value.ToString(), out value);
                            if (parsed)
                            {
                                switch (filterOperator)
                                {
                                    case FilterOperator.IsGreaterThanOrEqualTo:
                                        expression = Expression.GreaterThanOrEqual(property, Expression.Constant(value, typeof(DateTime?)));
                                        break;
                                    case FilterOperator.IsLessThanOrEqualTo:
                                        expression = Expression.LessThanOrEqual(property, Expression.Constant(value, typeof(DateTime?)));
                                        break;
                                    default:
                                        // Default to "Equals"
                                        expression = Expression.Equal(property, Expression.Constant(value, typeof(DateTime?)));
                                        break;
                                }
                            }
                        }
                        if ( expression != null )
                        {
                            // Add expression to dynamic lambda
                            dynamiclambda = null == dynamiclambda
                                                    ? expression
                                                    : Expression.And( dynamiclambda, expression );
                        }
                    }
                }
            }
            return dynamiclambda;
        }

        /// <summary>
        /// Special filtering for Time Type fields where the columns in the database are
        /// TimeType1, TimeType2, etc
        /// </summary>
        public static IQueryable<T> ApplyTimeTypeFiltering<T>( this IQueryable<T> query, IList<IFilterDescriptor> filterDescriptors )
        {
            if( filterDescriptors != null && filterDescriptors.Any() )
            {
                FilterDescriptor reference = null;

                foreach( IFilterDescriptor filter in filterDescriptors )
                {
                    if( filter is CompositeFilterDescriptor )
                    {
                        query = ApplyTimeTypeFiltering<T>( query, ( filter as CompositeFilterDescriptor ).FilterDescriptors );
                    }
                    else
                    {
                        FilterDescriptor filterDescriptor = (FilterDescriptor)filter;

                        if( filterDescriptor.Member == "TimeType" )
                        {
                            reference = (FilterDescriptor)filter;

                            string queryString = String.Format( "TimeType1={0} Or TimeType2={0} Or TimeType3={0} Or TimeType4={0} Or TimeType5={0}", filterDescriptor.Value );

                            query = query.Where( queryString );

                            break;
                        }
                    }
                }

                if( reference != null )
                {
                    bool removed = filterDescriptors.Remove( reference );
                }
            }

            return query;
        }

        #endregion

        #region "Grouping - user list model"
        public static IEnumerable ApplyGrouping(this IQueryable<ListUserModel> data, IList<GroupDescriptor>
                                                                                         groupDescriptors)
        {
            if (groupDescriptors != null && groupDescriptors.Any())
            {
                Func<IEnumerable<ListUserModel>, IEnumerable<AggregateFunctionsGroup>> selector = null;
                foreach (var group in groupDescriptors.Reverse())
                {
                    if (selector == null)
                    {
                        selector =
                            items =>
                            BuildInnerGroup(items, o => GetPropertyValue(o, group.Member).ToString().ToLower(),
                                            group.Member);
                    }
                    else
                    {
                        GroupDescriptor currentGroup = @group;
                        selector = BuildGroup(o => GetPropertyValue(o, currentGroup.Member).ToString().ToLower(),
                                              selector, group.Member);
                    }
                }

                if (selector != null) return selector.Invoke(data).ToList();
            }
            return data.ToList();
        }

        private static Func<IEnumerable<ListUserModel>, IEnumerable<AggregateFunctionsGroup>>
            BuildGroup<T>(Expression<Func<ListUserModel, T>> groupSelector, Func<IEnumerable<ListUserModel>,
                                                                                IEnumerable<AggregateFunctionsGroup>>
                                                                                selectorBuilder, string memberName)
        {
            var tempSelector = selectorBuilder;
            return g => g.GroupBy(groupSelector.Compile())
                         .Select(c => new AggregateFunctionsGroup
                             {
                                 Key = c.Key,
                                 HasSubgroups = true,
                                 Member = memberName,
                                 Items = tempSelector.Invoke(c).ToList()
                             });
        }

        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<ListUserModel>
                                                                                   group,
                                                                               Expression<Func<ListUserModel, T>>
                                                                                   groupSelector, string memberName)
        {
            var retVal = group.GroupBy(groupSelector.Compile())
                              .Select(i => new AggregateFunctionsGroup
                                  {
                                      Key = i.Key,
                                      Member = memberName,
                                      Items = i.ToList()
                                  });
            return retVal;
        }

        #endregion

        #region "Grouping - role list model"


        public static IEnumerable ApplyGrouping(this IQueryable<ListRoleModel> data, IList<GroupDescriptor>
                                                                                         groupDescriptors)
        {
            if (groupDescriptors != null && groupDescriptors.Any())
            {
                Func<IEnumerable<ListRoleModel>, IEnumerable<AggregateFunctionsGroup>> selector = null;
                foreach (var group in groupDescriptors.Reverse())
                {
                    if (selector == null)
                    {
                        selector =
                            items =>
                            BuildInnerGroup(items, o => GetPropertyValue(o, group.Member).ToString().ToLower(),
                                            group.Member);
                    }
                    else
                    {
                        GroupDescriptor currentGroup = @group;
                        selector = BuildGroup(o => GetPropertyValue(o, currentGroup.Member).ToString().ToLower(),
                                              selector, group.Member);
                    }
                }

                if (selector != null) return selector.Invoke(data).ToList();
            }
            return data.ToList();
        }


        private static Func<IEnumerable<ListRoleModel>, IEnumerable<AggregateFunctionsGroup>>
            BuildGroup<T>(Expression<Func<ListRoleModel, T>> groupSelector, Func<IEnumerable<ListRoleModel>,
                                                                                IEnumerable<AggregateFunctionsGroup>>
                                                                                selectorBuilder, string memberName)
        {
            var tempSelector = selectorBuilder;
            return g => g.GroupBy(groupSelector.Compile())
                         .Select(c => new AggregateFunctionsGroup
                             {
                                 Key = c.Key,
                                 HasSubgroups = true,
                                 Member = memberName,
                                 Items = tempSelector.Invoke(c).ToList()
                             });
        }

        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<ListRoleModel>
                                                                                   group,
                                                                               Expression<Func<ListRoleModel, T>>
                                                                                   groupSelector, string memberName)
        {
            var retVal = group.GroupBy(groupSelector.Compile())
                              .Select(i => new AggregateFunctionsGroup
                                  {
                                      Key = i.Key,
                                      Member = memberName,
                                      Items = i.ToList()
                                  });
            return retVal;
        }

        #endregion
        
    }

    public static class OrderByHelper
    {
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string orderBy)
        {
            return enumerable.AsQueryable().OrderBy(orderBy).AsEnumerable();
        }

        public static IEnumerable<T> ThenBy<T>(this IEnumerable<T> enumerable, string orderBy)
        {
            return enumerable.AsQueryable().ThenBy(orderBy).AsEnumerable();
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> collection, string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                Type type = typeof(T);
                orderBy = type.GetProperties()[0].Name;
            } 
            foreach (OrderByInfo orderByInfo in ParseOrderBy(orderBy))
                collection = ApplyOrderBy<T>(collection, orderByInfo);

            return collection;
        }

        private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> collection, OrderByInfo orderByInfo)
        {
            string[] props = orderByInfo.PropertyName.Split('.');
            Type type = typeof(T);

            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
            string methodName = String.Empty;

            if (!orderByInfo.Initial && collection is IOrderedQueryable<T>)
            {
                if (orderByInfo.Direction == SortDirection.Ascending)
                    methodName = "ThenBy";
                else
                    methodName = "ThenByDescending";
            }
            else
            {
                if (orderByInfo.Direction == SortDirection.Ascending)
                    methodName = "OrderBy";
                else
                    methodName = "OrderByDescending";
            }

            return (IOrderedQueryable<T>)typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { collection, lambda });

        }

        private static IEnumerable<OrderByInfo> ParseOrderBy(string orderBy)
        {
            if (String.IsNullOrEmpty(orderBy))
                yield break;

            string[] items = orderBy.Split(',');
            bool initial = true;
            foreach (string item in items)
            {
                string[] pair = item.Trim().Split(' ');

                if (pair.Length > 2)
                    throw new ArgumentException(String.Format("Invalid OrderBy string '{0}'. Order By Format: Property, Property2 ASC, Property2 DESC", item));

                string prop = pair[0].Trim();

                if (String.IsNullOrEmpty(prop))
                    throw new ArgumentException("Invalid Property. Order By Format: Property, Property2 ASC, Property2 DESC");

                var dir = SortDirection.Ascending;

                if (pair.Length == 2)
                    dir = ("desc".Equals(pair[1].Trim(), StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending);

                yield return new OrderByInfo() { PropertyName = prop, Direction = dir, Initial = initial };

                initial = false;
            }

        }

        private class OrderByInfo
        {
            public string PropertyName { get; set; }
            public SortDirection Direction { get; set; }
            public bool Initial { get; set; }
        }
        
        private enum SortDirection
        {
            Ascending = 0,
            Descending = 1
        }
    }
}
