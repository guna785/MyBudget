using MyBudget.Infrastructure.Models.Audit;
using MyBudget.Application.Specifications.Base;

namespace MyBudget.Infrastructure.Specifications
{
    public class AuditFilterSpecification : Specification<Audit>
    {
        public AuditFilterSpecification(string userId, string searchString, bool searchInOldValues, bool searchInNewValues)
        {
            Criteria = !string.IsNullOrEmpty(searchString)
                ? (p => (p.TableName.Contains(searchString) || (searchInOldValues && p.OldValues.Contains(searchString)) || (searchInNewValues && p.NewValues.Contains(searchString))) && p.UserId == userId)
                : (p => p.UserId == userId);
        }
    }
}
