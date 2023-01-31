using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Entities
{
    public class AccountTranfers
    {
        public int AccountId { get; set; }
        public int TransferAccountId { get; set; }
        public double TransferedAmount { get; set; }
        public int UserId { get; set; }

    }
}
