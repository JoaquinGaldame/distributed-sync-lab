using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace OtaASimulator.Models.Properties;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class StayPolicyRequest
{
    public required TimeOnly CheckInFrom { get; init; }

    public required TimeOnly CheckInUntil { get; init; }

    public required TimeOnly CheckOutFrom { get; init; }

    public required TimeOnly CheckOutUntil { get; init; }

    [Range(0, int.MaxValue)]
    public required int MinimumGuestAge { get; init; }

    public required bool ChildrenAllowed { get; init; }

    public required bool PetsAllowed { get; init; }

    public required bool SmokingAllowed { get; init; }

    public required bool PartiesAllowed { get; init; }

}
