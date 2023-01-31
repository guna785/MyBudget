

using MyBudget.Application.Interfaces.Common;

namespace MyBudget.Application.Interfaces.Services
{
    public interface ICurrentUserService : IService
    {
        int UserId { get; }
        string UserName { get; }
        string IpAddress { get; }
    }
}
