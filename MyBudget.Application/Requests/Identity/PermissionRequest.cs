namespace MyBudget.Application.Requests.Identity
{
    public class PermissionRequest
    {
        public int RoleId { get; set; }
        public IList<RoleClaimRequest> RoleClaims { get; set; }
    }
}
