using EFCore.BulkExtensions.SqlAdapters;
using NavOS.Basecode.Data;
using NavOS.Basecode.Data.Models;
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
                Credentials = new NetworkCredential("bookhubnavos@gmail.com", "fmlewnloinlybjha")
            };
        }
        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="host">The host.</param>
        /// <param name="adminName">Name of the admin.</param>
        /// <param name="adminId">The admin identifier.</param>
        /// <param name="token">The token.</param>
        public void PasswordReset(string email, string host, string adminName, string adminId, string token)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("bookhubnavos@gmail.com", "BookHub"),
                Subject = "BOOKHUB | PASSWORD RESET",
                Body = PasswordResetEmailBody(host, adminName, adminId, token),
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(email));

            mail.Send(mailMessage);
        }
        /// <summary>
        /// Sends a thank-you email for reviewing/rating a book.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="host">The host.</param>
        /// <param name="username">The username.</param>
        /// <param name="bookTitle">The book title.</param>
        public void UserFeedBackReview(string email, string host, string username, string bookTitle)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("bookhubnavos@gmail.com", "BookHub"),
                Subject = "BOOKHUB | USER REVIEW",
                Body = ReviewEmailBody(host, username, bookTitle),
                IsBodyHtml = true
            };

			mailMessage.To.Add(new MailAddress(email));

			mail.Send(mailMessage);
		}

        #region email bodies	
        /// <summary>
        /// Email body for Reset Password.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="adminName">Name of the admin.</param>
        /// <param name="adminId">The admin identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        private static string PasswordResetEmailBody(string host, string adminName, string adminId, string token)
        {
            string emailBody = string.Empty;
            string emailTemplatePath = PathManager.DirectoryPath.EmailTemplateDirectory;
            string emailBodyFilePath = Path.Combine(emailTemplatePath, "reset-password") + ".html";

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
        /// <summary>
        /// Thank-you email for book review/rating.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="username">The username.</param>
        /// <param name="bookTitle">The book title.</param>
        /// <returns></returns>
        private static string ReviewEmailBody(string host, string username, string bookTitle)
        {
			string emailBody = string.Empty;
			string emailTemplatePath = PathManager.DirectoryPath.EmailTemplateDirectory;
			string emailBodyFilePath = Path.Combine(emailTemplatePath, "user-feedback-review") + ".html";

			if (File.Exists(emailBodyFilePath))
			{
				emailBody = File.ReadAllText(emailBodyFilePath);
			}
			else
			{
				emailBody = "Email body template not found.";
			}

			emailBody = emailBody.Replace("{Host}", host);
			emailBody = emailBody.Replace("{UserName}", username);
			emailBody = emailBody.Replace("{BookTitle}", bookTitle);

			return emailBody;
		}
		#endregion
	}
}