using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;
namespace Pos.NotificationService.WebApi.Policies;
public static class AppPolicies
{
    public const string AccessOwnNotifications = "Notifications.Own";
    public static IServiceCollection AddAppPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            var tenantPolicy = new AuthorizationPolicyBuilder(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireClaim("user_type", "Tenant")
                .RequireAssertion(context =>
                    Guid.TryParse(context.User.FindFirstValue("tenant_id"), out var tenantId) && tenantId != Guid.Empty &&
                    Guid.TryParse(context.User.FindFirstValue("sub") ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) && userId != Guid.Empty)
                .Build();
            options.AddPolicy(AccessOwnNotifications, tenantPolicy);
            options.FallbackPolicy = tenantPolicy;
        });
        // Billing notifications remain accessible after subscription expiry.
        // Future handlers must filter by both TenantId and RecipientUserId.
        return services;
    }
}
