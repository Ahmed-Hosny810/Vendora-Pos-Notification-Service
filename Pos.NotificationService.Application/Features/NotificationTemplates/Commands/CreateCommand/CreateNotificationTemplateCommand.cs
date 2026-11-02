using MediatR;
using Pos.NotificationService.Application.Interfaces;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Application.Features.NotificationTemplates.Commands.CreateCommand;

public class CreateNotificationTemplateCommand : IRequest<Result<Guid>>
{
    public string Code { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class CreateNotificationTemplateCommandHandler(
    INotificationTemplateRepositoryAsync repository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateNotificationTemplateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateNotificationTemplateCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId) || userId == Guid.Empty)
            return Result<Guid>.Failure("A valid user is required.");

        if (currentUser.UserType != "Platform" ||
            (!currentUser.Roles.Contains("Admin") && !currentUser.Roles.Contains("SuperAdmin")))
        {
            return Result<Guid>.Failure("Only platform administrators can manage notification templates.");
        }

        var code = request.Code.Trim().ToUpperInvariant();
        var language = request.Language.Trim().ToLowerInvariant();

        if (await repository.ExistsAsync(code, request.Channel, language, cancellationToken))
            return Result<Guid>.Failure("A template with this code, channel and language already exists.");

        var template = new NotificationTemplate
        {
            Id = Guid.NewGuid(),
            Code = code,
            Channel = request.Channel,
            Language = language,
            Subject = request.Subject.Trim(),
            Body = request.Body.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(template, cancellationToken);
        var saved = await unitOfWork.TrySaveChangesAsync(cancellationToken);
        if (saved.IsFailure)
            return Result<Guid>.Failure(saved.Errors.ToArray());

        return Result<Guid>.Success(template.Id);
    }
}

