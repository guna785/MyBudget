using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MyBudget.Infrastructure.Contexts;
using MyBudget.Infrastructure.Helpers;
using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Shared.Constants.Permission;
using MyBudget.Shared.Constants.Role;
using MyBudget.Shared.Constants.User;

namespace MyBudget.Infrastructure
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IStringLocalizer<DatabaseSeeder> _localizer;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public DatabaseSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext db,
            ILogger<DatabaseSeeder> logger,
            IStringLocalizer<DatabaseSeeder> localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
            _localizer = localizer;
        }

        public void Initialize()
        {
            AddAdministrator();
            //AddBasicUser();
            _ = _db.SaveChanges();
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                ApplicationRole adminRole = new(RoleConstants.AdministratorRole, _localizer["Administrator role with full permissions"]);
                ApplicationRole? adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                if (adminRoleInDb == null)
                {
                    _ = await _roleManager.CreateAsync(adminRole);
                    adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                    _logger.LogInformation(_localizer["Seeded Administrator Role."]);
                }
                //Check if User Exists
                ApplicationUser superUser = new()
                {
                    FirstName = "Super",
                    LastName = "User",
                    Email = "info@b2lsolutions.in",
                    UserName = "admin",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };
                ApplicationUser superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                if (superUserInDb == null)
                {
                    _ = await _userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                    IdentityResult result = await _userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(_localizer["Seeded Default SuperAdmin User."]);
                    }
                    else
                    {
                        foreach (IdentityError? error in result.Errors)
                        {
                            _logger.LogError(error.Description);
                        }
                    }
                }
                foreach (string permission in Permissions.GetRegisteredPermissions())
                {
                    _ = await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
                }
            }).GetAwaiter().GetResult();
        }

        private void AddBasicUser()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                ApplicationRole basicRole = new(RoleConstants.BasicRole, _localizer["Basic role with default permissions"]);
                ApplicationRole basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);
                if (basicRoleInDb == null)
                {
                    _ = await _roleManager.CreateAsync(basicRole);
                    _logger.LogInformation(_localizer["Seeded Basic Role."]);
                }
                //Check if User Exists
                ApplicationUser basicUser = new()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@blazorhero.com",
                    UserName = "johndoe",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };
                ApplicationUser basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
                if (basicUserInDb == null)
                {
                    _ = await _userManager.CreateAsync(basicUser, UserConstants.DefaultPassword);
                    _ = await _userManager.AddToRoleAsync(basicUser, RoleConstants.BasicRole);
                    _logger.LogInformation(_localizer["Seeded User with Basic Role."]);
                }
            }).GetAwaiter().GetResult();
        }
    }
}
