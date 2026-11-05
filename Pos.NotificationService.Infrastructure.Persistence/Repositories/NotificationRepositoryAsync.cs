using Pos.NotificationService.Application.Features.Notifications.DTOs;
using Microsoft.EntityFrameworkCore;
using Pos.NotificationService.Application.Features.Notifications.Queries.GetMyNotificationsQuery;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Domain.Constants;
using Pos.NotificationService.Domain.Models;
using Pos.NotificationService.Infrastructure.Persistence.Contexts;

namespace Pos.NotificationService.Infrastructure.Persistence.Repositories;

public class NotificationRepositoryAsync : GenericRepositoryAsync<Notification, Guid>, INotificationRepositoryAsync
{
    private readonly ApplicationDbContext _context;

    public NotificationRepositoryAsync(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PagedResponse<IEnumerable<NotificationDetailsDto>>> GetMyNotificationsAsync(
    Guid tenantId,
    Guid recipientUserId,
    GetMyNotificationsQueryParameter parameter,
    CancellationToken cancellationToken)
    {
        var filteredResult = _context.Notifications
            .AsNoTracking()
            .Where(n =>
                n.TenantId == tenantId &&
                n.RecipientUserId == recipientUserId &&
                n.Channel == NotificationChannels.InApp);

        var isRead = parameter.Filter?.IsRead;

        if (isRead.HasValue)
        {
            filteredResult = filteredResult.Where(n =>
                n.ReadAt.HasValue == isRead.Value);
        }

        var orderedResult = parameter.OrderDescending
            ? filteredResult.OrderByDescending(n => n.CreatedAt)
                .ThenByDescending(n => n.Id)
            : filteredResult.OrderBy(n => n.CreatedAt)
                .ThenBy(n => n.Id);

        var offset = checked((parameter.PageNumber - 1) * parameter.PageSize);

        var rows = await orderedResult
            .Skip(offset)
            .Take(parameter.PageSize)
            .Select(n => new NotificationDetailsDto
            {
                Id = n.Id,
                BranchId = n.BranchId,
                Type = n.Type,
                Title = n.Title,
                Body = n.Body,
                ReferenceType = n.ReferenceType,
                ReferenceId = n.ReferenceId,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            })
            .ToListAsync(cancellationToken);

        var totalCountIsKnown =
            (rows.Count > 0 && rows.Count < parameter.PageSize) ||
            (rows.Count == 0 && offset == 0);

        var totalCount = totalCountIsKnown
            ? offset + rows.Count
            : await filteredResult.CountAsync(cancellationToken);

        return new PagedResponse<IEnumerable<NotificationDetailsDto>>(
            rows,
            parameter.PageNumber,
            parameter.PageSize,
            totalCount);
    }

    public Task<NotificationDetailsDto?> GetMyNotificationByIdAsync(
        Guid tenantId, Guid recipientUserId, Guid notificationId, CancellationToken cancellationToken)
    {
        return _context.Notifications.AsNoTracking()
            .Where(n => n.TenantId == tenantId && n.RecipientUserId == recipientUserId &&
                n.Id == notificationId && n.Channel == NotificationChannels.InApp)
            .Select(n => new NotificationDetailsDto
            {
                Id = n.Id,
                BranchId = n.BranchId,
                Type = n.Type,
                Title = n.Title,
                Body = n.Body,
                ReferenceType = n.ReferenceType,
                ReferenceId = n.ReferenceId,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<int> GetUnreadNotificationCountAsync(
        Guid tenantId, Guid recipientUserId, CancellationToken cancellationToken)
    {
        return _context.Notifications.CountAsync(n =>
            n.TenantId == tenantId && n.RecipientUserId == recipientUserId &&
            n.Channel == NotificationChannels.InApp && n.ReadAt == null, cancellationToken);
    }

    public async Task<bool> MarkNotificationAsReadAsync(
        Guid tenantId, Guid recipientUserId, Guid notificationId,
        DateTime readAt, CancellationToken cancellationToken)
    {
        var query = _context.Notifications.Where(n =>
            n.TenantId == tenantId && n.RecipientUserId == recipientUserId &&
            n.Id == notificationId && n.Channel == NotificationChannels.InApp);

        var affected = await query.Where(n => n.ReadAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.ReadAt, (DateTime?)readAt),
                cancellationToken);

        if (affected > 0)
            return true;

        return await query.AnyAsync(cancellationToken);
    }

    public Task<int> MarkAllNotificationsAsReadAsync(
        Guid tenantId, Guid recipientUserId, DateTime readAt, CancellationToken cancellationToken)
    {
        return _context.Notifications
            .Where(n => n.TenantId == tenantId && n.RecipientUserId == recipientUserId &&
                n.Channel == NotificationChannels.InApp && n.ReadAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.ReadAt, (DateTime?)readAt),
                cancellationToken);
    }
}
