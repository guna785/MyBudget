using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
/* Unmerged change from project 'MyBudget.MAUI (net6.0-maccatalyst)'
Before:
using System;
After:
using MudBlazor;
using MudBlazor.Services;
using MyBudget.Shared.Constants.Permission;
using System;
*/

/* Unmerged change from project 'MyBudget.MAUI (net6.0-ios)'
Before:
using System;
After:
using MudBlazor;
using MudBlazor.Services;
using MyBudget.Shared.Constants.Permission;
using System;
*/

/* Unmerged change from project 'MyBudget.MAUI (net6.0-windows10.0.19041.0)'
Before:
using System;
After:
using MudBlazor;
using MudBlazor.Services;
using MyBudget.Shared.Constants.Permission;
using System;
*/
using MudBlazor.Services;
using MyBudget.MAUI.Authentication;
using MyBudget.MAUI.Managers;
using MyBudget.MAUI.Managers.Preferences;
using MyBudget.Shared.Constants.Permission;
using System.Globalization;
using System.Reflection;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Permissions = MyBudget.Shared.Constants.Permission.Permissions;

namespace MyBudget.MAUI.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {
        private const string ClientName = "DentalManagement.Server";

        public static MauiAppBuilder AddClientServices(this MauiAppBuilder builder)
        {
            _ = builder
                .Services
                .AddLocalization(options =>
                {
                    options.ResourcesPath = @"Resources\Languages";
                })
                .AddAuthorizationCore(options =>
                {
                    RegisterPermissionClaims(options);
                })
                .AddMudServices(configuration =>
                {
                    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                    configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                    configuration.SnackbarConfiguration.ShowCloseIcon = false;
                })
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddScoped<ClientPreferenceManager>()
                .AddScoped<DentalAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider, DentalAuthenticationStateProvider>()
                .AddManagers()
                .AddTransient<AuthenticationHeaderHandler>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(ClientName).EnableIntercept(sp))
                .AddHttpClient(ClientName, client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();                    
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri("https://localhost:7172/");
                })
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
            builder.Services.AddHttpClientInterceptor();
            return builder;
        }

        public static IServiceCollection AddManagers(this IServiceCollection services)
        {
            Type managers = typeof(IManager);

            var types = managers
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(t => t.Service != null);

            foreach (var type in types)
            {
                if (managers.IsAssignableFrom(type.Service))
                {
                    _ = services.AddTransient(type.Service, type.Implementation);
                }
            }

            return services;
        }



        private static void RegisterPermissionClaims(AuthorizationOptions options)
        {
            foreach (FieldInfo prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                object propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                }
            }
        }
    }
}
