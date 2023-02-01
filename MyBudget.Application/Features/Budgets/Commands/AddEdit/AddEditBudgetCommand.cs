using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using MyBudget.Application.Features.Categories.Commands.AddEdit;
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

namespace MyBudget.Application.Features.Budgets.Commands.AddEdit
{
    public class AddEditBudgetCommand : IRequest<Result<int>>
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public double Amount { get; set; }
        public double Balance { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    internal class AddEditBudgetCommandHandler : IRequestHandler<AddEditBudgetCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditBudgetCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;

        public AddEditBudgetCommandHandler(IUnitOfWork<int> unitOfWork, IUploadService uploadService, IMapper mapper, IStringLocalizer<AddEditBudgetCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _uploadService = uploadService;
        }

        public async Task<Result<int>> Handle(AddEditBudgetCommand command, CancellationToken cancellationToken)
        {

            if (command.Id == 0)
            {
                Budget org = _mapper.Map<Budget>(command);

                _ = await _unitOfWork.Repository<Budget>().AddAsync(org);
                _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBudgetCacheKey);
                return await Result<int>.SuccessAsync(org.Id, _localizer["Budget Saved"]);
            }
            else
            {
                Budget org = await _unitOfWork.Repository<Budget>().GetByIdAsync(command.Id);
                if (org != null)
                {
                    org.Name = command.Name ?? org.Name;
                    org.Balance = command.Balance;
                    org.Amount=command.Amount;
                    org.CategoryId = command.CategoryId;
                    org.Description = command.Description;  
                    org.EndDate = command.EndDate?? DateTime.Now;
                    org.StartDate= command.StartDate ?? DateTime.Now;                   
                    org.UserId = command.UserId;

                    await _unitOfWork.Repository<Budget>().UpdateAsync(org);
                    _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBudgetCacheKey);
                    return await Result<int>.SuccessAsync(org.Id, _localizer["Budget Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Budget Not Found!"]);
                }
            }
        }
    }
}
