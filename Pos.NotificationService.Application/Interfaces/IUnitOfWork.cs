using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<Pos.NotificationService.Application.Wrappers.Result<int>> TrySaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
