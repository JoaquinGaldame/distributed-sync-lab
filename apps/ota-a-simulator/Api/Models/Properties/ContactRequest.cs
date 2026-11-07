using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace OtaASimulator.Models.Properties;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class ContactRequest : IValidatableObject
{
    [Range(1, int.MaxValue)]
    public required int ProfileTypeId { get; init; }

    [StringLength(150)]
    public required string? Name { get; init; }

    [StringLength(254), EmailAddress]
    public required string? Email { get; init; }

    [StringLength(50)]
    public required string? Phone { get; init; }

    [Range(1, int.MaxValue)]
    public required int LanguageId { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Phone))
            yield return new ValidationResult("El contacto requiere email o teléfono.", [nameof(Email), nameof(Phone)]);
    }
}
