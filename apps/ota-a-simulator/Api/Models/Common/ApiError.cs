namespace OtaASimulator.Models.Common;

public sealed record ApiError(
    string Code,
    string Detail,
    IReadOnlyDictionary<string, string[]>? ValidationErrors = null,
    string? TraceId = null);
