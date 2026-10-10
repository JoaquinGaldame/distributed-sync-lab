using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class PropertyDescriptionConfiguration
    : IEntityTypeConfiguration<PropertyDescription>
{
    public void Configure(EntityTypeBuilder<PropertyDescription> builder)
    {
        builder.ToTable("property_descriptions", "public", table =>
        {
            table.HasCheckConstraint("ck_property_descriptions_1",
                "length(trim(description)) > 0");
        });
        builder.HasKey(x => new { x.PropertyId, x.LanguageId });

        builder.Property(x => x.PropertyId)
            .HasColumnName("property_id")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.LanguageId)
            .HasColumnName("language_id")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("text")
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

        builder.HasOne(x => x.Language)
            .WithMany()
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
