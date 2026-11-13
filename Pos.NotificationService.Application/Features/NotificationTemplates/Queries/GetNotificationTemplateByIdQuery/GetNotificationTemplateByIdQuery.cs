using MediatR;
using Pos.NotificationService.Application.Interfaces;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Application.Features.NotificationTemplates.DTOs;
namespace Pos.NotificationService.Application.Features.NotificationTemplates.Queries.GetNotificationTemplateByIdQuery;
public class GetNotificationTemplateByIdQuery : IRequest<Result<NotificationTemplateDto>>
{
    public Guid TemplateId { get; set; }
}
public class GetNotificationTemplateByIdQueryHandler(INotificationTemplateRepositoryAsync repository,
    ICurrentUserService currentUser) : IRequestHandler<GetNotificationTemplateByIdQuery, Result<NotificationTemplateDto>>
{
    public async Task<Result<NotificationTemplateDto>> Handle(GetNotificationTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId) || userId == Guid.Empty ||
            currentUser.UserType != "Platform" ||
            (!currentUser.Roles.Contains("Admin") && !currentUser.Roles.Contains("SuperAdmin")))
            throw new UnauthorizedAccessException("Only platform administrators can manage notification templates.");
        var template = await repository.GetDetailsByIdAsync(request.TemplateId, cancellationToken);
        return template == null ? Result<NotificationTemplateDto>.Failure("Template was not found.")
            : Result<NotificationTemplateDto>.Success(template);
    }
}
