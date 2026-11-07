namespace OtaASimulator.Models.Properties;

public sealed record StayPolicyResponse(
    TimeOnly CheckInFrom,
    TimeOnly CheckInUntil,
    TimeOnly CheckOutFrom,
    TimeOnly CheckOutUntil,
    int MinimumGuestAge,
    bool ChildrenAllowed,
    bool PetsAllowed,
    bool SmokingAllowed,
    bool PartiesAllowed,
    DateTime UpdatedAt);
