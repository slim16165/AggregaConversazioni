using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AggregaConversazioni.Domain.Entities;

namespace AggregaConversazioni.Infrastructure.Data.Configurations;

public class TransformationConfiguration : IEntityTypeConfiguration<Transformation>
{
    public void Configure(EntityTypeBuilder<Transformation> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Input)
            .IsRequired()
            .HasMaxLength(100000); // Supporta testi lunghi

        builder.Property(t => t.Output)
            .HasMaxLength(100000);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.SourceType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new());

        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.TenantId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.CreatedAt);

        builder.HasOne(t => t.User)
            .WithMany(u => u.Transformations)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Tenant)
            .WithMany(t => t.Transformations)
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
