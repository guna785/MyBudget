using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MyBudget.Application.Features.Accounts.Commands.AddEdit;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Requests;
using MyBudget.Domain.Entities;
using MyBudget.Shared.Constants.Application;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Accounts.Commands.Import
{
    public class ImportAccountCommand : IRequest<Result<int>>
    {
        public UploadRequest UploadRequest { get; set; }
    }
    internal class ImportAccountCommandHandler : IRequestHandler<ImportAccountCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IExcelService _excelService;
        private readonly IMapper _mapper;
        private readonly IValidator<AddEditAccountCommand> _addBrandValidator;
        private readonly IStringLocalizer<ImportAccountCommandHandler> _localizer;
        private readonly ILogger<ImportAccountCommandHandler> _logger;
        private readonly ICurrentUserService _userService;
        public ImportAccountCommandHandler(
            IUnitOfWork<int> unitOfWork,
            IExcelService excelService,
            IMapper mapper,
            IValidator<AddEditAccountCommand> addBrandValidator,
            IStringLocalizer<ImportAccountCommandHandler> localizer,
            ICurrentUserService userService,
            ILogger<ImportAccountCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _excelService = excelService;
            _mapper = mapper;
            _addBrandValidator = addBrandValidator;
            _localizer = localizer;
            _logger = logger;
            _userService = userService;
        }

        public async Task<Result<int>> Handle(ImportAccountCommand request, CancellationToken cancellationToken)
        {
            MemoryStream stream = new(request.UploadRequest.Data);
            IResult<IEnumerable<Account>> result = await _excelService.ImportAsync(stream, mappers: new Dictionary<string, Func<DataRow, Account, object>>
            {
                { _localizer["AccountName"], (row,item) => item.AccountName = row[_localizer["AccountName"]].ToString() },
                { _localizer["InitialAmount"], (row,item) => item.InitialAmount =double.Parse( row[_localizer["InitialAmount"]].ToString()) },
                 { _localizer["OverDraft"], (row,item) => item.OverDraft = row[_localizer["OverDraft"]].ToString() },
                 
            }, _localizer["Accounts"]);

            if (result.Succeeded)
            {
                IEnumerable<Account> importedBrands = result.Data;
                List<string> errors = new();
                bool errorsOccurred = false;
                foreach (Account? brand in importedBrands)
                {
                    brand.UserId = _userService.UserId;
                    FluentValidation.Results.ValidationResult validationResult = await _addBrandValidator.ValidateAsync(_mapper.Map<AddEditAccountCommand>(brand), cancellationToken);
                    if (validationResult.IsValid)
                    {
                        _ = await _unitOfWork.Repository<Account>().AddAsync(brand);
                    }
                    else
                    {
                        errorsOccurred = true;
                        errors.AddRange(validationResult.Errors.Select(e => $"{(!string.IsNullOrWhiteSpace(brand.AccountName) ? $"{brand.AccountName} - " : string.Empty)}{e.ErrorMessage}"));
                    }
                }
                _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountsCacheKey);
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
