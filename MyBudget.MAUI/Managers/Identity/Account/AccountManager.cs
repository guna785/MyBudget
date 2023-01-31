using MyBudget.Application.Requests.Identity;
using MyBudget.Shared.Wrapper;
using MyBudget.UI.Infrastructure.Extensions;
using MyBudget.UI.Infrastructure.Routes;
using System.Net.Http.Json;

namespace MyBudget.MAUI.Managers.Identity.Account
{
    public class AccountManager : IAccountManager
    {
        private readonly HttpClient _httpClient;

        public AccountManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest model)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(AccountEndpoints.ChangePassword, model);
            return await response.ToResult();
        }

        public async Task<IResult> UpdateProfileAsync(UpdateProfileRequest model)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(AccountEndpoints.UpdateProfile, model);
            return await response.ToResult();
        }

        public async Task<IResult<string>> GetProfilePictureAsync(string userId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(AccountEndpoints.GetProfilePicture(userId));
            return await response.ToResult<string>();
        }

        public async Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(AccountEndpoints.UpdateProfilePicture(userId), request);
            return await response.ToResult<string>();
        }
    }
}
