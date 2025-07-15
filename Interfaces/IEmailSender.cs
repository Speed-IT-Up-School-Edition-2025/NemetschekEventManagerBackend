using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace NemetschekEventManagerBackend.Interfaces
{
    public class GmailEmailSender : IEmailSender
    {
        private readonly string _gmailUser;
        private readonly string _gmailPass;

        public GmailEmailSender(string gmailUser, string gmailPass)
        {
            _gmailUser = gmailUser;
            _gmailPass = gmailPass;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_gmailUser));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            message.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.abv.bg", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_gmailUser, _gmailPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendEventDeletionEmail(string userEmail, string userName, string eventName, DateTime eventDate)
        {
            string subject = $"Събитието е отменено: {eventName}";

            string htmlMessage = $@"
            <html>
                <body>
                    <p>Уважаеми/а {userName},</p>
                    <p>Съжаляваме да ви информираме, че събитието <strong>{eventName}</strong>, насрочено за {eventDate:MMMM d, yyyy}, е отменено.</p>
                    <p>В резултат на това вашето участие в това събитие е <strong>окончателно изтрито</strong>.</p>
                    <p>Ако имате въпроси, моля свържете се с администратор.</p>
                </body>
            </html>";

            await SendEmailAsync(userEmail, subject, htmlMessage);
        }

    }
}