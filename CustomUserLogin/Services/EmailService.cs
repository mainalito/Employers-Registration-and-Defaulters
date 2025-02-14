
using CustomUserLogin.Intefaces;
using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string fromEmail, string toEmail, string subject, string body)
    {
        try
        {
            // Load SMTP settings from appsettings.json
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            string smtpHost = smtpSettings["Host"];
            string smtpPortString = smtpSettings["Port"];
            string smtpUser = smtpSettings["Username"];
            string smtpPass = smtpSettings["Password"];
            string enableSslString = smtpSettings["EnableSSL"];

            // Validate that all required settings are provided
            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPortString) ||
                string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass) || string.IsNullOrEmpty(enableSslString))
            {
                throw new ArgumentException("One or more SMTP settings are missing in appsettings.json");
            }


            var client = new SmtpClient(smtpHost, int.Parse(smtpPortString))
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };
            client.Send("from@example.com", toEmail, subject, body);
            System.Console.WriteLine("Sent");
           


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email sending failed: {ex.Message}");
            throw;
        }
    }

}
