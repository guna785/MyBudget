using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Application.Specifications.Base;

namespace MyBudget.Infrastructure.Specifications
{
    public class UserFilterSpecification : Specification<ApplicationUser>
    {
        public UserFilterSpecification(string searchString)
        {
            Criteria = !string.IsNullOrEmpty(searchString)
                ? (p => p.FirstName.Contains(searchString) || p.LastName.Contains(searchString) || p.Email.Contains(searchString) || p.PhoneNumber.Contains(searchString) || p.UserName.Contains(searchString))
                : (p => true);
        }
    }
}
