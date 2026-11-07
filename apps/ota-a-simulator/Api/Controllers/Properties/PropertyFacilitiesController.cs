using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Properties;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/facilities")]
public sealed class PropertyFacilitiesController(PropertyFacilityService service) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<IReadOnlyList<string>>> Replace(long propertyId, [FromBody, Required] List<string> request, CancellationToken ct) =>
        Ok(await service.ReplaceAsync(propertyId, request, ct));
}
