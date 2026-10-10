using Microsoft.EntityFrameworkCore;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence;

public sealed class OtaADbContext(DbContextOptions<OtaADbContext> options)
    : DbContext(options)
{
    public DbSet<PropertyType> PropertyTypes => Set<PropertyType>();
    public DbSet<UnitType> UnitTypes => Set<UnitType>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<UnitFacilityType> UnitFacilityTypes => Set<UnitFacilityType>();
    public DbSet<BathroomConfiguration> BathroomConfigurations => Set<BathroomConfiguration>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<CurrencyType> CurrencyTypes => Set<CurrencyType>();
    public DbSet<ProfileType> ProfileTypes => Set<ProfileType>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyLocation> PropertyLocations => Set<PropertyLocation>();
    public DbSet<PropertyContact> PropertyContacts => Set<PropertyContact>();
    public DbSet<PropertyDescription> PropertyDescriptions => Set<PropertyDescription>();
    public DbSet<PropertyFacility> PropertyFacilities => Set<PropertyFacility>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<UnitBathroom> UnitBathrooms => Set<UnitBathroom>();
    public DbSet<UnitFacility> UnitFacilities => Set<UnitFacility>();
    public DbSet<StayPolicy> StayPolicies => Set<StayPolicy>();
    public DbSet<CancellationPolicy> CancellationPolicies => Set<CancellationPolicy>();
    public DbSet<PropertyCharge> PropertyCharges => Set<PropertyCharge>();
    public DbSet<RatePlan> RatePlans => Set<RatePlan>();
    public DbSet<UnitInventory> UnitInventories => Set<UnitInventory>();
    public DbSet<RateCalendar> RateCalendars => Set<RateCalendar>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OtaADbContext).Assembly);
    }
}
