using Pos.NotificationService.Application.Features.Notifications.DTOs;
using MediatR;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Features.Notifications.Queries.GetMyNotificationsQuery
{
    public class GetMyNotificationsQuery
    : IRequest<PagedResponse<IEnumerable<NotificationDetailsDto>>>
    {
        public GetMyNotificationsQueryParameter Parameter { get; set; } = new();
    }

    public class GetMyNotificationsQueryHandler
        : IRequestHandler<GetMyNotificationsQuery, PagedResponse<IEnumerable<NotificationDetailsDto>>>
    {
        private readonly INotificationRepositoryAsync _notificationRepository;
        private readonly ICurrentUserService _currentUser;

        public GetMyNotificationsQueryHandler(
            INotificationRepositoryAsync notificationRepository,
            ICurrentUserService currentUser)
        {
            _notificationRepository = notificationRepository;
            _currentUser = currentUser;
        }

        public async Task<PagedResponse<IEnumerable<NotificationDetailsDto>>> Handle(
            GetMyNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUser.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                throw new UnauthorizedAccessException("A valid tenant is required.");

            if (!Guid.TryParse(_currentUser.UserId, out var userId) ||
                userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("A valid user is required.");
            }

            return await _notificationRepository.GetMyNotificationsAsync(
                tenantId.Value,
                userId,
                request.Parameter,
                cancellationToken);
        }
    }
}