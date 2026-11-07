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
    public async Task<ActionResult<PagedResponse<RatePlanResponse>>> List(long propertyId, long unitId, [FromQuery, Range(1, 1000000)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 50, CancellationToken ct = default) =>
        Ok(await service.ListAsync(propertyId, unitId, page, pageSize, ct));

    [HttpGet("{ratePlanId:long:min(1)}")]
    public async Task<ActionResult<RatePlanResponse>> Get(long propertyId, long unitId, long ratePlanId, CancellationToken ct) => Ok(await service.GetAsync(propertyId, unitId, ratePlanId, ct));

    [HttpPost]
    public async Task<ActionResult<RatePlanResponse>> Create(long propertyId, long unitId, RatePlanRequest request, CancellationToken ct)
    {
        var response = await service.CreateAsync(propertyId, unitId, request, ct);
        return CreatedAtAction(nameof(Get), new { propertyId, unitId, ratePlanId = response.Id }, response);
    }

    [HttpPut("{ratePlanId:long:min(1)}")]
    public async Task<ActionResult<RatePlanResponse>> Update(long propertyId, long unitId, long ratePlanId, RatePlanRequest request, CancellationToken ct) => Ok(await service.UpdateAsync(propertyId, unitId, ratePlanId, request, ct));
}
