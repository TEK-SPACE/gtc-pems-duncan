using System;

namespace Duncan.PEMS.Entities.WorkOrders
{
    public class WorkOrderImage
    {
        public long WorkOrderImageId { get; set; }
        public long WorkOrderId { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageSource
        {
            get
            {
                string base64 = Convert.ToBase64String(ImageData);
                base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
                return string.Format("data:{0};base64,{1}", "image", base64);
            }
        }
        public DateTime DateTaken { get; set; }
        public string ImageName { get
        {
            //work order id and date
            return WorkOrderId.ToString() + "-" + DateTaken.ToString("g");
        } }
    }
}
