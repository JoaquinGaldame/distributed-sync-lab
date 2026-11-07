using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.HealthCheck;
using OtaASimulator.Application.HealthCheck.Models;
using OtaASimulator.Models.Common;

namespace OtaASimulator.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthCheckController(HealthCheckService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponse<HealthCheckResult>> CheckLiveness()
    {
        var result = service.CheckLiveness();
        return Ok(ApiResponse<HealthCheckResult>.Success(result, "La API está disponible."));
    }

    [HttpGet("ready")]
    public async Task<ActionResult<ApiResponse<HealthCheckResult>>> CheckReadiness(CancellationToken ct)
    {
        var result = await service.CheckReadinessAsync(ct);

        if (result.Status == "healthy")
            return Ok(ApiResponse<HealthCheckResult>.Success(result, "La API está lista para recibir tráfico."));

        var error = new ApiError(
            "database_unavailable",
            "La API responde, pero no puede conectarse con PostgreSQL.",
            TraceId: HttpContext.TraceIdentifier);

        return StatusCode(
            StatusCodes.Status503ServiceUnavailable,
            ApiResponse<HealthCheckResult>.Failure("La API no está lista para recibir tráfico.", error));
    }
}
