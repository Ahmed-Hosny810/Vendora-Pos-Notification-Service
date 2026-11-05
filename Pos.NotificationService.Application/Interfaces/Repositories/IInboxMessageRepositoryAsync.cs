using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Application.Interfaces.Repositories;

public interface IInboxMessageRepositoryAsync : IGenericRepositoryAsync<InboxMessage, Guid>
{
    Task<bool> ExistsAsync(Guid messageId, string consumerName, CancellationToken cancellationToken);
}
