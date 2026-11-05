using MediatR;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Features.Notifications.Commands.MarkNotificationAsReadCommand;

public class MarkNotificationAsReadCommand : IRequest<Result<Guid>>
{
    public Guid NotificationId { get; set; }
}

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result<Guid>>
{
    private readonly INotificationRepositoryAsync _repository;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationAsReadCommandHandler(INotificationRepositoryAsync repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId;
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
            throw new UnauthorizedAccessException("A valid tenant is required.");

        if (!Guid.TryParse(_currentUser.UserId, out var userId) || userId == Guid.Empty)
            throw new UnauthorizedAccessException("A valid user is required.");

        var found = await _repository.MarkNotificationAsReadAsync(
            tenantId.Value, userId, request.NotificationId, DateTime.UtcNow, cancellationToken);

        return found
            ? Result<Guid>.Success(request.NotificationId)
            : Result<Guid>.Failure("Notification was not found.");
    }
}
