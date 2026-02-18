using GymManagmentBLL.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IGymSettingsService _settingsService;

        public EmailService(IConfiguration configuration, IGymSettingsService settingsService)
        {
            _configuration = configuration;
            _settingsService = settingsService;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var settings = await _settingsService.GetSettingsAsync();
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var smtpUser = _configuration["EmailSettings:SmtpUser"] ?? "ahmedomar11345@gmail.com";
            var smtpPass = _configuration["EmailSettings:SmtpPass"] ?? "your-app-password";

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            // Add Footer to message
            var fullMessage = $@"
                <div style='font-family: sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                    <div style='padding-bottom: 20px; border-bottom: 2px solid #f8f9fa; margin-bottom: 20px;'>
                        <h2 style='color: #0d6efd; margin: 0;'>{settings.GymName}</h2>
                    </div>
                    <div>{message}</div>
                    <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #777;'>
                        <p style='margin: 5px 0;'><strong>{settings.GymName}</strong></p>
                        <p style='margin: 5px 0;'>📍 {settings.Address}</p>
                        <p style='margin: 5px 0;'>📞 {settings.Phone}</p>
                        <p style='margin: 15px 0 0 0; opacity: 0.5;'>Powered by IronPulse System</p>
                    </div>
                </div>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser, settings.GymName),
                Subject = subject,
                Body = fullMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }

        public async Task SendEmailWithImageAsync(string email, string subject, string htmlBody, byte[] imageBytes, string imageContentId)
        {
            var settings = await _settingsService.GetSettingsAsync();
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var smtpUser = _configuration["EmailSettings:SmtpUser"] ?? "ahmedomar11345@gmail.com";
            var smtpPass = _configuration["EmailSettings:SmtpPass"] ?? "your-app-password";

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            // Add Header and Footer to the htmlBody
            var fullHtmlBody = $@"
                <div style='font-family: sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                    <div style='padding-bottom: 20px; border-bottom: 2px solid #f8f9fa; margin-bottom: 20px;'>
                        <h2 style='color: #0d6efd; margin: 0;'>{settings.GymName}</h2>
                    </div>
                    <div>{htmlBody}</div>
                    <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #777;'>
                        <p style='margin: 5px 0;'><strong>{settings.GymName}</strong></p>
                        <p style='margin: 5px 0;'>📍 {settings.Address}</p>
                        <p style='margin: 5px 0;'>📞 {settings.Phone}</p>
                        <p style='margin: 15px 0 0 0; opacity: 0.5;'>Powered by IronPulse System</p>
                    </div>
                </div>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser, settings.GymName),
                Subject = subject,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            // Create the HTML view
            var htmlView = AlternateView.CreateAlternateViewFromString(fullHtmlBody, null, MediaTypeNames.Text.Html);

            // Create the embedded image as LinkedResource
            using var imageStream = new MemoryStream(imageBytes);
            var linkedResource = new LinkedResource(imageStream, MediaTypeNames.Image.Png)
            {
                ContentId = imageContentId,
                TransferEncoding = TransferEncoding.Base64
            };
            htmlView.LinkedResources.Add(linkedResource);

            mailMessage.AlternateViews.Add(htmlView);

            await client.SendMailAsync(mailMessage);
        }
    }
}
