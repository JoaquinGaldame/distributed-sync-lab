using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class PropertyConfiguration
    : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("properties", "public", table =>
        {
            table.HasCheckConstraint("ck_properties_1",
                "length(trim(name)) > 0");
            table.HasCheckConstraint("ck_properties_2",
                "status IN ('draft', 'active', 'inactive')");
        });
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.ExternalCode).IsUnique();

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.ExternalCode)
            .HasColumnName("external_code")
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PropertyTypeId)
            .HasColumnName("property_type_id")
            .IsRequired();

        builder.Property(x => x.CurrencyId)
            .HasColumnName("currency_id")
            .IsRequired();

        builder.Property(x => x.PrimaryLanguageId)
            .HasColumnName("primary_language")
            .IsRequired();

        builder.Property(x => x.Timezone)
            .HasColumnName("timezone")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("draft")
            .HasSentinel("draft");

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

        builder.HasOne(x => x.PropertyType)
            .WithMany()
            .HasForeignKey(x => x.PropertyTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.CurrencyType)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.PrimaryLanguage)
            .WithMany()
            .HasForeignKey(x => x.PrimaryLanguageId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
