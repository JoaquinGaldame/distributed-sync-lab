using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace OtaASimulator.Models.Properties;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class PropertyRequest : IValidatableObject
{
    [Required, StringLength(60)]
    public required string ExternalCode { get; init; }

    [Required, StringLength(200)]
    public required string Name { get; init; }

    [Range(1, int.MaxValue)]
    public required int PropertyTypeId { get; init; }

    [Range(1, int.MaxValue)]
    public required int CurrencyId { get; init; }

    [Range(1, int.MaxValue)]
    public required int PrimaryLanguageId { get; init; }

    [Required, StringLength(100)]
    public required string Timezone { get; init; }

    [Required, RegularExpression("^(draft|active|inactive)$")]
    public required string Status { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Timezone))
        {
            bool valid;
            try { TimeZoneInfo.FindSystemTimeZoneById(Timezone); valid = true; }
            catch (TimeZoneNotFoundException) { valid = false; }
            catch (InvalidTimeZoneException) { valid = false; }
            if (!valid) yield return new ValidationResult("Zona horaria desconocida.", [nameof(Timezone)]);
        }
    }
}
