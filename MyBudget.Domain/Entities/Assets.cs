using MyBudget.Domain.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Entities
{
    public class Assets : AuditableEntity<int>
    {
        public string AssetName { get; set; }
        public double AssetValue { get; set; }
        public DateTime AssetDate { get; set; }
        public int UserId { get; set; }
        public int TransactionId { get; set; }
        [ForeignKey(nameof(TransactionId))]
        public virtual Transactions transactions { get; set; }
    }
}
