using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OtaASimulator.Models.Properties;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class PropertyFacilityRequest
{
    [Range(1, int.MaxValue)]
    public required int FacilityId { get; init; }

    [Required, RegularExpression("^(Present|Missing)$")]
    public required string Status { get; init; }
}
