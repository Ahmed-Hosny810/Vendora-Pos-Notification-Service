using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Pos.NotificationService.Infrastructure.Shared;
public static class ServiceRegistrations
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        return services;
    }
}
