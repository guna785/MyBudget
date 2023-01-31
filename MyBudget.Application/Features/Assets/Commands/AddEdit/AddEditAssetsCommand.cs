using MediatR;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Assets.Commands.AddEdit
{
    public class AddEditAssetsCommand:IRequest<Result<int>>
    {

    }
}
