using FluentValidation;

namespace Pos.NotificationService.Application.Features.Notifications.Queries.GetMyNotificationsQuery;

public class GetMyNotificationsQueryParameterValidator : AbstractValidator<GetMyNotificationsQueryParameter>
{
    public GetMyNotificationsQueryParameterValidator()
    {
        RuleFor(x => x.OrderKey).IsInEnum();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
        RuleFor(x => x).Must(x =>
            x.PageNumber <= 0 || x.PageSize <= 0 ||
            ((long)x.PageNumber - 1) * x.PageSize <= int.MaxValue)
            .WithMessage("The requested page exceeds the supported pagination range.");
    }
}
