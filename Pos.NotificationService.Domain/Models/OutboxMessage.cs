using Pos.NotificationService.Domain.Constants;

namespace Pos.NotificationService.Domain.Models;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? NextAttemptAt { get; set; }
    public string? LastError { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
