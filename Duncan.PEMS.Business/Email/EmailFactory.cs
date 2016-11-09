using System.Net.Mail;
using Mvc.Mailer;

namespace Duncan.PEMS.Business.Email
{
   public class EmailFactory : MailerBase
    {
       /// <summary>
       /// Emails a user an update to thier discount scheme status
       /// </summary>
       public virtual MvcMailMessage UserSchemeStatus(string to, string from, string subject, string body,
                                                      string viewName)
       {
           return Populate(x =>
               {
                   x.ViewName = viewName;
                   x.Subject = subject;
                   x.Body = body;
                   x.From = new MailAddress(from);
                   x.To.Add(to);
               });
       }
    }
}
