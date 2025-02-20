using EMS.Domain.Identity;
using EMS.Domain.Models;
using EMS.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EMS.Service
{
    public class MailService : IMailService
    {
        private readonly UserManager<EMSApplicationUser> _userManager;
        private readonly SmtpSettings _smtpSettings;

        public MailService(IOptions<SmtpSettings> smtpSettings, UserManager<EMSApplicationUser> userManager)
        {
            _smtpSettings = smtpSettings.Value;
            _userManager = userManager;
        }

        public async Task<bool> SendEmailToUser(string userId, string subject, string body)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var mailData = new MailData
                {
                    ToEmail = user.Email,
                    ToName = user.UserName,
                    Subject = subject,
                    Body = body
                };

                try
                {
                    await SendEmailAsync(mailData);
                    return true;
                }
                catch
                {
                    // Log the exception or handle the failure as needed
                    return false;
                }
            }

            return false;
        }

        public async Task SendEmailAsync(MailData mailData)
        {
            using var smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = mailData.Subject,
                Body = mailData.Body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(mailData.ToEmail, mailData.ToName));

            await smtpClient.SendMailAsync(mailMessage);
        }
    }

}
