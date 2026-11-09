using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Properties;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/charges")]
public sealed class PropertyChargesController(ChargeService service) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ChargeResponse>>>> Replace(long propertyId, [FromBody, Required] List<ChargeRequest> request, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<ChargeResponse>>.Success(
            await service.ReplaceAsync(propertyId, request, ct),
            "Cargos de la propiedad actualizados correctamente."));
}
