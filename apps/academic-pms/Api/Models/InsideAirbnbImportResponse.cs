namespace AcademicPms.Api.Models;

public sealed record InsideAirbnbImportResponse(
    string SourceSha256,
    int OriginalRows,
    int ExcludedMissingPrice,
    int ExcludedMissingMinimumNights,
    int ImportedProperties,
    DateTime ImportedAt);