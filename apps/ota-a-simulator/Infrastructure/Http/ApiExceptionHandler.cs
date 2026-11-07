using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OtaASimulator.Application.Common;
using OtaASimulator.Models.Common;

namespace OtaASimulator.Infrastructure.Http;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is OperationCanceledException && context.RequestAborted.IsCancellationRequested)
            return false;

        var postgres = exception as PostgresException ?? exception.InnerException as PostgresException;
        int status;
        string code;
        string detail;

        if (exception is ApiException api)
        {
            status = api.StatusCode;
            code = api.Code;
            detail = api.Message;
        }
        else if (postgres is not null && postgres.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            status = StatusCodes.Status409Conflict;
            code = "duplicate_code";
            detail = "Ya existe un registro con la misma clave o código.";
        }
        else if (postgres is not null && postgres.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            status = StatusCodes.Status409Conflict;
            code = "reference_conflict";
            detail = "Una referencia ya no está disponible o está siendo utilizada.";
        }
        else if (postgres is not null && postgres.SqlState is PostgresErrorCodes.CheckViolation
                     or PostgresErrorCodes.NotNullViolation
                     or PostgresErrorCodes.NumericValueOutOfRange
                     or PostgresErrorCodes.StringDataRightTruncation)
        {
            status = StatusCodes.Status400BadRequest;
            code = "invalid_data";
            detail = "Los datos no cumplen las restricciones del recurso.";
        }
        else if (postgres is not null && postgres.SqlState is PostgresErrorCodes.RaiseException
                     or PostgresErrorCodes.DeadlockDetected
                     or PostgresErrorCodes.SerializationFailure)
        {
            status = StatusCodes.Status409Conflict;
            code = "state_conflict";
            detail = "La operación entra en conflicto con el estado actual. Consulte el recurso antes de reintentar.";
        }
        else
        {
            status = StatusCodes.Status500InternalServerError;
            code = "internal_error";
            detail = "No se pudo completar la operación.";
            logger.LogError(exception, "Fallo al procesar {Method} {Path}", context.Request.Method, context.Request.Path);
        }

        context.Response.StatusCode = status;
        var message = status switch
        {
            StatusCodes.Status400BadRequest => "Solicitud inválida.",
            StatusCodes.Status404NotFound => "Recurso inexistente.",
            StatusCodes.Status409Conflict => "Conflicto de estado.",
            _ => "Error interno."
        };
        var response = ApiResponse<object?>.Failure(
            message,
            new ApiError(code, detail, TraceId: context.TraceIdentifier));

        await context.Response.WriteAsJsonAsync(response, cancellationToken: ct);
        return true;
    }
}
