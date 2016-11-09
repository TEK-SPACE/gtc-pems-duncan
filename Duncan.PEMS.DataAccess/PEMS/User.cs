//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duncan.PEMS.DataAccess.PEMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.DiscountUserCards = new HashSet<DiscountUserCard>();
            this.DiscountUserSchemes = new HashSet<DiscountUserScheme>();
            this.webpages_Roles = new HashSet<webpages_Roles>();
            this.AI_EXPORT = new HashSet<AI_EXPORT>();
        }
    
        public int UserID { get; set; }
        public string UserFName { get; set; }
        public string UserLName { get; set; }
        public string UserPassword { get; set; }
        public string UserType { get; set; }
        public int DefaultCustomerID { get; set; }
        public string Role { get; set; }
        public string PasswordHash { get; set; }
        public Nullable<System.DateTime> ActivationDate { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<bool> MultipleLogins { get; set; }
        public Nullable<bool> Enabled { get; set; }
        public string UserAddress { get; set; }
        public string UserEmail { get; set; }
        public Nullable<System.DateTime> RegisteredTS { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string AddressState { get; set; }
        public string PostalCode { get; set; }
        public int AccountStatus { get; set; }
        public System.DateTime AccountStatusUpdated { get; set; }
        public System.DateTime Created { get; set; }
        public Nullable<System.DateTime> LastUsed { get; set; }
        public string StreetSuffix { get; set; }
        public string MailingNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string SecurityQuestion1 { get; set; }
        public string SecurityAnswer1 { get; set; }
        public string SecurityQuestion2 { get; set; }
        public string SecurityAnswer2 { get; set; }
        public string UserNote { get; set; }
        public Nullable<int> LastEditedUserId { get; set; }
        public string PhoneNumber { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public string UserName { get; set; }
    
        public virtual AccountStatu AccountStatu { get; set; }
        public virtual ICollection<DiscountUserCard> DiscountUserCards { get; set; }
        public virtual ICollection<DiscountUserScheme> DiscountUserSchemes { get; set; }
        public virtual UserType UserType1 { get; set; }
        public virtual ICollection<webpages_Roles> webpages_Roles { get; set; }
        public virtual ICollection<AI_EXPORT> AI_EXPORT { get; set; }
    }
}