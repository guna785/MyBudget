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
    public class DebtTransaction:AuditableEntity<int>
    {
        public int TransactionId { get; set; }
        public int DebtId { get; set; }
        public DebtTransactionType TransactionType { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(TransactionId))]
        public virtual Transactions transactions { get; set; }
        [ForeignKey(nameof(DebtId))]
        public virtual Debt debt { get; set; }
    }
}
