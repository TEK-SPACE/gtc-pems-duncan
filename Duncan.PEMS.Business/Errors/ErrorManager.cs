using System;
using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Errors;

namespace Duncan.PEMS.Business.Errors
{
    public class ErrorManager : RbacBaseFactory
    {

        #region ErrorItem objects

        /// <summary>
        /// Gets an ErrorItem object by ErrorMessageId
        /// </summary> 
        /// <param name="errorId"></param>
        /// <returns></returns>
        public  ErrorItem GetErrorByErrorId(int errorId)
        {
            ErrorItem item = new ErrorItem();
            try
            {
                ErrorMessage mess = GetErrorMessageByErrorId(errorId);
                item = mess != null ? MakeErrorItem(mess) : null;
            }
            catch (Exception ex)
            {
                item = null;
            }
            return item;
        }

        /// <summary>
        /// Gets an ErrorItem object by errorCode and locale
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        public  ErrorItem GetErrorByErrorCodeAndLocale(string errorCode, string locale)
        {
            ErrorItem item = new ErrorItem();
            try
            {
                ErrorMessage mess = GetErrorMessageByErrorCodeAndLocale(errorCode, locale);
                if (mess != null)
                    item = MakeErrorItem(mess);
                else
                {
                    //the item was null, try to get the fallback, which is the error message for the 
                    //same error code but in english
                    mess = GetErrorMessageByErrorCodeAndLocale(errorCode, "en-us");
                    if (mess != null)
                    {
                        item = MakeErrorItem(mess);
                    }
                    else
                    {
                        item.Locale = locale;
                        item.ErrorCode = errorCode;
                        item.ErrorMessage = "Error Message Undefined";
                    }
                }
            }
            catch (Exception ex)
            {
                item.Locale = locale;
                item.ErrorCode = errorCode;
                item.ErrorMessage = "Error Message Undefined";
            }
            return item;
        }

        /// <summary>
        /// Creates an ErrorItem Object from an ErrorMessage object
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private  ErrorItem MakeErrorItem(ErrorMessage errorMessage)
        {
            ErrorItem item = new ErrorItem();
            if (errorMessage != null)
            {
                item.ErrorCode = errorMessage.ErrorCode;
                item.ErrorMessage = errorMessage.ErrorMessage1;
                item.Locale = errorMessage.Locale;
                return item;
            }
            else
                return null;
        }

        #endregion

        #region ErrorMessage objects

        /// <summary>
        /// Gets all the ErrorMessage objects from the database for a locale
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        public  List<ErrorMessage> GetAllErrorMessagesForLocale(string locale)
        {
            List<ErrorMessage> errorMessages = new List<ErrorMessage>();
            try
            {
                errorMessages = RbacEntities.ErrorMessages.Where(x => x.Locale == locale).ToList<ErrorMessage>();
            }
            catch (Exception ex)
            {
                errorMessages = null;
            }
            return errorMessages;
        }

        /// <summary>
        /// Gets all the ErrorMessage objects from the database
        /// </summary>
        /// <returns>Returns a list of <see cref="ErrorMessage"/> or empty list.</returns>
        public  List<ErrorMessage> GetAllErrorMessages()
        {
            List<ErrorMessage> errorMessages = new List<ErrorMessage>();
            try
            {
                errorMessages = RbacEntities.ErrorMessages.ToList<ErrorMessage>();
            }
            catch (Exception)
            {
                // Do nothing
            }
            return errorMessages;
        }

        /// <summary>
        /// Gets an ErrorMessage object by ErrorCode and Locale
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        private  ErrorMessage GetErrorMessageByErrorCodeAndLocale(string errorCode, string locale)
        {
            var message = new ErrorMessage();
            try
            {
                message = RbacEntities.ErrorMessages.FirstOrDefault(x => x.ErrorCode == errorCode && x.Locale == locale);
            }
            catch(Exception ex)
            {
                message = null;
            }

            return message;
        }

        /// <summary>
        /// Gets an ErrorMessage object by ErrorMessageId
        /// </summary>
        /// <param name="errorId"></param>
        /// <returns></returns>
        public  ErrorMessage GetErrorMessageByErrorId(int errorId)
        {
            var message = new ErrorMessage();
            try
            {
                message = RbacEntities.ErrorMessages.FirstOrDefault(x => x.ErrorMessageID == errorId);
            }
            catch (Exception ex)
            {
                message = null;
            }
            return message;
        }

        /// <summary>
        /// Create an ErrorMessage object in the database
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        public  ErrorMessage CreateErrorMessage(string errorCode, string errorMessage, string locale)
        {
            var newMessage = new ErrorMessage();
            try
            {
                newMessage.ErrorMessage1 = errorMessage;
                newMessage.Locale = locale;
                newMessage.ErrorCode = errorCode;
                newMessage = RbacEntities.ErrorMessages.Add(newMessage);
            }
            catch (Exception ex)
            {
                newMessage = null;
            }
            return newMessage;
        }

        /// <summary>
        /// Update an ErrorMessage object in the database
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="locale"></param>
        /// <param name="errorMessageId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public  ErrorMessage UpdateErrorMessage(string errorCode, string errorMessage, string locale, int errorMessageId, bool active)
        {
            try
            {
                ErrorMessage message = GetErrorMessageByErrorId(errorMessageId);
                if (message != null)
                {
                    message.ErrorCode = errorCode;
                    message.ErrorMessage1 = errorMessage;
                    message.Locale = locale;
                    message.Active = active;
                    message.DateModified = DateTime.Now;
                    RbacEntities.SaveChanges();
                }
                return message;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion


    }
}
