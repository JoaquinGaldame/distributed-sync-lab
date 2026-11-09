using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OtaASimulator.Models.Units;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class UnitFacilityRequest
{
    [Range(1, int.MaxValue)]
    public required int UnitFacilityTypeId { get; init; }

    [Required, RegularExpression("^(PRESENT|MISSING)$")]
    public required string Status { get; init; }
}
