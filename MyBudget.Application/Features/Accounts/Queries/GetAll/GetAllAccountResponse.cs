using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Accounts.Queries.GetAll
{
    public class GetAllAccountResponse
    {
        public int Id { get; set; } = 0;
        public string AccountName { get; set; }
        public double InitialAmount { get; set; }
        public string OverDraft { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
