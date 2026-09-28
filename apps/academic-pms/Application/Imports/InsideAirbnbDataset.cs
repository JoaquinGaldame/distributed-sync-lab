namespace AcademicPms.Application.Imports;

public sealed record InsideAirbnbProperty(
    string Id,
    string SourceListingId,
    string Name,
    decimal Price,
    string RoomType,
    string Neighbourhood,
    decimal Latitude,
    decimal Longitude,
    int MinimumNights);

public sealed record InsideAirbnbDataset(
    string Sha256,
    int OriginalRows,
    int ExcludedMissingPrice,
    int ExcludedMissingMinimumNights,
    IReadOnlyList<InsideAirbnbProperty> Properties);

public sealed record InsideAirbnbImportResult(
    string SourceSha256,
    int OriginalRows,
    int ExcludedMissingPrice,
    int ExcludedMissingMinimumNights,
    int ImportedProperties,
    DateTime ImportedAt);