using System.Text.Json.Serialization;

namespace OtaASimulator.Models.Availability;

public sealed record AvailabilityDayResponse(
    [property: JsonPropertyName("date")] DateOnly Date,
    [property: JsonPropertyName("price")] decimal? Price,
    [property: JsonPropertyName("available_units")] int? AvailableUnits,
    [property: JsonPropertyName("closed")] short? Closed,
    [property: JsonPropertyName("status")] string? Status);
