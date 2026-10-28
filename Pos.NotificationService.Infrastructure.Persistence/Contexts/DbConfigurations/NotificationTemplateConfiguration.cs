using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Infrastructure.Persistence.Contexts.DbConfigurations;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Channel).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Language).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Subject).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Body).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();

        // Separate indexes cover global defaults and tenant overrides.
        builder.HasIndex(x => new { x.TenantId, x.Code, x.Channel, x.Language })
            .IsUnique().HasFilter("[TenantId] IS NOT NULL");
        builder.HasIndex(x => new { x.Code, x.Channel, x.Language })
            .IsUnique().HasFilter("[TenantId] IS NULL");

    }
}
