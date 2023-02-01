using MyBudget.Domain.Contract;
using MyBudget.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Entities
{
    public class Account : AuditableEntity<int>
    {
        public string Name { get; set; }
        public AccountTypeData AccountType { get; set; }
        public double Amount { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
