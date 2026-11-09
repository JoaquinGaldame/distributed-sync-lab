using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.RatePlans;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.RatePlans;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/units/{unitId:long:min(1)}/rate-plans")]
public sealed class RatePlansController(RatePlanService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<RatePlanResponse>>>> List(long propertyId, long unitId, [FromQuery, Range(1, 1000000)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 50, CancellationToken ct = default) =>
        Ok(ApiResponse<PagedResponse<RatePlanResponse>>.Success(
            await service.ListAsync(propertyId, unitId, page, pageSize, ct),
            "Planes tarifarios obtenidos correctamente."));

    [HttpGet("{ratePlanId:long:min(1)}")]
    public async Task<ActionResult<ApiResponse<RatePlanResponse>>> Get(long propertyId, long unitId, long ratePlanId, CancellationToken ct) =>
        Ok(ApiResponse<RatePlanResponse>.Success(
            await service.GetAsync(propertyId, unitId, ratePlanId, ct),
            "Plan tarifario obtenido correctamente."));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<RatePlanResponse>>> Create(long propertyId, long unitId, RatePlanRequest request, CancellationToken ct)
    {
        var response = await service.CreateAsync(propertyId, unitId, request, ct);
        return CreatedAtAction(
            nameof(Get),
            new { propertyId, unitId, ratePlanId = response.Id },
            ApiResponse<RatePlanResponse>.Success(response, "Plan tarifario creado correctamente."));
    }

    [HttpPut("{ratePlanId:long:min(1)}")]
    public async Task<ActionResult<ApiResponse<RatePlanResponse>>> Update(long propertyId, long unitId, long ratePlanId, RatePlanRequest request, CancellationToken ct) =>
        Ok(ApiResponse<RatePlanResponse>.Success(
            await service.UpdateAsync(propertyId, unitId, ratePlanId, request, ct),
            "Plan tarifario actualizado correctamente."));

    [HttpDelete("{ratePlanId:long:min(1)}")]
    public async Task<ActionResult<ApiResponse<DeletedResourceResponse>>> Delete(long propertyId, long unitId, long ratePlanId, CancellationToken ct)
    {
        await service.DeleteAsync(propertyId, unitId, ratePlanId, ct);
        return Ok(ApiResponse<DeletedResourceResponse>.Success(
            new DeletedResourceResponse(true, "ratePlan", ratePlanId),
            "Plan tarifario eliminado correctamente."));
    }
}
