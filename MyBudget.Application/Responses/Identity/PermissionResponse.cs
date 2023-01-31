namespace MyBudget.Application.Responses.Identity
{
    public class PermissionResponse
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RoleClaimResponse> RoleClaims { get; set; }
    }
}
