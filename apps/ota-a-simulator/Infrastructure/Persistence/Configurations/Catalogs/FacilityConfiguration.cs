using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class FacilityConfiguration
    : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("facilities", "public");
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => x.Code);

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