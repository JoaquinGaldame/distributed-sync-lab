using OtaASimulator.Domain;
using OtaASimulator.Models.Properties;

namespace OtaASimulator.Mappers;

public static class LocationMapper
{
    public static void Apply(LocationRequest request, PropertyLocation entity)
    {
        entity.CountryCode = request.CountryCode;
        entity.Region = request.Region;
        entity.City = request.City;
        entity.Neighbourhood = request.Neighbourhood;
        entity.AddressLine = request.AddressLine;
        entity.PostalCode = request.PostalCode;
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
    }

    public static LocationResponse ToResponse(PropertyLocation entity) => new(
        entity.PropertyId,
        entity.CountryCode,
        entity.Region,
        entity.City,
        entity.Neighbourhood,
        entity.AddressLine,
        entity.PostalCode,
        entity.Latitude,
        entity.Longitude,
        entity.CreatedAt,
        entity.UpdatedAt);
}
