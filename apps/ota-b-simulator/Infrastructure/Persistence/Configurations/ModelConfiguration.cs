using System.Text;
using Microsoft.EntityFrameworkCore;
using OtaBSimulator.Domain;

namespace OtaBSimulator.Infrastructure.Persistence.Configurations;

public static class ModelConfiguration
{
    public static void ConfigureOtaBModel(this ModelBuilder b)
    {
        b.Entity<Currency>().ToTable("currencies");
        b.Entity<Language>().ToTable("languages");
        b.Entity<ListingCategory>().ToTable("listing_categories");
        b.Entity<RentalMode>().ToTable("rental_modes").HasKey(x => x.Code);
        b.Entity<AmenityGroup>().ToTable("amenity_groups").HasKey(x => x.Code);
        b.Entity<Amenity>().ToTable("amenities");
        b.Entity<ContactRole>().ToTable("contact_roles").HasKey(x => x.Code);
        b.Entity<ListingStatus>().ToTable("listing_statuses");
        b.Entity<RoomType>().ToTable("room_types");
        b.Entity<BedType>().ToTable("bed_types");
        b.Entity<AvailabilityState>().ToTable("availability_states").HasKey(x => x.Code);
        b.Entity<CancellationTerm>().ToTable("cancellation_terms").HasKey(x => x.Code);
        b.Entity<FeeBasis>().ToTable("fee_bases").HasKey(x => x.Code);

        b.Entity<Listing>(e =>
        {
            e.ToTable("listings");
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
            e.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId);
            e.HasOne(x => x.RentalMode).WithMany().HasForeignKey(x => x.RentalModeCode);
            e.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyCode).HasPrincipalKey(x => x.Code);
            e.HasOne(x => x.PrimaryLanguage).WithMany().HasForeignKey(x => x.PrimaryLanguageId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.SecondaryLanguage).WithMany().HasForeignKey(x => x.SecondaryLanguageId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
            e.HasMany(x => x.Rooms).WithOne().HasForeignKey(x => x.ListingId);
            e.HasOne(x => x.Location).WithOne().HasForeignKey<Location>(x => x.ListingId);
            e.HasMany(x => x.Contacts).WithOne().HasForeignKey(x => x.ListingId);
            e.HasMany(x => x.Texts).WithOne().HasForeignKey(x => x.ListingId);
            e.HasMany(x => x.Amenities).WithOne().HasForeignKey(x => x.ListingId);
            e.HasOne(x => x.HouseRules).WithOne().HasForeignKey<HouseRules>(x => x.ListingId);
            e.HasOne(x => x.BookingRules).WithOne().HasForeignKey<BookingRules>(x => x.ListingId);
            e.HasOne(x => x.Pricing).WithOne().HasForeignKey<ListingPricing>(x => x.ListingId);
            e.HasMany(x => x.Fees).WithOne().HasForeignKey(x => x.ListingId);
        });

        b.Entity<Room>(e =>
        {
            e.ToTable("rooms");
            e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
            e.HasOne(x => x.RoomType).WithMany().HasForeignKey(x => x.RoomTypeId);
            e.HasMany(x => x.Beds).WithOne().HasForeignKey(x => x.RoomId);
        });
        b.Entity<RoomBed>(e => { e.ToTable("room_beds"); e.HasKey(x => new { x.RoomId, x.BedTypeId }); e.HasOne(x => x.BedType).WithMany().HasForeignKey(x => x.BedTypeId); });
        b.Entity<Location>().ToTable("locations").HasKey(x => x.ListingId);
        b.Entity<Contact>(e => { e.ToTable("contacts"); e.HasKey(x => new { x.ListingId, x.RoleCode }); });
        b.Entity<ListingText>(e => { e.ToTable("listing_texts"); e.HasKey(x => new { x.ListingId, x.LanguageId }); e.HasOne(x => x.Language).WithMany().HasForeignKey(x => x.LanguageId); });
        b.Entity<ListingAmenity>(e => { e.ToTable("listing_amenities"); e.HasKey(x => new { x.ListingId, x.AmenityId }); e.HasOne(x => x.Amenity).WithMany().HasForeignKey(x => x.AmenityId); });
        b.Entity<HouseRules>().ToTable("listing_house_rules").HasKey(x => x.ListingId);
        b.Entity<BookingRules>().ToTable("listing_booking_rules").HasKey(x => x.ListingId);
        b.Entity<ListingPricing>().ToTable("listing_pricing").HasKey(x => x.ListingId);
        b.Entity<ListingFee>(e => { e.ToTable("listing_fees"); e.HasKey(x => new { x.ListingId, x.Code }); });
        b.Entity<DailyPrice>(e => { e.ToTable("listing_daily_prices"); e.HasKey(x => new { x.ListingId, x.StayDate }); });
        b.Entity<DailyAvailability>(e => { e.ToTable("listing_daily_availability"); e.HasKey(x => new { x.ListingId, x.StayDate }); });

        foreach (var entity in b.Model.GetEntityTypes())
            foreach (var property in entity.GetProperties())
                property.SetColumnName(ToSnakeCase(property.Name));
    }

    private static string ToSnakeCase(string value)
    {
        var result = new StringBuilder();
        for (var i = 0; i < value.Length; i++)
        {
            if (i > 0 && char.IsUpper(value[i])) result.Append('_');
            result.Append(char.ToLowerInvariant(value[i]));
        }
        return result.ToString();
    }
}
