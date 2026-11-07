using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace OtaASimulator.Models.Properties;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class ChargeRequest : IValidatableObject
{
    [Required, StringLength(60)]
    public required string Code { get; init; }

    [Required, StringLength(150)]
    public required string Name { get; init; }

    [Required, RegularExpression("^(per_stay|per_night|per_person_per_night|percentage)$")]
    public required string Calculation { get; init; }

    [Range(typeof(decimal), "0", "9999999999.99")]
    public required decimal Amount { get; init; }

    public required bool IncludedInPrice { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Calculation == "percentage" && Amount > 100)
            yield return new ValidationResult("El porcentaje no puede superar 100.", [nameof(Amount)]);
        if (decimal.Round(Amount, 2) != Amount)
            yield return new ValidationResult("El importe admite hasta dos decimales.", [nameof(Amount)]);
    }
}
