using Pos.NotificationService.Application.Features.NotificationTemplates.Queries.GetNotificationTemplatesQuery;
using Pos.NotificationService.Domain.Models;
namespace Pos.NotificationService.Infrastructure.Persistence.QueryExtensions;
public static class NotificationTemplateQueryExtensions
{
    public static IQueryable<NotificationTemplate> ApplyFilter(
        this IQueryable<NotificationTemplate> query, NotificationTemplateFilter? filter)
    {
        if (filter == null) return query;
        if (!string.IsNullOrWhiteSpace(filter.Code))
        {
            var code = filter.Code.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.Contains(code));
        }
        if (!string.IsNullOrWhiteSpace(filter.Channel))
            query = query.Where(x => x.Channel == filter.Channel);
        if (!string.IsNullOrWhiteSpace(filter.Language))
        {
            var language = filter.Language.Trim().ToLowerInvariant();
            query = query.Where(x => x.Language == language);
        }
        if (filter.IsActive.HasValue)
            query = query.Where(x => x.IsActive == filter.IsActive.Value);
        return query;
    }

    public static IOrderedQueryable<NotificationTemplate> ApplyOrder(
        this IQueryable<NotificationTemplate> query, GetNotificationTemplatesQueryParameter parameter)
    {
        var ordered = parameter.OrderKey == NotificationTemplateOrderKey.Code
            ? (parameter.OrderDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code))
            : (parameter.OrderDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt));
        return ordered.ThenBy(x => x.Id);
    }
}
