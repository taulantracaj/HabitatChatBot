namespace Sitecore.Feature.ChatBotHabitat.Utils
{
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Configuration;

    public static class EmailSender
    {
        //private static string apiKey = ConfigurationManager.AppSettings["SendGridKey"];
        public static async Task<bool> SendEmail(string recipient, string sender, string subject, string body)
        {
            try
            {
                var msg = new MailMessage(sender, recipient, subject, body);
                var client = new SmtpClient();
                client.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
