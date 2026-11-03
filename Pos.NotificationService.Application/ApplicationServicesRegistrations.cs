using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.NotificationService.Application.Behaviours;
using System.Reflection;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Features.NotificationTemplates.Services;
using Pos.NotificationService.Application.Features.Notifications.Services;

namespace Pos.NotificationService.Application
{
    public static class ApplicationServicesRegistrations
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(cfg =>
                cfg.AddMaps(Assembly.GetExecutingAssembly())
            );
            services.AddScoped<INotificationCreationService, NotificationCreationService>();
            services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        }
    }
}
