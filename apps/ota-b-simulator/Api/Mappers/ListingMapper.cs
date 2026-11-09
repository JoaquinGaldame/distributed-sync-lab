using OtaBSimulator.Api.Models.Listings;
using OtaBSimulator.Domain;

namespace OtaBSimulator.Api.Mappers;

public static class ListingMapper
{
    public static ListingResponse ToResponse(Listing x) => new(
        x.Id, x.Title, x.Category.Code, x.RentalModeCode, x.CurrencyCode,
        x.PrimaryLanguage.Code, x.SecondaryLanguage?.Code, x.Timezone,
        x.Status.Code, x.MaxGuests, x.CreatedAt, x.UpdatedAt);

    public static void Apply(ListingRequest request, Listing entity, ListingCategory category,
        Language primaryLanguage, Language? secondaryLanguage, ListingStatus status)
    {
        entity.Title = request.Title.Trim();
        entity.CategoryId = category.Id;
        entity.RentalModeCode = request.RentalModeCode;
        entity.CurrencyCode = request.CurrencyCode;
        entity.PrimaryLanguageId = primaryLanguage.Id;
        entity.SecondaryLanguageId = secondaryLanguage?.Id;
        entity.Timezone = request.Timezone.Trim();
        entity.StatusId = status.Id;
        entity.MaxGuests = request.MaxGuests;
    }

    public static LocationResponse ToResponse(Location x) => new(x.CountryCode, x.Region, x.City,
        x.Neighbourhood, x.AddressLine, x.PostalCode, x.Latitude, x.Longitude);
    public static ContactResponse ToResponse(Contact x) => new(x.RoleCode, x.DisplayName, x.Email, x.Phone, x.LanguageCode);
    public static TextResponse ToResponse(ListingText x) => new(x.Language.Code, x.Summary, x.Description);
    public static HouseRulesResponse ToResponse(HouseRules x) => new(x.CheckInFrom, x.CheckInUntil,
        x.CheckOutFrom, x.CheckOutUntil, x.MinimumGuestAge, x.AllowsChildren, x.AllowsPets,
        x.AllowsSmoking, x.AllowsParties, x.AdditionalRules);
    public static BookingRulesResponse ToResponse(BookingRules x) => new(x.CancellationTermCode, x.MinNights, x.MaxNights);
    public static PricingResponse ToResponse(ListingPricing x) => new(x.BaseNightlyAmountMinor);
    public static FeeResponse ToResponse(ListingFee x) => new(x.Code, x.Name, x.BasisCode, x.AmountMinor, x.Percent, x.IsIncluded);
}
