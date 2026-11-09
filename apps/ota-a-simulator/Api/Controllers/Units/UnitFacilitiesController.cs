using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Units;
using OtaASimulator.Models.Units;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/units/{unitId:long:min(1)}/facilities")]
public sealed class UnitFacilitiesController(UnitFacilityService service) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<IReadOnlyList<UnitFacilityResponse>>> Configure(
        long propertyId,
        long unitId,
        [FromBody, Required] List<UnitFacilityRequest> request,
        CancellationToken ct) =>
        Ok(await service.ConfigureAsync(propertyId, unitId, request, ct));
}
