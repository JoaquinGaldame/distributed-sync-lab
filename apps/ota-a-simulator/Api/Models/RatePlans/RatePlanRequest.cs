using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace OtaASimulator.Models.RatePlans;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class RatePlanRequest : IValidatableObject
{
    [Required, StringLength(60)]
    public required string ExternalCode { get; init; }

    [Required, StringLength(150)]
    public required string Name { get; init; }

    [Range(1, int.MaxValue)]
    public required int CancellationPolicyId { get; init; }

    [Range(1, int.MaxValue)]
    public required int MinimumNights { get; init; }

    [Range(1, int.MaxValue)]
    public required int? MaximumNights { get; init; }

    public required bool IsActive { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaximumNights.HasValue && MaximumNights.Value < MinimumNights)
            yield return new ValidationResult("El máximo de noches debe ser mayor o igual al mínimo.", [nameof(MaximumNights)]);
    }
}
