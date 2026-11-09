using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Properties;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/descriptions")]
public sealed class PropertyDescriptionsController(DescriptionService service) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DescriptionResponse>>>> Replace(long propertyId, [FromBody, Required] List<DescriptionRequest> request, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<DescriptionResponse>>.Success(
            await service.ReplaceAsync(propertyId, request, ct),
            "Descripciones de la propiedad actualizadas correctamente."));
}
