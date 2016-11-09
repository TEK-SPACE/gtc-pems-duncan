using System;

namespace Duncan.PEMS.Entities.News
{
    public class NewsItem
    {
        public string Content { get; set; }

        public int meterCnt { get; set; }
        public DateTime EffectiveDate { get; set; }
 
        //** Code added on Oct 8th 2014 to display GPS coordinates for the map on Home page
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }




        public int TotalCount { get; set; }
    }
}