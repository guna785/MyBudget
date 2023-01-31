using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MyBudget.Infrastructure.Helpers;
using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Interfaces.Services.Identity;
using MyBudget.Application.Requests.Identity;
using MyBudget.Application.Responses.Identity;
using MyBudget.Shared.Constants.Permission;
using MyBudget.Shared.Constants.Role;
using MyBudget.Shared.Wrapper;

namespace MyBudget.Infrastructure.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleClaimService _roleClaimService;
        private readonly IStringLocalizer<RoleService> _localizer;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public RoleService(
            RoleManager<ApplicationRole> roleManager,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IRoleClaimService roleClaimService,
            IStringLocalizer<RoleService> localizer,
            ICurrentUserService currentUserService)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
            _roleClaimService = roleClaimService;
            _localizer = localizer;
            _currentUserService = currentUserService;
        }

        public async Task<Result<string>> DeleteAsync(int id)
        {
            ApplicationRole existingRole = await _roleManager.FindByIdAsync(id.ToString());
            if (existingRole.Name is not RoleConstants.AdministratorRole and not RoleConstants.BasicRole)
            {
                bool roleIsNotUsed = true;
                List<ApplicationUser> allUsers = await _userManager.Users.ToListAsync();
                foreach (ApplicationUser? user in allUsers)
                {
                    if (await _userManager.IsInRoleAsync(user, existingRole.Name))
                    {
                        roleIsNotUsed = false;
                    }
                }
                if (roleIsNotUsed)
                {
                    _ = await _roleManager.DeleteAsync(existingRole);
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Deleted."], existingRole.Name));
                }
                else
                {
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Not allowed to delete {0} Role as it is being used."], existingRole.Name));
                }
            }
            else
            {
                return await Result<string>.SuccessAsync(string.Format(_localizer["Not allowed to delete {0} Role."], existingRole.Name));
            }
        }

        public async Task<Result<List<RoleResponse>>> GetAllAsync()
        {
            List<ApplicationRole> roles = await _roleManager.Roles.ToListAsync();
            List<RoleResponse> rolesResponse = _mapper.Map<List<RoleResponse>>(roles);
            return await Result<List<RoleResponse>>.SuccessAsync(rolesResponse);
        }

        public async Task<Result<PermissionResponse>> GetAllPermissionsAsync(int roleId)
        {
            PermissionResponse model = new();
            List<RoleClaimResponse> allPermissions = GetAllPermissions();
            ApplicationRole role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role != null)
            {
                model.RoleId = role.Id;
                model.RoleName = role.Name;
                Result<List<RoleClaimResponse>> roleClaimsResult = await _roleClaimService.GetAllByRoleIdAsync(role.Id);
                if (roleClaimsResult.Succeeded)
                {
                    List<RoleClaimResponse> roleClaims = roleClaimsResult.Data;
                    List<string> allClaimValues = allPermissions.Select(a => a.Value).ToList();
                    List<string> roleClaimValues = roleClaims.Select(a => a.Value).ToList();
                    List<string> authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
                    foreach (RoleClaimResponse permission in allPermissions)
                    {
                        if (authorizedClaims.Any(a => a == permission.Value))
                        {
                            permission.Selected = true;
                            RoleClaimResponse? roleClaim = roleClaims.SingleOrDefault(a => a.Value == permission.Value);
                            if (roleClaim?.Description != null)
                            {
                                permission.Description = roleClaim.Description;
                            }
                            if (roleClaim?.Group != null)
                            {
                                permission.Group = roleClaim.Group;
                            }
                        }
                    }
                }
                else
                {
                    model.RoleClaims = new List<RoleClaimResponse>();
                    return await Result<PermissionResponse>.FailAsync(roleClaimsResult.Messages);
                }
            }
            model.RoleClaims = allPermissions;
            return await Result<PermissionResponse>.SuccessAsync(model);
        }

        private List<RoleClaimResponse> GetAllPermissions()
        {
            List<RoleClaimResponse> allPermissions = new();

            #region GetPermissions

            allPermissions.GetAllPermissions();

            #endregion GetPermissions

            return allPermissions;
        }

        public async Task<Result<RoleResponse>> GetByIdAsync(int id)
        {
            ApplicationRole? roles = await _roleManager.Roles.SingleOrDefaultAsync(x => x.Id == id);
            RoleResponse rolesResponse = _mapper.Map<RoleResponse>(roles);
            return await Result<RoleResponse>.SuccessAsync(rolesResponse);
        }

        public async Task<Result<string>> SaveAsync(RoleRequest request)
        {
            if (request.Id is null or 0)
            {
                request.Id = 0;
                ApplicationRole existingRole = await _roleManager.FindByNameAsync(request.Name);
                if (existingRole != null)
                {
                    return await Result<string>.FailAsync(_localizer["Similar Role already exists."]);
                }

                IdentityResult response = await _roleManager.CreateAsync(new ApplicationRole(request.Name, request.Description));
                return response.Succeeded
                    ? await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Created."], request.Name))
                    : await Result<string>.FailAsync(response.Errors.Select(e => _localizer[e.Description].ToString()).ToList());
            }
            else
            {
                ApplicationRole existingRole = await _roleManager.FindByIdAsync(request.Id.ToString());
                if (existingRole.Name is RoleConstants.AdministratorRole or RoleConstants.BasicRole)
                {
                    return await Result<string>.FailAsync(string.Format(_localizer["Not allowed to modify {0} Role."], existingRole.Name));
                }
                existingRole.Name = request.Name;
                existingRole.NormalizedName = request.Name.ToUpper();
                existingRole.Description = request.Description;
                _ = await _roleManager.UpdateAsync(existingRole);
                return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Updated."], existingRole.Name));
            }
        }

        public async Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request)
        {
            try
            {
                List<string> errors = new();
                ApplicationRole role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
                if (role.Name == RoleConstants.AdministratorRole)
                {
                    ApplicationUser currentUser = await _userManager.Users.SingleAsync(x => x.UserName == _currentUserService.UserName);
                    if (await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
                    {
                        return await Result<string>.FailAsync(_localizer["Not allowed to modify Permissions for this Role."]);
                    }
                }

                List<RoleClaimRequest> selectedClaims = request.RoleClaims.Where(a => a.Selected).ToList();
                if (role.Name == RoleConstants.AdministratorRole)
                {
                    if (!selectedClaims.Any(x => x.Value == Permissions.Roles.View)
                       || !selectedClaims.Any(x => x.Value == Permissions.RoleClaims.View)
                       || !selectedClaims.Any(x => x.Value == Permissions.RoleClaims.Edit))
                    {
                        return await Result<string>.FailAsync(string.Format(
                            _localizer["Not allowed to deselect {0} or {1} or {2} for this Role."],
                            Permissions.Roles.View, Permissions.RoleClaims.View, Permissions.RoleClaims.Edit));
                    }
                }

                IList<System.Security.Claims.Claim> claims = await _roleManager.GetClaimsAsync(role);
                foreach (System.Security.Claims.Claim? claim in claims)
                {
                    _ = await _roleManager.RemoveClaimAsync(role, claim);
                }
                foreach (RoleClaimRequest? claim in selectedClaims)
                {
                    IdentityResult addResult = await _roleManager.AddPermissionClaim(role, claim.Value);
                    if (!addResult.Succeeded)
                    {
                        errors.AddRange(addResult.Errors.Select(e => _localizer[e.Description].ToString()));
                    }
                }

                Result<List<RoleClaimResponse>> addedClaims = await _roleClaimService.GetAllByRoleIdAsync(role.Id);
                if (addedClaims.Succeeded)
                {
                    foreach (RoleClaimRequest? claim in selectedClaims)
                    {
                        RoleClaimResponse? addedClaim = addedClaims.Data.SingleOrDefault(x => x.Type == claim.Type && x.Value == claim.Value);
                        if (addedClaim != null)
                        {
                            claim.Id = addedClaim.Id;
                            claim.RoleId = addedClaim.RoleId;
                            Result<string> saveResult = await _roleClaimService.SaveAsync(claim);
                            if (!saveResult.Succeeded)
                            {
                                errors.AddRange(saveResult.Messages);
                            }
                        }
                    }
                }
                else
                {
                    errors.AddRange(addedClaims.Messages);
                }

                return errors.Any() ? await Result<string>.FailAsync(errors) : await Result<string>.SuccessAsync(_localizer["Permissions Updated."]);
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync(ex.Message);
            }
        }

        public async Task<int> GetCountAsync()
        {
            int count = await _roleManager.Roles.CountAsync();
            return count;
        }
    }
}
