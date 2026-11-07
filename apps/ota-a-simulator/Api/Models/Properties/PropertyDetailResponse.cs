namespace OtaASimulator.Models.Properties;

public sealed record PropertyDetailResponse(
    PropertyResponse Property,
    LocationResponse? Location,
    IReadOnlyList<ContactResponse> Contacts,
    IReadOnlyList<DescriptionResponse> Descriptions,
    IReadOnlyList<string> FacilityCodes,
    StayPolicyResponse? StayPolicy,
    IReadOnlyList<ChargeResponse> Charges);
