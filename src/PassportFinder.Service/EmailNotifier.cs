using Microsoft.Extensions.Logging;
using PassportFinder.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PassportFinder.Service
{
    public class EmailNotifier : IEmailNotifier
    {
        private readonly ILogger<EmailNotifier> _logger;

        public EmailNotifier(ILogger<EmailNotifier> logger)
        {
            this._logger = logger;
        }

        public bool Send(string[] to, string subject, string messageTxt)
        {
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;

            // fill here your SMTP server connection options
            
            var message = new MailMessage()
            {
                IsBodyHtml = true,
                Subject = subject,
                Body = messageTxt
            };

            foreach (var t in to)
                message.To.Add(t);

            try
            {
                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error sending email", new object[0]);
            }

            return false;
        }
    }
}
