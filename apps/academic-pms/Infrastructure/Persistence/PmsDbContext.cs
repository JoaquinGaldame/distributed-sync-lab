using AcademicPms.Domain;
using Microsoft.EntityFrameworkCore;


namespace AcademicPms.Infrastructure.Persistence;

public sealed class PmsDbContext(DbContextOptions<PmsDbContext> options)
    : DbContext(options)
{
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyChange> PropertyChanges => Set<PropertyChange>();
    public DbSet<CalendarDays> CalendarDays => Set<CalendarDays>();
    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<Publication> Publications => Set<Publication>();

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
            
            entity.Property(x => x.SourceListingId)
                .HasColumnName("source_listing_id")
                .HasMaxLength(50);

            entity.HasIndex(x => x.SourceListingId)
                .IsUnique();

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
                
            entity.Property(x => x.SourceListingId)
                .HasColumnName("source_listing_id")
                .HasMaxLength(50);

            entity.Property(x => x.StateRoomType)
                .HasColumnName("state_room_type")
                .HasMaxLength(30);

            entity.Property(x => x.StateNeighbourhood)
                .HasColumnName("state_neighbourhood")
                .HasMaxLength(100);

            entity.Property(x => x.StateLatitude)
                .HasColumnName("state_latitude")
                .HasPrecision(18, 15);

            entity.Property(x => x.StateLongitude)
                .HasColumnName("state_longitude")
                .HasPrecision(18, 15);

            entity.Property(x => x.StateMinimumNights)
                .HasColumnName("state_minimum_nights");
        });

        modelBuilder.Entity<CalendarDays>(entity =>
        {
            entity.ToTable("calendar_days");
            entity.HasKey(x => new { x.PropertyId, x.Date });

            entity.Property(x => x.PropertyId)
                .HasColumnName("property_id")
                .HasMaxLength(50);

            entity.Property(x => x.Date)
                .HasColumnName("date");

            entity.Property(x => x.IsAvailable)
                .HasColumnName("is_available");

            entity.Property(x => x.MinimumNights)
                .HasColumnName("minimum_nights");

            entity.Property(x => x.MaximumNights)
                .HasColumnName("maximum_nights");

            entity.HasOne<Property>()
                .WithMany()
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.Date, x.IsAvailable });
        });

        modelBuilder.Entity<Channel>(entity =>
        {
            entity.ToTable("channels");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .UseIdentityByDefaultColumn()
                .HasIdentityOptions(startValue: 1000);

            entity.Property(x => x.Code)
                .HasColumnName("code")
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(x => x.Code)
                .IsUnique();

            entity.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.IsEnabled)
                .HasColumnName("is_enabled");

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at");

            var seededAt = new DateTime(
                2026, 9, 29, 0, 0, 0, DateTimeKind.Utc);

            entity.HasData(
                new Channel
                {
                    Id = 1,
                    Code = "ota-a",
                    Name = "OTA A",
                    IsEnabled = true,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
                },
                new Channel
                {
                    Id = 2,
                    Code = "ota-b",
                    Name = "OTA B",
                    IsEnabled = true,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
                },
                new Channel
                {
                    Id = 3,
                    Code = "ota-replace-simulator",
                    Name = "OTA Replace Simulator",
                    IsEnabled = true,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
                });
        });

        modelBuilder.Entity<Publication>(entity =>
        {
            entity.ToTable("publications");
            entity.HasKey(x => new { x.PropertyId, x.ChannelId });

            entity.Property(x => x.PropertyId)
                .HasColumnName("property_id")
                .HasMaxLength(50);

            entity.Property(x => x.ChannelId)
                .HasColumnName("channel_id");

            entity.Property(x => x.ExternalPropertyId)
                .HasColumnName("external_property_id")
                .HasMaxLength(100);

            entity.Property(x => x.Published)
                .HasColumnName("published");

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasOne<Property>()
                .WithMany()
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Channel>()
                .WithMany()
                .HasForeignKey(x => x.ChannelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new
                { x.ChannelId, x.ExternalPropertyId })
                .IsUnique();

            entity.HasIndex(x => new { x.ChannelId, x.Published });
        });
    }
}
