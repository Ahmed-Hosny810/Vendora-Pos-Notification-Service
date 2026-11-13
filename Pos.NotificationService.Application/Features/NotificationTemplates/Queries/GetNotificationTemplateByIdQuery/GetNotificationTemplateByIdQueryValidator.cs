using FluentValidation;
namespace Pos.NotificationService.Application.Features.NotificationTemplates.Queries.GetNotificationTemplateByIdQuery;
public class GetNotificationTemplateByIdQueryValidator : AbstractValidator<GetNotificationTemplateByIdQuery>
{
    public GetNotificationTemplateByIdQueryValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty();
    }
}
