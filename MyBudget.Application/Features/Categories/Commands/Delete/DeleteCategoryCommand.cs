using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MyBudget.Application.Features.Accounts.Commands.Delete;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Domain.Entities;
using MyBudget.Shared.Constants.Application;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Categories.Commands.Delete
{
    public class DeleteCategoryCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }
    internal class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteCategoryCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;
        public DeleteCategoryCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteCategoryCommandHandler> localizer, ILogger<DeleteCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                Category account = await _unitOfWork.Repository<Category>().GetByIdAsync(command.Id);
                if (account != null)
                {
                    await _unitOfWork.Repository<Category>().DeleteAsync(account);
                    _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCategoryCacheKey);
                    return await Result<int>.SuccessAsync(account.Id, _localizer["Category Deleted"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Category Not Found!"]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return await Result<int>.FailAsync(_localizer[ex.Message]);
            }

        }
    }
}
