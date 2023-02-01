using MyBudget.Domain.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Entities
{
    public class Transaction : AuditableEntity<int>
    {
        public int AccountId { get; set; }
        public int CategoryId { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category category { get; set; }
        [ForeignKey(nameof(AccountId))]
        public virtual Account account { get; set; }
    }
}
