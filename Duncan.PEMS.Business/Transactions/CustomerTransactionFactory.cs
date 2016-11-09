/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________        __________________________________________________________________________________________________
 * 02/06/2014       R Howard                    JIRA: DPTXPEMS-225  Added CustomerId to where clause for any selects from PEMS Areas table
 * 02/06/2014       Sergey Ostrerov             JIRA: DPTXPEMS-213  TimePaid formatted 00:00:00 
 * *****************************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Entities.Transactions;
using Duncan.PEMS.Utilities;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using NLog;

namespace Duncan.PEMS.Business.CustomerTransactions
{
    /// <summary>
    /// The <see cref="Duncan.PEMS.Business.CustomerTransactions"/> namespace contains classes for managing transactions.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    public class CustomerTransactionFactory : BaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Factory constructor taking a connection string name.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        public CustomerTransactionFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        /// <summary>
        /// Gets a list of transactions based on a request and any associated filters.
        /// </summary>
        public List<CustomerTransactionModel> GetTransactions([DataSourceRequest] DataSourceRequest request, string defaultOrderBy)
        {
            // If filtered by first 6/last 4 of CC, have to update filter value with cc hash
            ParseCreditCardFilter(request.Filters);

            string paramValues;
            //add the view specific name
            request.Filters.Add(new FilterDescriptor { Member = "viewName", Value = "pv_CustomerTransactions", Operator = FilterOperator.IsEqualTo });
            SqlParameter[] spParams = GetSpParams(request, defaultOrderBy, out paramValues);
            IEnumerable<CustomerTransactionModel> items = PemsEntities.Database.SqlQuery<CustomerTransactionModel>("sp_GetCustomerTransactions " + paramValues, spParams);

            List<CustomerTransactionModel> transactionModels = items.ToList();
            // Format the currency field (better globalization support on server)
            foreach (CustomerTransactionModel transaction in transactionModels)
            {
                transaction.AmountPaidDisplay = FormatHelper.FormatCurrency(transaction.AmountPaid);
                transaction.TimePaidDisplay = FormatHelper.FormatTimeFromSeconds(transaction.TimePaid);
            }

            return transactionModels;
        }

        /// <summary>
        /// Find the filter descriptor for "CardNumHash", which will be first 6 + last 4
        /// of CC#, and replace the value with the hash using an algorithm provided
        /// by Duncan.
        /// </summary>
        private void ParseCreditCardFilter(IList<IFilterDescriptor> filterDescriptors)
        {
            foreach (IFilterDescriptor filter in filterDescriptors)
            {
                if (filter is CompositeFilterDescriptor)
                    ParseCreditCardFilter(((CompositeFilterDescriptor)filter).FilterDescriptors);
                else
                {
                    FilterDescriptor filterDescriptor = (FilterDescriptor)filter;
                    if (filterDescriptor.Member == "CardNumHash" && !String.IsNullOrEmpty(filterDescriptor.Value.ToString()))
                    {
                        string subString = filterDescriptor.Value.ToString();

                        filterDescriptor.Value = GetSHA1Hash(subString);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// See document titled 'Ripnet Credit Card Hashing Algorithm' for details
        /// </summary>
        private static string GetSHA1Hash(string ccd)
        {
            // pad with 'F' to make 16 digits
            while (ccd.Length < 16)
            {
                ccd += "F";
            }

            byte[] nibbleRepresentation = Enumerable.Range(0, ccd.Length)
                                                    .Where(x => x % 2 == 0)
                                                    .Select(x => Convert.ToByte(ccd.Substring(x, 2), 16))
                                                    .ToArray();

            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(nibbleRepresentation);

            string digest = BitConverter.ToString(result).Replace("-", "").ToLower();
            return digest;
        }

        /// <summary>
        /// Get the details for a sensor transaction
        /// </summary>
        public CustomerTransactionSensorModel GetSensorTransaction(int transactionId, int sensorPmtTxId, int customerId)
        {
            var tx = PemsEntities.SensorPaymentTransactions.FirstOrDefault(stx => stx.SensorPaymentTransactionID == sensorPmtTxId && stx.ParkingSpace.CustomerID == customerId);
            if (tx != null)
            {
                var model = new CustomerTransactionSensorModel
                {
                    TransactionId = tx.LastTxID,
                    SensorId = tx.SensorId,
                    SensorName = tx.Sensor != null ? tx.Sensor.SensorName : "-",
                    GatewayId = tx.GatewayId,
                    GatewayName = tx.Gateway != null ? tx.Gateway.Description : "-",
                    ArrivalDateTime = tx.ArrivalTime,
                    DepartureDateTime = tx.DepartureTime,
                    ZeroOutTime = tx.ZeroOutTime,
                    Peak = tx.TimeType4.HasValue,
                    Latitude = tx.ParkingSpace.Latitude,
                    Longitude = tx.ParkingSpace.Longitude,
                    TotalNumberOfPayments = tx.TotalNumberOfPayment,
                    TotalAmountInCent = tx.TotalAmountInCent,
                    TotalTimePaidMinutes = tx.TotalTimePaidMinute,
                    TotalOccupiedMinutes = tx.TotalOccupiedMinute,
                    RemaingPaidTimeMinutes = tx.RemaingPaidTimeMinute,
                    FreeParkingTimeMinutes = tx.FreeParkingMinute,
                    GracePeriodMinutes = tx.GracePeriodMinute,
                    ViolationMinutes = tx.ViolationMinute,
                    OperationalStatus = tx.OperationalStatu != null ? tx.OperationalStatu.OperationalStatusDesc : "-",
                    NonComplianceStatus = tx.NonCompliantStatu != null ? tx.NonCompliantStatu.NonCompliantStatusDesc : "-",
                    OccupancyStatus = tx.OccupancyStatu != null ? tx.OccupancyStatu.StatusDesc : "-",
                    DiscountSchemaUsed = tx.DiscountSchema.HasValue,
                    FirstTxPaymentTime = tx.FirstTxPaymentTime,
                    FirstTxStartTime = tx.FirstTxStartTime,
                    FirstTxExpiryTime = tx.FirstTxExpiryTime,
                    FirstTxAmountInCent = tx.FirstTxAmountInCent,
                    FirstTxTimePaidMinute = tx.FirstTxTimePaidMinute,
                    LastTxPaymentTime = tx.LastTxPaymentTime,
                    LastTxExpiryTime = tx.LastTxExpiryTime,
                    LastTxAmountInCent = tx.LastTxAmountInCent,
                    LastTxTimePaidMinute = tx.LastTxTimePaidMinute,
                    //might have to add last tx start time - duncan to fix this as well.
                };

                if (tx.ParkingSpace.Meter != null)
                {
                    model.Street = tx.ParkingSpace.Meter.Location;
                    model.MeterId = tx.ParkingSpace.Meter.MeterId;
                    model.MeterName = tx.ParkingSpace.Meter.MeterName;

                    if (tx.ParkingSpace.Meter.DemandZone1 != null)
                    {
                        model.DemandType = tx.ParkingSpace.Meter.DemandZone1.DemandZoneDesc;
                    }

                    var meterMap = PemsEntities.MeterMaps.FirstOrDefault(mm => mm.MeterId == tx.ParkingSpace.MeterId
                                                                                && mm.Customerid == tx.ParkingSpace.CustomerID
                                                                                && mm.Areaid == tx.ParkingSpace.Meter.AreaID);

                    if (meterMap != null)
                    {
                        Area area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == meterMap.AreaId2.Value);
                        if (area != null)
                            model.Area = area.AreaName;
                        model.Suburb = meterMap.CustomGroup11 == null ? "" : meterMap.CustomGroup11.DisplayName;
                        model.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName;
                    }
                }

                if (tx.FirstTxPaymentMethod != null)
                {
                    TransactionType transactionType = PemsEntities.TransactionTypes.FirstOrDefault(x => x.TransactionTypeId == tx.FirstTxPaymentMethod);
                    if (transactionType != null) model.FirstTxPaymentMethod = transactionType.TransactionTypeDesc;
                }

                if (tx.LastTxPaymentMethod != null)
                {
                    TransactionType transactionType = PemsEntities.TransactionTypes.FirstOrDefault(x => x.TransactionTypeId == tx.LastTxPaymentMethod);
                    if (transactionType != null)
                        model.LastTxPaymentMethod = transactionType.TransactionTypeDesc;
                }

                var transaction = PemsEntities.Transactions.FirstOrDefault(t => t.TransactionsID == transactionId && t.CustomerID == customerId);
                if (transaction != null)
                {
                    model.TransactionDateTime = transaction.TransDateTime;
                }
                return model;
            }

            return new CustomerTransactionSensorModel();
        }

        /// <summary>
        /// Gets the details for a meter transaction
        /// </summary>
        public CustomerTransactionMeterModel GetMeterTransaction(int transactionId, int customerId)
        {
            Transaction trx = PemsEntities.Transactions.FirstOrDefault(t => t.TransactionsID == transactionId && t.CustomerID == customerId);
            if (trx == null)
            {
                return null;
            }

            // Create the model and fill in properties from the Transactions table
            var model = new CustomerTransactionMeterModel
            {
                MeterId = trx.MeterID,
                BayNumber = trx.BayNumber,
                TransactionId = trx.TransactionsID,
                TransactionType = trx.TransactionType,
                TransactionTypeDesc = trx.TransactionType1 != null ? trx.TransactionType1.TransactionTypeDesc : null,
                TransactionDate = trx.TransDateTime,
                Peak = trx.TimeType4 != null, // dbo.TimeType - Peak maps to TimeType4
                TimePaidInSeconds = trx.TimePaid,
                TimePaidInMinutes = trx.TimePaid,
                AmountInCents = trx.AmountInCents,
                ExpirationTime = trx.ExpiryTime,
                PrePayUsed = trx.PrepayUsed,
                DiscountSchemeUsed = trx.DiscountSchemeId != null,
                FreeMinutesUsed = trx.FreeParkingUsed
            };

            // Fill in properties from misc tables
            TransactionStatu transactionStatus = PemsEntities.TransactionStatus.FirstOrDefault(ts => ts.StatusID == trx.TransactionStatus);
            if (transactionStatus != null)
                model.TransactionStatus = transactionStatus.Description;

            // Get details from MeterMaps table
            MeterMap mmap = PemsEntities.MeterMaps.FirstOrDefault(mm => mm.Customerid == trx.CustomerID &&
                                                                         mm.MeterId == trx.MeterID &&
                                                                         mm.Areaid == trx.AreaID);
            if (mmap != null)
            {
                if (mmap.CustomGroup11 != null)
                    model.Suburb = mmap.CustomGroup11.DisplayName;

                if (mmap.Zone != null)
                    model.Zone = mmap.Zone.ZoneName;

                if (mmap.AreaId2.HasValue)
                    model.Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mmap.AreaId2.Value) == null
                        ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mmap.AreaId2.Value).AreaName;

                model.SensorId = mmap.SensorID;
            }

            // Get details from Meter table
            Meter meter = PemsEntities.Meters.FirstOrDefault(m => m.MeterId == trx.MeterID &&
                                                                   m.CustomerID == trx.CustomerID &&
                                                                   m.AreaID == trx.AreaID);
            if (meter != null)
            {
                model.MeterName = meter.MeterName;
                model.Longitude = meter.Longitude;
                model.Latitude = meter.Latitude;
                model.Street = meter.Location;
                if (meter.DemandZone1 != null)
                    model.DemandZone = meter.DemandZone1.DemandZoneDesc;
            }

            // Get transaction type specific details
            switch (trx.TransactionType) // [dbo].[TransactionType]
            {
                case 1: // 'Credit Card'
                    GetCreditCardTxDetails(model, trx);
                    break;

                case 2: // 'Smart Card'
                    GetSmartCardTxDetails(trx, model);
                    break;

                case 5: // 'Pay By Phone'
                    GetPayByCellTxDetails(trx, model);
                    break;

                case 3: // 'Cash'
                    GetCashTxDetails(customerId, trx, model);
                    break;

                case 4: // 'Technician Credit'
                    if (trx.OriginalTxId != null)
                    {
                        TechCreditEvent techcredit = PemsEntities.TechCreditEvents.FirstOrDefault(tc => tc.EventId == trx.OriginalTxId &&
                                                                                                         tc.CustomerId == trx.CustomerID &&
                                                                                                         tc.MeterId == trx.MeterID &&
                                                                                                         tc.AreaId == trx.AreaID);
                        if (techcredit != null)
                        {
                            model.UserId = techcredit.TechnicianKeyID;
                            if (techcredit.TechnicianKeyID.HasValue)
                            {
                                //go get the tech from technician details table and display the name here
                                var techDetails =
                                    PemsEntities.TechnicianDetails.FirstOrDefault(
                                        x => x.TechnicianKeyID == techcredit.TechnicianKeyID);
                                if (techDetails != null)
                                    model.UserName = techDetails.Name;
                            }
                            model.Note = techcredit.Note;
                        }
                    }
                    break;

                case 11: // 'Remote Reset'
                    if (trx.OriginalTxId != null)
                    {
                        MeterPushSchedule meterPushSchedule = PemsEntities.MeterPushSchedules.FirstOrDefault(mps => mps.PamPushId == trx.OriginalTxId);
                        if (meterPushSchedule != null)
                        {
                            model.UserId = meterPushSchedule.UserId;

                        }
                    }
                    break;
            }

            return model;
        }

        /// <summary>
        /// Gets details of a Pay By Cell transaction
        /// </summary>
        private void GetPayByCellTxDetails(Transaction trx, CustomerTransactionMeterModel model)
        {
            var mPark = PemsEntities.TransactionsMParks.FirstOrDefault(tmp => tmp.CustomerID == trx.CustomerID &&
                                                                               tmp.AreaID == trx.AreaID &&
                                                                               tmp.MeterId == trx.MeterID &&
                                                                               tmp.TransDateTime == trx.TransDateTime);
            if (mPark != null)
            {
                model.ReconDate = mPark.ReconDateTime;
                model.ReconFileId = mPark.ReconFileID;
            }
        }

        /// <summary>
        /// Gets details of a Smart Card transaction
        /// </summary>
        private void GetSmartCardTxDetails(Transaction trx, CustomerTransactionMeterModel model)
        {
            var smartCard = PemsEntities.TransactionsSmartCards.FirstOrDefault(tsc => tsc.CustomerId == trx.CustomerID &&
                                                                                       tsc.AreaId == trx.AreaID &&
                                                                                       tsc.MeterId == trx.MeterID &&
                                                                                       tsc.TransDateTime == trx.TransDateTime);
            if (smartCard != null)
            {
                model.SmartCardAcquirerTransRef = smartCard.AcquirerTransReference;
                model.SmartCardId = smartCard.TransactionsSmartCardId;
                model.SmartCardMeterTransRef = smartCard.MeterTransReference;
                model.SmartCardSerialNo = smartCard.SerialNo;
            }
        }

        /// <summary>
        /// Gets details of a Credit Card transaction
        /// </summary>
        private void GetCreditCardTxDetails(CustomerTransactionMeterModel model, Transaction trx)
        {
            model.ReceiptNumber = trx.ReceiptNo;
            if (trx.CreditCardType1 != null)
            {
                model.CardType = trx.CreditCardType1.Name;
            }

            // TransactionsCreditCards table
            var cctrx = PemsEntities.TransactionsCreditCards.FirstOrDefault(tcc => tcc.CustomerId == trx.CustomerID &&
                                                                                    tcc.AreaId == trx.AreaID &&
                                                                                    tcc.MeterId == trx.MeterID &&
                                                                                    tcc.ReceiptNo == trx.ReceiptNo &&
                                                                                    trx.TransDateTime == tcc.TransDateTime);
            if (cctrx != null)
            {
                model.BatchId = cctrx.BatchId;
                model.CCLast4 = cctrx.Last4;
            }

            // Audit table
            if (trx.OriginalTxId != null)
            {
                List<AcquirerResponse> acquirerResponses = new List<AcquirerResponse>();
                List<TransactionsAudit> transactionAudits = PemsEntities.TransactionsAudits.Where(ta => ta.TransactionID == trx.OriginalTxId).ToList();
                foreach (var audit in transactionAudits)
                {
                    var transactionsAcquirerResps = PemsEntities.TransactionsAcquirerResps.Where(tr => tr.TransAuditID == audit.TransAuditId);
                    foreach (var acquirerResp in transactionsAcquirerResps)
                    {
                        acquirerResponses.Add(new AcquirerResponse()
                        {
                            AuditDate = audit.TransAuditDate,
                            TransactionAuditId = audit.TransAuditId,
                            ResponseCode = acquirerResp.AcquirerResponseCode,
                            ResponseDetail = acquirerResp.AcquirerResponseDetail,
                            ResponseTransRef = acquirerResp.AcquirerTransactionRef
                        });
                    }
                }

                model.AcquirerResponses = acquirerResponses.OrderBy(x => x.AuditDate).ToList();
            }
        }

        /// <summary>
        /// Gets details of a Cash transaction
        /// </summary>
        private void GetCashTxDetails(int customerId, Transaction trx, CustomerTransactionMeterModel model)
        {
            // Get the numbers of each coin type
            var transactionsCash = PemsEntities.TransactionsCashes.FirstOrDefault(tc => tc.CustomerId == trx.CustomerID &&
                                                                                         tc.AreaId == trx.AreaID &&
                                                                                         tc.MeterId == trx.MeterID &&
                                                                                         tc.TransDateTime == trx.TransDateTime);
            if (transactionsCash != null)
            {
                int? coinCent1 = transactionsCash.CoinCent1;
                int? coinCent5 = transactionsCash.CoinCent5;
                int? coinCent10 = transactionsCash.CoinCent10;
                int? coinCent20 = transactionsCash.CoinCent20;
                int? coinCent25 = transactionsCash.CoinCent25;
                int? coinCent50 = transactionsCash.CoinCent50;
                int? coinDollar1 = transactionsCash.CoinDollar1;
                int? coinDollar2 = transactionsCash.CoinDollar2;

                // get the customer-specific coin denominations
                var coinDenominations = from cd in PemsEntities.CoinDenominations
                                        join cdc in PemsEntities.CoinDenominationCustomers on cd.CoinDenominationId equals cdc.CoinDenominationId
                                        where cdc.CustomerId == customerId
                                        select new
                                        {
                                            cd.TransactionsCashMap, // maps to column name in TransactionsCash table
                                            cd.CoinValue, // amount in cents
                                            cdc.CoinName // display name
                                        };

                // Iterate over the coin denominations and, if there is a matching value in the transaction table, calculate values
                foreach (var coinDenomination in coinDenominations)
                {
                    switch (coinDenomination.TransactionsCashMap)
                    {
                        case "CoinCent1":
                            if (coinCent1.HasValue)
                            {
                                model.CoinType1Description = coinDenomination.CoinName;
                                model.CoinType1Amount = coinDenomination.CoinValue * coinCent1.Value;
                                model.CoinType1Count = coinCent1.Value;
                            }
                            break;
                        case "CoinCent5":
                            if (coinCent5.HasValue)
                            {
                                model.CoinType2Description = coinDenomination.CoinName;
                                model.CoinType2Amount = coinDenomination.CoinValue * coinCent5.Value;
                                model.CoinType2Count = coinCent5.Value;

                            }
                            break;
                        case "CoinCent10":
                            if (coinCent10.HasValue)
                            {
                                model.CoinType3Description = coinDenomination.CoinName;
                                model.CoinType3Amount = coinDenomination.CoinValue * coinCent10.Value;
                                model.CoinType3Count = coinCent10.Value;

                            }
                            break;
                        case "CoinCent20":
                            if (coinCent20.HasValue)
                            {
                                model.CoinType4Description = coinDenomination.CoinName;
                                model.CoinType4Amount = coinDenomination.CoinValue * coinCent20.Value;
                                model.CoinType4Count = coinCent20.Value;

                            }
                            break;
                        case "CoinCent25":
                            if (coinCent25.HasValue)
                            {
                                model.CoinType5Description = coinDenomination.CoinName;
                                model.CoinType5Amount = coinDenomination.CoinValue * coinCent25.Value;
                                model.CoinType5Count = coinCent25.Value;

                            }
                            break;
                        case "CoinCent50":
                            if (coinCent50.HasValue)
                            {
                                model.CoinType6Description = coinDenomination.CoinName;
                                model.CoinType6Amount = coinDenomination.CoinValue * coinCent50.Value;
                                model.CoinType6Count = coinCent50.Value;

                            }
                            break;
                        case "CoinDollar1":
                            if (coinDollar1.HasValue)
                            {
                                model.CoinType7Description = coinDenomination.CoinName;
                                model.CoinType7Amount = coinDenomination.CoinValue * coinDollar1.Value;
                                model.CoinType7Count = coinDollar1.Value;

                            }
                            break;
                        case "CoinDollar2":
                            if (coinDollar2.HasValue)
                            {
                                model.CoinType8Description = coinDenomination.CoinName;
                                model.CoinType8Amount = coinDenomination.CoinValue * coinDollar2.Value;
                                model.CoinType8Count = coinDollar2.Value;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of dropdown options that represent customer specific card types
        /// </summary>
        public List<StringIdTextDDLModel> GetCardTypes(int customerID)
        {
            var query = PemsEntities.CreditCardTypesCustomers.Where(x => x.CustomerId == customerID).Select(tt => new StringIdTextDDLModel
            {
                Id = SqlFunctions.StringConvert((double?)tt.CreditCardType).Trim(),
                Text = tt.CreditCardType1.Name
            });
            return query.ToList();
        }

        /// <summary>
        /// Gets a list of dropdown options that represent transaction status's
        /// </summary>
        public List<StringIdTextDDLModel> GetPaymentStatusFilterItems()
        {
            var query = PemsEntities.TransactionStatus.Select(tt => new StringIdTextDDLModel
            {
                Id = tt.Description,
                Text = tt.Description
            });
            return query.ToList();
        }

        public List<StringIdTextDDLModel> GetPaymentStatusReleventFilterItems()
        {
            var idList = new[] { 100, 101, 102, 103, 104, 105, 106, 108, 112,120,121,150};
            var query = PemsEntities.TransactionStatus.Where(t => idList.Contains(t.StatusID)).Select(tt => new StringIdTextDDLModel
            {
                Id = tt.Description,
                Text = tt.Description
            }).OrderBy(t => t.Text);

            var dataList = query.ToList();
            dataList.Insert(0, new StringIdTextDDLModel { Id = "-1", Text = "ALL" });

            return dataList;
        }

        /// <summary>
        /// Gets a list of dropdown options that represent transaction types
        /// </summary>
        public List<StringIdTextDDLModel> GetTransactionTypesFilterItems()
        {
            var query = PemsEntities.TransactionTypes.Where(x => x.TransactionTypeDesc != "Sensor Reset").Select(tt => new StringIdTextDDLModel
            {
                Id = tt.TransactionTypeDesc,
                Text = tt.TransactionTypeDesc
            });
            return query.ToList();
        }

        public List<StringIdTextDDLModel> GetTransactionTypesFilterMinSensorItems()
        {
            List<StringIdTextDDLModel> filters = this.GetTransactionTypesFilterItems();
            var itemToRemove = filters.Single(r => r.Id == "Sensor"); //21 is sensor Id
            filters.Remove(itemToRemove);
            return filters;
        }

        public List<StringIdTextDDLModel> GetTransactionTypesFilterMinSensorOtheritems()
        {
            var idList = new[] { 1,2,3,4,5,6,7 };

            var query = PemsEntities.TransactionTypes.Where(t => idList.Contains(t.TransactionTypeId)).Select(tt => new StringIdTextDDLModel
            {
                Id = tt.TransactionTypeDesc,
                Text = tt.TransactionTypeDesc
            }).OrderBy( t => t.Text);
            return query.ToList();

        }

        public List<StringIdTextDDLModel> GetTransactionTypesFilterOnlySensor()
        {
            var idList = new[] { 21 };

            var query = PemsEntities.TransactionTypes.Where(t => idList.Contains(t.TransactionTypeId)).Select(tt => new StringIdTextDDLModel
            {
                Id = tt.TransactionTypeDesc,
                Text = tt.TransactionTypeDesc
            }).OrderBy(t => t.Text);
            return query.ToList();

        }

        /// <summary>
        /// Gets a list of dropdown options that represent asset types for a specific city
        /// </summary>
        public List<string> GetAssetTypeFilterItems(int currentCityId)
        {
            //cashbox and gateways are invalid for transactions, so remove the items with the correct metergroup
            IQueryable<string> assetTypesQuery = from at in PemsEntities.AssetTypes
                                                 where at.CustomerId == currentCityId && at.IsDisplay == true
                                                 where at.MeterGroupId != (int)MeterGroups.Cashbox
                                                 where at.MeterGroupId != (int)MeterGroups.Gateway
                                                 select at.MeterGroupDesc;
            return assetTypesQuery.ToList();
        }

        #region Revenue audit summary

        /// <summary>
        /// Description: This action is used to provide detailed info about the batch date vice transactions happened
        /// Modified By: Sairam on Dec 12th 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="batchIDIs"></param>
        /// <param name="batchDateIs"></param>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IEnumerable<RevenueAudit> GetRevenueAuditDetails(DataSourceRequest request, string batchIDIs, string batchDateIs, int CurrentCity)
        {

            //** Now execute the sql commands
            IEnumerable<RevenueAudit> RevenueAuditsummary = Enumerable.Empty<RevenueAudit>();
            List<RevenueAudit> resultList = new List<RevenueAudit>();

            DateTime startTimeOutput;
            DateTime endTimeOutput;


            if (DateTime.TryParse(batchDateIs, out startTimeOutput))
            {
                // ** Valid start date 
            }

            if (DateTime.TryParse(batchDateIs, out endTimeOutput))
            {
                //** Valid end date 
            }
            try
            {

                var innerQuery = from SB in PemsEntities.SettledBatches where SB.CustomerId == CurrentCity && SB.BatchDate >= startTimeOutput && SB.BatchDate <= endTimeOutput select SB.BatchId;
                var result = (from TCC in PemsEntities.TransactionsCreditCards
                              join CrType in PemsEntities.CreditCardTypes on TCC.CreditCardType equals CrType.CreditCardType1
                              join TRS in PemsEntities.TransactionStatus on TCC.Status equals TRS.StatusID
                              where innerQuery.Contains(TCC.SettleBatchID) && TCC.CustomerId == CurrentCity
                              select new
                              {
                                  RevenueBatchID = TCC.SettleBatchID,
                                  areaID = TCC.AreaId,
                                  meterID = TCC.MeterId,
                                  bayNumber = TCC.BayNumber,
                                  transDateTime = TCC.TransDateTime,
                                  creditCardType = CrType.Name,
                                  timePaid = TCC.TimePaid,
                                  amount = TCC.AmountInCents,
                                  acqReference = TCC.AcquirerTransReference,
                                  status = TRS.Description
                              }).ToList();


                RevenueAuditsummary = (from R in result
                                       select new RevenueAudit
                                       {
                                           RevenueBatchID = R.RevenueBatchID,
                                           areaID = R.areaID,
                                           meterID = R.meterID,
                                           bayNumber = R.bayNumber,
                                           transDateTime = Convert.ToString(R.transDateTime),
                                           creditCardType = R.creditCardType,
                                           //timePaid = ((TimeSpan.FromSeconds((double)R.timePaid).Hours).ToString() == "0") ? string.Format("{0:D1} mins", TimeSpan.FromSeconds((double)R.timePaid).Minutes) : string.Format("{0:D1} hrs {1:D1} mins", TimeSpan.FromSeconds((double)R.timePaid).Hours, TimeSpan.FromSeconds((double)R.timePaid).Minutes),
                                           timePaid = ((TimeSpan.FromSeconds((double)R.timePaid).Hours).ToString() == "0" ? string.Format("{0:D1} mins", TimeSpan.FromSeconds((double)R.timePaid).Minutes) : ((TimeSpan.FromSeconds((double)R.timePaid).Hours).ToString() == "1" ? string.Format("{0:D1} hr {1:D1} mins", TimeSpan.FromSeconds((double)R.timePaid).Hours, TimeSpan.FromSeconds((double)R.timePaid).Minutes) : string.Format("{0:D1} hrs {1:D1} mins", TimeSpan.FromSeconds((double)R.timePaid).Hours, TimeSpan.FromSeconds((double)R.timePaid).Minutes))),
                                           amount = "$" + ((Decimal)R.amount / (Decimal)100).ToString("0.00"),
                                           acqReference = R.acqReference,
                                           status = R.status
                                       });

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetRevenueAuditDetails Method (Transaction Menu - Rev Aud Summary Report)", ex);
            }
            return RevenueAuditsummary;
        }

        /// <summary>
        /// Description: This action is used to provide detailed info about the batch date vice transactions happened
        /// Modified By: Sairam on Dec 12th 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startBatchDateIs"></param>
        /// <param name="endBatchDateIs"></param>
        /// <param name="CurrentCity"></param>
        /// <returns></returns>
        public IEnumerable<RevenueAudit> GetRevenueAuditDetails_2(DataSourceRequest request, string startBatchDateIs, string endBatchDateIs, int CurrentCity)
        {

            //** Now execute the sql commands
            IEnumerable<RevenueAudit> RevenueAuditsummary = Enumerable.Empty<RevenueAudit>();

            DateTime startTimeOutput;
            DateTime endTimeOutput;

            if (DateTime.TryParse(startBatchDateIs, out startTimeOutput))
            {
                // ** Valid start date 
            }

            if (DateTime.TryParse(endBatchDateIs, out endTimeOutput))
            {
                //** Valid end date 
            }
            try
            {

                var innerQuery = from SB in PemsEntities.SettledBatches where SB.CustomerId == CurrentCity && SB.BatchDate >= startTimeOutput && SB.BatchDate <= endTimeOutput select SB.BatchId;
                //    var result = from TCC in PemsEntities.TransactionsCreditCards where innerQuery.Contains(TCC.BatchId) select TCC;

                var result = (from TCC in PemsEntities.TransactionsCreditCards
                              join CrType in PemsEntities.CreditCardTypes on TCC.CreditCardType equals CrType.CreditCardType1
                              join TRS in PemsEntities.TransactionStatus on TCC.Status equals TRS.StatusID
                              where innerQuery.Contains(TCC.SettleBatchID) && TCC.CustomerId == CurrentCity
                              select new
                              {
                                  RevenueBatchID = TCC.SettleBatchID,
                                  areaID = TCC.AreaId,
                                  meterID = TCC.MeterId,
                                  bayNumber = TCC.BayNumber,
                                  transDateTime = TCC.TransDateTime,
                                  creditCardType = CrType.Name,
                                  timePaid = TCC.TimePaid,
                                  amount = TCC.AmountInCents,
                                  acqReference = TCC.AcquirerTransReference,
                                  status = TRS.Description

                              }).ToList();



                RevenueAuditsummary = (from R in result
                                       select new RevenueAudit
                                       {
                                           RevenueBatchID = R.RevenueBatchID,
                                           areaID = R.areaID,
                                           meterID = R.meterID,
                                           bayNumber = R.bayNumber,
                                           transDateTime = Convert.ToString(R.transDateTime),
                                           creditCardType = R.creditCardType,
                                           // timePaid = Convert.ToString(((int)R.timePaid % 60 == 0) ? ((int)R.timePaid / 60).ToString() + " hrs" : ((int)R.timePaid / 60).ToString() + " hrs " + ((int)R.timePaid % 60).ToString()+" mins"),
                                           timePaid = ((TimeSpan.FromSeconds((double)R.timePaid).Hours).ToString() == "0" ? string.Format("{0:D1} mins", TimeSpan.FromSeconds((double)R.timePaid).Minutes) : ((TimeSpan.FromSeconds((double)R.timePaid).Hours).ToString() == "1" ? string.Format("{0:D1} hr {1:D1} mins", TimeSpan.FromSeconds((double)R.timePaid).Hours, TimeSpan.FromSeconds((double)R.timePaid).Minutes) : string.Format("{0:D1} hrs {1:D1} mins", TimeSpan.FromSeconds((double)R.timePaid).Hours, TimeSpan.FromSeconds((double)R.timePaid).Minutes))),
                                           amount = "$" + ((Decimal)R.amount / (Decimal)100).ToString("0.00"),
                                           acqReference = R.acqReference,
                                           status = R.status
                                       });



            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetRevenueAuditDetails Method (Transaction Menu - Rev Aud Summary Report)", ex);
            }
            return RevenueAuditsummary;
        }


        /// <summary>
        /// Description: This action is used to fetch Revenue Audit Summary data for the given date range
        /// Modified By: Sairam on Dec 12th 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="CurrentCity"></param>
        /// <param name="defaultOrderBy"></param>
        /// <param name="spName"></param>
        /// <returns></returns>
        public IQueryable<RevenueAudit> GetRevenueAuditsummary(DataSourceRequest request, int CurrentCity, out int total)
        {

            //** Execute the stored procedure so as to get the updated BatchIDs and also transactions
          
            try
            {
                var SPResult = PemsEntities.sp_SettledBatch(CurrentCity);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN Stored Procedure sp_SettledBatch (Transaction Menu - Rev Aud Summary Report)", ex);
            }
          

            total = 0;
            string paramValues = string.Empty;
            var spParams = GetSpParams(request, "BatchDate desc", out paramValues);
            string startDate = spParams[3].Value.ToString();
            string endDate = spParams[4].Value.ToString();

            //** Now execute the sql commands
            List<RevenueAudit> RevenueAuditsummary = new List<RevenueAudit>();

            DateTime startTimeOutput;
            DateTime endTimeOutput;

            if (DateTime.TryParse(startDate, out startTimeOutput))
            {
                // ** Valid start date 
            }

            if (DateTime.TryParse(endDate, out endTimeOutput))
            {
                //** Valid end date 
            }
            try
            {

                var items = (from SB in PemsEntities.SettledBatches
                             where SB.CustomerId == CurrentCity && SB.BatchDate >= startTimeOutput && SB.BatchDate <= endTimeOutput
                             select new RevenueAudit
                             {
                                 RevenueAuditDate = SB.BatchDate,
                                 RevenueBatchID = SB.BatchId,
                                 TransAmount = SB.SettleAmtInCent,
                                 TransCount = SB.SettleCount,
                                 RefundAmount = SB.RefundAmtInCent,
                                 RefundCount = SB.RefundCount,
                                 SettleAmount = SB.SettleAmtInCent,
                                 SettleCount = SB.SettleCount,
                                 DeclinedAmount = SB.DeclinedAmt,
                                 DeclinedCount = SB.DeclinedCount,
                                 UnprocessedAmount = SB.UnprocessedAmt,
                                 UnprocessedCount = SB.UnprocessedCount
                             }).Distinct();

                //items = items.ApplyFiltering(request.Filters);
                //total = items.Count();

                //items = items.ApplySorting(request.Groups, request.Sorts);
                //items = items.ApplyPaging(request.Page, request.PageSize);
                RevenueAuditsummary = items.ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetRevenueAuditsummary Method (Transaction Menu - Rev Aud Summary Report)", ex);
            }
            return RevenueAuditsummary.AsQueryable();
        }

        public List<RevenueAudit> ExportGridToExcel(DataSourceRequest request, string startDate, string endDate, int CurrentCity, string defaultOrderBy, out int total)
        {
            total = 0;
            string paramValues = string.Empty;
            var spParams = GetSpParams(request, defaultOrderBy, out paramValues);
            //IEnumerable<OccupancyInquiryItem> RevAud = PemsEntities.Database.SqlQuery<OccupancyInquiryItem>(spName + " " + paramValues, spParams);

           
           // ** Execute the stored procedure so as to get the updated BatchIDs and also transactions
            var SPResult = PemsEntities.sp_SettledBatch(CurrentCity);
            

            //** Now execute the sql commands
            List<RevenueAudit> RevenueAuditsummary = new List<RevenueAudit>();
            IQueryable<RevenueAudit> items = null;

            DateTime startTimeOutput;
            DateTime endTimeOutput;

            if (DateTime.TryParse(startDate, out startTimeOutput))
            {
                // ** Valid start date 
            }

            if (DateTime.TryParse(endDate, out endTimeOutput))
            {
                //** Valid end date 
            }
            try
            {
                RevenueAuditsummary = (from SB in PemsEntities.SettledBatches
                                       where SB.CustomerId == CurrentCity && SB.BatchDate >= startTimeOutput && SB.BatchDate <= endTimeOutput
                                       select new RevenueAudit
                                       {
                                           RevenueAuditDate = SB.BatchDate,
                                           RevenueBatchID = SB.BatchId,
                                           TransAmount = SB.SettleAmtInCent,
                                           TransCount = SB.SettleCount,
                                           RefundAmount = SB.RefundAmtInCent,
                                           RefundCount = SB.RefundCount,
                                           SettleAmount = SB.SettleAmtInCent,
                                           SettleCount = SB.SettleCount
                                       }).ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetRevenueAuditsummary Method (Transaction Menu - Rev Aud Summary Report)", ex);
            }

            items = RevenueAuditsummary.AsQueryable();

            if (items != null)
            {
                items = items.ApplyFiltering(request.Filters);
                total = items.Count();
                items = items.ApplySorting(request.Groups, request.Sorts);
                items = items.ApplyPaging(request.Page, request.PageSize);
                return items.ToList();
            }

            return new List<RevenueAudit>();
        }

        #endregion

        /// <summary>
        /// Gets a list of batch transactions based on a request and any associated filters.
        /// </summary>
        public List<CustomerTransactionModel> GetBatchTransactions([DataSourceRequest] DataSourceRequest request, string defaultOrderBy, int customerId)
        {

            // If filtered by first 6/last 4 of CC, have to update filter value with cc hash
            ParseCreditCardFilter(request.Filters);


            string paramValues;
            //add the view specific name
            List<CustomerTransactionModel> transactionModels = new List<CustomerTransactionModel>();
            switch (customerId)
            {
                case 4125:  //For Lincon only
                    request.Filters.Add(new FilterDescriptor { Member = "viewName", Value = "pv_CustomerTransactions_lincoln", Operator = FilterOperator.IsEqualTo });
                    SqlParameter[] spParams = GetSpParams(request, defaultOrderBy, out paramValues);
                    IEnumerable<CustomerTransactionModel> items = PemsEntities.Database.SqlQuery<CustomerTransactionModel>("sp_GetCustomerTransactions " + paramValues, spParams);
                    transactionModels = items.ToList();
                    break;
            }



            /////////////////for testing only/////
            //  string paramValues;
            // //add the view specific name
            // request.Filters.Add( new FilterDescriptor {Member = "viewName", Value = "pv_CustomerTransactions", Operator = FilterOperator.IsEqualTo} );
            // SqlParameter[] spParams = GetSpParams( request, defaultOrderBy, out paramValues );
            // IEnumerable<CustomerTransactionModel> items = PemsEntities.Database.SqlQuery<CustomerTransactionModel>("sp_GetCustomerTransactions " + paramValues, spParams);
            // List<CustomerTransactionModel> transactionModels = items.ToList();
            /////////////////////


            // Format the currency field (better globalization support on server)
            foreach (CustomerTransactionModel transaction in transactionModels)
            {
                transaction.AmountPaidDisplay = FormatHelper.FormatCurrency(transaction.AmountPaid);
                transaction.TimePaidDisplay = FormatHelper.FormatTimeFromSeconds(transaction.TimePaid);
            }

            return transactionModels;
        }


        /// <summary>
        /// Gets a list of transactions based on a request and any associated filters.
        /// </summary>
        public List<CustomerSensorTransactionModel> GetSensorTransactions([DataSourceRequest] DataSourceRequest request, string defaultOrderBy)
        {
            // If filtered by first 6/last 4 of CC, have to update filter value with cc hash
            ParseCreditCardFilter(request.Filters);

            string paramValues;
            //add the view specific name
            request.Filters.Add(new FilterDescriptor { Member = "viewName", Value = "pv_CustomerSensorTransactions", Operator = FilterOperator.IsEqualTo });
            SqlParameter[] spParams = GetSpParams(request, defaultOrderBy, out paramValues);
            IEnumerable<CustomerSensorTransactionModel> items = PemsEntities.Database.SqlQuery<CustomerSensorTransactionModel>("sp_GetCustomerTransactions " + paramValues, spParams);

            List<CustomerSensorTransactionModel> transactionModels = items.ToList();
            // Format the currency field (better globalization support on server)

            return transactionModels;
        }

        /// <summary>
        /// Gets a list of transactions based on a request and any associated filters.
        /// </summary>
        public List<CustomerSensorTransactionModel> GetRTSensorTransactions([DataSourceRequest] DataSourceRequest request, string defaultOrderBy)
        {
            // If filtered by first 6/last 4 of CC, have to update filter value with cc hash
            ParseCreditCardFilter(request.Filters);

            string paramValues;
            //add the view specific name
            //request.Filters.Add(new FilterDescriptor { Member = "viewName", Value = "pv_CustomerSensorTransactions", Operator = FilterOperator.IsEqualTo });
            request.Filters.Add(new FilterDescriptor { Member = "viewName", Value = "pv_CustomerSensorRealtimeTransactions", Operator = FilterOperator.IsEqualTo });
            
            SqlParameter[] spParams = GetSpParams(request, defaultOrderBy, out paramValues);
            IEnumerable<CustomerSensorTransactionModel> items = PemsEntities.Database.SqlQuery<CustomerSensorTransactionModel>("sp_GetCustomerTransactions " + paramValues, spParams);

            List<CustomerSensorTransactionModel> transactionModels = items.ToList();
            // Format the currency field (better globalization support on server)

           // transactionModels = transactionModels.GroupBy(t => new { t.CustomerId, t.AssetId, t.AreaId, t.SpaceId, t.SpaceStatus, t.DateTime }).Select(t => t.First()).ToList();

            return transactionModels;
        }
    }
}