namespace Pos.NotificationService.Infrastructure.Shared.Options;

public class IdentityRecipientClientOptions
{
    public const string SectionName = "IdentityRecipientClient";
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 15;
}
