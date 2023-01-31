using System.ComponentModel.DataAnnotations;

namespace MyBudget.Application.Requests.Identity
{
    public class TokenRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
