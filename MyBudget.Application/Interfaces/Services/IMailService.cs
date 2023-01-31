using MyBudget.Application.Requests.Mail;

namespace MyBudget.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}
