using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class PropertyFacilityConfiguration
    : IEntityTypeConfiguration<PropertyFacility>
{
    public void Configure(EntityTypeBuilder<PropertyFacility> builder)
    {
        builder.ToTable("property_facilities", "public");
        builder.HasKey(x => new { x.PropertyId, x.FacilityCode });

        builder.Property(x => x.PropertyId)
            .HasColumnName("property_id")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.FacilityCode)
            .HasColumnName("facility_code")
            .HasMaxLength(100)
            .IsRequired()
            .ValueGeneratedNever();

        builder.HasOne(x => x.Property)
            .WithMany()
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Facility)
            .WithMany()
            .HasForeignKey(x => x.FacilityCode)
            .HasPrincipalKey(x => x.Code)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
