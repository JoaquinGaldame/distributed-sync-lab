namespace OtaBSimulator.Domain;

public sealed class Currency { public int Id { get; set; } public string Code { get; set; } = ""; public string Name { get; set; } = ""; public short DecimalPlaces { get; set; } }
public sealed class Language { public int Id { get; set; } public string Code { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class ListingCategory { public int Id { get; set; } public string Code { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class RentalMode { public string Code { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class AmenityGroup { public string Code { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class Amenity { public int Id { get; set; } public string Code { get; set; } = ""; public string GroupCode { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class ContactRole { public string Code { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class ListingStatus { public int Id { get; set; } public string Code { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class RoomType { public int Id { get; set; } public string Code { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class BedType { public int Id { get; set; } public string Code { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class AvailabilityState { public string Code { get; set; } = ""; public string Name { get; set; } = ""; public bool IsOpen { get; set; } }
public sealed class CancellationTerm { public string Code { get; set; } = ""; public string Name { get; set; } = ""; public int? FreeCancellationDaysBefore { get; set; } public decimal RefundPercentAfterDeadline { get; set; } }
public sealed class FeeBasis { public string Code { get; set; } = ""; public string Name { get; set; } = ""; }

public sealed class Listing
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public int CategoryId { get; set; }
    public ListingCategory Category { get; set; } = null!;
    public string RentalModeCode { get; set; } = "";
    public RentalMode RentalMode { get; set; } = null!;
    public string CurrencyCode { get; set; } = "";
    public Currency Currency { get; set; } = null!;
    public int PrimaryLanguageId { get; set; }
    public Language PrimaryLanguage { get; set; } = null!;
    public int? SecondaryLanguageId { get; set; }
    public Language? SecondaryLanguage { get; set; }
    public string Timezone { get; set; } = "";
    public int StatusId { get; set; }
    public ListingStatus Status { get; set; } = null!;
    public int MaxGuests { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<Room> Rooms { get; set; } = [];
    public Location? Location { get; set; }
    public List<Contact> Contacts { get; set; } = [];
    public List<ListingText> Texts { get; set; } = [];
    public List<ListingAmenity> Amenities { get; set; } = [];
    public HouseRules? HouseRules { get; set; }
    public BookingRules? BookingRules { get; set; }
    public ListingPricing? Pricing { get; set; }
    public List<ListingFee> Fees { get; set; } = [];
}

public sealed class Room
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public int RoomTypeId { get; set; }
    public RoomType RoomType { get; set; } = null!;
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public decimal? AreaM2 { get; set; }
    public int? FloorNumber { get; set; }
    public bool IsShared { get; set; }
    public int DisplayOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<RoomBed> Beds { get; set; } = [];
}

public sealed class RoomBed { public Guid RoomId { get; set; } public int BedTypeId { get; set; } public BedType BedType { get; set; } = null!; public int Quantity { get; set; } }
public sealed class Location { public Guid ListingId { get; set; } public string CountryCode { get; set; } = ""; public string? Region { get; set; } public string City { get; set; } = ""; public string? Neighbourhood { get; set; } public string? AddressLine { get; set; } public string? PostalCode { get; set; } public decimal? Latitude { get; set; } public decimal? Longitude { get; set; } public DateTimeOffset CreatedAt { get; set; } public DateTimeOffset UpdatedAt { get; set; } }
public sealed class Contact { public Guid ListingId { get; set; } public string RoleCode { get; set; } = ""; public string? DisplayName { get; set; } public string? Email { get; set; } public string? Phone { get; set; } public string LanguageCode { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } public DateTimeOffset UpdatedAt { get; set; } }
public sealed class ListingText { public Guid ListingId { get; set; } public int LanguageId { get; set; } public Language Language { get; set; } = null!; public string? Summary { get; set; } public string Description { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } public DateTimeOffset UpdatedAt { get; set; } }
public sealed class ListingAmenity { public Guid ListingId { get; set; } public int AmenityId { get; set; } public Amenity Amenity { get; set; } = null!; }
public sealed class HouseRules { public Guid ListingId { get; set; } public TimeOnly CheckInFrom { get; set; } public TimeOnly CheckInUntil { get; set; } public TimeOnly CheckOutFrom { get; set; } public TimeOnly CheckOutUntil { get; set; } public int MinimumGuestAge { get; set; } public bool AllowsChildren { get; set; } public bool AllowsPets { get; set; } public bool AllowsSmoking { get; set; } public bool AllowsParties { get; set; } public string? AdditionalRules { get; set; } public DateTimeOffset UpdatedAt { get; set; } }
public sealed class BookingRules { public Guid ListingId { get; set; } public string CancellationTermCode { get; set; } = ""; public int MinNights { get; set; } public int? MaxNights { get; set; } public DateTimeOffset UpdatedAt { get; set; } }
public sealed class ListingPricing { public Guid ListingId { get; set; } public long BaseNightlyAmountMinor { get; set; } public DateTimeOffset UpdatedAt { get; set; } }
public sealed class ListingFee { public Guid ListingId { get; set; } public string Code { get; set; } = ""; public string Name { get; set; } = ""; public string BasisCode { get; set; } = ""; public long? AmountMinor { get; set; } public decimal? Percent { get; set; } public bool IsIncluded { get; set; } public DateTimeOffset UpdatedAt { get; set; } }
public sealed class DailyPrice { public Guid ListingId { get; set; } public DateOnly StayDate { get; set; } public long AmountMinor { get; set; } public DateTimeOffset UpdatedAt { get; set; } }
public sealed class DailyAvailability { public Guid ListingId { get; set; } public DateOnly StayDate { get; set; } public string StateCode { get; set; } = ""; public DateTimeOffset UpdatedAt { get; set; } }
