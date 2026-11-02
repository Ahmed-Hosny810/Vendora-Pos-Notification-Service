using System.Text.RegularExpressions;
using Pos.NotificationService.Application.DTOs.NotificationTemplates;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Domain.Constants;

namespace Pos.NotificationService.Application.Services;

public class NotificationTemplateService : INotificationTemplateService
{
    private const int MaxTitleLength = 200;

    // Matches {{Name}} or {{ Name }}; names are case-sensitive.
    private static readonly Regex Placeholder = new(
        @"\{\{\s*([A-Za-z]\w*)\s*\}\}",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    private static readonly HashSet<string> Channels = new()
    {
        NotificationChannels.InApp,
        NotificationChannels.Email,
        NotificationChannels.Sms,
        NotificationChannels.WhatsApp
    };

    private readonly INotificationTemplateRepositoryAsync _repository;

    public NotificationTemplateService(INotificationTemplateRepositoryAsync repository)
        => _repository = repository;

    public async Task<Result<RenderedNotificationTemplateDto>> RenderAsync(
        string code,
        string channel,
        string language,
        IReadOnlyDictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(language))
            return Fail("Template code and language are required.");
        if (!Channels.Contains(channel))
            return Fail("Unsupported notification channel.");
        if (values == null)
            return Fail("Placeholder values are required.");

        var template = await _repository.GetActiveTemplateAsync(
            code.Trim().ToUpperInvariant(), channel,
            language.Trim().ToLowerInvariant(), cancellationToken);

        if (template == null)
            return Fail("No active template was found for this code, channel and language.");

        if (HasMalformedPlaceholders(template.Subject) || HasMalformedPlaceholders(template.Body))
            return Fail("Template placeholders must use {{Name}} with letters, digits or underscores, starting with a letter.");

        // TryGetValue on the caller's dictionary; null values count as missing.
        var missing = new List<string>();

        string Replace(string text) => Placeholder.Replace(text, m =>
        {
            var name = m.Groups[1].Value;
            if (values.TryGetValue(name, out var value) && value != null)
                return value;
            if (!missing.Contains(name))
                missing.Add(name);
            return m.Value;
        });

        var title = Replace(template.Subject);
        var body = Replace(template.Body);

        if (missing.Count > 0)
            return Fail($"Missing template values: {string.Join(", ", missing)}.");

        if (title.Length > MaxTitleLength)
            return Fail($"The rendered notification title exceeds {MaxTitleLength} characters.");

        return Result<RenderedNotificationTemplateDto>.Success(new RenderedNotificationTemplateDto
        {
            TemplateId = template.Id,
            Title = title,
            Body = body
        });
    }

    private static bool HasMalformedPlaceholders(string text)
    {
        var rest = Placeholder.Replace(text, string.Empty);
        return rest.Contains("{{") || rest.Contains("}}");
    }

    private static Result<RenderedNotificationTemplateDto> Fail(string message)
        => Result<RenderedNotificationTemplateDto>.Failure(message);
}