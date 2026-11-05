using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Interfaces.Services
{
    public interface IInboxEventDispatcher
    {
        Task<Result> DispatchAsync(
            Guid eventId,
            string eventType,
            string payload,
            CancellationToken cancellationToken);
    }
}
