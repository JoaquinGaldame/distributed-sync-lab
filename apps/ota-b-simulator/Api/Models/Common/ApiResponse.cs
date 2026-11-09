namespace OtaBSimulator.Api.Models.Common;

public sealed record ApiResponse<T>(T? Data, string Warning, string Message, IReadOnlyList<ApiError> Errors)
{
    public static ApiResponse<T> Success(T data, string message, string warning = "") =>
        new(data, warning, message, []);

    public static ApiResponse<T> Failure(string message, ApiError error, string warning = "") =>
        new(default, warning, message, [error]);

    public static ApiResponse<T> Failure(string message, IReadOnlyList<ApiError> errors, string warning = "") =>
        new(default, warning, message, errors);
}

public sealed record ApiError(
    string Code,
    string Detail,
    IReadOnlyDictionary<string, string[]>? ValidationErrors = null,
    string? TraceId = null);

public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, long Total);
public sealed record DeletedResourceResponse(bool Deleted, string Resource, object Id);
