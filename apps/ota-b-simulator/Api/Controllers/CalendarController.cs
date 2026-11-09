using Microsoft.AspNetCore.Mvc;
using OtaBSimulator.Api.Models.Calendar;
using OtaBSimulator.Api.Models.Common;
using OtaBSimulator.Application.Calendar;

namespace OtaBSimulator.Api.Controllers;

[ApiController, Route("listings/{listingId:guid}")]
public sealed class CalendarController(CalendarService service) : ControllerBase
{
    [HttpPut("prices")]
    public async Task<ActionResult<ApiResponse<CalendarWriteResponse>>> Prices(Guid listingId, PricePeriodsRequest request, CancellationToken ct) =>
        Ok(ApiResponse<CalendarWriteResponse>.Success(await service.PutPrices(listingId, request, ct), "Precios actualizados correctamente."));

    [HttpDelete("prices")]
    public async Task<ActionResult<ApiResponse<CalendarDeleteResponse>>> DeletePrices(Guid listingId, [FromQuery] DateOnly from, [FromQuery] DateOnly until, CancellationToken ct) =>
        Ok(ApiResponse<CalendarDeleteResponse>.Success(await service.DeletePrices(listingId, from, until, ct), "Excepciones de precio eliminadas correctamente."));

    [HttpPut("availability")]
    public async Task<ActionResult<ApiResponse<CalendarWriteResponse>>> Availability(Guid listingId, AvailabilityPeriodsRequest request, CancellationToken ct) =>
        Ok(ApiResponse<CalendarWriteResponse>.Success(await service.PutAvailability(listingId, request, ct), "Disponibilidad actualizada correctamente."));

    [HttpDelete("availability")]
    public async Task<ActionResult<ApiResponse<CalendarDeleteResponse>>> DeleteAvailability(Guid listingId, [FromQuery] DateOnly from, [FromQuery] DateOnly until, CancellationToken ct) =>
        Ok(ApiResponse<CalendarDeleteResponse>.Success(await service.DeleteAvailability(listingId, from, until, ct), "Estados de disponibilidad eliminados correctamente."));

    [HttpGet("calendar")]
    public async Task<ActionResult<ApiResponse<CalendarResponse>>> Calendar(Guid listingId, [FromQuery] DateOnly from, [FromQuery] DateOnly until, CancellationToken ct) =>
        Ok(ApiResponse<CalendarResponse>.Success(await service.Get(listingId, from, until, ct), "Consulta completada correctamente."));
}
