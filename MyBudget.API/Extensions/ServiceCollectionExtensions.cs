using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyBudget.API.Localization;
using MyBudget.API.Managers.Preferences;
using MyBudget.API.Services;
using MyBudget.API.Settings;
using Newtonsoft.Json;
using MyBudget.Application.Configurations;
using MyBudget.Application.Interfaces.Serialization.Options;
using MyBudget.Application.Interfaces.Serialization.Serializers;
using MyBudget.Application.Interfaces.Serialization.Settings;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Interfaces.Services.Account;
using MyBudget.Application.Interfaces.Services.Identity;
using MyBudget.Application.Serialization.JsonConverters;
using MyBudget.Application.Serialization.Options;
using MyBudget.Application.Serialization.Serializers;
using MyBudget.Application.Serialization.Settings;
using MyBudget.Infra.Shared.Services;
using MyBudget.Infrastructure;
using MyBudget.Infrastructure.Contexts;
using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Infrastructure.Services;
using MyBudget.Infrastructure.Services.Identity;
using MyBudget.Shared.Constants.Application;
using MyBudget.Shared.Constants.Localization;
using MyBudget.Shared.Constants.Permission;
using MyBudget.Shared.Wrapper;
using Serilog;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace MyBudget.API.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static async Task<IStringLocalizer> GetRegisteredServerLocalizerAsync<T>(this IServiceCollection services) where T : class
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            await SetCultureFromServerPreferenceAsync(serviceProvider);
            IStringLocalizer<T>? localizer = serviceProvider.GetService<IStringLocalizer<T>>();
            await serviceProvider.DisposeAsync();
            return localizer!;
        }

        internal static IServiceCollection AddForwarding(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            AppConfiguration config = applicationSettingsConfiguration.Get<AppConfiguration>();
            if (config.BehindSSLProxy)
            {
                _ = services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    if (!string.IsNullOrWhiteSpace(config.ProxyIP))
                    {
                        string ipCheck = config.ProxyIP;
                        if (IPAddress.TryParse(ipCheck, out IPAddress? proxyIP))
                        {
                            options.KnownProxies.Add(proxyIP);
                        }
                        else
                        {
                            Log.Logger.Warning("Invalid Proxy IP of {IpCheck}, Not Loaded", ipCheck);
                        }
                    }
                });

                _ = services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            _ = builder
                                .AllowCredentials()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowAnyOrigin();
                        });
                });
            }

            return services;
        }

        private static async Task SetCultureFromServerPreferenceAsync(IServiceProvider serviceProvider)
        {
            ServerPreferenceManager? storageService = serviceProvider.GetService<ServerPreferenceManager>();
            if (storageService != null)
            {
                // TODO - should implement ServerStorageProvider to work correctly!
                CultureInfo culture = await storageService.GetPreference() is ServerPreference preference
                    ? (new(preference.LanguageCode))
                    : (new(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US"));
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }

        internal static IServiceCollection AddServerLocalization(this IServiceCollection services)
        {
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));
            return services;
        }

        internal static AppConfiguration GetApplicationSettings(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            IConfigurationSection applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            _ = services.Configure<AppConfiguration>(applicationSettingsConfiguration);
            return applicationSettingsConfiguration.Get<AppConfiguration>();
        }

        internal static void RegisterSwagger(this IServiceCollection services)
        {
            _ = services.AddSwaggerGen(async c =>
            {
                //TODO - Lowercase Swagger Documents
                //c.DocumentFilter<LowercaseDocumentFilter>();
                //Refer - https://gist.github.com/rafalkasa/01d5e3b265e5aa075678e0adfd54e23f

                // include all project's xml comments
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.IsDynamic)
                    {
                        string xmlFile = $"{assembly.GetName().Name}.xml";
                        string xmlPath = Path.Combine(baseDirectory, xmlFile);
                        if (File.Exists(xmlPath))
                        {
                            c.IncludeXmlComments(xmlPath);
                        }
                    }
                }

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MyBudget.API",
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                IStringLocalizer localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = localizer["Input your Bearer token in this format - Bearer {your token here} to access this API"],
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });
            });
        }

        internal static IServiceCollection AddSerialization(this IServiceCollection services)
        {
            _ = services
                .AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>()
                .Configure<SystemTextJsonOptions>(configureOptions =>
                {
                    if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                    {
                        configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                    }
                });
            _ = services.AddScoped<IJsonSerializerSettings, NewtonsoftJsonSettings>();

            _ = services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); // you can change it
            return services;
        }

        internal static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services
                        .AddDbContext<ApplicationDbContext>(options => options
                            .UseMySQL(configuration.GetConnectionString("DefaultConnection")))
                    .AddTransient<IDatabaseSeeder, DatabaseSeeder>();
        }

        internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {
            _ = services.AddHttpContextAccessor();
            _ = services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }

        internal static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            _ = services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        internal static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddTransient<IDateTimeService, SystemDateTimeService>();
            _ = services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
            _ = services.AddTransient<IMailService, SMTPMailService>();
            return services;
        }

        internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            _ = services.AddTransient<IRoleClaimService, RoleClaimService>();
            _ = services.AddTransient<ITokenService, IdentityService>();
            _ = services.AddTransient<IRoleService, RoleService>();
            _ = services.AddTransient<IAccountService, AccountService>();
            _ = services.AddTransient<IUserService, UserService>();
            _ = services.AddTransient<IChatService, ChatService>();
            _ = services.AddTransient<IUploadService, UploadService>();
            _ = services.AddTransient<IAuditService, AuditService>();
            _ = services.AddScoped<IExcelService, ExcelService>();
            return services;
        }

        internal static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services, AppConfiguration config)
        {
            byte[] key = Encoding.UTF8.GetBytes(config.Secret);
            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(async bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };

                    IStringLocalizer localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                    bearer.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            Microsoft.Extensions.Primitives.StringValues accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            PathString path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments(ApplicationConstants.SignalR.HubUrl))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = c =>
                        {
                            if (c.Exception is SecurityTokenExpiredException)
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                c.Response.ContentType = "application/json";
                                string result = JsonConvert.SerializeObject(Result.Fail(localizer["The Token is expired."]));
                                return c.Response.WriteAsync(result);
                            }
                            else
                            {
#if DEBUG
                                c.NoResult();
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "text/plain";
                                return c.Response.WriteAsync(c.Exception.ToString());
#else
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));
                                return c.Response.WriteAsync(result);
#endif
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                string result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not Authorized."]));
                                return context.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";
                            string result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not authorized to access this resource."]));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });
            services.AddAuthorization(options =>
            {
                // Here I stored necessary permissions/roles in a constant
                foreach (FieldInfo? prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    object? propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                    {
                        options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                    }
                }
            });
            return services;
        }
    }
}
