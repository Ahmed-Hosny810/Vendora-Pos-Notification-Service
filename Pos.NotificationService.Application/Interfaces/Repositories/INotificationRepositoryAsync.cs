using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Application.Interfaces.Repositories;

public interface INotificationRepositoryAsync : IGenericRepositoryAsync<Notification, Guid>
{
}
