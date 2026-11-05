using FluentValidation;

namespace Pos.NotificationService.Application.Features.Notifications.Commands.MarkNotificationAsReadCommand;

public class MarkNotificationAsReadCommandValidator : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadCommandValidator()
    {
        RuleFor(x => x.NotificationId).NotEmpty();
    }
}
