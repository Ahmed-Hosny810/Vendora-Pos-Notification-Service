using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Infrastructure.Persistence.Contexts.DbConfigurations;

public class NotificationDeliveryAttemptConfiguration : IEntityTypeConfiguration<NotificationDeliveryAttempt>
{
    public void Configure(EntityTypeBuilder<NotificationDeliveryAttempt> builder)
    {
        builder.ToTable("NotificationDeliveryAttempts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Provider).HasMaxLength(80).IsRequired();
        builder.Property(x => x.ProviderMessageId).HasMaxLength(200);
        builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
        builder.Property(x => x.FailureReason).HasMaxLength(2000);

        builder.HasIndex(x => new { x.TenantId, x.NotificationId, x.AttemptNumber }).IsUnique();
        builder.HasOne(x => x.Notification).WithMany(x => x.DeliveryAttempts)
            .HasForeignKey(x => new { x.TenantId, x.NotificationId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
