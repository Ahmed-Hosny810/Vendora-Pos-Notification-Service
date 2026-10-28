using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.NotificationService.Domain.Models;

namespace Pos.NotificationService.Infrastructure.Persistence.Contexts.DbConfigurations;

public class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("InboxMessages");
        builder.HasKey(x => new { x.MessageId, x.ConsumerName });
        builder.Property(x => x.MessageId).ValueGeneratedNever();
        builder.Property(x => x.ConsumerName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.EventType).HasMaxLength(150).IsRequired();
        builder.HasIndex(x => x.ProcessedAt);
    }
}
