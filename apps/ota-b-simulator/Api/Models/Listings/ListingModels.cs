using System.ComponentModel.DataAnnotations;

namespace OtaBSimulator.Api.Models.Listings;

public sealed record ListingRequest(
    [Required, StringLength(200, MinimumLength = 1)] string Title,
    [Required, StringLength(40)] string CategoryCode,
    [Required, StringLength(30)] string RentalModeCode,
    [Required, StringLength(3, MinimumLength = 3)] string CurrencyCode,
    [Required, StringLength(15)] string PrimaryLanguageCode,
    [StringLength(15)] string? SecondaryLanguageCode,
    [Required, StringLength(100, MinimumLength = 1)] string Timezone,
    [Required, StringLength(20)] string StatusCode,
    [Range(1, int.MaxValue)] int MaxGuests);

public sealed record ListingResponse(
    Guid Id, string Title, string CategoryCode, string RentalModeCode, string CurrencyCode,
    string PrimaryLanguageCode, string? SecondaryLanguageCode, string Timezone,
    string StatusCode, int MaxGuests, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

public sealed record LocationRequest(
    [Required, RegularExpression("^[A-Z]{2}$")] string CountryCode,
    [StringLength(150)] string? Region,
    [Required, StringLength(150, MinimumLength = 1)] string City,
    [StringLength(150)] string? Neighbourhood,
    [StringLength(250)] string? AddressLine,
    [StringLength(30)] string? PostalCode,
    [Range(-90, 90)] decimal? Latitude,
    [Range(-180, 180)] decimal? Longitude);
public sealed record LocationResponse(string CountryCode, string? Region, string City, string? Neighbourhood, string? AddressLine, string? PostalCode, decimal? Latitude, decimal? Longitude);

public sealed record ContactRequest(
    [Required, StringLength(30)] string RoleCode,
    [StringLength(150)] string? DisplayName,
    [EmailAddress, StringLength(254)] string? Email,
    [StringLength(50)] string? Phone,
    [Required, StringLength(15)] string LanguageCode);
public sealed record ContactResponse(string RoleCode, string? DisplayName, string? Email, string? Phone, string LanguageCode);

public sealed record TextRequest(
    [Required, StringLength(15)] string LanguageCode,
    [StringLength(500)] string? Summary,
    [Required, MinLength(1)] string Description);
public sealed record TextResponse(string LanguageCode, string? Summary, string Description);

public sealed record HouseRulesRequest(
    TimeOnly CheckInFrom, TimeOnly CheckInUntil, TimeOnly CheckOutFrom, TimeOnly CheckOutUntil,
    [Range(0, int.MaxValue)] int MinimumGuestAge,
    bool AllowsChildren, bool AllowsPets, bool AllowsSmoking, bool AllowsParties, string? AdditionalRules);
public sealed record HouseRulesResponse(TimeOnly CheckInFrom, TimeOnly CheckInUntil, TimeOnly CheckOutFrom, TimeOnly CheckOutUntil, int MinimumGuestAge, bool AllowsChildren, bool AllowsPets, bool AllowsSmoking, bool AllowsParties, string? AdditionalRules);

public sealed record BookingRulesRequest(
    [Required, StringLength(60)] string CancellationTermCode,
    [Range(1, int.MaxValue)] int MinNights,
    [Range(1, int.MaxValue)] int? MaxNights);
public sealed record BookingRulesResponse(string CancellationTermCode, int MinNights, int? MaxNights);

public sealed record PricingRequest([Range(1, long.MaxValue)] long BaseNightlyAmountMinor);
public sealed record PricingResponse(long BaseNightlyAmountMinor);

public sealed record FeeRequest(
    [Required, StringLength(60, MinimumLength = 1)] string Code,
    [Required, StringLength(150, MinimumLength = 1)] string Name,
    [Required, StringLength(30)] string BasisCode,
    [Range(0, long.MaxValue)] long? AmountMinor,
    [Range(typeof(decimal), "0", "100")] decimal? Percent,
    bool IsIncluded);
public sealed record FeeResponse(string Code, string Name, string BasisCode, long? AmountMinor, decimal? Percent, bool IsIncluded);

public sealed record ListingDetailResponse(
    ListingResponse Listing,
    LocationResponse? Location,
    IReadOnlyList<ContactResponse> Contacts,
    IReadOnlyList<TextResponse> Texts,
    IReadOnlyList<string> Amenities,
    HouseRulesResponse? HouseRules,
    BookingRulesResponse? BookingRules,
    PricingResponse? Pricing,
    IReadOnlyList<FeeResponse> Fees,
    IReadOnlyList<Rooms.RoomResponse> Rooms);
