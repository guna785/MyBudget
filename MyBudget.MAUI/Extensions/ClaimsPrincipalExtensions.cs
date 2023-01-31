using System.Security.Claims;

namespace MyBudget.MAUI.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        internal static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Email);
        }

        internal static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Name);
        }

        internal static string GetLastName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Surname);
        }

        internal static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);
        }

        internal static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return Convert.ToInt32(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
