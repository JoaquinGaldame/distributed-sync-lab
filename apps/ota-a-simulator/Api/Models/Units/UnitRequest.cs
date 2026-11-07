using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace OtaASimulator.Models.Units;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class UnitRequest : IValidatableObject
{
    [Required, StringLength(60)]
    public required string ExternalCode { get; init; }

    [Required, StringLength(200)]
    public required string Name { get; init; }

    [Range(1, int.MaxValue)]
    public required int UnitTypeId { get; init; }

    [Range(1, int.MaxValue)]
    public required int TotalUnits { get; init; }

    [Range(1, int.MaxValue)]
    public required int MaximumOccupancy { get; init; }

    [Range(1, int.MaxValue)]
    public required int MaximumAdults { get; init; }

    [Range(0, int.MaxValue)]
    public required int MaximumChildren { get; init; }

    public required bool IsActive { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaximumAdults > MaximumOccupancy || MaximumChildren > MaximumOccupancy)
            yield return new ValidationResult("Los máximos de adultos y niños no pueden superar la ocupación total.", [nameof(MaximumAdults), nameof(MaximumChildren)]);
    }
}
