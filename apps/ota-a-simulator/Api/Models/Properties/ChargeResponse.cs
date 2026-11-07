namespace OtaASimulator.Models.Properties;

public sealed record ChargeResponse(
    long Id,
    string Code,
    string Name,
    string Calculation,
    decimal Amount,
    bool IncludedInPrice);
