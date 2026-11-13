using MediatR;
using Pos.NotificationService.Application.Interfaces;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.Application.Features.NotificationTemplates.Commands.UpdateCommand;
public class UpdateNotificationTemplateCommand : IRequest<Result<Guid>>
{
    public Guid TemplateId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
public class UpdateNotificationTemplateCommandHandler(
    INotificationTemplateRepositoryAsync repository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateNotificationTemplateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateNotificationTemplateCommand request, CancellationToken cancellationToken)
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

        var code = request.Code.Trim().ToUpperInvariant();
        var language = request.Language.Trim().ToLowerInvariant();
        if (await repository.ExistsExceptAsync(code, request.Channel, language, template.Id, cancellationToken))
            return Result<Guid>.Failure("A template with this code, channel and language already exists.");

        template.Code = code;
        template.Channel = request.Channel;
        template.Language = language;
        template.Subject = request.Subject.Trim();
        template.Body = request.Body.Trim();

        template.UpdatedAt = DateTime.UtcNow;
        repository.PrepareUpdate(template, request.RowVersion);
        var saved = await unitOfWork.TrySaveChangesAsync(cancellationToken);
        return saved.IsSuccess
            ? Result<Guid>.Success(template.Id)
            : Result<Guid>.Failure(saved.Errors.ToArray());
    }
}
