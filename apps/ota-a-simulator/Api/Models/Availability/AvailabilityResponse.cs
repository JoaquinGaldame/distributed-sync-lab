using System.Text.Json.Serialization;

namespace OtaASimulator.Models.Availability;

public sealed record AvailabilityResponse(
    [property: JsonPropertyName("property_id")] long PropertyId,
    [property: JsonPropertyName("unit_id")] long UnitId,
    [property: JsonPropertyName("rate_plan_id")] long RatePlanId,
    [property: JsonPropertyName("days")] IReadOnlyList<AvailabilityDayResponse> Days);
