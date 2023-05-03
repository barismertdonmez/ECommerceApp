namespace WebUI.EmailServices
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string emaili,string subject,string htmlMessage);
    }
}
