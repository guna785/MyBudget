using AutoMapper;
using LazyCache;
using MediatR;
using Microsoft.Extensions.Logging;
using MyBudget.Application.Features.Accounts.Queries.GetAll;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Domain.Entities;
using MyBudget.Shared.Constants.Application;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Categories.Queries.GetAll
{
    public class GetAllCategoryQuery : IRequest<Result<List<GetAllCategoryResponse>>>
    {
        public GetAllCategoryQuery()
        {
        }
    }
    internal class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryQuery, Result<List<GetAllCategoryResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;
        private readonly ILogger<GetAllCategoryQueryHandler> _logger;

        public GetAllCategoryQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache, ILogger<GetAllCategoryQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result<List<GetAllCategoryResponse>>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Task<List<Category>> getAllAccounts()
                {
                    return _unitOfWork.Repository<Category>().GetAllAsync();
                }

                List<Category> treatementList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllAccountsCacheKey, getAllAccounts);
                List<GetAllCategoryResponse> mappedAccounts = _mapper.Map<List<GetAllCategoryResponse>>(treatementList);
                return await Result<List<GetAllCategoryResponse>>.SuccessAsync(mappedAccounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return await Result<List<GetAllCategoryResponse>>.FailAsync(ex.Message);
            }

        }
    }
}
