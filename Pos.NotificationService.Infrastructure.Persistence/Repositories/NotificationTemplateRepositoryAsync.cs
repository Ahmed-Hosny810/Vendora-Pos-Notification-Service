using Microsoft.EntityFrameworkCore;
using Pos.NotificationService.Application.Interfaces.Repositories;
using Pos.NotificationService.Domain.Models;
using Pos.NotificationService.Infrastructure.Persistence.Contexts;

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
}

