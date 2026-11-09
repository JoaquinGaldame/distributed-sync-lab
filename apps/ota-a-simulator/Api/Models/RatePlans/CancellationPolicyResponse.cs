namespace OtaASimulator.Models.RatePlans;

public sealed record CancellationPolicyResponse(
    int Id,
    string Code,
    string Name,
    int? FreeUntilDaysBefore,
    decimal PenaltyPercent);
