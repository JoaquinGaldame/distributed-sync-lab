namespace OtaASimulator.Domain;

public sealed class PropertyLocation
{
    public long PropertyId { get; set; }
    public string CountryCode { get; set; } = "";
    public string? Region { get; set; }
    public string City { get; set; } = "";
    public string? Neighbourhood { get; set; }
    public string? AddressLine { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Property Property { get; set; } = null!;
}
