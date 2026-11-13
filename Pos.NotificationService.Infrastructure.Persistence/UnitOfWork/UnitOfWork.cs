using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pos.NotificationService.Application.Interfaces;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Infrastructure.Persistence.Contexts;

namespace Pos.NotificationService.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);

    public async Task<Result<int>> TrySaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return Result<int>.Success(await SaveChangesAsync(cancellationToken));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<int>.Failure("The record changed while saving. Reload it and try again.");
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            return Result<int>.Failure("A record with the same unique values already exists.");
        }
    }
}

