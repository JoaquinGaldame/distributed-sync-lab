using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OtaBSimulator.Api.Models.Common;
using OtaBSimulator.Application.Common;

namespace OtaBSimulator.Infrastructure.Http;

public static class ApiValidationResponseFactory
{
    public static IActionResult Create(ActionContext context)
    {
        var validationErrors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(x => x.Key, x => x.Value!.Errors.Select(e =>
                string.IsNullOrWhiteSpace(e.ErrorMessage) ? "El valor proporcionado no es válido." : e.ErrorMessage).ToArray());
        return new BadRequestObjectResult(ApiResponse<object?>.Failure(
            "Solicitud inválida.",
            new ApiError("validation_error", "Uno o más campos de la solicitud no son válidos.", validationErrors, context.HttpContext.TraceIdentifier)));
    }
}

public static class ApiStatusCodeResponseWriter
{
    public static async Task WriteAsync(StatusCodeContext statusContext)
    {
        var context = statusContext.HttpContext;
        var (code, message, detail) = context.Response.StatusCode switch
        {
            400 => ("bad_request", "Solicitud inválida.", "La solicitud no pudo ser procesada."),
            404 => ("not_found", "Recurso inexistente.", "La ruta o el recurso solicitado no existe."),
            405 => ("method_not_allowed", "Método no permitido.", "El método HTTP no está permitido para esta ruta."),
            415 => ("unsupported_media_type", "Formato no soportado.", "El formato del contenido enviado no está soportado."),
            _ => ("http_error", "No se pudo completar la solicitud.", "La solicitud finalizó con un estado de error.")
        };
        await context.Response.WriteAsJsonAsync(ApiResponse<object?>.Failure(message,
            new ApiError(code, detail, TraceId: context.TraceIdentifier)), cancellationToken: context.RequestAborted);
    }
}

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is OperationCanceledException && context.RequestAborted.IsCancellationRequested) return false;
        var postgres = exception as PostgresException ?? exception.InnerException as PostgresException;
        int status;
        string code;
        string detail;
        if (exception is ApiException api)
            (status, code, detail) = (api.StatusCode, api.Code, api.Message);
        else if (postgres?.SqlState == PostgresErrorCodes.UniqueViolation)
            (status, code, detail) = (409, "duplicate_code", "Ya existe un registro con la misma clave o código.");
        else if (postgres?.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            (status, code, detail) = (409, "reference_conflict", "Una referencia ya no está disponible o está siendo utilizada.");
        else if (postgres?.SqlState is PostgresErrorCodes.CheckViolation or PostgresErrorCodes.NotNullViolation
                     or PostgresErrorCodes.NumericValueOutOfRange or PostgresErrorCodes.StringDataRightTruncation)
            (status, code, detail) = (400, "invalid_data", "Los datos no cumplen las restricciones del recurso.");
        else
        {
            (status, code, detail) = (500, "internal_error", "No se pudo completar la operación.");
            logger.LogError(exception, "Fallo al procesar {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        context.Response.StatusCode = status;
        var message = status switch { 400 => "Solicitud inválida.", 404 => "Recurso inexistente.", 409 => "Conflicto de estado.", _ => "Error interno." };
        await context.Response.WriteAsJsonAsync(ApiResponse<object?>.Failure(message,
            new ApiError(code, detail, TraceId: context.TraceIdentifier)), cancellationToken: ct);
        return true;
    }
}
