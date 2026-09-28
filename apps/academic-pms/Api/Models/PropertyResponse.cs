using AcademicPms.Domain;

namespace AcademicPms.Api.Models;

public sealed record PropertyResponse(
    string Id,
    string? SourceListingId,
    string Name,
    decimal Price,
    string? RoomType,
    string? Neighbourhood,
    decimal? Latitude,
    decimal? Longitude,
    int? MinimumNights,
    long Version,
    DateTime UpdatedAt)
{
    public static PropertyResponse From(Property property) => new(
        property.Id,
        property.SourceListingId,
        property.Name,
        property.Price,
        property.RoomType,
        property.Neighbourhood,
        property.Latitude,
        property.Longitude,
        property.MinimumNights,
        property.Version,
        property.UpdatedAt);
}