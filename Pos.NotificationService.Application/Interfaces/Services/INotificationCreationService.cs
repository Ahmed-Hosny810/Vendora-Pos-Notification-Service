using Pos.NotificationService.Application.DTOs.Notifications;
using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Interfaces.Services;

public interface INotificationCreationService
{
    /// <summary>Stages in-app notifications and returns their count. The caller commits them with the inbox record.</summary>
    Task<Result<int>> CreateAsync(CreateNotificationRequest request, CancellationToken cancellationToken);
}
