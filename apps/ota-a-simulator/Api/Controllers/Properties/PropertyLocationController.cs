using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Properties;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/location")]
public sealed class PropertyLocationController(LocationService service) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<LocationResponse>>> Replace(long propertyId, [FromBody, Required] LocationRequest request, CancellationToken ct) =>
        Ok(ApiResponse<LocationResponse>.Success(
            await service.ReplaceAsync(propertyId, request, ct),
            "Ubicación de la propiedad actualizada correctamente."));
}
