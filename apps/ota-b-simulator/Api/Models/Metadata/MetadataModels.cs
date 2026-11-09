namespace OtaBSimulator.Api.Models.Metadata;

public sealed record NamedIdCatalogItem(int Id, string Code, string Name);
public sealed record NamedCatalogItem(string Code, string Name);
public sealed record CurrencyMetadata(int Id, string Code, string Name, short DecimalPlaces);
public sealed record AmenityMetadata(int Id, string Code, string GroupCode, string Name);
public sealed record AvailabilityStateMetadata(string Code, string Name, bool IsOpen);
public sealed record CancellationTermMetadata(string Code, string Name, int? FreeCancellationDaysBefore, decimal RefundPercentAfterDeadline);
public sealed record MetadataResponse(
    IReadOnlyList<CurrencyMetadata> Currencies,
    IReadOnlyList<NamedIdCatalogItem> Languages,
    IReadOnlyList<NamedIdCatalogItem> ListingCategories,
    IReadOnlyList<NamedCatalogItem> RentalModes,
    IReadOnlyList<NamedCatalogItem> AmenityGroups,
    IReadOnlyList<NamedIdCatalogItem> RoomTypes,
    IReadOnlyList<NamedIdCatalogItem> BedTypes,
    IReadOnlyList<AmenityMetadata> Amenities,
    IReadOnlyList<NamedCatalogItem> ContactRoles,
    IReadOnlyList<NamedIdCatalogItem> ListingStatuses,
    IReadOnlyList<AvailabilityStateMetadata> AvailabilityStates,
    IReadOnlyList<CancellationTermMetadata> CancellationTerms,
    IReadOnlyList<NamedCatalogItem> FeeBases);
