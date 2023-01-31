using MyBudget.Domain.Contract;
using MyBudget.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Entities
{
    public class Transactions : AuditableEntity<int>
    {
        public int AccountId { get; set; }
        public string Description { get; set; }
        public TransactionTypeData TransactionType { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        [ForeignKey(nameof(AccountId))]
        public virtual Account account { get; set; }
        public int UserId { get; set; }
        public ModeOfTransaction Mode { get; set; }
        public string? ModeComments { get; set; }
    }
}
