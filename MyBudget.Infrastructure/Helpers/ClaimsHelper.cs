using Microsoft.AspNetCore.Identity;
using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Application.Responses.Identity;
using MyBudget.Shared.Constants.Permission;
using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;

namespace MyBudget.Infrastructure.Helpers
{
    public static class ClaimsHelper
    {
        public static void GetAllPermissions(this List<RoleClaimResponse> allPermissions)
        {
            Type[] modules = typeof(Permissions).GetNestedTypes();

            foreach (Type module in modules)
            {
                string moduleName = string.Empty;
                string moduleDescription = string.Empty;

                if (module.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                    .FirstOrDefault() is DisplayNameAttribute displayNameAttribute)
                {
                    moduleName = displayNameAttribute.DisplayName;
                }

                if (module.GetCustomAttributes(typeof(DescriptionAttribute), true)
                    .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                {
                    moduleDescription = descriptionAttribute.Description;
                }

                FieldInfo[] fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                foreach (FieldInfo fi in fields)
                {
                    object? propertyValue = fi.GetValue(null);

                    if (propertyValue is not null)
                    {
                        allPermissions.Add(new RoleClaimResponse { Value = propertyValue.ToString(), Type = ApplicationClaimTypes.Permission, Group = moduleName, Description = moduleDescription });
                    }
                }
            }

        }

        public static async Task<IdentityResult> AddPermissionClaim(this RoleManager<ApplicationRole> roleManager, ApplicationRole role, string permission)
        {
            IList<Claim> allClaims = await roleManager.GetClaimsAsync(role);
            return !allClaims.Any(a => a.Type == ApplicationClaimTypes.Permission && a.Value == permission)
                ? await roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, permission))
                : IdentityResult.Failed();
        }
    }
}
