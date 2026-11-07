namespace OtaASimulator.Models.Units;

public sealed record UnitDetailResponse(
    UnitResponse Unit,
    IReadOnlyList<int> FacilityTypeIds,
    IReadOnlyList<BathroomResponse> Bathrooms);
