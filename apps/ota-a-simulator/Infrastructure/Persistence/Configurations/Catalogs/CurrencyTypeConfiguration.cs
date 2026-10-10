using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class CurrencyTypeConfiguration
    : IEntityTypeConfiguration<CurrencyType>
{
    public void Configure(EntityTypeBuilder<CurrencyType> builder)
    {
        builder.ToTable("currency_types", "public", table =>
        {
            table.HasCheckConstraint("ck_currency_types_1",
                "code ~ '^[A-Z]{3}$'");
        });
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseSerialColumn();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(3)
            .IsRequired();
    }
}
