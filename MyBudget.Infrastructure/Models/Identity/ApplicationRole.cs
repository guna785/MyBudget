using Microsoft.AspNetCore.Identity;
using MyBudget.Domain.Contract;

namespace MyBudget.Infrastructure.Models.Identity
{
    public class ApplicationRole : IdentityRole<int>, IAuditableEntity<int>
    {
        public string Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
        public string? IPAddress { get; set; }
        public bool IsDeleted { get; set; }
        public ApplicationRole() : base()
        {
            RoleClaims = new HashSet<ApplicationRoleClaim>();
        }

        public ApplicationRole(string roleName, string roleDescription = null) : base(roleName)
        {
            RoleClaims = new HashSet<ApplicationRoleClaim>();
            Description = roleDescription;
        }
    }
}
