using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Entities.News
{
    public class DashboardRevenue
    {
        public int PaymentAccepted { get; set; }
        public int PaymentPending { get; set; }
        public int PaymentRefunded { get; set; }
        public int TotelPayment
        {
            get
            {
                return this.PaymentAccepted + this.PaymentPending + this.PaymentRefunded;
            }
        }
    }
}
