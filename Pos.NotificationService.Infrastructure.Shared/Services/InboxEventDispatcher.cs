using System.Text.Json;
using Microsoft.Extensions.Logging;
using Pos.NotificationService.Application.Interfaces;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Interfaces.Services;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Infrastructure.Shared.Services;

public class InboxEventDispatcher : IInboxEventDispatcher
{
    // LowStockDetected uses DetectedAt, not OccurredAt. Each handler validates its timestamp.
    private sealed record EventMetadata(Guid EventId, Guid TenantId);
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly Dictionary<string, IIntegrationEventHandler> _handlers;
    private readonly IInboxMessageRepositoryAsync _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InboxEventDispatcher> _logger;

    public InboxEventDispatcher(IEnumerable<IIntegrationEventHandler> handlers,
        IInboxMessageRepositoryAsync repository, IUnitOfWork unitOfWork,
        ILogger<InboxEventDispatcher> logger)
    {
        _handlers = handlers.ToDictionary(x => x.EventType, StringComparer.Ordinal);
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> DispatchAsync(Guid eventId, string eventType, string payload, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(eventType) || !_handlers.TryGetValue(eventType, out var handler))
            return Result.Failure($"No handler registered for '{eventType}'.");

        EventMetadata? metadata;
        try
        {
            metadata = JsonSerializer.Deserialize<EventMetadata>(payload, JsonOptions);
        }
        catch (JsonException)
        {
            return Result.Failure("The event payload is not valid JSON.");
        }

        if (metadata == null || eventId == Guid.Empty || metadata.EventId != eventId ||
            metadata.TenantId == Guid.Empty)
            return Result.Failure("Invalid event metadata, or payload EventId does not match MessageId.");

        if (await _repository.ExistsAsync(eventId, handler.ConsumerName, ct))
            return Result.Success();

        var result = await handler.HandleAsync(payload, ct);
        if (result.IsFailure)
            return result;

        await _repository.AddAsync(new InboxMessage
        {
            MessageId = eventId,
            ConsumerName = handler.ConsumerName,
            TenantId = metadata.TenantId,
            EventType = eventType,
            ProcessedAt = DateTime.UtcNow
        }, ct);

        // One SaveChanges commits notifications and the inbox atomically.
        var saved = await _unitOfWork.TrySaveChangesAsync(ct);
        if (saved.IsSuccess)
            return Result.Success();

        // Only acknowledge a unique-key collision if another transaction committed this inbox entry.
        if (await _repository.ExistsAsync(eventId, handler.ConsumerName, ct))
        {
            _logger.LogInformation("Event {EventId} was processed concurrently; skipping.", eventId);
            return Result.Success();
        }

        throw new InvalidOperationException(string.Join("; ", saved.Errors));
    }
}
