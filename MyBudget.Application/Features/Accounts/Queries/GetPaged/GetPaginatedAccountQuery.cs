using MediatR;
using Microsoft.Extensions.Logging;
using MyBudget.Application.Extensions;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Application.Specifications.Features.Accounts;
using MyBudget.Domain.Entities;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Accounts.Queries.GetPaged
{
    public class GetPaginatedAccountQuery : IRequest<PaginatedResult<GetPaginatedAccountResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetPaginatedAccountQuery(int pageNumber, int pageSize, string searchString, string orderBy)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllAccountPaginatedsCachedQueryHandler : IRequestHandler<GetPaginatedAccountQuery, PaginatedResult<GetPaginatedAccountResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ILogger<GetAllAccountPaginatedsCachedQueryHandler> _logger;
        public GetAllAccountPaginatedsCachedQueryHandler(IUnitOfWork<int> unitOfWork, ILogger<GetAllAccountPaginatedsCachedQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PaginatedResult<GetPaginatedAccountResponse>> Handle(GetPaginatedAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Expression<Func<Account, GetPaginatedAccountResponse>> expression = e => new GetPaginatedAccountResponse
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    AccountName = e.AccountName,
                    OverDraft = e.OverDraft,
                    InitialAmount = e.InitialAmount,
                    CreatedTime = e.CreatedOn ?? DateTime.Now
                };
                AccountFilterSpecification AccountFilterSpec = new(request.SearchString);
                if (request.OrderBy?.Any() != true)
                {
                    PaginatedResult<GetPaginatedAccountResponse> data = await _unitOfWork.Repository<Account>().Entities
                       .Specify(AccountFilterSpec)
                       .Select(expression)
                       .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                    return data;
                }
                else
                {
                    string ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                    PaginatedResult<GetPaginatedAccountResponse> data = await _unitOfWork.Repository<Account>().Entities
                       .Specify(AccountFilterSpec)
                       .OrderBy(ordering) // require system.linq.dynamic.core
                       .Select(expression)
                       .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                    return data;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return (PaginatedResult<GetPaginatedAccountResponse>)await PaginatedResult<GetPaginatedAccountResponse>.FailAsync(ex.Message);
            }

        }

    }
}
