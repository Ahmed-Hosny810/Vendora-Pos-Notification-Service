using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Infrastructure.Shared.Services;
using Pos.NotificationService.Infrastructure.Shared.Workers;
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

        services.AddScoped<IInboxEventDispatcher, InboxEventDispatcher>();
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.Configure<RabbitMqConsumerOptions>(configuration.GetSection("RabbitMqConsumer"));

        if (configuration.GetValue<bool>("RabbitMqConsumer:Enabled"))
        {
            services.AddOptions<RabbitMqOptions>()
                .Validate(x => !string.IsNullOrWhiteSpace(x.HostName) && x.Port is > 0 and <= 65535 &&
                    !string.IsNullOrWhiteSpace(x.UserName) && !string.IsNullOrWhiteSpace(x.Password) &&
                    !string.IsNullOrWhiteSpace(x.VirtualHost), "RabbitMQ connection settings are required.")
                .ValidateOnStart();
            services.AddOptions<RabbitMqConsumerOptions>()
                .Validate(x => !string.IsNullOrWhiteSpace(x.SourceExchangeName) &&
                    !string.IsNullOrWhiteSpace(x.QueueName) && !string.IsNullOrWhiteSpace(x.DeadLetterExchange) &&
                    !string.IsNullOrWhiteSpace(x.DeadLetterQueue) &&
                    x.RoutingKeys is { Length: > 0 } && x.RoutingKeys.All(k => !string.IsNullOrWhiteSpace(k)) &&
                    x.PrefetchCount > 0 && x.MaxDeliveryAttempts > 0, "Invalid RabbitMQ consumer settings.")
                .ValidateOnStart();
            services.AddHostedService<RabbitMqConsumerWorker>();
        }

        return services;
    }
}
