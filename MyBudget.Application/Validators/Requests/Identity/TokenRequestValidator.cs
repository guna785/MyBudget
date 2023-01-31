using FluentValidation;
using Microsoft.Extensions.Localization;
using MyBudget.Application.Requests.Identity;

namespace MyBudget.Application.Validators.Requests.Identity
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        public TokenRequestValidator(IStringLocalizer<TokenRequestValidator> localizer)
        {
            _ = RuleFor(request => request.UserName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["UserName is required"]);
            _ = RuleFor(request => request.Password)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Password is required!"]);
        }
    }
}
