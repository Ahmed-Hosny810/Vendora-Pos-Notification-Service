using Pos.NotificationService.Application.Features.NotificationTemplates.DTOs;
using Pos.NotificationService.Application.Features.NotificationTemplates.Queries.GetNotificationTemplatesQuery;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Application.Interfaces.Repositories;

public interface INotificationTemplateRepositoryAsync : IGenericRepositoryAsync<NotificationTemplate, Guid>
{
    Task<NotificationTemplate?> GetActiveTemplateAsync(string code,
        string channel, string language, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string code, string channel, string language,
        CancellationToken cancellationToken);

    Task<NotificationTemplate?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken);
    void PrepareUpdate(NotificationTemplate template, byte[] rowVersion);
    Task<bool> ExistsExceptAsync(string code, string channel, string language, Guid excludedId, CancellationToken cancellationToken);
    Task<NotificationTemplateDto?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResponse<IEnumerable<NotificationTemplateDto>>> GetTemplatesAsync(
        GetNotificationTemplatesQueryParameter parameter, CancellationToken cancellationToken);
}

