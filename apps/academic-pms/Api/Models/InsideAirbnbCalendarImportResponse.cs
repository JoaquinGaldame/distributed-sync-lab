namespace AcademicPms.Api.Models;

public sealed record InsideAirbnbCalendarImportResponse(
    string SourceSha256,
    int OriginalRows,
    int ImportedCalendarDays,
    int ExcludedUnknownProperties,
    int PropertiesWithCalendar,
    DateOnly MinimumDate,
    DateOnly MaximumDate,
    DateTime ImportedAt);
