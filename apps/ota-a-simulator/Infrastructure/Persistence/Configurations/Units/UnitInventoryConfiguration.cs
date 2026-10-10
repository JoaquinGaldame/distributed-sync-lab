using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class UnitInventoryConfiguration
    : IEntityTypeConfiguration<UnitInventory>
{
    public void Configure(EntityTypeBuilder<UnitInventory> builder)
    {
        builder.ToTable("unit_inventory", "public", table =>
        {
            table.HasCheckConstraint("ck_unit_inventory_1",
                "available_units >= 0");
        });
        builder.HasKey(x => new { x.UnitId, x.StayDate });

        builder.Property(x => x.UnitId)
            .HasColumnName("unit_id")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.StayDate)
            .HasColumnName("stay_date")
            .HasColumnType("date")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.AvailableUnits)
            .HasColumnName("available_units")
            .IsRequired();

        builder.Property(x => x.Closed)
            .HasColumnName("closed")
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

        builder.HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.StayDate).HasDatabaseName("ix_inventory_date");
    }
}
