using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class ProfileTypeConfiguration
    : IEntityTypeConfiguration<ProfileType>
{
    public void Configure(EntityTypeBuilder<ProfileType> builder)
    {
        builder.ToTable("profiles_types", "public");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Required)
            .HasColumnName("required")
            .IsRequired()
            .HasDefaultValue(false)
            .HasSentinel(false);
    }
}
