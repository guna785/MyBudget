using Microsoft.Extensions.DependencyInjection;
using MyBudget.Infrastructure.Repositories;
using MyBudget.Infrastructure.Services.Storage;
using MyBudget.Infrastructure.Services.Storage.Provider;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Application.Interfaces.Serialization.Serializers;
using MyBudget.Application.Interfaces.Services.Storage;
using MyBudget.Application.Interfaces.Services.Storage.Provider;
using MyBudget.Application.Serialization.JsonConverters;
using MyBudget.Application.Serialization.Options;
using MyBudget.Application.Serialization.Serializers;
using System.Reflection;

namespace MyBudget.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void InfrastructureMappings(this IServiceCollection services)
        {
            _ = services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
            .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))

                .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        }


        public static IServiceCollection AddServerStorage(this IServiceCollection services)
        {
            return AddServerStorage(services, null!);
        }

        public static IServiceCollection AddServerStorage(this IServiceCollection services, Action<SystemTextJsonOptions> configure)
        {
            return services
                .AddScoped<IJsonSerializer, SystemTextJsonSerializer>()
                .AddScoped<IStorageProvider, ServerStorageProvider>()
                .AddScoped<IServerStorageService, ServerStorageService>()
                .AddScoped<ISyncServerStorageService, ServerStorageService>()
                .Configure<SystemTextJsonOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                    if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                    {
                        configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                    }
                });
        }
    }
}
