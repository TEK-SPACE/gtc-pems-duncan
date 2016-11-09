using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System;
using System.Web.Routing;


namespace Duncan.PEMS.Entities.Transactions
{
    public class RevenueAudit
    {
        //public long RevenueAuditID { get; set; }
        public DateTime? RevenueAuditDate { get; set; }

        [OriginalGridPosition(Position = 0)]
        public string RevenueBatchID { get; set; }
        public string RevenueAuditDate_final { get; set; }

        //** For Aggregation in Revenue Audit Summary
        public string TotalRefundAmount { get; set; }
        public string TotalRefundCount { get; set; }

        public int gridTotal { get; set; }

        public string TotalSettleAmount { get; set; }
        public string TotalSettleCount { get; set; }

        public string TotalTransactionAmount { get; set; }
        public string TotalTransactionCount { get; set; }

        public string TotalDeclinedAmount { get; set; }
        public string TotalDeclinedCount { get; set; }
        public string TotalUnprocessedAmount { get; set; }
        public string TotalUnprocessedCount { get; set; }
        //** End of Aggregation in Revenue Audit Summary

        //** For Display in Revenue Audit Summary Grid Columns
        [OriginalGridPosition(Position = 5)]
        public string TransAmountInDollars { get; set; }  //** Sairam added this

        [OriginalGridPosition(Position = 6)]
        public int? TransCount { get; set; }

        [OriginalGridPosition(Position = 1)]
        public string RefundAmountInDollars { get; set; }  //** Sairam added this

        [OriginalGridPosition(Position = 2)]
        public int? RefundCount { get; set; }

        [OriginalGridPosition(Position = 3)]
        public string SettleAmountInDollars { get; set; }  //** Sairam added this    

        [OriginalGridPosition(Position = 4)]
        public int? SettleCount { get; set; }

        [OriginalGridPosition(Position = 7)]
        public string DeclinedAmountInDollars { get; set; }

        [OriginalGridPosition(Position = 8)]
        public int? DeclinedCount { get; set; }

        [OriginalGridPosition(Position = 9)]
        public string UnprocessedAmountInDollars { get; set; }

        [OriginalGridPosition(Position = 10)]
        public int? UnprocessedCount { get; set; }

        public string TransAmount_generic { get; set; }
        public string RefundAmount_generic { get; set; }
        public string SettleAmount_generic { get; set; }

        //** End of Display in Revenue Audit Summary Grid Columns

        //** Fetching Values for Trans, settle, Refund in CENT format
        public int? TransAmount { get; set; }
        public int? RefundAmount { get; set; }
        public int? SettleAmount { get; set; }

        public int? DeclinedAmount { get; set; }
        public int? UnprocessedAmount { get; set; }

        //** End of Cent values 


        public int areaID { get; set; }
        public int meterID { get; set; }
        public int? bayNumber { get; set; }
        public string transDateTime { get; set; }
        public string creditCardType { get; set; }
        public string timePaid { get; set; }
        public string hrsForTimePaid { get; set; }
        public string minForTimePaid { get; set; }
        public string amount { get; set; }
        public string acqReference { get; set; }
        public string status { get; set; }

        //Revenue AuditDetails
        public string RevenueAuditDetailsBatchID { get; set; }
        public string RevenueAuditDetailsBatchDate { get; set; }
        public string RevenueAuditDetailsBatchEndDate { get; set; }
        public string RevenueAuditDetailsDate_Display { get; set; }

        public DateTime? RevenueAuditDetailsStartDate { get; set; }
        public string RevenueAuditDetailsStartDate_Display { get; set; }
        public DateTime? RevenueAuditDetailsEndDate { get; set; }
        public string RevenueAuditDetailsEndDate_Display { get; set; }

        public RouteValueDictionary getRouteValues()
        {
            return new RouteValueDictionary(new
            {
                BatchDateIs = !String.IsNullOrEmpty(RevenueAuditDetailsBatchDate) ? RevenueAuditDetailsBatchDate : String.Empty,
                endBatchDateIs = !String.IsNullOrEmpty(RevenueAuditDetailsBatchEndDate) ? RevenueAuditDetailsBatchEndDate : String.Empty,
                transAmount = !String.IsNullOrEmpty(TransAmount_generic) ? TransAmount_generic : String.Empty,
                transCount = TransCount,
                refundAmount = !String.IsNullOrEmpty(RefundAmount_generic) ? RefundAmount_generic : String.Empty,
                refundCount = RefundCount,
                settleCount = !String.IsNullOrEmpty(SettleAmount_generic) ? SettleAmount_generic : String.Empty
            });
        }

    }
}
