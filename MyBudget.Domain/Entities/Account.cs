using MyBudget.Domain.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Entities
{
    public class Account : AuditableEntity<int>
    {
        public string AccountName { get; set; }
        public double InitialAmount { get; set; }
        public string OverDraft { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<Transactions> Transactions { get; set; }
    }
}
