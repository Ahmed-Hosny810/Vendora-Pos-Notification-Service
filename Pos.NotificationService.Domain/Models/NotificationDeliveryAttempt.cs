using Pos.NotificationService.Domain.Constants;

namespace Pos.NotificationService.Domain.Models;

public class NotificationDeliveryAttempt
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid NotificationId { get; set; }
    public int AttemptNumber { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? ProviderMessageId { get; set; }
    public string Status { get; set; } = DeliveryAttemptStatuses.Unknown;
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    public string? FailureReason { get; set; }
    public Notification Notification { get; set; } = null!;
}
