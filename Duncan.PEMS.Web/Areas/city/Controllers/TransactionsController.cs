using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.CustomerTransactions;
using Duncan.PEMS.Business.Events;
using Duncan.PEMS.Business.Exports;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.Entities.Transactions;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class TransactionsController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        private const string defaultOrderBy = "DateTime desc";
        private const string RevAuditSumSpName = "sp_SettledBatch"; //** Sairam added this on Dec 2nd 2014 for Revenue Audit Summary

        #region Payment Transaction Inquiry

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTransactions([DataSourceRequest] DataSourceRequest request)
        {
            //get models
            List<CustomerTransactionModel> items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTransactions(request, defaultOrderBy);

            int total = 0;
            if (items.Any())
                total = items.First().Count;

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Batch Transaction Inquiry

        public ActionResult BatchIndex()
        {
            return View();
        }

        public ActionResult GetBatchLatency([DataSourceRequest] DataSourceRequest request)
        {
            //get models
            List<CustomerTransactionModel> items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBatchTransactions(request, defaultOrderBy, CurrentCity.Id);

            int total = 0;
            if (items.Any())
                total = items.First().Count;

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Sensor Transaction Inquiry

        public ActionResult SensorIndex()
        {
            return View();
        }

        public ActionResult GetSensorTransactions([DataSourceRequest] DataSourceRequest request)
        {
            //get models
            List<CustomerSensorTransactionModel> items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSensorTransactions(request, defaultOrderBy);

            int total = 0;
            if (items.Any())
                total = items.First().Count;
             

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
            //return new LargeJsonResult() { Data = items, MaxJsonLength = int.MaxValue };
        }

        public FileResult ExportToCsvSensorTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {
            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSensorTransactions(request, defaultOrderBy);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetSensorTransactions", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Sensor_Transaction_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }
        public FileResult ExportToPdfSensorTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {

            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSensorTransactions(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetSensorTransactions", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Sensor_Transaction_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }
        public FileResult ExportToExcelSensorTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {
            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSensorTransactions(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetSensorTransactions", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "Sensor_Transaction_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");
        }

        #endregion

        #region Real Time Occupancy

        public ActionResult SensorRTIndex()
        {
            return View();
        }

        public ActionResult GetRTSensorTransactions([DataSourceRequest] DataSourceRequest request)
        {
            //get models
            List<CustomerSensorTransactionModel> items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetRTSensorTransactions(request, defaultOrderBy);

            int total = 0;
            if (items.Any())
                total = items.First().Count;
                //total = items.Count;

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
            //return new LargeJsonResult() { Data = items, MaxJsonLength = int.MaxValue };
        }

        public FileResult ExportToCsvRTSensorTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {
            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetRTSensorTransactions(request, defaultOrderBy);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetRTSensorTransactions", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Sensor_Transaction_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }
        public FileResult ExportToPdfRTSensorTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {

            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetRTSensorTransactions(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetRTSensorTransactions", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Sensor_Transaction_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }
        public FileResult ExportToExcelRTSensorTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {
            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetRTSensorTransactions(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetRTSensorTransactions", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "Sensor_Transaction_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");
        }

        #endregion

        #region Details

        public ActionResult MeterDetails(int txId)
        {
            CustomerTransactionFactory factory = new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());

            var customerTransactionMeterModel = factory.GetMeterTransaction(txId, CurrentCity.Id);

            if (customerTransactionMeterModel == null)
            {
                ModelState.AddModelError("NoTransactionFound", "No transaction matching that criteria found or transaction does not belong to current client");
            }

            return View(customerTransactionMeterModel);
        }

        public ActionResult SensorDetails(int txId, int sensorPmtTxId)
        {
            CustomerTransactionFactory factory = new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());

            var customerTransactionSensorModel = factory.GetSensorTransaction(txId, sensorPmtTxId, CurrentCity.Id);

            if (customerTransactionSensorModel == null)
            {
                ModelState.AddModelError("NoTransactionFound", "No transaction matching that criteria found or transaction does not belong to current client");
            }

            return View(customerTransactionSensorModel);
        }

        #endregion

        #region Drop Down List Values

        public JsonResult GetFilterValues()
        {
            var eventFactory = new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());
            var transFactory = new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());

            // Execute the queries
            //cashbox and gateways are invalid for transactions, so remove the items with the correct metergroup
            //this is why we are using tans factory instead of event factory
            var assetTypes = transFactory.GetAssetTypeFilterItems(CurrentCity.Id);


            var timeTypes = eventFactory.GetTimeTypesFilterItems();
            var paymentStatusTypes = transFactory.GetPaymentStatusReleventFilterItems();
            //var transactionTypes = transFactory.GetTransactionTypesFilterItems();
            var transactionTypes = transFactory.GetTransactionTypesFilterMinSensorOtheritems();

       

            //customer specific card types
            var cardTypes = transFactory.GetCardTypes(CurrentCity.Id);

            // Concatenate the data into an anonymous typed object
            var filterValues = new { timeTypes, assetTypes, paymentStatusTypes, transactionTypes, cardTypes };

            // return as JSON
            return Json(filterValues, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSensorFilterValues()
        {
            var eventFactory = new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());
            var transFactory = new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());

            // Execute the queries
            //cashbox and gateways are invalid for transactions, so remove the items with the correct metergroup
            //this is why we are using tans factory instead of event factory
            var assetTypes = transFactory.GetAssetTypeFilterItems(CurrentCity.Id);


            var timeTypes = eventFactory.GetTimeTypesFilterItems();
            var paymentStatusTypes = transFactory.GetPaymentStatusReleventFilterItems();
            //var transactionTypes = transFactory.GetTransactionTypesFilterItems();
            var transactionTypes = transFactory.GetTransactionTypesFilterOnlySensor();



            //customer specific card types
            var cardTypes = transFactory.GetCardTypes(CurrentCity.Id);

            // Concatenate the data into an anonymous typed object
            var filterValues = new { timeTypes, assetTypes, paymentStatusTypes, transactionTypes, cardTypes };

            // return as JSON
            return Json(filterValues, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Exporting"

        public FileResult ExportToCsv([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt, string declTotalAmt, string declTotalCnt, string unprosTotalAmt, string unprosTotalCnt)
        {
            var items = GetExportData(request, startDate, endDate);

            var filters = new List<FilterDescriptor>();

            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters.Add(new FilterDescriptor { Member = "Aggregation", Value = "Value" });
            filters.Add(new FilterDescriptor { Member = "Total Refund Amount", Value = "$" + totalRefundAmount });
            filters.Add(new FilterDescriptor { Member = "Total Refund Count", Value = totalRefundCnt });
            filters.Add(new FilterDescriptor { Member = "Total Settle Amount", Value = "$" + totalSettleAmount });
            filters.Add(new FilterDescriptor { Member = "Total Settle Count", Value = totalSettleCnt });
            //filters.Add(new FilterDescriptor { Member = "Total Transaction Amount", Value = "$" + totalSettleAmount });
            //filters.Add(new FilterDescriptor { Member = "Total Transaction Count", Value = totalSettleCnt });

            filters.Add(new FilterDescriptor { Member = "Total Declined Amount", Value = "$" + declTotalAmt });
            filters.Add(new FilterDescriptor { Member = "Total Declined Count", Value = declTotalCnt });
            filters.Add(new FilterDescriptor { Member = "Total Unprocessed Amount", Value = "$" + unprosTotalAmt });
            filters.Add(new FilterDescriptor { Member = "Total Unprocessed Count", Value = unprosTotalCnt });

            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetRevenueAuditSummary", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Revenue_Audit_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        //public FileResult ExportToPdf([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        //{

        //    var items = GetExportData(request, startDate, endDate);
        //    var filters = new List<FilterDescriptor>();

        //    filters = GetFilterDescriptors(request.Filters, filters);
        //    filters.Add(new FilterDescriptor { Member = " ", Value = " " });
        //    filters.Add(new FilterDescriptor { Member = "Aggregation", Value = "Value" });
        //    filters.Add(new FilterDescriptor { Member = "Total Refund Amount", Value = totalRefundAmount });
        //    filters.Add(new FilterDescriptor { Member = "Total Refund Count", Value = totalRefundCnt });
        //    filters.Add(new FilterDescriptor { Member = "Total Settle Amount", Value = totalSettleAmount });
        //    filters.Add(new FilterDescriptor { Member = "Total Settle Count", Value = totalSettleCnt });
        //    filters.Add(new FilterDescriptor { Member = "Total Transaction Amount", Value = totalSettleAmount });
        //    filters.Add(new FilterDescriptor { Member = "Total Transaction Count", Value = totalSettleCnt });

        //    var itemToRemove = filters.FirstOrDefault(r => r.Member == "CustomerId");
        //    if (itemToRemove != null) filters.Remove(itemToRemove);
        //    var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetRevenueAuditSummary", CurrentCity.Id, filters, 1);

        //    // send the memory stream as File
        //    return File(output.ToArray(), "application/pdf", "Revenue_Audit_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        //}


        public FileResult ExportToPdf([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt, string declTotalAmt, string declTotalCnt, string unprosTotalAmt, string unprosTotalCnt)
        {

            var items = GetExportData(request, startDate, endDate);
            var filters = new List<FilterDescriptor>();

            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters.Add(new FilterDescriptor { Member = "Aggregation", Value = "Value" });
            filters.Add(new FilterDescriptor { Member = "Total Refund Amount", Value =  "$" + totalRefundAmount });
            filters.Add(new FilterDescriptor { Member = "Total Refund Count", Value = totalRefundCnt });
            filters.Add(new FilterDescriptor { Member = "Total Settle Amount", Value = "$" + totalSettleAmount });
            filters.Add(new FilterDescriptor { Member = "Total Settle Count", Value = totalSettleCnt });
            //filters.Add(new FilterDescriptor { Member = "Total Transaction Amount", Value = "$" + totalSettleAmount });
            //filters.Add(new FilterDescriptor { Member = "Total Transaction Count", Value = totalSettleCnt });

            filters.Add(new FilterDescriptor { Member = "Total Declined Amount", Value = "$" + declTotalAmt });
            filters.Add(new FilterDescriptor { Member = "Total Declined Count", Value = declTotalCnt });
            filters.Add(new FilterDescriptor { Member = "Total Unprocessed Amount", Value = "$" + unprosTotalAmt });
            filters.Add(new FilterDescriptor { Member = "Total Unprocessed Count", Value = unprosTotalCnt });

            var itemToRemove = filters.FirstOrDefault(r => r.Member == "CustomerId");
            if (itemToRemove != null) filters.Remove(itemToRemove);
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetRevenueAuditSummary", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Revenue_Audit_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private List<FilterDescriptor> GetFilterDescriptors(IList<IFilterDescriptor> filterDescriptors, List<FilterDescriptor> filters)
        {
            List<FilterDescriptor> descriptors = base.GetFilterDescriptors(filterDescriptors, filters);
            descriptors.RemoveAll(x => x.Member.ToLower() == "customerid");
            descriptors.RemoveAll(x => x.Member.ToLower() == "viewname");
            return descriptors;
        }

        #region payments Transaction
        public FileResult ExportToCsvPaymentTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {
            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTransactions(request, defaultOrderBy);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetTransactions", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Payment_Transaction_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToPdfPaymentTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {

            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTransactions(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetTransactions", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Payment_Transaction_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }
        public FileResult ExportToExcelPaymentTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {
            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTransactions(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetTransactions", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "Payment_Transaction_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");
        }
        #endregion

        #region Batch Latency Report
        public FileResult ExportToCsvBatchLatencyTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {
            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTransactions(request, defaultOrderBy);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetBatchLatency", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Batch_Latency_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToPdfBatchLatencyTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {

            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTransactions(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetBatchLatency", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Batch_Latency_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }
        public FileResult ExportToExcelBatchLatencyTrans([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt)
        {
            var items = (new CustomerTransactionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTransactions(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetBatchLatency", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "Batch_Latency_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");
        }
        #endregion

        #endregion

        #region Revenue Audit Summary

        /// <summary>
        /// Modified By: Sairam on Dec 12th 2014
        /// Description: This method is used to display the summary page for Revenue Audit Summary
        /// </summary>
        /// <returns></returns>
        public ActionResult RevenueAuditSummary()
        {
            RevenueAudit RevenueAuditDetails_model = new RevenueAudit();

            RevenueAuditDetails_model.gridTotal = 0;

            return View(RevenueAuditDetails_model);
        }


        /// <summary>
        /// Description: This action is used to provide detailed info about the batch date vice transactions happened
        /// Modified By: Sairam on Dec 12th 2014
        /// </summary>
        /// <param name="RevenueID"></param>
        /// <param name="Date"></param>
        /// <returns></returns>
        //public ActionResult RevenueAuditDetails(string BatchID, string BatchDateIs, string transAmount, string transCount, string refundAmount, string refundCount, string settleAmount, string settleCount)
        //{
        //    //** Here we are sending the selected row (i.e. batch date) from the grid and hence sending the row specific trans, settle, refund values as aggregation totals to Revenue audit details page

        //    RevenueAudit RevenueAuditDetails_model = new RevenueAudit();

        //    RevenueAuditDetails_model.RevenueAuditDetailsBatchID = BatchID;
        //    RevenueAuditDetails_model.RevenueAuditDetailsBatchDate = BatchDateIs;
        //    RevenueAuditDetails_model.TotalTransactionAmount = transAmount;
        //    RevenueAuditDetails_model.TotalTransactionCount = transCount;
        //    RevenueAuditDetails_model.TotalRefundAmount = refundAmount;
        //    RevenueAuditDetails_model.TotalRefundCount = refundCount;
        //    RevenueAuditDetails_model.TotalSettleAmount = settleAmount;
        //    RevenueAuditDetails_model.TotalSettleCount = settleCount;

        //    return View(RevenueAuditDetails_model);

        //}

        public ActionResult RevenueAuditDetails(string BatchID, string BatchDateIs, string transAmount, string transCount, string refundAmount, string refundCount, string settleAmount, string settleCount, string declineAmt, string declineCnt, string unprocessedAmt, string unprocessedCnt)
        {
            //** Here we are sending the selected row (i.e. batch date) from the grid and hence sending the row specific trans, settle, refund values as aggregation totals to Revenue audit details page

            RevenueAudit RevenueAuditDetails_model = new RevenueAudit();

            RevenueAuditDetails_model.RevenueAuditDetailsBatchID = BatchID;
            RevenueAuditDetails_model.RevenueAuditDetailsBatchDate = BatchDateIs;
            //RevenueAuditDetails_model.TotalTransactionAmount = transAmount;
            //RevenueAuditDetails_model.TotalTransactionCount = transCount;
            RevenueAuditDetails_model.TotalRefundAmount = refundAmount;
            RevenueAuditDetails_model.TotalRefundCount = refundCount;
            RevenueAuditDetails_model.TotalSettleAmount = settleAmount;
            RevenueAuditDetails_model.TotalSettleCount = settleCount;

            RevenueAuditDetails_model.TotalDeclinedAmount = declineAmt;
            RevenueAuditDetails_model.TotalDeclinedCount = declineCnt;
            RevenueAuditDetails_model.TotalUnprocessedAmount = unprocessedAmt;
            RevenueAuditDetails_model.TotalUnprocessedCount = unprocessedCnt;

            return View(RevenueAuditDetails_model);

            //declineAmt=330&declineCnt=33&unprocessedAmt=3310&unprocessedCnt=331

        }

        /// <summary>
        /// Description: This action is used to provide detailed info about the batch date vice transactions happened
        /// Modified By: Sairam on Dec 12th 2014
        /// </summary>
        /// <param name="BatchDateIs"></param>
        /// <param name="endBatchDateIs"></param>
        /// <param name="transAmount"></param>
        /// <param name="transCount"></param>
        /// <param name="refundAmount"></param>
        /// <param name="refundCount"></param>
        /// <param name="settleAmount"></param>
        /// <param name="settleCount"></param>
        /// <returns></returns>
        public ActionResult RevenueAuditDetails2(string BatchDateIs, string endBatchDateIs, string transAmount, string transCount, string refundAmount, string refundCount, string settleAmount, string settleCount)
        {
            RevenueAudit RevenueAuditDetails_model = new RevenueAudit();

            RevenueAuditDetails_model.RevenueAuditDetailsBatchDate = BatchDateIs;
            RevenueAuditDetails_model.RevenueAuditDetailsBatchEndDate = endBatchDateIs;
            RevenueAuditDetails_model.TotalTransactionAmount = transAmount;
            RevenueAuditDetails_model.TotalTransactionCount = transCount;
            RevenueAuditDetails_model.TotalRefundAmount = refundAmount;
            RevenueAuditDetails_model.TotalRefundCount = refundCount;
            RevenueAuditDetails_model.TotalSettleAmount = settleAmount;
            RevenueAuditDetails_model.TotalSettleCount = settleCount;

            return View(RevenueAuditDetails_model);

        }

        /// <summary>
        /// Description: This action is used to fetch Revenue Audit Details data for the given date range
        /// Modified By: Preetha, Sairam on Dec 12th 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult GetRevenueAuditDetails([DataSourceRequest] DataSourceRequest request, string batchIDIs, string batchDateIs)
        {
            //if (request.Sorts.Count > 0)
            //    SetSavedSortValues(request.Sorts, "RevenueAuditDetails");

            IEnumerable<RevenueAudit> RevenueAuditDetailsResult = Enumerable.Empty<RevenueAudit>();

            RevenueAuditDetailsResult = (new CustomerTransactionFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetRevenueAuditDetails(request, batchIDIs, batchDateIs, CurrentCity.Id);

            DataSourceResult finalResult = RevenueAuditDetailsResult.ToDataSourceResult(request);

            return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };


        }

        /// <summary>
        /// Description: This action is used to just invoke the details page view by using $.ajax url from summary page
        /// Modified By: Sairam on Dec 12th 2014
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetEmptyResult([DataSourceRequest] DataSourceRequest request)
        {

            return Content("");
        }

        /// <summary>
        /// Description: This action is used to provide detailed info about the batch date vice transactions happened
        /// Modified By: Sairam on Dec 12th 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="batchIDIs"></param>
        /// <param name="batchDateIs"></param>
        /// <returns></returns>
        public ActionResult GetRevenueAuditDetails_2([DataSourceRequest] DataSourceRequest request, string batchIDIs, string batchDateIs)
        {
            //if (request.Sorts.Count > 0)
            //    SetSavedSortValues(request.Sorts, "RevenueAuditDetails");

            IEnumerable<RevenueAudit> RevenueAuditDetailsResult = Enumerable.Empty<RevenueAudit>();

            RevenueAuditDetailsResult = (new CustomerTransactionFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetRevenueAuditDetails_2(request, batchIDIs, batchDateIs, CurrentCity.Id);

            DataSourceResult finalResult = RevenueAuditDetailsResult.ToDataSourceResult(request);

            return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };


        }

        /// <summary>
        /// Description: This action is used to fetch Revenue Audit Summary data for the given date range
        /// Modified By: Preetha, Sairam on Dec 12th 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult GetRevenueAuditSummary([DataSourceRequest] DataSourceRequest request, string startDate, string endDate)
        {

            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "RevenueAuditSummary");

            var spName = RevAuditSumSpName;

            // IEnumerable<RevenueAudit> RevenueAuditSummaryResult = Enumerable.Empty<RevenueAudit>();
            int total = 0;
            var RevenueAuditSummaryResult_raw = (new CustomerTransactionFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetRevenueAuditsummary(request, CurrentCity.Id, out total);


            var RevenueAuditSummaryResult = from r in RevenueAuditSummaryResult_raw
                                            let totalSettleAmtInDollars = (double.IsNaN((double)RevenueAuditSummaryResult_raw.Sum(setamt => setamt.SettleAmount)) ? 0 : (decimal)RevenueAuditSummaryResult_raw.Sum(setamt => setamt.SettleAmount) / (decimal)100)
                                            let totalSettleCnt = (double.IsNaN((double)RevenueAuditSummaryResult_raw.Sum(setCnt => setCnt.SettleCount)) ? 0 : (decimal)RevenueAuditSummaryResult_raw.Sum(setCnt => setCnt.SettleCount))

                                            let totalRefundAmtInDollars = (double.IsNaN((double)RevenueAuditSummaryResult_raw.Sum(refAmt => refAmt.RefundAmount)) ? 0 : (decimal)RevenueAuditSummaryResult_raw.Sum(refAmt => refAmt.RefundAmount) / (decimal)100)
                                            let totalRefundCnt = (double.IsNaN((double)RevenueAuditSummaryResult_raw.Sum(refCnt => refCnt.RefundCount)) ? 0 : (decimal)RevenueAuditSummaryResult_raw.Sum(refCnt => refCnt.RefundCount))

                                            let totalDeclinedAmtInDollars = (double.IsNaN((double)RevenueAuditSummaryResult_raw.Sum(refAmt => refAmt.DeclinedAmount)) ? 0 : (decimal)RevenueAuditSummaryResult_raw.Sum(refAmt => refAmt.DeclinedAmount) / (decimal)100)
                                            let totalDeclinedCnt = (double.IsNaN((double)RevenueAuditSummaryResult_raw.Sum(refCnt => refCnt.DeclinedCount)) ? 0 : (decimal)RevenueAuditSummaryResult_raw.Sum(refCnt => refCnt.DeclinedCount))

                                            let totalUnprocessedAmtInDollars = (double.IsNaN((double)RevenueAuditSummaryResult_raw.Sum(refAmt => refAmt.UnprocessedAmount)) ? 0 : (decimal)RevenueAuditSummaryResult_raw.Sum(refAmt => refAmt.UnprocessedAmount) / (decimal)100)
                                            let totalUnprocessedCnt = (double.IsNaN((double)RevenueAuditSummaryResult_raw.Sum(refCnt => refCnt.UnprocessedCount)) ? 0 : (decimal)RevenueAuditSummaryResult_raw.Sum(refCnt => refCnt.UnprocessedCount))

                                            select new RevenueAudit
                                            {
                                                RevenueAuditDate_final = Convert.ToString(r.RevenueAuditDate),
                                                RevenueBatchID = r.RevenueBatchID,

                                                TransAmount = r.TransAmount,
                                                TransCount = r.TransCount,


                                                //** For Grid Column
                                                TransAmountInDollars = "$" + ((Decimal)r.TransAmount / (Decimal)100).ToString("0.00"),

                                                //** For Aggregation
                                                TotalTransactionAmount = totalSettleAmtInDollars.ToString("0.00"),
                                                TotalTransactionCount = totalSettleCnt.ToString(),


                                                RefundAmount = r.RefundAmount,
                                                RefundCount = r.RefundCount,

                                                //** For Grid Column
                                                RefundAmountInDollars = "$" + ((Decimal)r.RefundAmount / (Decimal)100).ToString("0.00"),

                                                //** For Aggregation
                                                TotalRefundAmount = totalRefundAmtInDollars.ToString("0.00"),
                                                TotalRefundCount = totalRefundCnt.ToString(),


                                                SettleAmount = r.SettleAmount,
                                                SettleCount = r.SettleCount,

                                                //** For Grid Column
                                                SettleAmountInDollars = "$" + ((Decimal)r.SettleAmount / (Decimal)100).ToString("0.00"),

                                                //** For Aggregation
                                                TotalSettleAmount = totalSettleAmtInDollars.ToString("0.00"),
                                                TotalSettleCount = totalSettleCnt.ToString(),


                                                //**Declined Amount
                                                DeclinedAmountInDollars = "$" + ((Decimal)r.DeclinedAmount / (Decimal)100).ToString("0.00"),
                                                DeclinedAmount = r.DeclinedAmount,
                                                DeclinedCount = r.DeclinedCount,

                                                TotalDeclinedAmount = totalDeclinedAmtInDollars.ToString("0.00"),
                                                TotalDeclinedCount = totalDeclinedCnt.ToString(),

                                                //**Unprocessed value
                                                UnprocessedAmountInDollars = "$" + ((Decimal)r.UnprocessedAmount / (Decimal)100).ToString("0.00"),
                                                UnprocessedAmount = r.UnprocessedAmount,
                                                UnprocessedCount = r.UnprocessedCount,

                                                TotalUnprocessedAmount = totalUnprocessedAmtInDollars.ToString("0.00"),
                                                TotalUnprocessedCount = totalUnprocessedCnt.ToString()

                                            };

            RevenueAuditSummaryResult = RevenueAuditSummaryResult.ApplyFiltering(request.Filters);
            total = RevenueAuditSummaryResult.Count();

            RevenueAuditSummaryResult = RevenueAuditSummaryResult.ApplySorting(request.Groups, request.Sorts);
            RevenueAuditSummaryResult = RevenueAuditSummaryResult.ApplyPaging(request.Page, request.PageSize);

            DataSourceResult result = new DataSourceResult()
            {
                Data = RevenueAuditSummaryResult.ToList(),
                Total = total,
            };

            return new LargeJsonResult() { Data = result, MaxJsonLength = int.MaxValue };
       }


        public ActionResult GetRevenueAuditSummary_Export([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string totalRefundAmount, string totalRefundCnt, string totalSettleAmount, string totalSettleCnt, string declTotalAmt, string declTotalCnt, string unprosTotalAmt, string unprosTotalCnt)
        {

            var data = GetExportData(request, startDate, endDate);
            var filters = new List<FilterDescriptor>();

            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters.Add(new FilterDescriptor { Member = "Aggregation", Value = "Value" });
            filters.Add(new FilterDescriptor { Member = "Total Refund Amount", Value = "$" + totalRefundAmount });
            filters.Add(new FilterDescriptor { Member = "Total Refund Count", Value = totalRefundCnt });
            filters.Add(new FilterDescriptor { Member = "Total Settle Amount", Value = "$" + totalSettleAmount });
            filters.Add(new FilterDescriptor { Member = "Total Settle Count", Value = totalSettleCnt });
            //filters.Add(new FilterDescriptor { Member = "Total Transaction Amount", Value = "$" + totalSettleAmount });
            //filters.Add(new FilterDescriptor { Member = "Total Transaction Count", Value = totalSettleCnt });

            filters.Add(new FilterDescriptor { Member = "Total Declined Amount", Value = "$" + declTotalAmt });
            filters.Add(new FilterDescriptor { Member = "Total Declined Count", Value = declTotalCnt });
            filters.Add(new FilterDescriptor { Member = "Total Unprocessed Amount", Value = "$" + unprosTotalAmt });
            filters.Add(new FilterDescriptor { Member = "Total Unprocessed Count", Value = unprosTotalCnt });

            var output = (new ExportFactory()).GetExcelFileMemoryStream(data, CurrentController, "GetRevenueAuditSummary", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "Revenue_Audit_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user

        }

        private IEnumerable<RevenueAudit> GetExportData(DataSourceRequest request, string startDate, string endDate)
        {
            int total = 0;
            var spName = RevAuditSumSpName;


            var items = (new CustomerTransactionFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetRevenueAuditsummary(request, CurrentCity.Id, out total);

            var RevenueAuditSummaryResult = from r in items
                                            let totalSettleAmtInDollars = (double.IsNaN((double)items.Sum(setamt => setamt.SettleAmount)) ? 0 : (decimal)items.Sum(setamt => setamt.SettleAmount) / (decimal)100)
                                            let totalSettleCnt = (double.IsNaN((double)items.Sum(setCnt => setCnt.SettleCount)) ? 0 : (decimal)items.Sum(setCnt => setCnt.SettleCount))

                                            let totalRefundAmtInDollars = (double.IsNaN((double)items.Sum(refAmt => refAmt.RefundAmount)) ? 0 : (decimal)items.Sum(refAmt => refAmt.RefundAmount) / (decimal)100)
                                            let totalRefundCnt = (double.IsNaN((double)items.Sum(refCnt => refCnt.RefundCount)) ? 0 : (decimal)items.Sum(refCnt => refCnt.RefundCount))

                                            let totalDeclinedAmtInDollars = (double.IsNaN((double)items.Sum(refAmt => refAmt.DeclinedAmount)) ? 0 : (decimal)items.Sum(refAmt => refAmt.DeclinedAmount) / (decimal)100)
                                            let totalDeclinedCnt = (double.IsNaN((double)items.Sum(refCnt => refCnt.DeclinedCount)) ? 0 : (decimal)items.Sum(refCnt => refCnt.DeclinedCount))

                                            let totalUnprocessedAmtInDollars = (double.IsNaN((double)items.Sum(refAmt => refAmt.UnprocessedAmount)) ? 0 : (decimal)items.Sum(refAmt => refAmt.UnprocessedAmount) / (decimal)100)
                                            let totalUnprocessedCnt = (double.IsNaN((double)items.Sum(refCnt => refCnt.UnprocessedCount)) ? 0 : (decimal)items.Sum(refCnt => refCnt.UnprocessedCount))

                                            select new RevenueAudit
                                            {
                                                RevenueAuditDate_final = Convert.ToString(r.RevenueAuditDate),
                                                RevenueBatchID = r.RevenueBatchID,

                                                TransAmount = r.TransAmount,
                                                TransCount = r.TransCount,

                                                gridTotal = total,

                                                //** For Grid Column
                                                TransAmountInDollars = "$" + ((Decimal)r.TransAmount / (Decimal)100).ToString("0.00"),

                                                //** For Aggregation
                                                TotalTransactionAmount = totalSettleAmtInDollars.ToString("0.00"),
                                                TotalTransactionCount = totalSettleCnt.ToString(),


                                                RefundAmount = r.RefundAmount,
                                                RefundCount = r.RefundCount,

                                                //** For Grid Column
                                                RefundAmountInDollars = "$" + ((Decimal)r.RefundAmount / (Decimal)100).ToString("0.00"),

                                                //** For Aggregation
                                                TotalRefundAmount = totalRefundAmtInDollars.ToString("0.00"),
                                                TotalRefundCount = totalRefundCnt.ToString(),


                                                SettleAmount = r.SettleAmount,
                                                SettleCount = r.SettleCount,

                                                //** For Grid Column
                                                SettleAmountInDollars = "$" + ((Decimal)r.SettleAmount / (Decimal)100).ToString("0.00"),

                                                //** For Aggregation
                                                TotalSettleAmount = totalSettleAmtInDollars.ToString("0.00"),
                                                TotalSettleCount = totalSettleCnt.ToString(),

                                                //**Declined Amount
                                                DeclinedAmountInDollars = "$" + ((Decimal)r.DeclinedAmount / (Decimal)100).ToString("0.00"),
                                                DeclinedAmount = r.DeclinedAmount,
                                                DeclinedCount = r.DeclinedCount,

                                                TotalDeclinedAmount = totalDeclinedAmtInDollars.ToString("0.00"),
                                                TotalDeclinedCount = totalDeclinedCnt.ToString(),

                                                //**Unprocessed value
                                                UnprocessedAmountInDollars = "$" + ((Decimal)r.UnprocessedAmount / (Decimal)100).ToString("0.00"),
                                                UnprocessedAmount = r.UnprocessedAmount,
                                                UnprocessedCount = r.UnprocessedCount,

                                                TotalUnprocessedAmount = totalUnprocessedAmtInDollars.ToString("0.00"),
                                                TotalUnprocessedCount = totalUnprocessedCnt.ToString()
                                            };

            RevenueAuditSummaryResult = RevenueAuditSummaryResult.ApplyFiltering(request.Filters);
            total = RevenueAuditSummaryResult.Count();

            RevenueAuditSummaryResult = RevenueAuditSummaryResult.ApplySorting(request.Groups, request.Sorts);
            //RevenueAuditSummaryResult = RevenueAuditSummaryResult.ApplyPaging(request.Page, request.PageSize);


            return RevenueAuditSummaryResult;
        }


        #endregion
    }
}