using Microsoft.EntityFrameworkCore;
using OtaBSimulator.Domain;
using OtaBSimulator.Infrastructure.Persistence.Configurations;

namespace OtaBSimulator.Infrastructure.Persistence;

public sealed class OtaBDbContext(DbContextOptions<OtaBDbContext> options) : DbContext(options)
{
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<ListingCategory> ListingCategories => Set<ListingCategory>();
    public DbSet<RentalMode> RentalModes => Set<RentalMode>();
    public DbSet<AmenityGroup> AmenityGroups => Set<AmenityGroup>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<ContactRole> ContactRoles => Set<ContactRole>();
    public DbSet<ListingStatus> ListingStatuses => Set<ListingStatus>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<BedType> BedTypes => Set<BedType>();
    public DbSet<AvailabilityState> AvailabilityStates => Set<AvailabilityState>();
    public DbSet<CancellationTerm> CancellationTerms => Set<CancellationTerm>();
    public DbSet<FeeBasis> FeeBases => Set<FeeBasis>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomBed> RoomBeds => Set<RoomBed>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<ListingText> ListingTexts => Set<ListingText>();
    public DbSet<ListingAmenity> ListingAmenities => Set<ListingAmenity>();
    public DbSet<HouseRules> HouseRules => Set<HouseRules>();
    public DbSet<BookingRules> BookingRules => Set<BookingRules>();
    public DbSet<ListingPricing> ListingPricing => Set<ListingPricing>();
    public DbSet<ListingFee> ListingFees => Set<ListingFee>();
    public DbSet<DailyPrice> DailyPrices => Set<DailyPrice>();
    public DbSet<DailyAvailability> DailyAvailability => Set<DailyAvailability>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureOtaBModel();
        base.OnModelCreating(modelBuilder);
    }
}
