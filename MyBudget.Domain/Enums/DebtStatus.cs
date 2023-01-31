﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Domain.Enums
{
    public enum DebtStatus:byte
    {
        Completed,
        InProgress,
        Cancelled,
        Tranfered,
        FullSettlement,
        OneTimePayment,
    }
}
