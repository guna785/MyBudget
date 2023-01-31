using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Domain.Entities;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Accounts.Queries.GetById
{
    internal class GetByIdAccountQuery : IRequest<Result<GetByIdAccountResponse>>
    {
        public int Id { get; set; }
    }
    internal class GetByIdAccountQueryHandler : IRequestHandler<GetByIdAccountQuery, Result<GetByIdAccountResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetByIdAccountQueryHandler> _logger;
        public GetByIdAccountQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper,
            ILogger<GetByIdAccountQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<GetByIdAccountResponse>> Handle(GetByIdAccountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                Account treatment = await _unitOfWork.Repository<Account>().GetByIdAsync(query.Id);
                GetByIdAccountResponse mappedTreatment = _mapper.Map<GetByIdAccountResponse>(treatment);
                return await Result<GetByIdAccountResponse>.SuccessAsync(mappedTreatment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return await Result<GetByIdAccountResponse>.FailAsync(ex.Message);
            }

        }
    }
}
