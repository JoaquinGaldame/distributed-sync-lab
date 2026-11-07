using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace OtaASimulator.Models.Properties;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class DescriptionRequest
{
    [Range(1, int.MaxValue)]
    public required int LanguageId { get; init; }

    [Required]
    public required string Description { get; init; }

}
