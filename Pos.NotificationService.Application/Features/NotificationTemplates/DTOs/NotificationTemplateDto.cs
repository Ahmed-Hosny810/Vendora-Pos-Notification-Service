namespace Pos.NotificationService.Application.Features.NotificationTemplates.DTOs;
public class NotificationTemplateDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
