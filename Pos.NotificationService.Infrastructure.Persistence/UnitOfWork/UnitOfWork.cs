using Pos.NotificationService.Application.Interfaces;
using Pos.NotificationService.Infrastructure.Persistence.Contexts;
namespace Pos.NotificationService.Infrastructure.Persistence.UnitOfWork;
public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
