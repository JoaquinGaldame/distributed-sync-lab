using OtaReplaceSimulator.Domain;

namespace OtaReplaceSimulator.Api.Models;

public sealed record ExternalPropertyResponse(
    string ExternalId,
    string Name,
    decimal Price,
    string? RoomType,
    string? Neighbourhood,
    decimal? Latitude,
    decimal? Longitude,
    int? MinimumNights,
    long SourceVersion)
{
    public static ExternalPropertyResponse From(ExternalProperty property) => new(
        property.ExternalId,
        property.Name,
        property.Price,
        property.RoomType,
        property.Neighbourhood,
        property.Latitude,
        property.Longitude,
        property.MinimumNights,
        property.SourceVersion);
}