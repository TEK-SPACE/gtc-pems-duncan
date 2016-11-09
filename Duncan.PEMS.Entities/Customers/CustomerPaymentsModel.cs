/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             ___________________________________________________________________________________________________
 * 
 * 01/24/2014       Sergey Ostrerov                 DPTXPEMS-8, 14, 45 - Can't create new customer; Can't add Asset
 * 02/20/2014       Sergey Ostrerov                 DPTXPEMS - 251 Payment Gateway Configuration page is missing 'Access Code' field
 * *****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerPaymentsModel : CustomerBaseModel, IValidatableObject
    {
        public char Separator = '_';

        public CustomerPaymentsCardModel SmartCardGateway { get; set; }
        public CustomerPaymentsCardModel PaymentGateway { get; set; }

        public CustomerPaymentsCreditDebitModel CreditDebit { get; set; }

        public CustomerPaymentsEmvModel EmvPhone { get; set; }

        public CustomerPaymentsCoinsModel Coins { get; set; }
        public CustomerPaymentsCoinsModel AllCoins { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if (!String.IsNullOrEmpty(PaymentGateway.MerchantName))
            {
                // Merchant Name
                if (PaymentGateway.MerchantName.Length > 64)
                {
                    errors.Add(new ValidationResult("The field MerchantName Code must be maximum length of '64'.", new[] { "MerchantName" }));
                }
            }

            if (!String.IsNullOrEmpty(PaymentGateway.AccessCode))
            {
                // Access Code
                if (PaymentGateway.AccessCode.Length > 20)
                {
                    errors.Add(new ValidationResult("The field Access Code must be maximum length of '20'.", new[] { "AccessCode" }));
                }
            }

            if (!String.IsNullOrEmpty(PaymentGateway.Password))
            {
                // Password
                if (PaymentGateway.Password.Length > 64)
                {
                    errors.Add(new ValidationResult("The field Password Code must be maximum length of '64'.", new[] { "Password" }));
                }
            }
            return errors;
        }
    }


    public class CustomerPaymentsCoinsModel
    {
        public List<SelectListItem> CoinCountry { get; set; }
        public string CoinCountryId { get; set; }

        public bool HasCoinsSelected { get { return Coins.Count > 0; } }

        public List<CustomerPaymentsCoinModel> Coins { get; set; }

        public CustomerPaymentsCoinsModel()
        {
            Coins = new List<CustomerPaymentsCoinModel>();
            CoinCountry = new List<SelectListItem>();
        }
    }


    public class CustomerPaymentsCoinModel : IComparable<CustomerPaymentsCoinModel>
    {
        public const string CoinPrefix = "COIN";
        //public const string CoinNew = "NEW";
        public const string CoinText = "TEXT";
        public const string CoinCheck = "CHK";
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int DenominationId { get; set; }
        public string DenominationName { get; set; }
        public int DenominationValue { get; set; }
        public string CountryCode { get; set; }
        public string DisplayName {
            get { return CountryCode + ": " + DenominationName ; }
        }

        public int CompareTo(CustomerPaymentsCoinModel other)
        {
            return DenominationValue.CompareTo(other.DenominationValue);
        }
    }

    public class CustomerPaymentsCardModel
    {
        public CustomerPaymentsCardModel()
        {
            Gateway = new List<SelectListItem>();
            VsignPartner = new List<SelectListItem>();
        }

        public List<SelectListItem> Gateway { get; set; }
        public int GatewayId { get; set; }
        public List<SelectListItem> VsignPartner { get; set; }
        public int VsignPartnerId { get; set; }

        public int OLTPActive { get; set; }
        public string Description { get; set; }

        public bool ReAuthorize { get; set; }
        public bool DelayedProcessing { get; set; }
        public bool CardPresent { get; set; }

        public string MerchantName { get; set; }
        public string AccessCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }


    public class CustomerPaymentsCreditDebitModel
    {
        public const string CardLabelsPrefix = "CARD";

        public CustomerPaymentsCreditDebitModel()
        {
            Cards = new List<SelectListItem>();
        }


        public int HashFirst { get; set; }
        public int HashLast { get; set; }

        public List<SelectListItem> Cards { get; set; }

        public int SecondsGap { get; set; }
        public int DaysToWaitToReconcile { get; set; }
    }

    public class CustomerPaymentsEmvModel
    {
        public bool Emv { get; set; }
        public bool PayByPhone { get; set; }
    }
}