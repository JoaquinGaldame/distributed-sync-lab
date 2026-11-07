using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace OtaASimulator.Models.Units;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class BathroomRequest
{
    [Range(1, int.MaxValue)]
    public required int BathroomConfigurationId { get; init; }

    [Required, RegularExpression("^(PRIVATE|SHARED)$")]
    public required string Scope { get; init; }

    [Range(1, int.MaxValue)]
    public required int Quantity { get; init; }

}
