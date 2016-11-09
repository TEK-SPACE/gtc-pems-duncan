using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Discounts
{

    public enum DiscountExpirationType
    {
        Yearly = 1,
        Monthly = 2, 
        Weekly = 3
    }


    //base model or user accounts for discount schemes
    public class UserAccountBase
    {
        public int UserId { get; set; }
          [OriginalGridPosition(Position = 0)]
        public string LastName { get; set; }
          [OriginalGridPosition(Position = 1)]
          public string FirstName { get; set; }
        [Required]
          [OriginalGridPosition(Position =2)]
          public string Email { get; set; }
          [OriginalGridPosition(Position = 6)]
          public string AccountStatus { get; set; }
        public int AccountStatusId { get; set; }
        public DateTime CreationDate { get; set; }
        [OriginalGridPosition(Position =7)]
        public string CreationDateDisplay { get { return CreationDate == DateTime.MinValue ? string.Empty : CreationDate.ToString("g"); } }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
         [Required]
        public string Phone { get; set; }
        public DateTime AccountStatusChangeTime { get; set; }
        public string AccountStatusChangeTimeDisplay { get { return AccountStatusChangeTime == DateTime.MinValue ? string.Empty : AccountStatusChangeTime.ToString("d"); } }
        public DateTime? LastUserAccess { get; set; }
        public string LastUserAccessDisplay { get { return LastUserAccess.HasValue ? LastUserAccess.Value.ToString("g") : string.Empty; } }
        public DateTime? LastDiscountUsedTime { get; set; }
        public string LastDiscountUsedName { get; set; }
        public string LastDiscountUsed { get { return LastDiscountUsedName + (LastDiscountUsedTime.HasValue ? " " + LastDiscountUsedTime.Value.ToString("g") : ""); } }
        public string Notes { get; set; }
        public int? LastEditiedByUserId { get; set; }
        public string LastEditiedByUserName { get; set; }
        public DateTime? AccountExpirationDate { get; set; }
        public string AccountExpirationDateDisplay { get { return AccountExpirationDate.HasValue ? AccountExpirationDate.Value.ToString("g") : string.Empty; } }
        public string CreditCardExpirationDate { get; set; }
        public string CreditCardLast4Digits { get; set; }
    }

    public class UserAccountDetails : UserAccountBase
    {
        public int CustomerId { get; set; }
        public string SecurityQuestion1 { get; set; }
        public string SecurityQuestion2 { get; set; }
        public string SecurityAnswer1 { get; set; }
        public string SecurityAnswer2 { get; set; }
        public List<UserDiscountScheme> DiscountSchemes { get; set; }
    }

    //listing model
    public class UserAccountListModel : UserAccountBase
    {
        [OriginalGridPosition(Position =3)]
        public int PendingCount { get; set; }
        [OriginalGridPosition(Position = 4)]
        public int ApprovedCount { get; set; }
        [OriginalGridPosition(Position = 5)]
        public int RejectedCount { get; set; }
    }

    public class UserDiscountScheme
    {
        public DateTime ApplicationDate { get; set; }
        public string ApplicationDateDisplay { get { return ApplicationDate == DateTime.MinValue ? string.Empty : ApplicationDate.ToString("g"); } }
        public int DiscountUserSchemeId { get; set; }
        public string SchemeName { get; set; }
        public string ApplicationStatus { get; set; }
        public int ApplicationStatusId { get; set; }
        public DateTime? ApplicationStatusDate  { get; set; }
        public string ApplicationStatusDisplay { get { return ApplicationStatusDate.HasValue ? ApplicationStatusDate.Value.ToString("g") : string.Empty; } }
        public string ApplicationStatusNote { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ExpirationDateDisplay { get { return ExpirationDate.HasValue ? ExpirationDate.Value.ToString("d") : string.Empty; } }
        public int? LastEditiedByUserId { get; set; }
        public string LastEditiedByUserName { get; set; }
    }

    public class AccountSchemeDetails : UserAccountBase
    {
        public UserDiscountScheme DiscountScheme { get; set; }
    }
}
