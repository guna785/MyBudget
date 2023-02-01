using MyBudget.Application.Specifications.Base;
using MyBudget.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Specifications.Features.Categories
{
    public class CategoryFilterSpecification : Specification<Category>
    {
        public CategoryFilterSpecification(string searchString)
        {
            Criteria = !string.IsNullOrEmpty(searchString) ? (x => x.Name.ToLower().Contains(searchString.ToLower())) : (x => true);
        }
    }
}
