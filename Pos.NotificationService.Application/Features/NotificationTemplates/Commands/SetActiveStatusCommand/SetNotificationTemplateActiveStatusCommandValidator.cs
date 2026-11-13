using FluentValidation;
using Pos.NotificationService.Domain.Constants;
namespace Pos.NotificationService.Application.Features.NotificationTemplates.Commands.SetActiveStatusCommand;
public class SetNotificationTemplateActiveStatusCommandValidator : AbstractValidator<SetNotificationTemplateActiveStatusCommand>
{
    public SetNotificationTemplateActiveStatusCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty();
        RuleFor(x => x.RowVersion).Must(value => value is { Length: 8 }).WithMessage("A valid row version is required.");
    }
}
