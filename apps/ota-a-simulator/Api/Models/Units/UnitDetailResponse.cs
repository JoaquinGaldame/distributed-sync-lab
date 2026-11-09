namespace OtaASimulator.Models.Units;

public sealed record UnitDetailResponse(
    UnitResponse Unit,
    IReadOnlyList<UnitFacilityResponse> UnitFacilities,
    IReadOnlyList<BathroomResponse> Bathrooms);
