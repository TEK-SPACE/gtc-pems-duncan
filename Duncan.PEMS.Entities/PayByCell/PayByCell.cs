using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Duncan.PEMS.Entities.PayByCell
{
    public class PayByCellModel
    {
       
        public int? VendorID { get; set; }
         [OriginalGridPosition(Position = 1)]
        public string VendorName { get; set; }
        public int CreatedByID { get; set; }
        public int CustomerID { get; set; }
        public string CreatedByName { get; set; }
        public bool Deprecated { get; set; }
        public long Value { get; set; }
        public string Text { get; set; }
        public string FileTypeName { set; get; }
        public DateTime? CreatedOnDate { get; set; }

        //For SpaceManagement gridDetails in client page
        [OriginalGridPosition(Position = 0)]
        public int MeterID { set; get; }

        public int AreaID { set; get; }

        public int ZoneID { set; get; }

        public int SubUrbID { set; get; }


        [OriginalGridPosition(Position = 2)]
        public string AreaName { set; get; }

        [OriginalGridPosition(Position = 3)]
        public string ZoneName { set; get; }

        [OriginalGridPosition(Position = 4)]
        public string Street { set; get; }
        [OriginalGridPosition(Position = 5)]
        public string SubUrbName { set; get; }
    }
    public class DuncanPropertyModel
    {
        public string KeyText { get; set; }
        public string ValueText { get; set; }
        public string KeyTextOld { get; set; }
        public string ValueTextOld { get; set; }

    }
    public class CustomerPropertyModel
    {
        public string KeyText { get; set; }
        public string ValueText { get; set; }
        public string KeyTextOld { get; set; }
        public string ValueTextOld { get; set; }

    }
    public class PayByCellModelAdmin
    {
        [OriginalGridPosition(Position = 0)]
        public int? VendorID { get; set; }

        [OriginalGridPosition(Position = 1)]
        public string VendorName { get; set; }
        public int CreatedByID { get; set; }
        public int CustomerID { get; set; }
        [OriginalGridPosition(Position = 3)]
        public string CreatedByName { get; set; }
        public bool Deprecated { get; set; }
        public long Value { get; set; }
        public string Text { get; set; }
        public string FileTypeName { set; get; }
        [OriginalGridPosition(Position = 2)]
        public DateTime? CreatedOnDate { get; set; }
    }

   
}