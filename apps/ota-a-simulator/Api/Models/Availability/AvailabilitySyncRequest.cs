using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OtaASimulator.Models.Availability;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class AvailabilitySyncRequest : IValidatableObject
{
    [JsonPropertyName("property_id")]
    [Range(1, long.MaxValue)]
    public required long PropertyId { get; init; }

    [JsonPropertyName("unit_id")]
    [Range(1, long.MaxValue)]
    public required long UnitId { get; init; }

    [JsonPropertyName("rate_plan_id")]
    [Range(1, long.MaxValue)]
    public required long RatePlanId { get; init; }

    [JsonPropertyName("days")]
    [Required, MinLength(1), MaxLength(730)]
    public required IReadOnlyList<AvailabilityDayRequest> Days { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Days is null)
            yield break;

        var duplicateDates = Days
            .GroupBy(x => x.Date)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .OrderBy(x => x)
            .ToArray();

        if (duplicateDates.Length > 0)
            yield return new ValidationResult(
                $"Days contiene fechas duplicadas: {string.Join(", ", duplicateDates)}.",
                [nameof(Days)]);
    }
}
