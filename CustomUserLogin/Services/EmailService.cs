using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace CustomUserLogin.Services
{
    public class EmailService : Controller
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
                {
                    Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                    Credentials = new NetworkCredential(
                        _configuration["EmailSettings:SmtpUsername"],
                        _configuration["EmailSettings:SmtpPassword"]
                    ),
                    EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"])
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("test@example.com"), // Use a fake sender email
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true, // Set to true if sending HTML emails
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }
    }
}
