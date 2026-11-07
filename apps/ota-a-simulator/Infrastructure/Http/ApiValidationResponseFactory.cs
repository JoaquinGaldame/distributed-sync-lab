using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Models.Common;

namespace OtaASimulator.Infrastructure.Http;

public static class ApiValidationResponseFactory
{
    public static IActionResult Create(ActionContext context)
    {
        var validationErrors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "El valor proporcionado no es válido."
                        : error.ErrorMessage)
                    .ToArray());

        var error = new ApiError(
            "validation_error",
            "Uno o más campos de la solicitud no son válidos.",
            validationErrors,
            context.HttpContext.TraceIdentifier);
        var response = ApiResponse<object?>.Failure("Solicitud inválida.", error);

        return new BadRequestObjectResult(response);
    }
}
