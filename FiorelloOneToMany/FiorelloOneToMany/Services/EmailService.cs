using FiorelloOneToMany.Services.Interfaces;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using Microsoft.Extensions.Options;
using FiorelloOneToMany.Helpers;
using MailKit.Net.Smtp;

namespace FiorelloOneToMany.Services
{
    public class EmailService:IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> appSettings)
        {
            _emailSettings = appSettings.Value;
        }
        public void Send(string to, string subject, string html, string from = null)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _emailSettings.FromAdress));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Username, _emailSettings.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
