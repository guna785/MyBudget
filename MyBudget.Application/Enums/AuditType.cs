﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Enums
{
    public enum AuditType : byte
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3
    }
}
