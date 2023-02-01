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

namespace MyBudget.Application.Features.Categories.Commands.AddEdit
{
    public class AddEditCategoryCommand : IRequest<Result<int>>
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public CategoryTypeData CategoryType { get; set; }
        public int UserId { get; set; }
    }
    internal class AddEditCategoryCommandHandler : IRequestHandler<AddEditCategoryCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditCategoryCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;

        public AddEditCategoryCommandHandler(IUnitOfWork<int> unitOfWork, IUploadService uploadService, IMapper mapper, IStringLocalizer<AddEditCategoryCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _uploadService = uploadService;
        }

        public async Task<Result<int>> Handle(AddEditCategoryCommand command, CancellationToken cancellationToken)
        {

            if (command.Id == 0)
            {
                Category org = _mapper.Map<Category>(command);

                _ = await _unitOfWork.Repository<Category>().AddAsync(org);
                _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCategoryCacheKey);
                return await Result<int>.SuccessAsync(org.Id, _localizer["Category Saved"]);
            }
            else
            {
                Category org = await _unitOfWork.Repository<Category>().GetByIdAsync(command.Id);
                if (org != null)
                {
                    org.Name = command.Name ?? org.Name;
                    org.CategoryType = command.CategoryType;
                    org.UserId = command.UserId;

                    await _unitOfWork.Repository<Category>().UpdateAsync(org);
                    _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCategoryCacheKey);
                    return await Result<int>.SuccessAsync(org.Id, _localizer["Category Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Category Not Found!"]);
                }
            }
        }
    }
}
