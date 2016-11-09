using System.Net.Mail;
using Mvc.Mailer;

namespace Duncan.PEMS.Web.Mailers
{
    /// <summary>
    /// This class contains methods used to send user emails
    /// https://github.com/smsohan/MvcMailer/wiki/MvcMailer-Step-by-Step-Guide
    /// </summary>
    public class UserMailer : MailerBase
    {
        public UserMailer()
        {
            MasterName = "_Layout";
        }

        public virtual MvcMailMessage PasswordReset(string token, string to)
        {
            ViewBag.Token = token;

            return Populate( x =>
                                 {
                                     x.Subject = "Duncan PEMS - Forgot Password";
                                     x.ViewName = "ForgotPassword";
                                     x.To.Add( to );
                                 } );
        }

        public virtual MvcMailMessage ForgotUsername(string username, string to)
        {
            ViewBag.Username = username;
            return Populate(x =>
            {
                x.Subject = "Duncan PEMS - Forgot Username";
                x.ViewName = "ForgotUsername";
                x.To.Add(to);
            });
        }
     

    }
}