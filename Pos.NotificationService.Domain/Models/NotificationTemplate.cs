using Pos.NotificationService.Domain.Constants;

namespace Pos.NotificationService.Domain.Models;

public class NotificationTemplate
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Channel { get; set; } = NotificationChannels.InApp;
    public string Language { get; set; } = "en";
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
