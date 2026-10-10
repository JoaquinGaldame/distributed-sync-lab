using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class PropertyLocationConfiguration
    : IEntityTypeConfiguration<PropertyLocation>
{
    public void Configure(EntityTypeBuilder<PropertyLocation> builder)
    {
        builder.ToTable("property_locations", "public", table =>
        {
            table.HasCheckConstraint("ck_property_locations_1",
                "country_code ~ '^[A-Z]{2}$'");
            table.HasCheckConstraint("ck_property_locations_2",
                "latitude BETWEEN -90 AND 90");
            table.HasCheckConstraint("ck_property_locations_3",
                "longitude BETWEEN -180 AND 180");
            table.HasCheckConstraint("ck_property_locations_4",
                "(latitude IS NULL AND longitude IS NULL) OR (latitude IS NOT NULL AND longitude IS NOT NULL)");
        });
        builder.HasKey(x => x.PropertyId);

        builder.Property(x => x.PropertyId)
            .HasColumnName("property_id")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.CountryCode)
            .HasColumnName("country_code")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(x => x.Region)
            .HasColumnName("region")
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(x => x.City)
            .HasColumnName("city")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Neighbourhood)
            .HasColumnName("neighbourhood")
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(x => x.AddressLine)
            .HasColumnName("address_line")
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.PostalCode)
            .HasColumnName("postal_code")
            .HasMaxLength(30)
            .IsRequired(false);

        builder.Property(x => x.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(9, 6)
            .IsRequired(false);

        builder.Property(x => x.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(9, 6)
            .IsRequired(false);

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

        builder.HasOne(x => x.Property)
            .WithOne()
            .HasForeignKey<PropertyLocation>(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
