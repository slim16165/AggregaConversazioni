using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AggregaConversazioni.Domain.Entities;

namespace AggregaConversazioni.Infrastructure.Data.Configurations;

public class TransformationRuleConfiguration : IEntityTypeConfiguration<TransformationRule>
{
    public void Configure(EntityTypeBuilder<TransformationRule> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Pattern)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(r => r.SourceType)
            .IsRequired()
            .HasConversion<string>();

        builder.HasIndex(r => r.SourceType);
        builder.HasIndex(r => r.TenantId);
        builder.HasIndex(r => new { r.SourceType, r.IsActive, r.Order });

        builder.HasOne(r => r.Tenant)
            .WithMany()
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
