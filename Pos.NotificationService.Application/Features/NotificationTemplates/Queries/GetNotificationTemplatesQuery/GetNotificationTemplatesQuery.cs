using MediatR;
using Pos.NotificationService.Application.Interfaces;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Application.Features.NotificationTemplates.DTOs;
namespace Pos.NotificationService.Application.Features.NotificationTemplates.Queries.GetNotificationTemplatesQuery;
public class GetNotificationTemplatesQuery : IRequest<PagedResponse<IEnumerable<NotificationTemplateDto>>>
{
    public GetNotificationTemplatesQueryParameter Parameter { get; set; } = new();
}
public class GetNotificationTemplatesQueryHandler(INotificationTemplateRepositoryAsync repository,
    ICurrentUserService currentUser) : IRequestHandler<GetNotificationTemplatesQuery, PagedResponse<IEnumerable<NotificationTemplateDto>>>
{
    public async Task<PagedResponse<IEnumerable<NotificationTemplateDto>>> Handle(GetNotificationTemplatesQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId) || userId == Guid.Empty ||
            currentUser.UserType != "Platform" ||
            (!currentUser.Roles.Contains("Admin") && !currentUser.Roles.Contains("SuperAdmin")))
            throw new UnauthorizedAccessException("Only platform administrators can manage notification templates.");
        return await repository.GetTemplatesAsync(request.Parameter, cancellationToken);
    }
}
