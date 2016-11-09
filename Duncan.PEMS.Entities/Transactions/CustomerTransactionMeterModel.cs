/******************* CHANGE LOG *************************************************************************************************************************************
 * DATE                 NAME                   DESCRIPTION
 * ___________      ___________________        _________________________________________________________________________________________________________
 * 
 * 01/09/2014       Sergey Ostrerov                 Issue: DPTXPEMS-213 - Transaction Inquiry grid page shows incorrect 'TimePaid' value however
 *                                                                        details page still shows correct value.
 *                                                                        Display TimePaid in minutes.                             
 * 
 * *******************************************************************************************************************************************************************/

using System;
using System.Collections.Generic;

namespace Duncan.PEMS.Entities.Transactions
{
    public class CustomerTransactionMeterModel
    {

        public CustomerTransactionMeterModel()
        {
            Coins = new List<CoinType>();
            AcquirerResponses = new List<AcquirerResponse>();
        }


        public int? MeterId { get; set; }
        public string MeterName { get; set; }
        public int? SensorId { get; set; }

        public string Area { get; set; }
        public string Zone { get; set; }
        public string Suburb { get; set; }
        public string Street { get; set; }
        public int? BayNumber { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string DemandZone { get; set; }

        public long? TransactionId { get; set; }
        public int? TransactionType { get; set; }
        public string TransactionTypeDesc { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? TimePaidInSeconds { get; set; }
        public int? TimePaidInMinutes { get; set; }
        public int? AmountInCents { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public bool? Peak { get; set; }
        public bool? PrePayUsed { get; set; }
        public bool? DiscountSchemeUsed { get; set; }
        public bool? FreeMinutesUsed { get; set; }

        public string CardType { get; set; }
        public string CCLast4 { get; set; }
        public int? ReceiptNumber { get; set; }
        public string BatchId { get; set; }
        public List<AcquirerResponse> AcquirerResponses { get; set; }
        public List<CoinType> Coins { get; set; } 

        public string CoinType1Description { get; set; }
        public string CoinType2Description { get; set; }
        public string CoinType3Description { get; set; }
        public string CoinType4Description { get; set; }
        public string CoinType5Description { get; set; }
        public string CoinType6Description { get; set; }
        public string CoinType7Description { get; set; }
        public string CoinType8Description { get; set; }
        public int? CoinType1Count { get; set; }
        public int? CoinType2Count { get; set; }
        public int? CoinType3Count { get; set; }
        public int? CoinType4Count { get; set; }
        public int? CoinType5Count { get; set; }
        public int? CoinType6Count { get; set; }
        public int? CoinType7Count { get; set; }
        public int? CoinType8Count { get; set; }
        public int? CoinType1Amount { get; set; }
        public int? CoinType2Amount { get; set; }
        public int? CoinType3Amount { get; set; }
        public int? CoinType4Amount { get; set; }
        public int? CoinType5Amount { get; set; }
        public int? CoinType6Amount { get; set; }
        public int? CoinType7Amount { get; set; }
        public int? CoinType8Amount { get; set; }

        public int? SmartCardSerialNo { get; set; }
        public long SmartCardId { get; set; }
        public string SmartCardAcquirerTransRef { get; set; }
        public string SmartCardMeterTransRef { get; set; }

        public long? ReconFileId { get; set; }
        public DateTime? ReconDate { get; set; }

        public string Note { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
    }

    public class CoinType
    {
        public int Count { get; set; }
        public int Ordinal { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string DisplayName {
            get { return CountryCode + ": " + Name; }
        }
        public int Value { get; set; }
    }


    public class AcquirerResponse
    {
        public DateTime? AuditDate { get; set; }
        public long? TransactionAuditId { get; set; }
        public int? ResponseCode { get; set; }
        public string ResponseDetail { get; set; }
        public string ResponseTransRef { get; set; }
    }
}