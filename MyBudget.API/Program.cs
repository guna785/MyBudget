using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using MyBudget.API.Extensions;
using MyBudget.API.Filters;
using MyBudget.API.Managers.Preferences;
using MyBudget.API.Middlewares;
using MyBudget.Application.Extensions;
using MyBudget.Infrastructure.Contexts;
using MyBudget.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddForwarding(builder.Configuration);
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});
builder.Services.AddCurrentUserService();
builder.Services.AddSerialization();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddServerStorage(); //TODO - should implement ServerStorageProvider to work correctly!
builder.Services.AddScoped<ServerPreferenceManager>();
builder.Services.AddServerLocalization();
builder.Services.AddIdentity();
builder.Services.AddJwtAuthentication(builder.Services.GetApplicationSettings(builder.Configuration));
builder.Services.AddSignalR();
builder.Services.AddApplicationLayer();
builder.Services.AddApplicationServices();
builder.Services.AddRepositories();
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.RegisterSwagger();
builder.Services.InfrastructureMappings();
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
builder.Services.AddControllers().AddValidators();
//services.AddRazorPages();
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});
builder.Services.AddLazyCache();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
IStringLocalizer<Program> localizer;
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        if (context.Database.IsSqlServer())
        {
            context.Database.Migrate();
        }
        localizer = services.GetRequiredService<IStringLocalizer<Program>>();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while migrating or seeding the database.");

        throw;
    }
}
// Configure the HTTP request pipeline.
app.UseForwarding(app.Configuration);
app.UseExceptionHandling(app.Environment);
app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlerMiddleware>();
//app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
    RequestPath = new PathString("/Files")
});
app.UseRequestLocalizationByCulture();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    DashboardTitle = localizer["BlazorHero Jobs"],
    Authorization = new[] { new HangfireAuthorizationFilter() }
});
app.UseEndpoints();
app.ConfigureSwagger();
app.Initialize(app.Configuration);
app.MapControllers();

app.Run();
