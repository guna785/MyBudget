using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyBudget.Application.Interfaces.Chat;
using MyBudget.Application.Models.Chat;
using MyBudget.Domain.Contract;
using MyBudget.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBudget.Infrastructure.Models.Identity
{
    public class ApplicationUser : IdentityUser<int>, IChatUser, IAuditableEntity<int>
    {
        public SalutaionType Salutaion { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string? Designation { get; set; }
        public DateTime? DoB { get; set; }
        public string? BirthCity { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? AddressLine1 { get; set; }
        public string? Addressline2 { get; set; }
        public string? PostalCode { get; set; }
        public string? CreatedBy { get; set; }

        [Column(TypeName = "text")]
        public string? ProfilePictureDataUrl { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public virtual ICollection<ChatHistory<ApplicationUser>> ChatHistoryFromUsers { get; set; }
        public virtual ICollection<ChatHistory<ApplicationUser>> ChatHistoryToUsers { get; set; }
        public string? IPAddress { get; set; }

        public ApplicationUser()
        {
            ChatHistoryFromUsers = new HashSet<ChatHistory<ApplicationUser>>();
            ChatHistoryToUsers = new HashSet<ChatHistory<ApplicationUser>>();
        }
    }
}
