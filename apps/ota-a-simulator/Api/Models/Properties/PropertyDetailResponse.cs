namespace OtaASimulator.Models.Properties;

public sealed record PropertyDetailResponse(
    PropertyResponse Property,
    LocationResponse? Location,
    IReadOnlyList<ContactResponse> Contacts,
    IReadOnlyList<DescriptionResponse> Descriptions,
    IReadOnlyList<PropertyFacilityResponse> Facilities,
    StayPolicyResponse? StayPolicy,
    IReadOnlyList<ChargeResponse> Charges);
