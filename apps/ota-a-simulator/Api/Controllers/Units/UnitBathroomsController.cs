using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Units;
using OtaASimulator.Models.Units;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/units/{unitId:long:min(1)}/bathrooms")]
public sealed class UnitBathroomsController(BathroomService service) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<IReadOnlyList<BathroomResponse>>> Replace(long propertyId, long unitId, [FromBody, Required] List<BathroomRequest> request, CancellationToken ct) =>
        Ok(await service.ReplaceAsync(propertyId, unitId, request, ct));
}
