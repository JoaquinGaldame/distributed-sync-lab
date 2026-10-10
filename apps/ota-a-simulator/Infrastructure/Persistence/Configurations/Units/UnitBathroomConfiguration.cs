using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class UnitBathroomConfiguration
    : IEntityTypeConfiguration<UnitBathroom>
{
    public void Configure(EntityTypeBuilder<UnitBathroom> builder)
    {
        builder.ToTable("unit_bathrooms", "public", table =>
        {
            table.HasCheckConstraint("ck_unit_bathrooms_1",
                "scope IN ('PRIVATE', 'SHARED')");
            table.HasCheckConstraint("ck_unit_bathrooms_2",
                "quantity > 0");
        });
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UnitId, x.BathroomConfigurationId, x.Scope }).IsUnique();

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.UnitId)
            .HasColumnName("unit_id")
            .IsRequired();

        builder.Property(x => x.BathroomConfigurationId)
            .HasColumnName("bathroom_configuration_id")
            .IsRequired();

        builder.Property(x => x.Scope)
            .HasColumnName("scope")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired()
            .HasDefaultValue(1)
            .HasSentinel(1);

        builder.HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.BathroomConfiguration)
            .WithMany()
            .HasForeignKey(x => x.BathroomConfigurationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
