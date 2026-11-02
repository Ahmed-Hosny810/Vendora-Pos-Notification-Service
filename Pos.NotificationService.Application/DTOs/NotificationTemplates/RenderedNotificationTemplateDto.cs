namespace Pos.NotificationService.Application.DTOs.NotificationTemplates;

public class RenderedNotificationTemplateDto
{
    public Guid TemplateId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
