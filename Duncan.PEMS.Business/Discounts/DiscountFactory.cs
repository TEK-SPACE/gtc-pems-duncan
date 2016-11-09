using System;
using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.Business.Email;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Alarms;
using Duncan.PEMS.Entities.Discounts;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Business.Discounts
{
    /// <summary>
    /// The <see cref="Duncan.PEMS.Business.Discounts"/> namespace contains classes for managing discounts.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    public class DiscountFactory : BaseFactory
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
        public DiscountFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        #region "Listing"
        /// <summary>
        /// Gets the status of a discount scheme
        /// </summary>
        public string GetSchemeStatus(string schemeStatusId)
        {
            int statusID;
            bool parsed = int.TryParse(schemeStatusId, out statusID);
              //see if we can get the discount status base don the id passed in
            if (parsed)
            {
                var state = PemsEntities.DiscountSchemeStatus.FirstOrDefault(x => x.DiscountSchemeStatusId == statusID);
                if (state != null)
                    return state.DiscountSchemeStatusDesc;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the name of a discount scheme
        /// </summary>
        public string GetSchemeName(string schemeId)
        {
            int schemeID;
            bool parsed = int.TryParse(schemeId, out schemeID);
            if (parsed)
            {
                var item = PemsEntities.DiscountSchemes.FirstOrDefault(x => x.DiscountSchemeID == schemeID);
                if (item != null)
                    return item.SchemeName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets all alarm objects from the data source
        /// </summary>
        /// <returns></returns>
        public List<UserAccountListModel> GetSummaryAccounts([DataSourceRequest] DataSourceRequest request, out int total,
                                                          string scheme, string schemeStatusId, string customerId,
                                                          bool applyPaging = false, int exportCount = -1)
        {
            int custID;
            int.TryParse(customerId, out custID);
            //for this listpage, we need to generate a list of all user ids that match the criteria. default to grab them all
            IEnumerable<int> userIds = GenerateSummaryAccounts(scheme, schemeStatusId, custID);

            //need to generate a list of collection runs, for each dy
            var allItems = new List<UserAccountListModel>();
            foreach (var userId in userIds)
            {
                var user = PemsEntities.Users.FirstOrDefault(x => x.UserID == userId);
               if (user != null)
               {
                     var item = new UserAccountListModel
                    {
                        FirstName = user.UserFName,
                        LastName = user.UserLName,
                        Email = user.UserEmail,
                        UserId = user.UserID,
                        AccountStatus = user.AccountStatu.AccountStatusDesc,
                        CreationDate = user.Created,
                        PendingCount = user.PendingCount,
                        ApprovedCount = user.ApprovedCount,
                        RejectedCount = user.RejectedCount
                    };
                     allItems.Add(item);
               }
            }

            //now we build a list of CR aggregate items needed
            var query = allItems.AsQueryable();

            // Step 2: Filter data
            query = query.ApplyFiltering(request.Filters);

            // Step 3: Get count of filtered items
            total = query.Count();

            // Step 4: Apply sorting
            query = query.ApplySorting(request.Groups, request.Sorts);

            // Step 5: Apply paging
            if (applyPaging)
                query = exportCount > -1 ? query.Take(exportCount) : query.ApplyPaging(request.Page, request.PageSize);

            // Step 6: Get the data!
            return query.ToList();
        }

        private IEnumerable<int> GenerateSummaryAccounts(string scheme, string schemeStatusId, int custID)
        {
          //  first we need to generate a lsit of all discount schemes for this customer

            //then we need to get  alist of users that have access to these discounts. 
            //we have to do this due to the scheme and schemestatusID filters
            //get a list of all the discount schemes for this customer (jsut need the IDs for out purposes
            var discountSchemeIds = PemsEntities.DiscountSchemeCustomers.Where(x => x.CustomerId == custID).Select(x=>x.DiscountSchemeId).ToList();
            //if none were found, return empty list - no schemes, no users can be assigned.
            if (!discountSchemeIds.Any())
                return new List<int>();

            var userIds = new List<int>();
            bool useFilteredIds = false;
           

            //if they are searching for both, only include that have both, otherwise test individually
            if (!string.IsNullOrEmpty(scheme) && !string.IsNullOrEmpty(schemeStatusId))
            {
                useFilteredIds = true;
                int statusID;
                bool parsed = int.TryParse(schemeStatusId, out statusID);
                int schemeID;
                bool parsed2 = int.TryParse(scheme, out schemeID);
                if (parsed && parsed2)
                {
                    //see if we can get the discount status base don the id passed in
                    var state = PemsEntities.DiscountSchemeStatus.FirstOrDefault(x => x.DiscountSchemeStatusId == statusID);
                    if (state != null)
                    {
                        //if we found something, we then need to only get users that have access to a discount scheme with this status

                        //get all user schemes with this status
                        var schemes = PemsEntities.DiscountUserSchemes.Where(x => x.SchemeStatus == state.DiscountSchemeStatusId && x.DiscountScheme.DiscountSchemeID == schemeID);
                        foreach (var item in schemes.Where(item => !userIds.Contains(item.UserId)))
                            userIds.Add(item.UserId);
                    }
                }
            }
            //scheme
            else if (!string.IsNullOrEmpty(scheme))
            {
                useFilteredIds = true;
                int schemeID;
                bool parsed = int.TryParse(scheme, out schemeID);
                if (parsed)
                {
                    var items =
                        PemsEntities.DiscountUserSchemes.Where(x => x.DiscountScheme.DiscountSchemeID == schemeID);
                    if (items.Any())
                        foreach (var item in items.Where(item => discountSchemeIds.Contains(item.DiscountScheme.DiscountSchemeID)))
                        {
                            if (!userIds.Contains(item.UserId))
                                userIds.Add(item.UserId);
                        }
                }
            }
            //scheme status id
            else  if (!string.IsNullOrEmpty(schemeStatusId))
            {
                useFilteredIds = true;
                int statusID;
                bool parsed = int.TryParse(schemeStatusId, out statusID);
                if (parsed)
                {
                    //see if we can get the discount status base don the id passed in
                    var state = PemsEntities.DiscountSchemeStatus.FirstOrDefault(x => x.DiscountSchemeStatusId == statusID);
                    if (state != null)
                    {
                        //if we found something, we then need to only get users that have access to a discount scheme with this status

                        //get all user schemes with this status
                        var schemes =PemsEntities.DiscountUserSchemes.Where(x => x.SchemeStatus == state.DiscountSchemeStatusId);
                        foreach (var item in schemes.Where(item => !userIds.Contains(item.UserId)))
                            userIds.Add(item.UserId);
                    }
                }
            }

            //if we need to bring back filtered results, do this here
            if (useFilteredIds)
            {
                var users = PemsEntities.Users.Where(x => userIds.Contains(x.UserID)).Select(x=>x.UserID).ToList();
                return users.Any() ? users : new List<int>(); 
            }

            //otherwise bring back a list of user ids for all discountuserscheme with hte matching customerID
            var schemeUsers = PemsEntities.DiscountUserSchemes.Where(x => x.CustomerId == custID).Select(x => x.UserId).Distinct().ToList();
            return schemeUsers.Any() ? schemeUsers : new List<int>();
        }
        #endregion

        #region "User Details"
        /// <summary>
        /// Gets a user account for a discount scheme user
        /// </summary>
        public UserAccountDetails GetUserAcount(int userID)
        {
            //ge the user acount
            var userAccount = new UserAccountDetails {DiscountSchemes = new List<UserDiscountScheme>()};
            var user = PemsEntities.Users.FirstOrDefault(x => x.UserID == userID);
            if (user != null)
            {
                userAccount.CustomerId = user.DefaultCustomerID;
                userAccount.AccountExpirationDate = user.ExpirationDate;
                userAccount.AccountStatus = user.AccountStatu.AccountStatusDesc;
                userAccount.AccountStatusId = user.AccountStatu.AccountStatusId;
                userAccount.AccountStatusChangeTime = user.AccountStatusUpdated;
                userAccount.Address1 = user.UserAddress;
                userAccount.Address2 = user.Address2;
                userAccount.City = user.City;
                userAccount.CreationDate = user.Created;
                userAccount.Email = user.UserEmail;
                userAccount.FirstName = user.UserFName;

                //now the teh RBAC user that last modified this.
                if (user.LastEditedUserId.HasValue)
                {
                    userAccount.LastEditiedByUserId = user.LastEditedUserId.Value;
                    userAccount.LastEditiedByUserName = (new UserFactory()).GetUsername(user.LastEditedUserId.Value);
                }
                userAccount.LastName = user.UserLName;
                userAccount.LastUserAccess = user.LastUsed;
                userAccount.Notes = user.UserNote;
                userAccount.Phone = user.PhoneNumber;
                userAccount.PostalCode = user.PostalCode;
                userAccount.SecurityAnswer1 = user.SecurityAnswer1;
                userAccount.SecurityAnswer2 = user.SecurityAnswer2;
                userAccount.SecurityQuestion1 = user.SecurityQuestion1;
                userAccount.SecurityQuestion2 = user.SecurityQuestion2;
                userAccount.State = user.AddressState;
                userAccount.UserId = user.UserID;

                //CC info
                var card = user.DiscountUserCards.OrderByDescending(x => x.RegisteredTS).FirstOrDefault();
                if (card != null)
                {
                    userAccount.CreditCardExpirationDate = card.CardExpiry;
                    userAccount.CreditCardLast4Digits = card.CardNumLast4;

                    //now get the most recent transaction base don this cards Hash value
                    var hash = card.PANHash;
                    //not get all transactiosn from the TransactionsCreditCard table that matches this hash.
                    var mostrecentTransaction = PemsEntities.TransactionsCreditCards.OrderByDescending(x => x.TransDateTime).FirstOrDefault(x => x.CardNumHash == hash );
                    if (mostrecentTransaction != null)
                    {
                        userAccount.LastDiscountUsedTime = mostrecentTransaction.TransDateTime;
                        //get the name of the scheme here
                        if (mostrecentTransaction.DiscountScheme != null)
                            userAccount.LastDiscountUsedName = mostrecentTransaction.DiscountScheme.SchemeName;
                    }
                }
                
                //now that we have user info filled out, lets get the Discount schemes for this user
               
                if (user.DiscountUserSchemes.Any())
                {
                    foreach (var scheme in user.DiscountUserSchemes)
                    {
                        var userScheme = new UserDiscountScheme();
                        userScheme.ApplicationDate = scheme.CreatedTS;
                        userScheme.SchemeName = scheme.DiscountScheme.SchemeName;
                        userScheme.ApplicationStatus = scheme.DiscountSchemeStatu == null ? "" : scheme.DiscountSchemeStatu.DiscountSchemeStatusDesc;
                        userScheme.ApplicationStatusDate = scheme.SchemeStatusDate;
                        userScheme.ApplicationStatusNote = scheme.StatusNote;
                        userScheme.ExpirationDate = scheme.ExpiryTS;
                        userScheme.ApplicationStatusId = scheme.DiscountSchemeStatu == null ? 0 : scheme.DiscountSchemeStatu.DiscountSchemeStatusId;
                        userScheme.DiscountUserSchemeId = scheme.DiscountUserSchemeId;
                        //now the teh RBAC user that last modified this.
                        if (scheme.ModifiedByUserId.HasValue)
                        {
                            userScheme.LastEditiedByUserId = scheme.ModifiedByUserId.Value;
                            userScheme.LastEditiedByUserName = (new UserFactory()).GetUsername(scheme.ModifiedByUserId.Value);
                        }
                        userAccount.DiscountSchemes.Add(userScheme);
                    }
                }
            }

            return userAccount;
        }

        /// <summary>
        /// Saves a discount scheme user account
        /// </summary>
        public void SaveUserAcount(UserAccountDetails userAccountDetails)
        {
            //ge the user acount
            var user = PemsEntities.Users.FirstOrDefault(x => x.UserID == userAccountDetails.UserId);
            if (user != null)
            {
                //only able to edit certain fields, so those are the ones we are saving.
                //email,phone, add 1 2, city, state, postal

                user.UserEmail = userAccountDetails.Email;
                user.PhoneNumber = userAccountDetails.Phone;
                user.UserAddress = userAccountDetails.Address1;
                user.Address2 = userAccountDetails.Address2;
                user.City = userAccountDetails.City;
                user.AddressState  =userAccountDetails.State ;
                user.PostalCode = userAccountDetails.PostalCode;
                PemsEntities.SaveChanges();
            }
        }

        /// <summary>
        /// Updates a discount scheme users status to 0 - Terminate
        /// </summary>
        public void TerminateUserAccount(int userID, string notes)
        {
            UpdateStatus(userID, notes, 0);
        }


        /// <summary>
        /// Updates a discount scheme users status to 1 - Active
        /// </summary>
        public void ReactivateUserAccount(int userID, string notes)
        {
            UpdateStatus(userID, notes, 1);
        }

        /// <summary>
        /// Updates a discount scheme users status to 1- Active
        /// </summary>
        public void UnlockUserAccount(int userID, string notes)
        {
            UpdateStatus(userID, notes, 1);
        }
        
        /// <summary>
        /// Updates a discount scheme users status
        /// </summary>
        private void UpdateStatus(int userId, string notes, int statusID)
        {
            //ge the user acount
            var user = PemsEntities.Users.FirstOrDefault(x => x.UserID == userId);
            if (user != null)
            {
                //just change status to 0 (terminated)
                user.AccountStatus = statusID;
                user.UserNote = notes;
                PemsEntities.SaveChanges();
            }
        }

        /// <summary>
        /// Checks to see if that email already exist in the system for another user.
        /// </summary>
        public bool CheckExistingEmail(int userId, string email, int custId)
        {
            email = email.Trim();
            var usersWithSameEmail = PemsEntities.Users.Where(x => x.UserID != userId && x.DefaultCustomerID == custId && x.UserEmail.Trim() == email);
            return usersWithSameEmail.Any();
        }
        #endregion

        #region "Filter Values"
        /// <summary>
        /// Get account states for the index filter (dropdown)
        /// </summary>
        public  List<AlarmDDLModel> GetAccountStates()
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
             var   items = PemsEntities.AccountStatus.ToList();
                if (items.Any())
                    ddlItems.AddRange( items.Select(item => new AlarmDDLModel {Value = item.AccountStatusId, Text = item.AccountStatusDesc}));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetAccountStates", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }
        /// <summary>
        /// Get discount schemes dropdown items for this customer
        /// </summary>
        public List<AlarmDDLModel> GetDiscountSchemes(int customerID)
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.DiscountSchemeCustomers.Where(x=>x.CustomerId == customerID && x.IsDisplay);
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.DiscountSchemeId, Text = item.DiscountScheme.SchemeName }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetDiscountSchemes", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Get discount states for the index filter (dropdown)
        /// </summary>
        public List<AlarmDDLModel> GetDiscountStates()
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.DiscountSchemeStatus.ToList();
                if (items.Any())
                    ddlItems.AddRange(items.Select(item => new AlarmDDLModel { Value = item.DiscountSchemeStatusId, Text = item.DiscountSchemeStatusDesc }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetDiscountStates", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }
        #endregion

        #region "User Scheme Details"
        /// <summary>
        /// Gets the user account details and the Discount Scheme details
        /// </summary>
        public AccountSchemeDetails GetAccountSchemeDetails(int userID, int userSchemeId)
        {
            //ge the user acount
            var userAccount = new AccountSchemeDetails { DiscountScheme = new UserDiscountScheme() };
            var user = PemsEntities.Users.FirstOrDefault(x => x.UserID == userID);
            if (user != null)
            {
                userAccount.AccountExpirationDate = user.ExpirationDate;
                userAccount.AccountStatus = user.AccountStatu.AccountStatusDesc;
                userAccount.AccountStatusId = user.AccountStatu.AccountStatusId;
                userAccount.AccountStatusChangeTime = user.AccountStatusUpdated;
                userAccount.Address1 = user.UserAddress;
                userAccount.Address2 = user.Address2;
                userAccount.City = user.City;
                userAccount.CreationDate = user.Created;
                userAccount.Email = user.UserEmail;
                userAccount.FirstName = user.UserFName;
                //now the teh RBAC user that last modified this.
                if (user.LastEditedUserId.HasValue)
                {
                    userAccount.LastEditiedByUserId = user.LastEditedUserId.Value;
                    userAccount.LastEditiedByUserName = (new UserFactory()).GetUsername(user.LastEditedUserId.Value);
                }
                userAccount.LastName = user.UserLName;
                userAccount.LastUserAccess = user.LastUsed;
                userAccount.Notes = user.UserNote;
                userAccount.Phone = user.PhoneNumber;
                userAccount.PostalCode = user.PostalCode;
                userAccount.State = user.AddressState;
                userAccount.UserId = user.UserID;

                //CC info
                var card = user.DiscountUserCards.OrderByDescending(x=>x.RegisteredTS).FirstOrDefault();
                if (card != null)
                {
                    userAccount.CreditCardExpirationDate = card.CardExpiry;
                    userAccount.CreditCardLast4Digits = card.CardNumLast4;
                    //now get the most recent transaction base don this cards Hash value
                    var hash = card.PANHash;
                    //not get all transactiosn from the TransactionsCreditCard table that matches this hash.
                    var mostrecentTransaction = PemsEntities.TransactionsCreditCards.OrderByDescending(x => x.TransDateTime).FirstOrDefault(x => x.CardNumHash == hash);
                    if (mostrecentTransaction != null)
                    {
                        userAccount.LastDiscountUsedTime = mostrecentTransaction.TransDateTime;
                        //get the name of the scheme here
                        if (mostrecentTransaction.DiscountScheme != null)
                            userAccount.LastDiscountUsedName = mostrecentTransaction.DiscountScheme.SchemeName;
                    }
                }

                //now populate the scheme details
                var scheme = PemsEntities.DiscountUserSchemes.FirstOrDefault(x => x.DiscountUserSchemeId == userSchemeId);
                if (scheme != null)
                {
                    userAccount.DiscountScheme.ApplicationDate = scheme.CreatedTS;
                    userAccount.DiscountScheme.SchemeName = scheme.DiscountScheme.SchemeName;
                    userAccount.DiscountScheme.ApplicationStatus = scheme.DiscountSchemeStatu == null ? "" : scheme.DiscountSchemeStatu.DiscountSchemeStatusDesc;
                    userAccount.DiscountScheme.ApplicationStatusDate = scheme.SchemeStatusDate;
                    userAccount.DiscountScheme.ApplicationStatusNote = scheme.StatusNote;
                    userAccount.DiscountScheme.ExpirationDate = scheme.ExpiryTS;
                    userAccount.DiscountScheme.ApplicationStatusId = scheme.DiscountSchemeStatu == null ? 0 : scheme.DiscountSchemeStatu.DiscountSchemeStatusId;
                    userAccount.DiscountScheme.DiscountUserSchemeId = scheme.DiscountUserSchemeId;
                    //now the teh RBAC user that last modified this.
                    if (scheme.ModifiedByUserId.HasValue)
                    {
                        userAccount.DiscountScheme.LastEditiedByUserId = scheme.ModifiedByUserId.Value;
                        userAccount.DiscountScheme.LastEditiedByUserName = (new UserFactory()).GetUsername(scheme.ModifiedByUserId.Value);
                    }
                }
            }
            return userAccount;
        }

        /// <summary>
        /// Approves a discount Scheme for a user
        /// </summary>
        public void ApproveApplication(int userID, int userSchemeId, string notes, DateTime localTime)
        {
           //get the discountuserscheme
            var userScheme = UpdateSchemeStatus(userSchemeId, notes, localTime, 2);
            if (userScheme != null)
            {
                //create a audit log of the change
                AuditChanges(localTime, userScheme);

                //need to update count for approvedcount for this user.
                var user = PemsEntities.Users.FirstOrDefault(x => x.UserID == userID);
                if (user != null)
                {
                    user.ApprovedCount++;
                    user.PendingCount--; 
                    PemsEntities.SaveChanges();
                }
                //email user of action
                EmailStatusChange(userID, userScheme.SchemeId, Constants.DiscountScheme.ApprovalEmailTemplateId);
            }
        }

        /// <summary>
        /// Rejects a discount scheme for a user
        /// </summary>
        public void RejectApplication(int userID, int userSchemeId, string notes, DateTime localTime)
        {
            //get the discountuserscheme
            var userScheme = UpdateSchemeStatus(userSchemeId, notes, localTime, 3, false);
            //create a audit log of the change
            if (userScheme != null)
            {
                AuditChanges(localTime, userScheme);
                //need to update count for approvedcount for this user.
                var user = PemsEntities.Users.FirstOrDefault(x => x.UserID == userID);
                if (user != null)
                {
                    user.RejectedCount++;
                    user.PendingCount--;
                    PemsEntities.SaveChanges();
                }
                //email user of action
                EmailStatusChange(userID,userScheme.SchemeId, Constants.DiscountScheme.RejectionEmailTemplateId);
            }
        }

        /// <summary>
        /// Emails the user notice of the application status change
        /// </summary>
        private void EmailStatusChange(int userId,int schemeId, int templateId)
        {
            //get the users email
            var user = PemsEntities.Users.FirstOrDefault(x => x.UserID == userId);
            if (user != null )
            {
              
                //now get the DiscountSchemeEmailTemplate for this customer and templateID passed in. this will get us the subject and body fo the email
                var dsEmailTemplate = PemsEntities.DiscountSchemeEmailTemplates.FirstOrDefault(x => x.CustomerId ==user.DefaultCustomerID && x.EmailTemplateTypeId == templateId);
                if (dsEmailTemplate != null)
                {
                    var to = user.UserEmail;
                    var from = "info@duncansolutions.com";
                    string subject = dsEmailTemplate.EmailSubject;
                    var body = dsEmailTemplate.EmailText;
                    //now lets get the "from". get the DSCustomerInfo for this templateID , customerid and scheme id
                    var dsCustomerInfo =
                        PemsEntities.DiscountSchemeCustomerInfoes.FirstOrDefault(
                            x => x.CustomerId == user.DefaultCustomerID
                                 && x.DiscountSchemeEmailTemplateId == dsEmailTemplate.DiscountSchemeEmailTemplateId
                                 && x.DiscountSchemeId == schemeId);
                    if (dsCustomerInfo != null)
                        from = dsCustomerInfo.FromEmailAddress;

                    //send email to user
                    var mailer = new EmailFactory();
                    mailer.UserSchemeStatus(to, from, subject, body, "EmailStatusChange").Send();
                }
            }
        }


        /// <summary>
        /// Records a approval or rejection int he DB
        /// </summary>
        private void AuditChanges(DateTime localTime, DiscountUserScheme userScheme)
        {
                var audit = new DiscountUserSchemeAudit
                {
                    CardId = userScheme.CardId,
                    ChangedByUserId = WebSecurity.CurrentUserId,
                    ChangedDate = localTime,
                    CreatedTS = userScheme.CreatedTS,
                    CustomerId = userScheme.CustomerId,
                    DiscountUserSchemeId = userScheme.DiscountUserSchemeId,
                    ExpiryTS = userScheme.ExpiryTS,
                    ModifiedByUserId = userScheme.ModifiedByUserId,
                    SchemeId = userScheme.SchemeId,
                    SchemeStatus = userScheme.SchemeStatus,
                    SchemeStatusDate = userScheme.SchemeStatusDate,
                    StatusNote = userScheme.StatusNote,
                    UserId = userScheme.UserId
                };
                PemsEntities.DiscountUserSchemeAudits.Add(audit);
                PemsEntities.SaveChanges();
            
        }

        /// <summary>
        /// updates the status of a discountuser scheme with notes
        /// </summary>
        private DiscountUserScheme UpdateSchemeStatus(int userSchemeId, string notes, DateTime localTime, int statusId,
                                                      bool setExpiry = true)
        {
            //get the discountuserscheme
            var userScheme = PemsEntities.DiscountUserSchemes.FirstOrDefault(x => x.DiscountUserSchemeId == userSchemeId);
            if (userScheme != null)
            {
                //update the item in the db with the new status, notes, last mod by, and status date
                userScheme.StatusNote = notes;
                userScheme.ModifiedByUserId = WebSecurity.CurrentUserId;
                userScheme.SchemeStatusDate = localTime;
                userScheme.SchemeStatus = statusId;

                //we only set a exp if its an approved, not a rejected

                if (setExpiry)
                {
                    //we have to set the expiration date of the discount user scheme to the expiration duration of the discount scheme.

                    //we will default it to a year if they dont have an expiration set rof rthis discount scheme
                    var originalDiscountSchemeID = userScheme.DiscountScheme.DiscountSchemeExpirationTypeId ?? 1;
                    var discountSchemeExpirationType = (DiscountExpirationType) originalDiscountSchemeID;
                    if (discountSchemeExpirationType == DiscountExpirationType.Monthly)
                        userScheme.ExpiryTS = localTime.AddMonths(1);
                    else if (discountSchemeExpirationType == DiscountExpirationType.Weekly)
                        userScheme.ExpiryTS = localTime.AddDays(7);
                    else if (discountSchemeExpirationType == DiscountExpirationType.Yearly)
                        userScheme.ExpiryTS = localTime.AddYears(1);
                }
                PemsEntities.SaveChanges();
            }
            return userScheme;
        }

        #endregion

    }
}
