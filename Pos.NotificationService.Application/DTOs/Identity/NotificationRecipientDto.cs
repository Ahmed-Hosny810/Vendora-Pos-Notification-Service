namespace Pos.NotificationService.Application.DTOs.Identity;

public class NotificationRecipientDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
