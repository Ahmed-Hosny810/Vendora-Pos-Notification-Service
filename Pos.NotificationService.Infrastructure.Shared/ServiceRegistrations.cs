using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pos.NotificationService.Application.Interfaces.Clients;
using Pos.NotificationService.Infrastructure.Shared.Clients;
using Pos.NotificationService.Infrastructure.Shared.Options;

namespace Pos.NotificationService.Infrastructure.Shared;

public static class ServiceRegistrations
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<IdentityRecipientClientOptions>()
            .Bind(configuration.GetSection(IdentityRecipientClientOptions.SectionName))
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri)
                && uri.Scheme == Uri.UriSchemeHttps, "IdentityRecipientClient:BaseUrl must be an absolute HTTPS URL.")
            .Validate(options => options.TimeoutSeconds is >= 1 and <= 120,
                "IdentityRecipientClient:TimeoutSeconds must be between 1 and 120.");

        services.AddHttpClient<INotificationRecipientClient, NotificationRecipientClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<IdentityRecipientClientOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });

        return services;
    }
}
