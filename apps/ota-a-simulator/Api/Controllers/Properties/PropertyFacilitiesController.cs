using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Properties;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/facilities")]
public sealed class PropertyFacilitiesController(PropertyFacilityService service) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PropertyFacilityResponse>>>> Configure(
        long propertyId,
        [FromBody, Required] List<PropertyFacilityRequest> request,
        CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<PropertyFacilityResponse>>.Success(
            await service.ConfigureAsync(propertyId, request, ct),
            "Facilities de la propiedad actualizadas correctamente."));
}
