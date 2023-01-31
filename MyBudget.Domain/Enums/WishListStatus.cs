using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Enums
{
    public enum WishListStatus : byte
    {
        Success = 0,
        OnHold = 1,
        PostPonded = 2,
        Cancelled = 3
    }
}
