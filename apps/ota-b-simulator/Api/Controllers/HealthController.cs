using Microsoft.AspNetCore.Mvc;
using OtaBSimulator.Api.Models.Common;
using OtaBSimulator.Application.Health;

namespace OtaBSimulator.Api.Controllers;

[ApiController, Route("health")]
public sealed class HealthController(HealthService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponse<HealthResult>> Live() => Ok(ApiResponse<HealthResult>.Success(service.Live(), "La API está disponible."));

    [HttpGet("ready")]
    public async Task<ActionResult<ApiResponse<HealthResult>>> Ready(CancellationToken ct)
    {
        var result = await service.Ready(ct);
        return result.Status == "healthy"
            ? Ok(ApiResponse<HealthResult>.Success(result, "La API está lista para recibir tráfico."))
            : StatusCode(503, ApiResponse<HealthResult>.Failure("La API no está lista para recibir tráfico.",
                new ApiError("database_unavailable", "La API responde, pero no puede conectarse con PostgreSQL.", TraceId: HttpContext.TraceIdentifier)));
    }
}
