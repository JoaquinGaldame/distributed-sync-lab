namespace OtaASimulator.Models.Units;

public sealed record UnitResponse(
    long Id,
    long PropertyId,
    string ExternalCode,
    string Name,
    int UnitTypeId,
    int TotalUnits,
    int MaximumOccupancy,
    int MaximumAdults,
    int MaximumChildren,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
