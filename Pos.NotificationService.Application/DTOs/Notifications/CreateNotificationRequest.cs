using Pos.NotificationService.Domain.Constants;

namespace Pos.NotificationService.Application.DTOs.Notifications;

public class CreateNotificationRequest
{
    public Guid TenantId { get; set; }
    public Guid SourceEventId { get; set; }
    public Guid? BranchId { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string Channel { get; set; } = NotificationChannels.InApp;
    public string Language { get; set; } = "en";
    public List<string> Roles { get; set; } = new();
    public Dictionary<string, string> Values { get; set; } = new();
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
}
