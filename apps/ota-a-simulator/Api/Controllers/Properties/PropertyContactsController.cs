using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Properties;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Controllers;

[ApiController]
[Route("properties/{propertyId:long:min(1)}/contacts")]
public sealed class PropertyContactsController(ContactService service) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<IReadOnlyList<ContactResponse>>> Replace(long propertyId, [FromBody, Required] List<ContactRequest> request, CancellationToken ct) =>
        Ok(await service.ReplaceAsync(propertyId, request, ct));
}
