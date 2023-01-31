using FluentValidation;
using Microsoft.Extensions.Localization;
using MyBudget.Application.Requests.Identity;

namespace MyBudget.Application.Validators.Requests.Identity
{
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator(IStringLocalizer<ForgotPasswordRequestValidator> localizer)
        {
            _ = RuleFor(request => request.Email)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Email is required"])
                .EmailAddress().WithMessage(x => localizer["Email is not correct"]);
        }
    }
}
