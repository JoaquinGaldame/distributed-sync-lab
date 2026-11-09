using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Units;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.Units;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/units")]
public sealed class UnitsController(UnitService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<UnitResponse>>>> List(long propertyId, [FromQuery, Range(1, 1000000)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 50, CancellationToken ct = default) =>
        Ok(ApiResponse<PagedResponse<UnitResponse>>.Success(
            await service.ListAsync(propertyId, page, pageSize, ct),
            "Unidades obtenidas correctamente."));

    [HttpGet("{unitId:long:min(1)}")]
    public async Task<ActionResult<ApiResponse<UnitDetailResponse>>> Get(long propertyId, long unitId, CancellationToken ct) =>
        Ok(ApiResponse<UnitDetailResponse>.Success(
            await service.GetAsync(propertyId, unitId, ct),
            "Unidad obtenida correctamente."));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UnitResponse>>> Create(long propertyId, UnitRequest request, CancellationToken ct)
    {
        var response = await service.CreateAsync(propertyId, request, ct);
        return CreatedAtAction(
            nameof(Get),
            new { propertyId, unitId = response.Id },
            ApiResponse<UnitResponse>.Success(response, "Unidad creada correctamente."));
    }

    [HttpPut("{unitId:long:min(1)}")]
    public async Task<ActionResult<ApiResponse<UnitResponse>>> Update(long propertyId, long unitId, UnitRequest request, CancellationToken ct) =>
        Ok(ApiResponse<UnitResponse>.Success(
            await service.UpdateAsync(propertyId, unitId, request, ct),
            "Unidad actualizada correctamente."));

    [HttpDelete("{unitId:long:min(1)}")]
    public async Task<ActionResult<ApiResponse<DeletedResourceResponse>>> Delete(long propertyId, long unitId, CancellationToken ct)
    {
        await service.DeleteAsync(propertyId, unitId, ct);
        return Ok(ApiResponse<DeletedResourceResponse>.Success(
            new DeletedResourceResponse(true, "unit", unitId),
            "Unidad eliminada correctamente."));
    }
}
