namespace OtaASimulator.Models.Common;

public sealed record ApiResponse<T>(
    T? Data,
    string Warning,
    string Message,
    ApiError? Error)
{
    public static ApiResponse<T> Success(T data, string message, string warning = "") =>
        new(data, warning, message, null);

    public static ApiResponse<T> Failure(string message, ApiError error, string warning = "") =>
        new(default, warning, message, error);
}
