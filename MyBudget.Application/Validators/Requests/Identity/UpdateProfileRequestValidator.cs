using FluentValidation;
using Microsoft.Extensions.Localization;
using MyBudget.Application.Requests.Identity;

namespace MyBudget.Application.Validators.Requests.Identity
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator(IStringLocalizer<UpdateProfileRequestValidator> localizer)
        {
            _ = RuleFor(request => request.FirstName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["First Name is required"]);
            _ = RuleFor(request => request.LastName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Last Name is required"]);
        }
    }
}
