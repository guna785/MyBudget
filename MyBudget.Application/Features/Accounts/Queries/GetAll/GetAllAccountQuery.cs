using AutoMapper;
using LazyCache;
using MediatR;
using Microsoft.Extensions.Logging;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Domain.Entities;
using MyBudget.Shared.Constants.Application;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Accounts.Queries.GetAll
{
    public class GetAllAccountQuery : IRequest<Result<List<GetAllAccountResponse>>>
    {
        public GetAllAccountQuery()
        {
        }
    }
    internal class GetAllAccountQueryHandler : IRequestHandler<GetAllAccountQuery, Result<List<GetAllAccountResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;
        private readonly ILogger<GetAllAccountQueryHandler> _logger;

        public GetAllAccountQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache, ILogger<GetAllAccountQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result<List<GetAllAccountResponse>>> Handle(GetAllAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Task<List<Account>> getAllAccounts()
                {
                    return _unitOfWork.Repository<Account>().GetAllAsync();
                }

                List<Account> treatementList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllAccountsCacheKey, getAllAccounts);
                List<GetAllAccountResponse> mappedAccounts = _mapper.Map<List<GetAllAccountResponse>>(treatementList);
                return await Result<List<GetAllAccountResponse>>.SuccessAsync(mappedAccounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return await Result<List<GetAllAccountResponse>>.FailAsync(ex.Message);
            }

        }
    }
}
