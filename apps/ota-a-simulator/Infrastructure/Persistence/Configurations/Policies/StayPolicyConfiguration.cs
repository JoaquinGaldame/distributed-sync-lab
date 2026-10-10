using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class StayPolicyConfiguration
    : IEntityTypeConfiguration<StayPolicy>
{
    public void Configure(EntityTypeBuilder<StayPolicy> builder)
    {
        builder.ToTable("stay_policies", "public", table =>
        {
            table.HasCheckConstraint("ck_stay_policies_1",
                "minimum_guest_age >= 0");
        });
        builder.HasKey(x => x.PropertyId);

        builder.Property(x => x.PropertyId)
            .HasColumnName("property_id")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.CheckInFrom)
            .HasColumnName("check_in_from")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(x => x.CheckInUntil)
            .HasColumnName("check_in_until")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(x => x.CheckOutFrom)
            .HasColumnName("check_out_from")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(x => x.CheckOutUntil)
            .HasColumnName("check_out_until")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(x => x.MinimumGuestAge)
            .HasColumnName("minimum_guest_age")
            .IsRequired()
            .HasDefaultValue(18)
            .HasSentinel(18);

        builder.Property(x => x.ChildrenAllowed)
            .HasColumnName("children_allowed")
            .IsRequired()
            .HasDefaultValue(true)
            .HasSentinel(true);

        builder.Property(x => x.PetsAllowed)
            .HasColumnName("pets_allowed")
            .IsRequired()
            .HasDefaultValue(false)
            .HasSentinel(false);

        builder.Property(x => x.SmokingAllowed)
            .HasColumnName("smoking_allowed")
            .IsRequired()
            .HasDefaultValue(false)
            .HasSentinel(false);

        builder.Property(x => x.PartiesAllowed)
            .HasColumnName("parties_allowed")
            .IsRequired()
            .HasDefaultValue(false)
            .HasSentinel(false);

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
            .WithOne()
            .HasForeignKey<StayPolicy>(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
