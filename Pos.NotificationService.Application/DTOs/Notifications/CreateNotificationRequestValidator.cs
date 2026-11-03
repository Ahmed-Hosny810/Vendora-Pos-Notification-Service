using FluentValidation;
using Pos.NotificationService.Application.Constants;
using Pos.NotificationService.Domain.Constants;

namespace Pos.NotificationService.Application.DTOs.Notifications;

public class CreateNotificationRequestValidator : AbstractValidator<CreateNotificationRequest>
{
    public CreateNotificationRequestValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.SourceEventId).NotEmpty();
        RuleFor(x => x.BranchId).NotEqual(Guid.Empty).When(x => x.BranchId.HasValue);
        // TemplateCode also becomes Notification.Type (maximum 80 characters).
        RuleFor(x => x.TemplateCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Channel).Equal(NotificationChannels.InApp)
            .WithMessage("Only in-app notification creation is supported.");
        RuleFor(x => x.Language).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Roles).NotEmpty();
        RuleForEach(x => x.Roles).Must(TenantRoles.IsSupported)
            .WithMessage("Unsupported recipient role.");
        RuleFor(x => x.Values).NotNull();
        RuleFor(x => x.ReferenceType).MaximumLength(80);
        RuleFor(x => x.ReferenceId).NotEqual(Guid.Empty).When(x => x.ReferenceId.HasValue);
        RuleFor(x => x.ReferenceType).NotEmpty().When(x => x.ReferenceId.HasValue);
        RuleFor(x => x.ReferenceId).NotNull()
            .When(x => !string.IsNullOrWhiteSpace(x.ReferenceType));
    }
}
