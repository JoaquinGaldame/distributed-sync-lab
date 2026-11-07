namespace OtaASimulator.Models.Properties;

public sealed record LocationResponse(
    long PropertyId,
    string CountryCode,
    string? Region,
    string City,
    string? Neighbourhood,
    string? AddressLine,
    string? PostalCode,
    decimal? Latitude,
    decimal? Longitude,
    DateTime CreatedAt,
    DateTime UpdatedAt);
