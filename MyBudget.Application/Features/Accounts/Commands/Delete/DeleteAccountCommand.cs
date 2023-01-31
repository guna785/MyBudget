using MediatR;
using Microsoft.Extensions.Localization;
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

namespace MyBudget.Application.Features.Accounts.Commands.Delete
{
    public class DeleteAccountCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }
    internal class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteAccountCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ILogger<DeleteAccountCommandHandler> _logger;
        public DeleteAccountCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteAccountCommandHandler> localizer, ILogger<DeleteAccountCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
        {
            try
            {
                Account account = await _unitOfWork.Repository<Account>().GetByIdAsync(command.Id);
                if (account != null)
                {
                    await _unitOfWork.Repository<Account>().DeleteAsync(account);
                    _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountsCacheKey);
                    return await Result<int>.SuccessAsync(account.Id, _localizer["Account Deleted"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Account Not Found!"]);
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
