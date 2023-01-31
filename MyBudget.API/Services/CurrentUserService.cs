using MyBudget.Application.Interfaces.Services;
using System.Security.Claims;

namespace MyBudget.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext == null ? 0 : Convert.ToInt32(httpContextAccessor.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier));
            Claims = httpContextAccessor.HttpContext == null ? null : httpContextAccessor.HttpContext!.User!.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList() ?? new List<KeyValuePair<string, string>>();
            IpAddress = httpContextAccessor.HttpContext == null ? null : httpContextAccessor.HttpContext!.Request!.Host!.Value;
            UserName = httpContextAccessor.HttpContext == null ? null : httpContextAccessor.HttpContext!.User!.FindFirstValue(ClaimTypes.Name);
        }

        public int UserId { get; }
        public List<KeyValuePair<string, string>> Claims { get; set; }
        public string UserName { get; }
        public string IpAddress { get; }
    }
}
