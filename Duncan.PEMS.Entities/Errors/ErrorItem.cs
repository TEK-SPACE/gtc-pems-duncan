using System;
using System.ComponentModel.DataAnnotations;

namespace Duncan.PEMS.Entities.Errors
{
    [Serializable]
    public class ErrorItem
    {
        public ErrorItem()
        {
        }

        public ErrorItem(string errorCode, string errorMessage)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }

        public ErrorItem(string errorCode, Exception ex)
        {
            ErrorMessage = ActualMessage(ex);
            ErrorCode = errorCode;
        }
        


        [Display(Name = "ErrorCode")]
        public string ErrorCode { get; set; }

        [Display(Name = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        [Display(Name = "Locale")]
        public string Locale { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }


        private string ActualMessage(Exception ex)
        {
            // Walk exception where "inner exception" is in the message and pick
            // inner exception.
            while (ex.Message.Contains("inner exception") && ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex != null ? ex.Message : "Undefined exception trapped.";
        }


    }
}