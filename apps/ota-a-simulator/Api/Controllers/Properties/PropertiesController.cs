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
    public async Task<ActionResult<ApiResponse<PagedResponse<PropertyResponse>>>> List([FromQuery, Range(1, 1000000)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 50, CancellationToken ct = default) =>
        Ok(ApiResponse<PagedResponse<PropertyResponse>>.Success(
            await service.ListAsync(page, pageSize, ct),
            "Propiedades obtenidas correctamente."));

    [HttpGet("{propertyId:long:min(1)}")]
    public async Task<ActionResult<ApiResponse<PropertyDetailResponse>>> Get(long propertyId, CancellationToken ct) =>
        Ok(ApiResponse<PropertyDetailResponse>.Success(
            await service.GetAsync(propertyId, ct),
            "Propiedad obtenida correctamente."));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PropertyResponse>>> Create(PropertyRequest request, CancellationToken ct)
    {
        var response = await service.CreateAsync(request, ct);
        return CreatedAtAction(
            nameof(Get),
            new { propertyId = response.Id },
            ApiResponse<PropertyResponse>.Success(response, "Propiedad creada correctamente."));
    }

    [HttpPut("{propertyId:long:min(1)}")]
    public async Task<ActionResult<ApiResponse<PropertyResponse>>> Update(long propertyId, PropertyRequest request, CancellationToken ct) =>
        Ok(ApiResponse<PropertyResponse>.Success(
            await service.UpdateAsync(propertyId, request, ct),
            "Propiedad actualizada correctamente."));

    [HttpDelete("{propertyId:long:min(1)}")]
    public async Task<ActionResult<ApiResponse<DeletedResourceResponse>>> Delete(long propertyId, CancellationToken ct)
    {
        await service.DeleteAsync(propertyId, ct);
        return Ok(ApiResponse<DeletedResourceResponse>.Success(
            new DeletedResourceResponse(true, "property", propertyId),
            "Propiedad eliminada correctamente."));
    }
}
