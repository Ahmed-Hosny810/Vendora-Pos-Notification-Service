using Microsoft.EntityFrameworkCore;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Domain.Models;
using Pos.NotificationService.Infrastructure.Persistence.Contexts;

namespace Pos.NotificationService.Infrastructure.Persistence.Repositories;

public class InboxMessageRepositoryAsync : GenericRepositoryAsync<InboxMessage, Guid>, IInboxMessageRepositoryAsync
{
    private readonly ApplicationDbContext _context;

    public InboxMessageRepositoryAsync(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<bool> ExistsAsync(Guid messageId, string consumerName, CancellationToken cancellationToken)
    {
        return _context.InboxMessages.AsNoTracking().AnyAsync(
            x => x.MessageId == messageId && x.ConsumerName == consumerName, cancellationToken);
    }
}
