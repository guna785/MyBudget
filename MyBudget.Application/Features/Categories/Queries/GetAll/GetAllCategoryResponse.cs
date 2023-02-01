using MyBudget.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Categories.Queries.GetAll
{
    public class GetAllCategoryResponse
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public CategoryTypeData CategoryType { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        
    }
}
