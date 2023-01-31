using MyBudget.Application.Requests.Identity;
using MyBudget.Shared.Wrapper;
using System.Security.Claims;

namespace MyBudget.MAUI.Managers.Identity.Authentication
{
    public interface IAuthenticationManager : IManager
    {
        Task<IResult> Login(TokenRequest model);

        Task<IResult> Logout();

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

        Task<ClaimsPrincipal> CurrentUser();
    }
}
