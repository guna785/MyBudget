using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using MyBudget.Application.Features.Accounts.Commands.AddEdit;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Domain.Entities;
using MyBudget.Domain.Enums;
using MyBudget.Shared.Constants.Application;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Transaction.Commands.AddEdit
{
    public class AddEditTransactionsCommand : IRequest<Result<int>>
    {
        public int Id { get; set; } = 0;
        public int AccountId { get; set; }
        public string Description { get; set; }
        public TransactionTypeData TransactionType { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public int UserId { get; set; }
        public ModeOfTransaction Mode { get; set; }
        public string? ModeComments { get; set; }
    }
    internal class AddEditTransactionsCommandHandler : IRequestHandler<AddEditTransactionsCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditTransactionsCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;

        public AddEditTransactionsCommandHandler(IUnitOfWork<int> unitOfWork, IUploadService uploadService, IMapper mapper, IStringLocalizer<AddEditTransactionsCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _uploadService = uploadService;
        }

        public async Task<Result<int>> Handle(AddEditTransactionsCommand command, CancellationToken cancellationToken)
        {

            if (command.Id == 0)
            {
                Transactions org = _mapper.Map<Transactions>(command);

                _ = await _unitOfWork.Repository<Transactions>().AddAsync(org);
                _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountsCacheKey);
                return await Result<int>.SuccessAsync(org.Id, _localizer["Transaction Saved"]);
            }
            else
            {
                Transactions org = await _unitOfWork.Repository<Transactions>().GetByIdAsync(command.Id);
                if (org != null)
                {
                    org.Amount = command.Amount;
                    org.AccountId = command.AccountId;
                    org.Description = command.Description;
                    org.TransactionDate = command.TransactionDate;
                    org.Mode = command.Mode;
                    org.ModeComments= command.ModeComments;
                    org.TransactionType = command.TransactionType;                    
                    org.UserId = command.UserId;

                    await _unitOfWork.Repository<Transactions>().UpdateAsync(org);
                    _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountsCacheKey);
                    return await Result<int>.SuccessAsync(org.Id, _localizer["Transaction Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Transaction Not Found!"]);
                }
            }
        }
    }
}
