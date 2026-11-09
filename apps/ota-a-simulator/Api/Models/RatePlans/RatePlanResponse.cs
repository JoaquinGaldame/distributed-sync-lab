namespace OtaASimulator.Models.RatePlans;

public sealed record RatePlanResponse(
    long Id,
    long PropertyId,
    long UnitId,
    string ExternalCode,
    string Name,
    CancellationPolicyResponse CancellationPolicy,
    int MinimumNights,
    int? MaximumNights,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
