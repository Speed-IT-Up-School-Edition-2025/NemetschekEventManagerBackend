using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend;

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
        string subject = $"Event Cancelled: {eventName}";

        string htmlMessage = $@"
        <html>
            <body>
                <p>Dear {userName},</p>
                <p>We regret to inform you that the event <strong>{eventName}</strong> scheduled for {eventDate:MMMM d, yyyy} has been cancelled.</p>
                <p>As a result, your submission for this event has been <strong>permanently deleted</strong>.</p>
                <p>If you have any questions, please contact an administrator .</p>
            </body>
        </html>";

        await SendEmailAsync(userEmail, subject, htmlMessage);
    }

}
