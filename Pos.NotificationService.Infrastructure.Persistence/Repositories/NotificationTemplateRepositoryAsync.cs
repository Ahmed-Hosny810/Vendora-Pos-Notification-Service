using Microsoft.EntityFrameworkCore;
using Pos.NotificationService.Application.Features.NotificationTemplates.DTOs;
using Pos.NotificationService.Application.Features.NotificationTemplates.Queries.GetNotificationTemplatesQuery;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.Domain.Models;
using Pos.NotificationService.Infrastructure.Persistence.Contexts;
using Pos.NotificationService.Infrastructure.Persistence.QueryExtensions;
using System.Linq.Expressions;

namespace Pos.NotificationService.Infrastructure.Persistence.Repositories;

public class NotificationTemplateRepositoryAsync
    : GenericRepositoryAsync<NotificationTemplate, Guid>, INotificationTemplateRepositoryAsync
{
    private readonly ApplicationDbContext _context;

    public NotificationTemplateRepositoryAsync(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<NotificationTemplate?> GetActiveTemplateAsync(string code,
        string channel, string language, CancellationToken cancellationToken)
    {
        return _context.NotificationTemplates
            .AsNoTracking()
            .Where(template => template.IsActive &&
                template.Code == code && template.Channel == channel &&
                template.Language == language)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(string code, string channel, string language,
        CancellationToken cancellationToken)
    {
        return _context.NotificationTemplates.AnyAsync(template =>
            template.Code == code &&
            template.Channel == channel && template.Language == language, cancellationToken);
    }

    private static readonly Expression<Func<NotificationTemplate, NotificationTemplateDto>> ToDto = _x =>
        new NotificationTemplateDto
        {
            Id = _x.Id, Code = _x.Code, Channel = _x.Channel, Language = _x.Language,
            Subject = _x.Subject, Body = _x.Body, IsActive = _x.IsActive,
            CreatedAt = _x.CreatedAt, UpdatedAt = _x.UpdatedAt, RowVersion = _x.RowVersion
        };

    public Task<NotificationTemplate?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken)
        => _context.NotificationTemplates.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public void PrepareUpdate(NotificationTemplate template, byte[] rowVersion)
    {
        _context.Entry(template).Property(x => x.RowVersion).OriginalValue = rowVersion;
        _context.Entry(template).Property(x => x.UpdatedAt).IsModified = true;
    }

    public Task<bool> ExistsExceptAsync(string code, string channel, string language,
        Guid excludedId, CancellationToken cancellationToken)
        => _context.NotificationTemplates.AnyAsync(x => x.Id != excludedId &&
            x.Code == code && x.Channel == channel && x.Language == language, cancellationToken);

    public Task<NotificationTemplateDto?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken)
        => _context.NotificationTemplates.AsNoTracking().Where(x => x.Id == id)
            .Select(ToDto).SingleOrDefaultAsync(cancellationToken);

    public async Task<PagedResponse<IEnumerable<NotificationTemplateDto>>> GetTemplatesAsync(
        GetNotificationTemplatesQueryParameter parameter, CancellationToken cancellationToken)
    {
        var query = _context.NotificationTemplates.AsNoTracking().ApplyFilter(parameter.Filter);
        var offset = checked((parameter.PageNumber - 1) * parameter.PageSize);
        var rows = await query.ApplyOrder(parameter).Skip(offset).Take(parameter.PageSize)
            .Select(ToDto).ToListAsync(cancellationToken);
        var totalKnown = (rows.Count > 0 && rows.Count < parameter.PageSize) ||
            (rows.Count == 0 && offset == 0);
        var total = totalKnown ? offset + rows.Count : await query.CountAsync(cancellationToken);
        return new PagedResponse<IEnumerable<NotificationTemplateDto>>(
            rows, parameter.PageNumber, parameter.PageSize, total);
    }
}

