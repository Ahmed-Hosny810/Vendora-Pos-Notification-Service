using System.Globalization;
using System.Text.Json;
using Pos.NotificationService.Application.Constants;
using Pos.NotificationService.Application.DTOs.Notifications;
using Pos.NotificationService.Application.Events;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Features.Notifications.EventHandlers;

public class LowStockDetectedHandler : IIntegrationEventHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly INotificationCreationService _notificationService;

    public LowStockDetectedHandler(INotificationCreationService notificationService)
    {
        _notificationService = notificationService;
    }

    public string EventType => nameof(LowStockDetected);
    public string ConsumerName => "Notification.LowStockDetected.v1";

    public async Task<Result> HandleAsync(string payload, CancellationToken cancellationToken)
    {
        LowStockDetected? notification;
        try
        {
            notification = JsonSerializer.Deserialize<LowStockDetected>(payload, JsonOptions);
        }
        catch (JsonException)
        {
            return Result.Failure("Invalid low-stock event JSON.");
        }

        if (notification == null || notification.EventId == Guid.Empty ||
            notification.TenantId == Guid.Empty || notification.AlertId == Guid.Empty ||
            notification.BranchId == Guid.Empty || notification.ProductId == Guid.Empty ||
            notification.ProductVariantId == Guid.Empty || notification.DetectedAt == default ||
            notification.Threshold < 0 || notification.QuantityOnHand > notification.Threshold)
            return Result.Failure("Invalid low-stock event data.");

        var result = await _notificationService.CreateAsync(new CreateNotificationRequest
        {
            TenantId = notification.TenantId,
            SourceEventId = notification.EventId,
            BranchId = notification.BranchId,
            TemplateCode = EventType,
            Language = "en",
            Roles = new List<string> { TenantRoles.TenantOwner, TenantRoles.Admin },
            ReferenceType = "LowStockAlert",
            ReferenceId = notification.AlertId,
            Values = new Dictionary<string, string>
            {
                ["AlertId"] = notification.AlertId.ToString(),
                ["BranchId"] = notification.BranchId.ToString(),
                ["ProductId"] = notification.ProductId.ToString(),
                ["ProductVariantId"] = notification.ProductVariantId?.ToString() ?? string.Empty,
                ["QuantityOnHand"] = notification.QuantityOnHand.ToString(CultureInfo.InvariantCulture),
                ["Threshold"] = notification.Threshold.ToString(CultureInfo.InvariantCulture),
                ["DetectedAt"] = notification.DetectedAt.ToString("O", CultureInfo.InvariantCulture)
            }
        }, cancellationToken);

        return result.IsSuccess ? Result.Success() : Result.Failure(result.Errors.ToArray());
    }
}
