namespace Pos.NotificationService.Application.DTOs.Identity;

public class GetNotificationRecipientsRequest
{
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public List<string> Roles { get; set; } = new();
}
