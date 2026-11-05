using MediatR;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Features.Notifications.Commands.MarkAllNotificationsAsReadCommand;

public class MarkAllNotificationsAsReadCommand : IRequest<int>
{
}

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, int>
{
    private readonly INotificationRepositoryAsync _repository;
    private readonly ICurrentUserService _currentUser;

    public MarkAllNotificationsAsReadCommandHandler(INotificationRepositoryAsync repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId;
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
            throw new UnauthorizedAccessException("A valid tenant is required.");

        if (!Guid.TryParse(_currentUser.UserId, out var userId) || userId == Guid.Empty)
            throw new UnauthorizedAccessException("A valid user is required.");

        return await _repository.MarkAllNotificationsAsReadAsync(
            tenantId.Value, userId, DateTime.UtcNow, cancellationToken);
    }
}
