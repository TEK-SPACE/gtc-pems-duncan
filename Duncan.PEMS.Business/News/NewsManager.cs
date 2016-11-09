using System;
using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.News;
using NLog;

namespace Duncan.PEMS.Business.News
{
    public class NewsManager : RbacBaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        private const int GlobalCustomerId = 0;
        private const string DefaultCultureCode = "en-US";

        /// <summary>
        /// Gets <see cref="List{NewsItem}"/> for a customer (<paramref name="customerId"/> within
        /// the current day as seen from the customer's current time (<paramref name="customerNow"/>).
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="cultureCode">Requested culture code (I.E. en-US)</param>
        /// <param name="customerNow">
        /// <see cref="DateTime"/> indication of 'now' for the factory.  Generally used to ensure that
        /// the factory is set to the customer's timezone-adjusted 'now'.
        /// </param>
        /// <returns><see cref="List{NewsItem}"/> of active news items.</returns>
        public List<NewsItem> GetNewsItems(int customerId, string cultureCode, DateTime customerNow)
        {
            var items = new List<NewsItem>();
            try
            {

                DateTime endOfDay = new DateTime(customerNow.Year, customerNow.Month, customerNow.Day, 23, 59, 59, 999);
                DateTime beginningOfDay = new DateTime(customerNow.Year, customerNow.Month, customerNow.Day, 0, 0, 0, 0);

                    IQueryable<DataAccess.RBAC.News> news = RbacEntities.News.Where(x => (x.CustomerId == customerId || x.CustomerId == GlobalCustomerId)
                                                                             && (x.ExpirationDate == null || x.ExpirationDate >= beginningOfDay)
                                                                             && (x.EffectiveDate == null || x.EffectiveDate <= endOfDay)
                                                                             && x.Display == true);

                    foreach (var article in news)
                    {
                        // IF a version of the news item exists for the current culture code, use that
                        // ELSE, use default
                        NewsContent content = article.NewsContents.FirstOrDefault( x => x.CultureCode == cultureCode || x.CultureCode == DefaultCultureCode );
                        if ( content != null )
                        {
                            items.Add( new NewsItem
                                {
                                               Content = content.Content,
                                               EffectiveDate = content.News.EffectiveDate
                                           } );
                        }
                    }

                    items = items.OrderBy( x => x.EffectiveDate ).ToList();
            }
            catch(Exception ex)
            {
                _logger.ErrorException( "Error getting news items", ex );
            }

            return items;
        }
    }
}