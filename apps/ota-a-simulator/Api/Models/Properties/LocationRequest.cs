using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace OtaASimulator.Models.Properties;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class LocationRequest : IValidatableObject
{
    [Required, RegularExpression("^[A-Z]{2}$")]
    public required string CountryCode { get; init; }

    [StringLength(150)]
    public required string? Region { get; init; }

    [Required, StringLength(150)]
    public required string City { get; init; }

    [StringLength(150)]
    public required string? Neighbourhood { get; init; }

    [StringLength(250)]
    public required string? AddressLine { get; init; }

    [StringLength(30)]
    public required string? PostalCode { get; init; }

    [Range(typeof(decimal), "-90", "90")]
    public required decimal? Latitude { get; init; }

    [Range(typeof(decimal), "-180", "180")]
    public required decimal? Longitude { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Latitude.HasValue != Longitude.HasValue)
            yield return new ValidationResult("Latitud y longitud deben enviarse juntas o ambas null.", [nameof(Latitude), nameof(Longitude)]);
        if ((Latitude.HasValue && decimal.Round(Latitude.Value, 6) != Latitude.Value)
            || (Longitude.HasValue && decimal.Round(Longitude.Value, 6) != Longitude.Value))
            yield return new ValidationResult("Las coordenadas admiten hasta seis decimales.", [nameof(Latitude), nameof(Longitude)]);
    }
}
