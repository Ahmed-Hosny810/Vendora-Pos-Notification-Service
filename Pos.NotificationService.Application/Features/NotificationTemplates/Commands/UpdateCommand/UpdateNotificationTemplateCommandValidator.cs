using FluentValidation;
using Pos.NotificationService.Domain.Constants;
namespace Pos.NotificationService.Application.Features.NotificationTemplates.Commands.UpdateCommand;
public class UpdateNotificationTemplateCommandValidator : AbstractValidator<UpdateNotificationTemplateCommand>
{
    public UpdateNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty();
        RuleFor(x => x.RowVersion).Must(value => value is { Length: 8 }).WithMessage("A valid row version is required.");
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100)
            .Must(code => !string.IsNullOrWhiteSpace(code) &&
                System.Text.RegularExpressions.Regex.IsMatch(code.Trim(), @"^[A-Za-z][A-Za-z0-9_.-]*$"))
            .WithMessage("Code must start with a letter and contain only letters, digits, dots, underscores or hyphens.");
        RuleFor(x => x.Channel).Must(channel => channel is NotificationChannels.InApp
            or NotificationChannels.Email or NotificationChannels.Sms or NotificationChannels.WhatsApp)
            .WithMessage("Select a supported notification channel.");
        RuleFor(x => x.Language).NotEmpty().MaximumLength(10)
            .Must(language => !string.IsNullOrWhiteSpace(language) &&
                System.Text.RegularExpressions.Regex.IsMatch(language.Trim(), @"^[A-Za-z]{2,3}(-[A-Za-z]{2})?$"))
            .WithMessage("Use a language code such as en, ar or ar-EG.");
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(20000);
    }
}
