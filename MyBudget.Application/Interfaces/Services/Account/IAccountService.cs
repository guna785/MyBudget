using MyBudget.Application.Interfaces.Common;
using MyBudget.Application.Requests.Identity;
using MyBudget.Shared.Wrapper;

namespace MyBudget.Application.Interfaces.Services.Account
{
    public interface IAccountService : IService
    {
        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model, int userId);

        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, int userId);

        Task<IResult<string>> GetProfilePictureAsync(int userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, int userId);
    }
}
