namespace AcademicPms.Application.Imports;

public sealed record InsideAirbnbCalendarRow(
    string PropertyId,
    DateOnly Date,
    bool IsAvailable,
    int MinimumNights,
    int MaximumNights);

public sealed record InsideAirbnbCalendarReadResult(
    int OriginalRows,
    int SourceListings,
    DateOnly MinimumDate,
    DateOnly MaximumDate);

public sealed record InsideAirbnbCalendarImportResult(
    string SourceSha256,
    int OriginalRows,
    int ImportedCalendarDays,
    int ExcludedUnknownProperties,
    int PropertiesWithCalendar,
    DateOnly MinimumDate,
    DateOnly MaximumDate,
    DateTime ImportedAt);
