using MyBudget.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Accounts.Queries.GetAll
{
    public class GetAllAccountResponse
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public AccountTypeData AccountType { get; set; }
        public double Amount { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
