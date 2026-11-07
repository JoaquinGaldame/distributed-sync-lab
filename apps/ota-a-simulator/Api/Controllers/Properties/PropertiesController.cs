using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Properties;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties")]
public sealed class PropertiesController(PropertyService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<PropertyResponse>>> List([FromQuery, Range(1, 1000000)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 50, CancellationToken ct = default) =>
        Ok(await service.ListAsync(page, pageSize, ct));

    [HttpGet("{propertyId:long:min(1)}")]
    public async Task<ActionResult<PropertyDetailResponse>> Get(long propertyId, CancellationToken ct) => Ok(await service.GetAsync(propertyId, ct));

    [HttpPost]
    public async Task<ActionResult<PropertyResponse>> Create(PropertyRequest request, CancellationToken ct)
    {
        var response = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { propertyId = response.Id }, response);
    }

    [HttpPut("{propertyId:long:min(1)}")]
    public async Task<ActionResult<PropertyResponse>> Update(long propertyId, PropertyRequest request, CancellationToken ct) => Ok(await service.UpdateAsync(propertyId, request, ct));
}
