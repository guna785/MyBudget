using MyBudget.Domain.Contract;
using MyBudget.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Entities
{
    public class WishList:AuditableEntity<int>
    {
        public string WishName { get; set; }
        public double Money { get; set; }
        public int UserId { get; set; }
        public WishListStatus Status { get; set; }
    }
}
