using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Domain.Models;
using Pos.NotificationService.Infrastructure.Persistence.Contexts;

namespace Pos.NotificationService.Infrastructure.Persistence.Repositories;

public class NotificationRepositoryAsync : GenericRepositoryAsync<Notification, Guid>, INotificationRepositoryAsync
{
    public NotificationRepositoryAsync(ApplicationDbContext context) : base(context)
    {
    }
}
