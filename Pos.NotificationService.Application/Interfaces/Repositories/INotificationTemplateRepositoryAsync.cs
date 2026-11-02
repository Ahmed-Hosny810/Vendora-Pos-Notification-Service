using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Application.Interfaces.Repositories;

public interface INotificationTemplateRepositoryAsync : IGenericRepositoryAsync<NotificationTemplate, Guid>
{
    Task<NotificationTemplate?> GetActiveTemplateAsync(string code,
        string channel, string language, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string code, string channel, string language,
        CancellationToken cancellationToken);
}

