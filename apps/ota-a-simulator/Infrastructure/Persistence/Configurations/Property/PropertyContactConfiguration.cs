using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class PropertyContactConfiguration
    : IEntityTypeConfiguration<PropertyContact>
{
    public void Configure(EntityTypeBuilder<PropertyContact> builder)
    {
        builder.ToTable("property_contacts", "public", table =>
        {
            table.HasCheckConstraint("ck_property_contacts_1",
                "NULLIF(trim(email), '') IS NOT NULL OR NULLIF(trim(phone), '') IS NOT NULL");
        });
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.PropertyId, x.ProfileTypeId }).IsUnique();

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.PropertyId)
            .HasColumnName("property_id")
            .IsRequired();

        builder.Property(x => x.ProfileTypeId)
            .HasColumnName("profile_type_id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(254)
            .IsRequired(false);

        builder.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.LanguageId)
            .HasColumnName("language_id")
            .IsRequired();

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
            .WithMany()
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProfileType)
            .WithMany()
            .HasForeignKey(x => x.ProfileTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Language)
            .WithMany()
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
