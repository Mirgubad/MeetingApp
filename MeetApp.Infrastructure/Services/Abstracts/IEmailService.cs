namespace MeetApp.Infrastructure.Services.Abstracts
{
    public interface IEmailService
    {
        Task<bool> SendMailAsync(string to, string subject, string body);
    }
}
