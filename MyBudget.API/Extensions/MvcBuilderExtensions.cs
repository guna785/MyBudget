using FluentValidation.AspNetCore;
using MyBudget.Application.Configurations;

namespace MyBudget.API.Extensions
{
    internal static class MvcBuilderExtensions
    {
        internal static IMvcBuilder AddValidators(this IMvcBuilder builder)
        {
            _ = builder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AppConfiguration>());
            return builder;
        }


    }
}
