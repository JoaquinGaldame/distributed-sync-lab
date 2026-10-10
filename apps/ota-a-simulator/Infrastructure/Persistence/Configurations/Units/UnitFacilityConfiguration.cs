using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class UnitFacilityConfiguration
    : IEntityTypeConfiguration<UnitFacility>
{
    public void Configure(EntityTypeBuilder<UnitFacility> builder)
    {
        builder.ToTable("unit_facilities", "public");
        builder.HasKey(x => new { x.UnitId, x.UnitFacilityTypeId });

        builder.Property(x => x.UnitId)
            .HasColumnName("unit_id")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.UnitFacilityTypeId)
            .HasColumnName("unit_facility_type_id")
            .IsRequired()
            .ValueGeneratedNever();

        builder.HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.UnitFacilityType)
            .WithMany()
            .HasForeignKey(x => x.UnitFacilityTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
