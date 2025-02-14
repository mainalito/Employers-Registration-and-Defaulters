namespace CustomUserLogin.Intefaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string fromEmail,string toEmail, string subject, string body);
    }
}
