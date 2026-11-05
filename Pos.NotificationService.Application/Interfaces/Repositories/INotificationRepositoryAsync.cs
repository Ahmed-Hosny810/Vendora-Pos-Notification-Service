using Pos.NotificationService.Application.Features.Notifications.DTOs;
using Pos.NotificationService.Application.Features.Notifications.Queries.GetMyNotificationsQuery;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Application.Interfaces.Repositories;

public interface INotificationRepositoryAsync : IGenericRepositoryAsync<Notification, Guid>
{
    // Scope by tenant and recipient; list InApp notifications only.
    // Order by CreatedAt, then Id for stable pagination.
    Task<PagedResponse<IEnumerable<NotificationDetailsDto>>> GetMyNotificationsAsync(
        Guid tenantId,
        Guid recipientUserId,
        GetMyNotificationsQueryParameter parameter,
        CancellationToken cancellationToken);
    Task<NotificationDetailsDto?> GetMyNotificationByIdAsync(
        Guid tenantId, Guid recipientUserId, Guid notificationId, CancellationToken cancellationToken);

    Task<int> GetUnreadNotificationCountAsync(
        Guid tenantId, Guid recipientUserId, CancellationToken cancellationToken);

    // These conditional updates execute immediately; they do not require SaveChanges.
    Task<bool> MarkNotificationAsReadAsync(
        Guid tenantId, Guid recipientUserId, Guid notificationId,
        DateTime readAt, CancellationToken cancellationToken);

    Task<int> MarkAllNotificationsAsReadAsync(
        Guid tenantId, Guid recipientUserId, DateTime readAt, CancellationToken cancellationToken);
}
