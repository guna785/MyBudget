using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Shared.Constants.Permission;

namespace MyBudget.API.Controllers.Utilities
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditsController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;

        public AuditsController(ICurrentUserService currentUserService, IAuditService auditService)
        {
            _currentUserService = currentUserService;
            _auditService = auditService;
        }

        /// <summary>
        /// Get Current User Audit Trails
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.AuditTrails.View)]
        [HttpGet]
        public async Task<IActionResult> GetUserTrailsAsync()
        {
            return Ok(await _auditService.GetCurrentUserTrailsAsync(_currentUserService.UserName));
        }

        /// <summary>
        /// Search Audit Trails and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="searchInOldValues"></param>
        /// <param name="searchInNewValues"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.AuditTrails.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel(string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false)
        {
            MyBudget.Shared.Wrapper.IResult<string> data = await _auditService.ExportToExcelAsync(_currentUserService.UserName, searchString, searchInOldValues, searchInNewValues);
            return Ok(data);
        }
    }
}
