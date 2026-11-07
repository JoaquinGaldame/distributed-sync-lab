using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Properties;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/stay-policy")]
public sealed class PropertyStayPolicyController(StayPolicyService service) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<StayPolicyResponse>> Replace(long propertyId, [FromBody, Required] StayPolicyRequest request, CancellationToken ct) =>
        Ok(await service.ReplaceAsync(propertyId, request, ct));
}
