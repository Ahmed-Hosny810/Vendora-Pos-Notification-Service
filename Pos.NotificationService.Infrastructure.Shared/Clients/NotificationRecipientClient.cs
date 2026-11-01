using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pos.NotificationService.Application.Constants;
using Pos.NotificationService.Application.DTOs.Identity;
using Pos.NotificationService.Application.Interfaces.Clients;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Infrastructure.Shared.Options;

namespace Pos.NotificationService.Infrastructure.Shared.Clients;

public class NotificationRecipientClient : INotificationRecipientClient
{
    private readonly HttpClient _httpClient;
    private readonly IdentityRecipientClientOptions _options;
    private readonly ILogger<NotificationRecipientClient> _logger;

    public NotificationRecipientClient(HttpClient httpClient,
        IOptions<IdentityRecipientClientOptions> options,
        ILogger<NotificationRecipientClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<NotificationRecipientDto>>> GetRecipientsAsync(
        GetNotificationRecipientsRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null || request.TenantId == Guid.Empty)
            return Result<IReadOnlyList<NotificationRecipientDto>>.Failure("A valid tenant is required.");

        if (request.BranchId == Guid.Empty)
            return Result<IReadOnlyList<NotificationRecipientDto>>.Failure("A valid branch is required when supplied.");

        if (request.Roles == null || request.Roles.Count == 0 ||
            request.Roles.Any(role => !TenantRoles.IsSupported(role)))
            return Result<IReadOnlyList<NotificationRecipientDto>>.Failure("Select at least one supported tenant role.");

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            return Result<IReadOnlyList<NotificationRecipientDto>>.Failure("Identity recipient API key is not configured.");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/internal/notification-recipients");
        httpRequest.Headers.Add("X-Internal-Api-Key", _options.ApiKey);
        httpRequest.Content = JsonContent.Create(new GetNotificationRecipientsRequest
        {
            TenantId = request.TenantId,
            BranchId = request.BranchId,
            Roles = request.Roles.Distinct(StringComparer.Ordinal).ToList()
        });

        try
        {
            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Identity recipient lookup returned HTTP {StatusCode}.", (int)response.StatusCode);
                return Result<IReadOnlyList<NotificationRecipientDto>>.Failure(
                    $"Identity recipient lookup failed. StatusCode: {(int)response.StatusCode}.");
            }

            var result = await response.Content.ReadFromJsonAsync<Response<List<NotificationRecipientDto>>>(
                cancellationToken);
            if (result?.Succeeded != true || result.Data == null)
                return Result<IReadOnlyList<NotificationRecipientDto>>.Failure("Identity returned an unsuccessful or empty response.");

            if (result.Data.Any(recipient => recipient == null || recipient.UserId == Guid.Empty))
                return Result<IReadOnlyList<NotificationRecipientDto>>.Failure("Identity returned invalid recipient data.");

            return Result<IReadOnlyList<NotificationRecipientDto>>.Success(
                result.Data.DistinctBy(recipient => recipient.UserId).ToList());
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Result<IReadOnlyList<NotificationRecipientDto>>.Failure("Identity recipient lookup timed out.");
        }
        catch (HttpRequestException)
        {
            return Result<IReadOnlyList<NotificationRecipientDto>>.Failure("Identity service is unavailable.");
        }
        catch (JsonException)
        {
            return Result<IReadOnlyList<NotificationRecipientDto>>.Failure("Identity returned an invalid response.");
        }
    }
}
