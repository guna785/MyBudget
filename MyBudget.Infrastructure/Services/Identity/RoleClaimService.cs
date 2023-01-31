using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MyBudget.Infrastructure.Contexts;
using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Interfaces.Services.Identity;
using MyBudget.Application.Requests.Identity;
using MyBudget.Application.Responses.Identity;
using MyBudget.Shared.Wrapper;

namespace MyBudget.Infrastructure.Services.Identity
{
    public class RoleClaimService : IRoleClaimService
    {
        private readonly IStringLocalizer<RoleClaimService> _localizer;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly ApplicationDbContext _db;

        public RoleClaimService(
            IStringLocalizer<RoleClaimService> localizer,
            IMapper mapper,
            ICurrentUserService currentUserService,
            ApplicationDbContext db)
        {
            _localizer = localizer;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _db = db;
        }

        public async Task<Result<List<RoleClaimResponse>>> GetAllAsync()
        {
            List<ApplicationRoleClaim> roleClaims = await _db.RoleClaims.ToListAsync();
            List<RoleClaimResponse> roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }

        public async Task<int> GetCountAsync()
        {
            int count = await _db.RoleClaims.CountAsync();
            return count;
        }

        public async Task<Result<RoleClaimResponse>> GetByIdAsync(int id)
        {
            ApplicationRoleClaim? roleClaim = await _db.RoleClaims
                .SingleOrDefaultAsync(x => x.Id == id);
            RoleClaimResponse roleClaimResponse = _mapper.Map<RoleClaimResponse>(roleClaim);
            return await Result<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
        }

        public async Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(int roleId)
        {
            List<ApplicationRoleClaim> roleClaims = await _db.RoleClaims
                .Include(x => x.Role)
                .Where(x => x.RoleId == roleId)
                .ToListAsync();
            List<RoleClaimResponse> roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }

        public async Task<Result<string>> SaveAsync(RoleClaimRequest request)
        {
            if (request.Id == 0)
            {
                ApplicationRoleClaim? existingRoleClaim =
                    await _db.RoleClaims
                        .SingleOrDefaultAsync(x =>
                            x.RoleId == request.RoleId && x.ClaimType == request.Type && x.ClaimValue == request.Value);
                if (existingRoleClaim != null)
                {
                    return await Result<string>.FailAsync(_localizer["Similar Role Claim already exists."]);
                }
                ApplicationRoleClaim roleClaim = _mapper.Map<ApplicationRoleClaim>(request);
                _ = await _db.RoleClaims.AddAsync(roleClaim);
                _ = await _db.SaveChangesAsync(_currentUserService.UserName);
                return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} created."], request.Value));
            }
            else
            {
                ApplicationRoleClaim? existingRoleClaim =
                    await _db.RoleClaims
                        .Include(x => x.Role)
                        .SingleOrDefaultAsync(x => x.Id == request.Id);
                if (existingRoleClaim == null)
                {
                    return await Result<string>.SuccessAsync(_localizer["Role Claim does not exist."]);
                }
                else
                {
                    existingRoleClaim.ClaimType = request.Type;
                    existingRoleClaim.ClaimValue = request.Value;
                    existingRoleClaim.Group = request.Group;
                    existingRoleClaim.Description = request.Description;
                    existingRoleClaim.RoleId = request.RoleId;
                    _ = _db.RoleClaims.Update(existingRoleClaim);
                    _ = await _db.SaveChangesAsync(_currentUserService.UserName);
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} for Role {1} updated."], request.Value, existingRoleClaim.Role.Name));
                }
            }
        }

        public async Task<Result<string>> DeleteAsync(int id)
        {
            ApplicationRoleClaim? existingRoleClaim = await _db.RoleClaims
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (existingRoleClaim != null)
            {
                _ = _db.RoleClaims.Remove(existingRoleClaim);
                _ = await _db.SaveChangesAsync(_currentUserService.UserName);
                return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} for {1} Role deleted."], existingRoleClaim.ClaimValue, existingRoleClaim.Role.Name));
            }
            else
            {
                return await Result<string>.FailAsync(_localizer["Role Claim does not exist."]);
            }
        }
    }
}
