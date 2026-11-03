using FluentValidation;
using Pos.NotificationService.Application.DTOs.Identity;
using Pos.NotificationService.Application.DTOs.Notifications;
using Pos.NotificationService.Application.Interfaces.Clients;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Domain.Constants;
using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Application.Features.Notifications.Services;

public class NotificationCreationService : INotificationCreationService
{
    private readonly INotificationRecipientClient _recipientClient;
    private readonly INotificationTemplateService _templateService;
    private readonly INotificationRepositoryAsync _repository;
    private readonly IValidator<CreateNotificationRequest> _validator;

    public NotificationCreationService(
        INotificationRecipientClient recipientClient,
        INotificationTemplateService templateService,
        INotificationRepositoryAsync repository,
        IValidator<CreateNotificationRequest> validator)
    {
        _recipientClient = recipientClient;
        _templateService = templateService;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result<int>> CreateAsync(
        CreateNotificationRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request == null)
            return Result<int>.Failure("Notification creation request is required.");

        
        var validation = await _validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return Result<int>.Failure(validation.Errors.Select(x => x.ErrorMessage).ToArray());

        var recipientsResult = await _recipientClient.GetRecipientsAsync(
            new GetNotificationRecipientsRequest
            {
                TenantId = request.TenantId,
                BranchId = request.BranchId,
                Roles = request.Roles.Distinct(StringComparer.Ordinal).ToList()
            }, cancellationToken);

        if (recipientsResult.IsFailure)
            return Result<int>.Failure(recipientsResult.Errors.ToArray());

        if (recipientsResult.Value == null ||
            recipientsResult.Value.Any(x => x == null || x.UserId == Guid.Empty))
            return Result<int>.Failure("Identity returned invalid notification recipients.");

        var recipients = recipientsResult.Value.DistinctBy(x => x.UserId).ToList();
        if (recipients.Count == 0)
            return Result<int>.Success(0);

        var templateResult = await _templateService.RenderAsync(
            request.TemplateCode, request.Channel, request.Language,
            request.Values, cancellationToken);

        if (templateResult.IsFailure)
            return Result<int>.Failure(templateResult.Errors.ToArray());

        var template = templateResult.Value;
        if (template == null)
            return Result<int>.Failure("The rendered notification template is missing.");

        var now = DateTime.UtcNow;
        var type = request.TemplateCode.Trim().ToUpperInvariant();

        foreach (var recipient in recipients)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _repository.AddAsync(new Notification
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                BranchId = request.BranchId,
                SourceEventId = request.SourceEventId,
                TemplateId = template.TemplateId,
                RecipientUserId = recipient.UserId,
                Channel = NotificationChannels.InApp,
                Type = type,
                Title = template.Title,
                Body = template.Body,
                ReferenceType = request.ReferenceType?.Trim(),
                ReferenceId = request.ReferenceId,
                CreatedAt = now,
                // In-app delivery is available on commit; ReadAt remains null until read.
                Status = NotificationStatuses.Sent,
                SentAt = now
            }, cancellationToken);
        }

        // The dispatcher saves these records and its inbox entry together.
        return Result<int>.Success(recipients.Count);
    }
}
