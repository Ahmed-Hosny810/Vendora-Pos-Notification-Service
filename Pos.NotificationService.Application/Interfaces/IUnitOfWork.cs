using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
