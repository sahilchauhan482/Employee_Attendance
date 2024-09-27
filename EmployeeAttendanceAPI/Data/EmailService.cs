using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace EmployeeAPI.Data
{
    public class EmailService : IEmailSender
    {
        private readonly EmailSettings _setting;
        public EmailService(IOptions<EmailSettings> setting)
        {
            _setting = setting.Value;
        }

        public async Task Execute(string email, string subject, string Message)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _setting.ToEmail : email;
                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(_setting.FromEmail, "HR,Sofwiz Infotech"),
                };
                mailMessage.To.Add(toEmail);
                mailMessage.CC.Add(_setting.CcEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = Message;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.High;
                using (SmtpClient smtp = new SmtpClient(_setting.PrimaryDomain, _setting.PrimaryPort))
                {
                    smtp.Credentials = new NetworkCredential(_setting.UserNameEmail, _setting.UserNamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mailMessage);
                };
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Execute(email, subject, htmlMessage).Wait();
            return Task.FromResult(0);
        }
    }
}
