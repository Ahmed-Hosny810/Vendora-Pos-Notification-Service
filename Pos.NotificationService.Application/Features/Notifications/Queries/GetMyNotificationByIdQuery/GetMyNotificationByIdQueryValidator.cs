using FluentValidation;

namespace Pos.NotificationService.Application.Features.Notifications.Queries.GetMyNotificationByIdQuery;

public class GetMyNotificationByIdQueryValidator : AbstractValidator<GetMyNotificationByIdQuery>
{
    public GetMyNotificationByIdQueryValidator()
    {
        RuleFor(x => x.NotificationId).NotEmpty();
    }
}
