using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBudget.Application.Interfaces.Services.Identity;
using MyBudget.Application.Requests.Identity;
using MyBudget.Shared.Constants.Permission;

namespace MyBudget.API.Controllers.Identity
{
    [Route("api/identity/roleClaim")]
    [ApiController]
    public class RoleClaimController : ControllerBase
    {
        private readonly IRoleClaimService _roleClaimService;

        public RoleClaimController(IRoleClaimService roleClaimService)
        {
            _roleClaimService = roleClaimService;
        }

        /// <summary>
        /// Get All Role Claims(e.g. Product Create Permission)
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            MyBudget.Shared.Wrapper.Result<List<MyBudget.Application.Responses.Identity.RoleClaimResponse>> roleClaims = await _roleClaimService.GetAllAsync();
            return Ok(roleClaims);
        }

        /// <summary>
        /// Get All Role Claims By Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.View)]
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetAllByRoleId([FromRoute] int roleId)
        {
            MyBudget.Shared.Wrapper.Result<List<MyBudget.Application.Responses.Identity.RoleClaimResponse>> response = await _roleClaimService.GetAllByRoleIdAsync(roleId);
            return Ok(response);
        }

        /// <summary>
        /// Add a Role Claim
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK </returns>
        [Authorize(Policy = Permissions.RoleClaims.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(RoleClaimRequest request)
        {
            MyBudget.Shared.Wrapper.Result<string> response = await _roleClaimService.SaveAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Delete a Role Claim
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            MyBudget.Shared.Wrapper.Result<string> response = await _roleClaimService.DeleteAsync(id);
            return Ok(response);
        }
    }
}
