using AcademicPms.Api.Models;
using AcademicPms.Api.Validation;
using AcademicPms.Application;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicPms.Api.Controllers;

[ApiController]
[Route("properties/{propertyId}/publications")]
public sealed class PublicationsController(
    PublicationService service,
    PmsDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PublicationResponse>>> GetAll(string propertyId, CancellationToken ct)
    {
        if (!PropertyRequestValidator.ValidId(propertyId))
            return BadRequest("PropertyId inválido.");

        if (!await db.Properties.AnyAsync(x => x.Id == propertyId, ct))
            return NotFound("La propiedad no existe.");

        var publications = await db.Publications
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId)
            .OrderBy(x => x.ChannelId)
            .ToListAsync(ct);

        return Ok(publications.Select(PublicationResponse.From));
    }

    [HttpGet("{channelId:long}")]
    public async Task<ActionResult<PublicationResponse>> GetByChannel(string propertyId, long channelId, CancellationToken ct)
    {
        if (!PropertyRequestValidator.ValidId(propertyId)
            || channelId <= 0)
        {
            return BadRequest("PropertyId o ChannelId inválidos.");
        }

        var publication = await db.Publications
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.PropertyId == propertyId
                    && x.ChannelId == channelId,
                ct);

        return publication is null
            ? NotFound()
            : Ok(PublicationResponse.From(publication));
    }

    [HttpPut("{channelId:long}")]
    public async Task<ActionResult<PublicationResponse>> Upsert(string propertyId, long channelId, [FromBody] PublicationInput input, CancellationToken ct)
    {
        var externalPropertyId = input.ExternalPropertyId?.Trim();

        if (!PropertyRequestValidator.ValidId(propertyId)
            || channelId <= 0
            || !PublicationRequestValidator.ValidExternalPropertyId(
                externalPropertyId))
        {
            return BadRequest("PropertyId, ChannelId o ExternalPropertyId inválidos.");
        }

        try
        {
            var result = await service.UpsertAsync(
                propertyId,
                channelId,
                externalPropertyId,
                input.Published,
                ct);

            if (result.Status == PublicationUpsertStatus.PropertyNotFound)
                return NotFound("La propiedad no existe.");

            if (result.Status == PublicationUpsertStatus.ChannelNotFound)
                return NotFound("El canal no existe.");

            var response = PublicationResponse.From(result.Publication!);

            if (result.Status == PublicationUpsertStatus.Created)
            {
                return CreatedAtAction(
                    nameof(GetByChannel),
                    new { propertyId, channelId },
                    response);
            }

            return Ok(response);
        }
        catch (DbUpdateException)
        {
            return Conflict(
                "El identificador externo ya está asociado a otra propiedad del canal.");
        }
    }
}
