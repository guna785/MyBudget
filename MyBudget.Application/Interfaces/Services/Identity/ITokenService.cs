using MyBudget.Application.Interfaces.Common;
using MyBudget.Application.Requests.Identity;
using MyBudget.Application.Responses.Identity;
using MyBudget.Shared.Wrapper;

namespace MyBudget.Application.Interfaces.Services.Identity
{
    public interface ITokenService : IService
    {
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);

        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}
