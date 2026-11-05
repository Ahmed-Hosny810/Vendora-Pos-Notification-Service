using MediatR;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Application.Features.Notifications.DTOs;

namespace Pos.NotificationService.Application.Features.Notifications.Queries.GetMyNotificationByIdQuery;

public class GetMyNotificationByIdQuery : IRequest<Result<NotificationDetailsDto>>
{
    public Guid NotificationId { get; set; }
}

public class GetMyNotificationByIdQueryHandler : IRequestHandler<GetMyNotificationByIdQuery, Result<NotificationDetailsDto>>
{
    private readonly INotificationRepositoryAsync _repository;
    private readonly ICurrentUserService _currentUser;

    public GetMyNotificationByIdQueryHandler(INotificationRepositoryAsync repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result<NotificationDetailsDto>> Handle(GetMyNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId;
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
            throw new UnauthorizedAccessException("A valid tenant is required.");

        if (!Guid.TryParse(_currentUser.UserId, out var userId) || userId == Guid.Empty)
            throw new UnauthorizedAccessException("A valid user is required.");

        var notification = await _repository.GetMyNotificationByIdAsync(
            tenantId.Value, userId, request.NotificationId, cancellationToken);

        return notification == null
            ? Result<NotificationDetailsDto>.Failure("Notification was not found.")
            : Result<NotificationDetailsDto>.Success(notification);
    }
}
