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
    public async Task<ActionResult<PagedResponse<UnitResponse>>> List(long propertyId, [FromQuery, Range(1, 1000000)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 50, CancellationToken ct = default) =>
        Ok(await service.ListAsync(propertyId, page, pageSize, ct));

    [HttpGet("{unitId:long:min(1)}")]
    public async Task<ActionResult<UnitDetailResponse>> Get(long propertyId, long unitId, CancellationToken ct) => Ok(await service.GetAsync(propertyId, unitId, ct));

    [HttpPost]
    public async Task<ActionResult<UnitResponse>> Create(long propertyId, UnitRequest request, CancellationToken ct)
    {
        var response = await service.CreateAsync(propertyId, request, ct);
        return CreatedAtAction(nameof(Get), new { propertyId, unitId = response.Id }, response);
    }

    [HttpPut("{unitId:long:min(1)}")]
    public async Task<ActionResult<UnitResponse>> Update(long propertyId, long unitId, UnitRequest request, CancellationToken ct) => Ok(await service.UpdateAsync(propertyId, unitId, request, ct));
}
