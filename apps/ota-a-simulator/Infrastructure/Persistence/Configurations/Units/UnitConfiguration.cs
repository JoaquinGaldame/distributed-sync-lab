using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class UnitConfiguration
    : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("units", "public", table =>
        {
            table.HasCheckConstraint("ck_units_1",
                "length(trim(name)) > 0");
            table.HasCheckConstraint("ck_units_2",
                "total_units > 0");
            table.HasCheckConstraint("ck_units_3",
                "maximum_occupancy > 0");
            table.HasCheckConstraint("ck_units_4",
                "maximum_adults > 0");
            table.HasCheckConstraint("ck_units_5",
                "maximum_children >= 0");
            table.HasCheckConstraint("ck_units_7",
                "maximum_adults <= maximum_occupancy");
            table.HasCheckConstraint("ck_units_8",
                "maximum_children <= maximum_occupancy");
        });
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.PropertyId, x.Id });
        builder.HasIndex(x => new { x.PropertyId, x.ExternalCode }).IsUnique();

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.PropertyId)
            .HasColumnName("property_id")
            .IsRequired();

        builder.Property(x => x.ExternalCode)
            .HasColumnName("external_code")
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.UnitTypeId)
            .HasColumnName("unit_type_id")
            .IsRequired();

        builder.Property(x => x.TotalUnits)
            .HasColumnName("total_units")
            .IsRequired();

        builder.Property(x => x.MaximumOccupancy)
            .HasColumnName("maximum_occupancy")
            .IsRequired();

        builder.Property(x => x.MaximumAdults)
            .HasColumnName("maximum_adults")
            .IsRequired();

        builder.Property(x => x.MaximumChildren)
            .HasColumnName("maximum_children")
            .IsRequired()
            .HasDefaultValue(0)
            .HasSentinel(0);

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

        builder.HasOne(x => x.UnitType)
            .WithMany()
            .HasForeignKey(x => x.UnitTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.PropertyId).HasDatabaseName("ix_units_property");
    }
}
