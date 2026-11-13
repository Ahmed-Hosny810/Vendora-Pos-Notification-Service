using Pos.NotificationService.Application.Parameters;
namespace Pos.NotificationService.Application.Features.NotificationTemplates.Queries.GetNotificationTemplatesQuery;

public class GetNotificationTemplatesQueryParameter : RequestParameter<NotificationTemplateOrderKey>
{
    public GetNotificationTemplatesQueryParameter()
    {
        OrderKey = NotificationTemplateOrderKey.CreatedAt;
        OrderDescending = true;
    }
    public NotificationTemplateFilter? Filter { get; set; }
}
public class NotificationTemplateFilter
{
    public string? Code { get; set; }
    public string? Channel { get; set; }
    public string? Language { get; set; }
    public bool? IsActive { get; set; }
}
public enum NotificationTemplateOrderKey { CreatedAt, Code }
