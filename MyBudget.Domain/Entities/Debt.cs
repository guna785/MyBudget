using MyBudget.Domain.Contract;
using MyBudget.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Entities
{
    public class Debt : AuditableEntity<int>
    {
        public string DebtName { get; set; }
        public double DebtAmount { get; set; }
        public DateTime DebtDate { get; set; }
        public double? Intrest { get; set; }
        public int? Duration { get; set; }
        public double DebtAmountWithIntrest { get; set; }
        public DebtIntrestType IntrestType { get; set; }
        public bool IsEMI { get; set; }
        public double EMIAmount { get; set; }
        public int NoEMIs { get; set; }
        public int PaidEMIs { get; set; }
        public int RemainingEMIs { get; set; }
        public int EMIDate { get; set; }
        public DebtStatus Status { get; set; }
        public int UserId { get; set; }
    }
}
