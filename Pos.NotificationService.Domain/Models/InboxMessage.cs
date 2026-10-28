using Pos.NotificationService.Domain.Constants;

namespace Pos.NotificationService.Domain.Models;

public class InboxMessage
{
    public Guid MessageId { get; set; }
    public string ConsumerName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
