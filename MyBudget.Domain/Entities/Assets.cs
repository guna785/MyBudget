using MyBudget.Domain.Contract;
using System;
using System.Collections.Generic;
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
    }
}
