using MediatR;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Debts.Commands.AddEdit
{
    public class AddEditDebtsCommand:IRequest<Result<int>>
    {
    }
}
