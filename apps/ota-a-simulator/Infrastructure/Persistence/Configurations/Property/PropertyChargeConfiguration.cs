using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class PropertyChargeConfiguration
    : IEntityTypeConfiguration<PropertyCharge>
{
    public void Configure(EntityTypeBuilder<PropertyCharge> builder)
    {
        builder.ToTable("property_charges", "public", table =>
        {
            table.HasCheckConstraint("ck_property_charges_1",
                "calculation IN ( 'per_stay', 'per_night', 'per_person_per_night', 'percentage' )");
            table.HasCheckConstraint("ck_property_charges_2",
                "amount >= 0");
            table.HasCheckConstraint("ck_property_charges_3",
                "calculation <> 'percentage' OR amount <= 100");
        });
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.PropertyId, x.Code }).IsUnique();

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.PropertyId)
            .HasColumnName("property_id")
            .IsRequired();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Calculation)
            .HasColumnName("calculation")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(x => x.IncludedInPrice)
            .HasColumnName("included_in_price")
            .IsRequired()
            .HasDefaultValue(false)
            .HasSentinel(false);

        builder.HasOne(x => x.Property)
            .WithMany()
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
