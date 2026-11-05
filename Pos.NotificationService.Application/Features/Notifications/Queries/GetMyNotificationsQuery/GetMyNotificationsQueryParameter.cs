using Pos.NotificationService.Application.Parameters;

namespace Pos.NotificationService.Application.Features.Notifications.Queries.GetMyNotificationsQuery;

public class GetMyNotificationsQueryParameter : RequestParameter<NotificationOrderKey>
{
    public GetMyNotificationsQueryParameter()
    {
        OrderKey = NotificationOrderKey.CreatedAt;
        OrderDescending = true;
    }

    public NotificationFilter? Filter { get; set; }
}

public class NotificationFilter
{
    // null: all, true: read, false: unread.
    public bool? IsRead { get; set; }
}

public enum NotificationOrderKey
{
    CreatedAt
}
