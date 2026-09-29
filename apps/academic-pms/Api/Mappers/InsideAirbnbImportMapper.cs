using AcademicPms.Api.Models;
using AcademicPms.Application.Imports;

namespace AcademicPms.Api.Mappers;

public static class InsideAirbnbImportMapper
{
    public static InsideAirbnbImportResponse ToResponse(
        InsideAirbnbImportResult result) => new(
        result.SourceSha256,
        result.OriginalRows,
        result.ExcludedMissingPrice,
        result.ExcludedMissingMinimumNights,
        result.ImportedProperties,
        result.ImportedAt);

    public static InsideAirbnbCalendarImportResponse ToResponse(
        InsideAirbnbCalendarImportResult result) => new(
        result.SourceSha256,
        result.OriginalRows,
        result.ImportedCalendarDays,
        result.ExcludedUnknownProperties,
        result.PropertiesWithCalendar,
        result.MinimumDate,
        result.MaximumDate,
        result.ImportedAt);
}
