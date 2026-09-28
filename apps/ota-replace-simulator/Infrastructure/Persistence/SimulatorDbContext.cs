using Microsoft.EntityFrameworkCore;
using OtaReplaceSimulator.Domain;

namespace OtaReplaceSimulator.Infrastructure.Persistence;

public sealed class SimulatorDbContext(DbContextOptions<SimulatorDbContext> options)
    : DbContext(options)
{
    public DbSet<ExternalProperty> ExternalProperties => Set<ExternalProperty>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExternalProperty>(entity =>
        {
            entity.ToTable("external_properties");
            entity.HasKey(x => x.ExternalId);

            entity.Property(x => x.ExternalId)
                .HasColumnName("external_id")
                .HasMaxLength(100);

            entity.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.Price)
                .HasColumnName("price")
                .HasPrecision(12, 2);

            entity.Property(x => x.RoomType)
                .HasColumnName("room_type")
                .HasMaxLength(30);

            entity.Property(x => x.Neighbourhood)
                .HasColumnName("neighbourhood")
                .HasMaxLength(100);

            entity.Property(x => x.Latitude)
                .HasColumnName("latitude")
                .HasPrecision(18, 15);

            entity.Property(x => x.Longitude)
                .HasColumnName("longitude")
                .HasPrecision(18, 15);

            entity.Property(x => x.MinimumNights)
                .HasColumnName("minimum_nights");

            entity.Property(x => x.SourceVersion)
                .HasColumnName("source_version");
        });
    }
}