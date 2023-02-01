using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MyBudget.Application.Extensions;
using MyBudget.Application.Features.Accounts.Queries.Export;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Specifications.Features.Accounts;
using MyBudget.Application.Specifications.Features.Categories;
using MyBudget.Domain.Entities;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Categories.Queries.Export
{
    public class ExportCategoryQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportCategoryQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }
    internal class ExportCategoryQueryHandler : IRequestHandler<ExportCategoryQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportCategoryQueryHandler> _localizer;
        private readonly ILogger<ExportCategoryQueryHandler> _logger;
        public ExportCategoryQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportCategoryQueryHandler> localizer
            , ILogger<ExportCategoryQueryHandler> logger)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(ExportCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                CategoryFilterSpecification brandFilterSpec = new(request.SearchString);
                List<Category> brands = await _unitOfWork.Repository<Category>().Entities
                    .Specify(brandFilterSpec)
                    .ToListAsync(cancellationToken);
                string data = await _excelService.ExportAsync(brands, mappers: new Dictionary<string, Func<Category, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Name },
                { _localizer["CategoryType"], item => item.CategoryType },
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
