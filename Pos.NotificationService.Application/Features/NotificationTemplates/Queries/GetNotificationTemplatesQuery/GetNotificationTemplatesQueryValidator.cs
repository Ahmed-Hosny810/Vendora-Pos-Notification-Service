using FluentValidation;
using Pos.NotificationService.Domain.Constants;
namespace Pos.NotificationService.Application.Features.NotificationTemplates.Queries.GetNotificationTemplatesQuery;
public class GetNotificationTemplatesQueryValidator : AbstractValidator<GetNotificationTemplatesQuery>
{
    public GetNotificationTemplatesQueryValidator()
    {
        RuleFor(x => x.Parameter).NotNull();
        When(x => x.Parameter != null, () =>
        {
            RuleFor(x => x.Parameter.PageNumber).GreaterThan(0)
                .Must((q, page) => ((long)page - 1) * q.Parameter.PageSize <= int.MaxValue)
                .WithMessage("The requested page offset is too large.");
            RuleFor(x => x.Parameter.PageSize).InclusiveBetween(1, 50);
            RuleFor(x => x.Parameter.OrderKey).IsInEnum();
            When(x => x.Parameter.Filter != null, () =>
            {
                RuleFor(x => x.Parameter.Filter!.Code).MaximumLength(100);
                RuleFor(x => x.Parameter.Filter!.Language).MaximumLength(10);
                RuleFor(x => x.Parameter.Filter!.Channel)
                    .Must(c => string.IsNullOrWhiteSpace(c) || c is NotificationChannels.InApp
                        or NotificationChannels.Email or NotificationChannels.Sms or NotificationChannels.WhatsApp)
                    .WithMessage("Select a supported notification channel.");
            });
        });
    }
}
