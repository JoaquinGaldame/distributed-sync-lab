using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OtaASimulator.Models.Availability;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class AvailabilityListRequest
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

    [JsonPropertyName("start_date")]
    public required DateOnly StartDate { get; init; }

    [JsonPropertyName("days")]
    [Range(1, 730)]
    public required int Days { get; init; }
}
