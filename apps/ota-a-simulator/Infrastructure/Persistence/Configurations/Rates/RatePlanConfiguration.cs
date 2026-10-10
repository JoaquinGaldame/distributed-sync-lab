using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class RatePlanConfiguration
    : IEntityTypeConfiguration<RatePlan>
{
    public void Configure(EntityTypeBuilder<RatePlan> builder)
    {
        builder.ToTable("rate_plans", "public", table =>
        {
            table.HasCheckConstraint("ck_rate_plans_1",
                "length(trim(name)) > 0");
            table.HasCheckConstraint("ck_rate_plans_2",
                "minimum_nights > 0");
            table.HasCheckConstraint("ck_rate_plans_4",
                "maximum_nights IS NULL OR maximum_nights >= minimum_nights");
        });
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.PropertyId, x.ExternalCode }).IsUnique();

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.PropertyId)
            .HasColumnName("property_id")
            .IsRequired();

        builder.Property(x => x.UnitId)
            .HasColumnName("unit_id")
            .IsRequired();

        builder.Property(x => x.ExternalCode)
            .HasColumnName("external_code")
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.CancellationPolicyId)
            .HasColumnName("cancellation_policy_id")
            .IsRequired();

        builder.Property(x => x.MinimumNights)
            .HasColumnName("minimum_nights")
            .IsRequired()
            .HasDefaultValue(1)
            .HasSentinel(1);

        builder.Property(x => x.MaximumNights)
            .HasColumnName("maximum_nights")
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true)
            .HasSentinel(true);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired()
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();
        builder.Property(x => x.CreatedAt).Metadata
            .SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        builder.Property(x => x.CreatedAt).Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired()
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAddOrUpdate();
        builder.Property(x => x.UpdatedAt).Metadata
            .SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        builder.Property(x => x.UpdatedAt).Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        builder.HasOne(x => x.Property)
            .WithMany()
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => new { x.PropertyId, x.UnitId })
            .HasPrincipalKey(x => new { x.PropertyId, x.Id })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CancellationPolicy)
            .WithMany()
            .HasForeignKey(x => x.CancellationPolicyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.UnitId).HasDatabaseName("ix_rate_plans_unit");
        builder.HasIndex(x => new { x.PropertyId, x.CancellationPolicyId })
            .HasDatabaseName("ix_rate_plans_cancellation_policy");
    }
}
