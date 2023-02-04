using Google.Apis.Auth.OAuth2;
using log4net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html, string from = null);
    }

    public class EmailService : IEmailService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly AppSettings _appSettings;

        public EmailService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public void Send(string to, string subject, string html, string from = null)
        {
            try
            {
                // create message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(from ?? _appSettings.EmailFrom));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = html };

                /* Start Adding security */
                //const string GMailAccount = "rejkid@gmail.com";

                //var clientSecrets = new ClientSecrets
                //{
                //    ClientId = "99280265451-a4rob8slq8hc6a83vu5m6egfhrl63tp4.apps.googleusercontent.com",
                //    ClientSecret = "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"
                //};




                //var certificate = new X509Certificate2(@"C:\path\to\certificate.p12", "password", X509KeyStorageFlags.Exportable);
                //var credential = new ServiceAccountCredential(new ServiceAccountCredential
                //    .Initializer("your-developer-id@developer.gserviceaccount.com")
                //{
                //    // Note: other scopes can be found here: https://developers.google.com/gmail/api/auth/scopes
                //    Scopes = new[] { "https://mail.google.com/" },
                //    User = "username@gmail.com"
                //}.FromCertificate(certificate));

                ////You can also use FromPrivateKey(privateKey) where privateKey
                //// is the value of the field 'private_key' in your serviceName.json file

                //bool result = await credential.RequestAccessTokenAsync(cancel.Token);

                // Note: result will be true if the access token was received successfully
                /* End Adding security */


                // send email
                using var smtp = new SmtpClient();


                smtp.Connect(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTls);
                smtp.Authenticate(_appSettings.SmtpUser, _appSettings.SmtpPass);
                smtp.Send(email);
                smtp.Disconnect(true);
                log.InfoFormat("Success sending e-mail \n Subject: {0} Message: {1} to: {2}",
                    subject,
                    html,
                    to);
            }
            catch (System.Exception ex)
            {
                log.InfoFormat("Failure sending e-mail \n Subject: {0} Message: {1} to: {2}",
                    subject,
                    html,
                    to);
                log.Warn(ex);

            }
        }
    }
}