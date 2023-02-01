using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MyBudget.Application.Features.Accounts.Commands.AddEdit;
using MyBudget.Application.Features.Accounts.Commands.Import;
using MyBudget.Application.Features.Categories.Commands.AddEdit;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Requests;
using MyBudget.Domain.Entities;
using MyBudget.Domain.Enums;
using MyBudget.Shared.Constants.Application;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Categories.Commands.Import
{
    public class ImportCategoryCommand : IRequest<Result<int>>
    {
        public UploadRequest UploadRequest { get; set; }
    }
    internal class ImportCategoryCommandHandler : IRequestHandler<ImportCategoryCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IExcelService _excelService;
        private readonly IMapper _mapper;
        private readonly IValidator<AddEditCategoryCommand> _addBrandValidator;
        private readonly IStringLocalizer<ImportCategoryCommandHandler> _localizer;
        private readonly ILogger<ImportCategoryCommandHandler> _logger;
        private readonly ICurrentUserService _userService;
        public ImportCategoryCommandHandler(
            IUnitOfWork<int> unitOfWork,
            IExcelService excelService,
            IMapper mapper,
            IValidator<AddEditCategoryCommand> addBrandValidator,
            IStringLocalizer<ImportCategoryCommandHandler> localizer,
            ICurrentUserService userService,
            ILogger<ImportCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _excelService = excelService;
            _mapper = mapper;
            _addBrandValidator = addBrandValidator;
            _localizer = localizer;
            _logger = logger;
            _userService = userService;
        }

        public async Task<Result<int>> Handle(ImportCategoryCommand request, CancellationToken cancellationToken)
        {
            MemoryStream stream = new(request.UploadRequest.Data);
            IResult<IEnumerable<Category>> result = await _excelService.ImportAsync(stream, mappers: new Dictionary<string, Func<DataRow, Category, object>>
            {
                { _localizer["Name"], (row,item) => item.Name = row[_localizer["Name"]].ToString() },
                { _localizer["CategoryType"], (row,item) => item.CategoryType = (CategoryTypeData)Enum.Parse(typeof(CategoryTypeData), ( row[_localizer["CategoryType"]].ToString())) },
                 { _localizer["UserId"], (row,item) => item.UserId =int.Parse( row[_localizer["UserId"]].ToString() )},

            }, _localizer["Categories"]);

            if (result.Succeeded)
            {
                IEnumerable<Category> importedBrands = result.Data;
                List<string> errors = new();
                bool errorsOccurred = false;
                foreach (Category? brand in importedBrands)
                {
                    brand.UserId = _userService.UserId;
                    FluentValidation.Results.ValidationResult validationResult = await _addBrandValidator.ValidateAsync(_mapper.Map<AddEditCategoryCommand>(brand), cancellationToken);
                    if (validationResult.IsValid)
                    {
                        _ = await _unitOfWork.Repository<Category>().AddAsync(brand);
                    }
                    else
                    {
                        errorsOccurred = true;
                        errors.AddRange(validationResult.Errors.Select(e => $"{(!string.IsNullOrWhiteSpace(brand.Name) ? $"{brand.Name} - " : string.Empty)}{e.ErrorMessage}"));
                    }
                }
                _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCategoryCacheKey);
                if (errorsOccurred)
                {
                    _logger.LogError("{@errors}", errors);
                    return await Result<int>.FailAsync(errors);
                }

                return await Result<int>.SuccessAsync(result.Data.FirstOrDefault()!.Id, result.Messages[0]);
            }
            else
            {
                return await Result<int>.FailAsync(result.Messages);
            }
        }
    }
}
