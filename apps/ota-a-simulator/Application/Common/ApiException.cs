namespace OtaASimulator.Application.Common;

public sealed class ApiException(int statusCode, string code, string detail, string? responseMessage = null) : Exception(detail)
{
    public int StatusCode { get; } = statusCode;
    public string Code { get; } = code;
    public string? ResponseMessage { get; } = responseMessage;
    public static ApiException Invalid(string detail) => new(400, "invalid_request", detail);
    public static ApiException NotFound(string detail) => new(404, "not_found", detail);
    public static ApiException Conflict(string detail) => new(409, "state_conflict", detail);
    public static ApiException AvailabilityNotConfigured(string detail) => new(
        409,
        "availability_not_configured",
        detail,
        "No existen precios ni disponibilidad para el período solicitado.");
}
