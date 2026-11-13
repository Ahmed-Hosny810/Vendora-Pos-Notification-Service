using MediatR;
using Pos.NotificationService.Application.Interfaces;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Features.NotificationTemplates.Commands.SetActiveStatusCommand;
public class SetNotificationTemplateActiveStatusCommand : IRequest<Result<Guid>>
{
    public Guid TemplateId { get; set; }
    public bool IsActive { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
public class SetNotificationTemplateActiveStatusCommandHandler(
    INotificationTemplateRepositoryAsync repository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<SetNotificationTemplateActiveStatusCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SetNotificationTemplateActiveStatusCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId) || userId == Guid.Empty ||
            currentUser.UserType != "Platform" ||
            (!currentUser.Roles.Contains("Admin") && !currentUser.Roles.Contains("SuperAdmin")))
            return Result<Guid>.Failure("Only platform administrators can manage notification templates.");

        var template = await repository.GetForUpdateAsync(request.TemplateId, cancellationToken);
        if (template == null)
            return Result<Guid>.Failure("Template was not found.");

        if (!template.RowVersion.SequenceEqual(request.RowVersion))
            return Result<Guid>.Failure("This template has changed. Reload it before updating.");

        template.IsActive = request.IsActive;

        template.UpdatedAt = DateTime.UtcNow;
        repository.PrepareUpdate(template, request.RowVersion);
        var saved = await unitOfWork.TrySaveChangesAsync(cancellationToken);
        return saved.IsSuccess
            ? Result<Guid>.Success(template.Id)
            : Result<Guid>.Failure(saved.Errors.ToArray());
    }
}
