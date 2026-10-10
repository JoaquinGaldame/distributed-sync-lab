using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class CancellationPolicyConfiguration
    : IEntityTypeConfiguration<CancellationPolicy>
{
    public void Configure(EntityTypeBuilder<CancellationPolicy> builder)
    {
        builder.ToTable("cancellation_policies", "public", table =>
        {
            table.HasCheckConstraint("ck_cancellation_policies_1",
                "length(trim(name)) > 0");
            table.HasCheckConstraint("ck_cancellation_policies_2",
                "free_until_days_before >= 0");
            table.HasCheckConstraint("ck_cancellation_policies_3",
                "penalty_percent BETWEEN 0 AND 100");
        });
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.FreeUntilDaysBefore)
            .HasColumnName("free_until_days_before")
            .IsRequired(false);

        builder.Property(x => x.PenaltyPercent)
            .HasColumnName("penalty_percent")
            .HasPrecision(5, 2)
            .IsRequired();
    }
}
