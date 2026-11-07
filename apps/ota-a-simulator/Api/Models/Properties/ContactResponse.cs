namespace OtaASimulator.Models.Properties;

public sealed record ContactResponse(
    long Id,
    int ProfileTypeId,
    string? Name,
    string? Email,
    string? Phone,
    int LanguageId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
