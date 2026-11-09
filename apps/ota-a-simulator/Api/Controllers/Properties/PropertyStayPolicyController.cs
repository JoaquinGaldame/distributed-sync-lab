using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Properties;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/stay-policy")]
public sealed class PropertyStayPolicyController(StayPolicyService service) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<StayPolicyResponse>>> Replace(long propertyId, [FromBody, Required] StayPolicyRequest request, CancellationToken ct) =>
        Ok(ApiResponse<StayPolicyResponse>.Success(
            await service.ReplaceAsync(propertyId, request, ct),
            "Política de estadía actualizada correctamente."));
}
