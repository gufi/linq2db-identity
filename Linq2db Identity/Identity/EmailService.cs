using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Linq2db_Identity.Identity
{
    public class EmailService : IIdentityMessageService
    {
        private static readonly string   _smtpHost = ConfigurationManager.AppSettings["smtpHost"];
        private static readonly int      _smtpPort = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
        private static readonly bool     _smtpEmableSSL = bool.Parse(ConfigurationManager.AppSettings["smtpEmableSSL"]);
        private static readonly int      _smtpTimeout = int.Parse(ConfigurationManager.AppSettings["smtpTimeout"]);
        private static readonly string   _smtpUserName = ConfigurationManager.AppSettings["smtpUserName"];
        private static readonly string   _smtpPassword = ConfigurationManager.AppSettings["smtpPassword"];
        private static readonly string   _smtpFromEmail = ConfigurationManager.AppSettings["smtpFromEmail"];
        private static readonly string   _smtpFromUser = ConfigurationManager.AppSettings["smtpFromUser"];

        public Task SendAsync(IdentityMessage message)
        {
            return configSendGridasync(message);
        }

        private Task configSendGridasync(IdentityMessage message)
        {
            var myMessage = new MailMessage();
            var client = new SmtpClient();
            client.Port = _smtpPort;
            client.Host = _smtpHost;
            client.EnableSsl = _smtpEmableSSL;
            client.Timeout = _smtpTimeout;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_smtpUserName, _smtpPassword);

            myMessage.To.Add(message.Destination);
            myMessage.From = new System.Net.Mail.MailAddress(
                _smtpFromEmail, _smtpFromUser);
            myMessage.Subject = message.Subject;
            myMessage.Body = message.Body;
            myMessage.IsBodyHtml = true;
#if !DEBUG
            client.Send(myMessage);
#endif
            return Task.FromResult(0);
        }
    }
}