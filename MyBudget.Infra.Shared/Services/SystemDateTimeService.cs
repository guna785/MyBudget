using MyBudget.Application.Interfaces.Services;

namespace MyBudget.Infra.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}
