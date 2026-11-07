namespace OtaASimulator.Models.Properties;

public sealed record PropertyResponse(
    long Id,
    string ExternalCode,
    string Name,
    int PropertyTypeId,
    int CurrencyId,
    int PrimaryLanguageId,
    string Timezone,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);
