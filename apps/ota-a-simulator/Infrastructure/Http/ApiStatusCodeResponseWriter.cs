using Microsoft.AspNetCore.Diagnostics;
using OtaASimulator.Models.Common;

namespace OtaASimulator.Infrastructure.Http;

public static class ApiStatusCodeResponseWriter
{
    public static async Task WriteAsync(StatusCodeContext statusContext)
    {
        var context = statusContext.HttpContext;
        var (code, message, detail) = context.Response.StatusCode switch
        {
            StatusCodes.Status400BadRequest => ("bad_request", "Solicitud inválida.", "La solicitud no pudo ser procesada."),
            StatusCodes.Status401Unauthorized => ("unauthorized", "Autenticación requerida.", "Debe autenticarse para acceder al recurso."),
            StatusCodes.Status403Forbidden => ("forbidden", "Acceso denegado.", "No tiene permisos para acceder al recurso."),
            StatusCodes.Status404NotFound => ("not_found", "Recurso inexistente.", "La ruta o el recurso solicitado no existe."),
            StatusCodes.Status405MethodNotAllowed => ("method_not_allowed", "Método no permitido.", "El método HTTP no está permitido para esta ruta."),
            StatusCodes.Status415UnsupportedMediaType => ("unsupported_media_type", "Formato no soportado.", "El formato del contenido enviado no está soportado."),
            _ => ("http_error", "No se pudo completar la solicitud.", "La solicitud finalizó con un estado de error.")
        };
        var error = new ApiError(code, detail, TraceId: context.TraceIdentifier);
        var response = ApiResponse<object?>.Failure(message, error);

        await context.Response.WriteAsJsonAsync(response, cancellationToken: context.RequestAborted);
    }
}
