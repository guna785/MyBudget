using MyBudget.Application.Specifications.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBudget.Domain.Entities;

namespace MyBudget.Application.Specifications.Features.Accounts
{
    public class AccountFilterSpecification : Specification<Account>
    {
        public AccountFilterSpecification(string searchString)
        {
            Criteria = !string.IsNullOrEmpty(searchString) ? (x => x.AccountName.ToLower().Contains(searchString.ToLower()) ||
                     x.OverDraft.ToLower().Contains(searchString.ToLower())) : (x => true);
        }
    }
}
