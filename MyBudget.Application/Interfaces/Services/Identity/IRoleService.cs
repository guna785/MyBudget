using MyBudget.Application.Interfaces.Common;
using MyBudget.Application.Requests.Identity;
using MyBudget.Application.Responses.Identity;
using MyBudget.Shared.Wrapper;

namespace MyBudget.Application.Interfaces.Services.Identity
{
    public interface IRoleService : IService
    {
        Task<Result<List<RoleResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleResponse>> GetByIdAsync(int id);

        Task<Result<string>> SaveAsync(RoleRequest request);

        Task<Result<string>> DeleteAsync(int id);

        Task<Result<PermissionResponse>> GetAllPermissionsAsync(int roleId);

        Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request);
    }
}
