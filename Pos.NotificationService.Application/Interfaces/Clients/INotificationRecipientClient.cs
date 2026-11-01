using Pos.NotificationService.Application.DTOs.Identity;
using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Interfaces.Clients;

public interface INotificationRecipientClient
{
    Task<Result<IReadOnlyList<NotificationRecipientDto>>> GetRecipientsAsync(
        GetNotificationRecipientsRequest request,
        CancellationToken cancellationToken = default);
}
