namespace OtaReplaceSimulator.Api.Models;

public sealed record ExternalPropertyPageResponse(
    IReadOnlyList<ExternalPropertyResponse> Items,
    int? NextOffset,
    bool HasMore);