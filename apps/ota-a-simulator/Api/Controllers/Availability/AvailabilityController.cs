using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Availability;
using OtaASimulator.Models.Availability;
using OtaASimulator.Models.Common;

namespace OtaASimulator.Controllers;

[ApiController]
[Route("availability")]
public sealed class AvailabilityController(AvailabilityService service) : ControllerBase
{
    [HttpPost("sync")]
    public async Task<ActionResult<ApiResponse<AvailabilityResponse>>> Sync(AvailabilitySyncRequest request, CancellationToken ct) =>
        Ok(ApiResponse<AvailabilityResponse>.Success(
            await service.SyncAsync(request, ct),
            "Precios y disponibilidad sincronizados correctamente."));

    [HttpPost("list")]
    public async Task<ActionResult<ApiResponse<AvailabilityResponse>>> List(AvailabilityListRequest request, CancellationToken ct) =>
        Ok(ApiResponse<AvailabilityResponse>.Success(
            await service.ListAsync(request, ct),
            "Precios y disponibilidad obtenidos correctamente."));
}
