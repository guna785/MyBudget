using System.ComponentModel.DataAnnotations;

namespace MyBudget.Application.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
