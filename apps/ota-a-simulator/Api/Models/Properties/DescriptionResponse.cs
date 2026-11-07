namespace OtaASimulator.Models.Properties;

public sealed record DescriptionResponse(
    int LanguageId,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);
