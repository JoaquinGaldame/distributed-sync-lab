using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class UnitFacilityTypeConfiguration
    : IEntityTypeConfiguration<UnitFacilityType>
{
    public void Configure(EntityTypeBuilder<UnitFacilityType> builder)
    {
        builder.ToTable("unit_facility_types", "public");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseSerialColumn();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();
    }
}
