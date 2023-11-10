using EFCore.BulkExtensions.SqlAdapters;
using NavOS.Basecode.Data;
using NavOS.Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Utilities
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpClient mail;

        public EmailSender()
        {
            mail = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential("frenchcries12@gmail.com", "ehlxzxdcksskipfe")
            };
        }

        public void PasswordReset(string email, string host, string adminName, string adminId, string token)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("frenchcries12@gmail.com"),
                Subject = "BOOKHUB | PASSWORD RESET",
                Body = GetMailBody(host, adminName, adminId, token),
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(email));

            mail.Send(mailMessage);
        }

        private static string GetMailBody(string host, string adminName, string adminId, string token)
        {
            string emailBody = string.Empty;
            string emailTemplatePath = PathManager.DirectoryPath.EmailTemplateDirectory;
            string emailBodyFilePath = Path.Combine(emailTemplatePath, "email-body") + ".html";

            if (File.Exists(emailBodyFilePath))
            {
                emailBody = File.ReadAllText(emailBodyFilePath);
            }
            else
            {
                emailBody = "Email body template not found.";
            }

            emailBody = emailBody.Replace("{AdminName}", adminName);
            emailBody = emailBody.Replace("{Host}", host);
            emailBody = emailBody.Replace("{AdminId}", adminId);
            emailBody = emailBody.Replace("{Token}", token);

            return emailBody;
        }
    }
}
