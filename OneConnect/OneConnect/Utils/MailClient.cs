using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace OneConnect.Utils
{
    public static class MailClient
    {
        private static readonly SmtpClient Client;
        static MailClient()
        {
            Client = new SmtpClient
            {
                Host =ConfigurationManager.AppSettings["SmtpServer"],
                Port =Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]), DeliveryMethod = SmtpDeliveryMethod.Network

            };
            Client.UseDefaultCredentials = false;
            Client.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
            Client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SmtpUser"],ConfigurationManager.AppSettings["SmtpPass"]);
        }


        public static bool SendMessage(string from, string to, string subject, bool isBodyHTML, string body)
        {
            MailMessage mm = null;
            bool isSent = false;
            try
            {
                // Create our message
                mm = new MailMessage(from, to);
                mm.Subject = subject;
                mm.IsBodyHtml = isBodyHTML;
                mm.Body = body;
                mm.DeliveryNotificationOptions =DeliveryNotificationOptions.OnFailure;
                // Send it
                Client.Send(mm);
                isSent = true;
            }
            // Catch any errors, these should be logged and
            // dealt with later
            catch (Exception ex)
            {
                // If you wish to log email errors,
                // add it here...
                var exMsg = ex.Message;
            }
            finally
            {
                mm.Dispose();
            }
            return isSent;
        }


    }
}