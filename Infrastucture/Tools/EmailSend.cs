using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastucture.Tools
{
    public interface IEmailSend
    {
        Task SendEmailAsync(EmailModel email);
    }
    public class EmailSend : IEmailSend
    {
        public async Task SendEmailAsync(EmailModel email)
        {
            MailMessage message = new MailMessage()
            {
                From = new MailAddress("address", "displayName"),
                To = { email.To },
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true
            };
            SmtpClient smtpClient = new SmtpClient("host OR Gmail", 25)
            {
                Credentials = new NetworkCredential("email", "password"),
                EnableSsl = false
            };
            smtpClient.Send(message);
            await Task.CompletedTask;
            //not-encrypted 25
            //secure tls 587
            //secure ssl 465
        }
    }
    public class EmailModel
    {
        public EmailModel(string to, string subject, string body)
        {
            To = to;
            Subject = subject;
            Body = body;
        }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
