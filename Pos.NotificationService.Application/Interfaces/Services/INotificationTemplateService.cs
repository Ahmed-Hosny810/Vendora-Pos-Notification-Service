using Pos.NotificationService.Application.DTOs.NotificationTemplates;
using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Interfaces.Services;

public interface INotificationTemplateService
{
    Task<Result<RenderedNotificationTemplateDto>> RenderAsync(
        string code,
        string channel,
        string language,
        IReadOnlyDictionary<string, string> values,
        CancellationToken cancellationToken);
}
