using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Requests;
using MyBudget.Domain.Entities;
using MyBudget.Shared.Constants.Application;
using MyBudget.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBudget.Application.Features.Accounts.Commands.AddEdit
{
    public class AddEditAccountCommand : IRequest<Result<int>>
    {
        public int Id { get; set; } = 0;
        public string AccountName { get; set; }
        public double InitialAmount { get; set; }
        public string OverDraft { get; set; }
        public int UserId { get; set; }
    }
    internal class AddEditAccountCommandHandler : IRequestHandler<AddEditAccountCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditAccountCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;

        public AddEditAccountCommandHandler(IUnitOfWork<int> unitOfWork, IUploadService uploadService, IMapper mapper, IStringLocalizer<AddEditAccountCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _uploadService = uploadService;
        }

        public async Task<Result<int>> Handle(AddEditAccountCommand command, CancellationToken cancellationToken)
        {

            if (command.Id == 0)
            {
                Account org = _mapper.Map<Account>(command);

                _ = await _unitOfWork.Repository<Account>().AddAsync(org);
                _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountsCacheKey);
                return await Result<int>.SuccessAsync(org.Id, _localizer["Account Saved"]);
            }
            else
            {
                Account org = await _unitOfWork.Repository<Account>().GetByIdAsync(command.Id);
                if (org != null)
                {
                    org.AccountName = command.AccountName ?? org.AccountName;
                    org.InitialAmount = command.InitialAmount;
                    org.OverDraft = command.OverDraft ?? org.OverDraft;
                    org.UserId= command.UserId;

                    await _unitOfWork.Repository<Account>().UpdateAsync(org);
                    _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountsCacheKey);
                    return await Result<int>.SuccessAsync(org.Id, _localizer["Account Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Account Not Found!"]);
                }
            }
        }
    }
}
