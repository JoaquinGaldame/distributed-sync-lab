using AcademicPms.Domain;
using Microsoft.EntityFrameworkCore;


namespace AcademicPms.Infrastructure.Persistence;

public sealed class PmsDbContext(DbContextOptions<PmsDbContext> options)
    : DbContext(options)
{
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyChange> PropertyChanges => Set<PropertyChange>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Property>(entity =>
        {
            entity.ToTable("properties");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasMaxLength(50);

            entity.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.Price)
                .HasColumnName("price")
                .HasPrecision(12, 2);

            entity.Property(x => x.Version)
                .HasColumnName("version")
                .IsConcurrencyToken();

            entity.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<PropertyChange>(entity =>
        {
            entity.ToTable("property_changes");
            entity.HasKey(x => x.Sequence);

            entity.Property(x => x.Sequence).HasColumnName("sequence");
            entity.Property(x => x.EventId).HasColumnName("event_id");
            entity.HasIndex(x => x.EventId).IsUnique();

            entity.Property(x => x.PropertyId)
                .HasColumnName("property_id")
                .HasMaxLength(50);

            entity.Property(x => x.Version).HasColumnName("version");

            entity.HasIndex(x => new { x.PropertyId, x.Version })
                .IsUnique();

            entity.Property(x => x.EventType)
                .HasColumnName("event_type")
                .HasMaxLength(100);

            entity.Property(x => x.OccurredAt)
                .HasColumnName("occurred_at");

            entity.Property(x => x.StateName)
                .HasColumnName("state_name")
                .HasMaxLength(255);

            entity.Property(x => x.StatePrice)
                .HasColumnName("state_price")
                .HasPrecision(12, 2);
        });
    }
}