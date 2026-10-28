using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Infrastructure.Persistence.Contexts.DbConfigurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.RecipientEmail).HasMaxLength(254);
        builder.Property(x => x.RecipientPhone).HasMaxLength(30);
        builder.Property(x => x.Channel).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Type).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Body).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
        builder.Property(x => x.ReferenceType).HasMaxLength(80);
        builder.Property(x => x.LastError).HasMaxLength(2000);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasAlternateKey(x => new { x.TenantId, x.Id });
        builder.HasIndex(x => new { x.TenantId, x.RecipientUserId, x.CreatedAt, x.Id });
        builder.HasIndex(x => new { x.TenantId, x.RecipientUserId, x.ReadAt });
        builder.HasIndex(x => new { x.Status, x.NextAttemptAt, x.CreatedAt }).HasFilter("[Status] = 'Pending'");
        builder.HasIndex(x => new { x.TenantId, x.SourceEventId, x.RecipientUserId, x.Channel, x.Type })
            .IsUnique().HasFilter("[SourceEventId] IS NOT NULL AND [RecipientUserId] IS NOT NULL");
        builder.HasIndex(x => new { x.TenantId, x.SourceEventId, x.RecipientEmail, x.Channel, x.Type })
            .IsUnique().HasFilter("[SourceEventId] IS NOT NULL AND [RecipientUserId] IS NULL AND [RecipientEmail] IS NOT NULL");
        builder.HasIndex(x => new { x.TenantId, x.SourceEventId, x.RecipientPhone, x.Channel, x.Type })
            .IsUnique().HasFilter("[SourceEventId] IS NOT NULL AND [RecipientUserId] IS NULL AND [RecipientPhone] IS NOT NULL");
        // Handlers must verify that a template is global or belongs to this tenant.
        builder.HasOne(x => x.Template).WithMany(x => x.Notifications)
            .HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Restrict);

    }
}
