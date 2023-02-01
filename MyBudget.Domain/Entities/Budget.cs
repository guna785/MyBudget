using MyBudget.Domain.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Entities
{
    public class Budget : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public double Amount { get; set; }
        public double Balance { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category category { get; set; }
        
    }
}
