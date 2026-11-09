using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using OtaASimulator.Domain;

namespace OtaASimulator.Models.Availability;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class AvailabilityDayRequest : IValidatableObject
{
    [JsonPropertyName("date")]
    public required DateOnly Date { get; init; }

    [JsonPropertyName("price")]
    public decimal? Price { get; init; }

    [JsonPropertyName("available_units")]
    [Range(0, int.MaxValue)]
    public int? AvailableUnits { get; init; }

    [JsonPropertyName("closed")]
    public AvailabilityStatus? Closed { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Price.HasValue && Price.Value <= 0)
            yield return new ValidationResult("El precio debe ser mayor que cero.", [nameof(Price)]);

        if (Closed.HasValue && !Enum.IsDefined(Closed.Value))
            yield return new ValidationResult("Closed solamente admite los valores 0, 1 y 2.", [nameof(Closed)]);

        if (AvailableUnits.HasValue != Closed.HasValue)
            yield return new ValidationResult("AvailableUnits y Closed deben enviarse juntos.", [nameof(AvailableUnits), nameof(Closed)]);

        if (!Price.HasValue && !AvailableUnits.HasValue)
            yield return new ValidationResult("Cada día debe incluir precio, disponibilidad o ambos.", [nameof(Price), nameof(AvailableUnits), nameof(Closed)]);

        if (AvailableUnits.HasValue && Closed.HasValue)
        {
            if (Closed.Value == AvailabilityStatus.Available && AvailableUnits.Value == 0)
                yield return new ValidationResult("Una fecha disponible debe tener al menos una unidad disponible.", [nameof(AvailableUnits), nameof(Closed)]);

            if (Closed.Value is AvailabilityStatus.Closed or AvailabilityStatus.Reserved && AvailableUnits.Value != 0)
                yield return new ValidationResult("Una fecha cerrada o reservada debe tener cero unidades disponibles.", [nameof(AvailableUnits), nameof(Closed)]);
        }
    }
}
