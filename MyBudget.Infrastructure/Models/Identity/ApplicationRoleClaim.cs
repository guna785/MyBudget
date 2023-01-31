using Microsoft.AspNetCore.Identity;
using MyBudget.Domain.Contract;

namespace MyBudget.Infrastructure.Models.Identity
{
    public class ApplicationRoleClaim : IdentityRoleClaim<int>, IAuditableEntity<int>
    {
        public string? Description { get; set; }
        public string? Group { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual ApplicationRole Role { get; set; }
        public string? IPAddress { get; set; }
        public bool IsDeleted { get; set; }

        public ApplicationRoleClaim() : base()
        {
        }

        public ApplicationRoleClaim(string roleClaimDescription = null, string roleClaimGroup = null) : base()
        {
            Description = roleClaimDescription;
            Group = roleClaimGroup;
        }
    }
}
