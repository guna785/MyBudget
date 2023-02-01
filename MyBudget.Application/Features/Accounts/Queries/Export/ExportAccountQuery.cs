using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MyBudget.Application.Extensions;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Specifications.Features.Accounts;
using MyBudget.Domain.Entities;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Accounts.Queries.Export
{
    public class ExportAccountQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportAccountQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }
    internal class ExportAccountQueryHandler : IRequestHandler<ExportAccountQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportAccountQueryHandler> _localizer;
        private readonly ILogger<ExportAccountQueryHandler> _logger;
        public ExportAccountQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportAccountQueryHandler> localizer
            , ILogger<ExportAccountQueryHandler> logger)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(ExportAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                AccountFilterSpecification brandFilterSpec = new(request.SearchString);
                List<Account> brands = await _unitOfWork.Repository<Account>().Entities
                    .Specify(brandFilterSpec)
                    .ToListAsync(cancellationToken);
                string data = await _excelService.ExportAsync(brands, mappers: new Dictionary<string, Func<Account, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Name },
                { _localizer["Amount"], item => item.Amount },
                { _localizer["AccountType"], item => item.AccountType },
                { _localizer["UserId"], item => item.UserId }
            }, sheetName: _localizer["Accountes"]);

                return await Result<string>.SuccessAsync(data: data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return await Result<string>.FailAsync(_localizer[ex.Message]);
            }

        }
    }
}
