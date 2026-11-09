using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Properties;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/contacts")]
public sealed class PropertyContactsController(ContactService service) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ContactResponse>>>> Replace(long propertyId, [FromBody, Required] List<ContactRequest> request, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<ContactResponse>>.Success(
            await service.ReplaceAsync(propertyId, request, ct),
            "Contactos de la propiedad actualizados correctamente."));
}
