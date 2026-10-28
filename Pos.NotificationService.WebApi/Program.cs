using Pos.NotificationService.Application;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Infrastructure.Persistence;
using Pos.NotificationService.Infrastructure.Shared;
using Pos.NotificationService.WebApi.Extensions;
using Pos.NotificationService.WebApi.Policies;
using Pos.NotificationService.WebApi.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, logger) => logger
    .ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services).Enrich.FromLogContext());
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddAppPolicies();
builder.Services.AddControllers();
builder.Services.AddApiVersioningExtension();
builder.Services.AddSwaggerExtension();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseExceptionHandler();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment()) app.UseSwaggerExtension();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health/live").AllowAnonymous();
app.Run();
public partial class Program { }
