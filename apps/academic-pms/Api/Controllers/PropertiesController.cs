using AcademicPms.Api.Mappers;
using AcademicPms.Api.Models;
using AcademicPms.Api.Validation;
using AcademicPms.Application;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicPms.Api.Controllers;

[ApiController]
[Route("properties")]
public sealed class PropertiesController( PropertyService service, PmsDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PropertyResponse>> Create([FromBody] PropertyInput input, CancellationToken ct)
    {
        if (!PropertyRequestValidator.ValidId(input.Id)
            || !PropertyRequestValidator.ValidSourceListingId(
                input.SourceListingId))
        {
            return BadRequest("Id o sourceListingId inválidos.");
        }

        var state = PropertyMapper.ToState(input);

        if (!PropertyRequestValidator.ValidState(state))
            return BadRequest("Estado de propiedad inválido.");

        var property = await service.CreateAsync(
            input.Id,
            input.SourceListingId,
            state,
            ct);

        if (property is null)
            return Conflict("La propiedad ya existe.");

        return CreatedAtAction(
            nameof(GetById),
            new { id = property.Id },
            PropertyMapper.ToResponse(property));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PropertyResponse>> GetById(string id, CancellationToken ct)
    {
        var property = await db.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (property is null)
            return NotFound();

        return Ok(PropertyMapper.ToResponse(property));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PropertyResponse>> Update(string id, [FromBody] PropertyUpdate input, CancellationToken ct)
    {
        var state = PropertyMapper.ToState(input);

        if (!PropertyRequestValidator.ValidState(state))
            return BadRequest("Estado de propiedad inválido.");

        try
        {
            var property = await service.UpdateAsync(id, state, ct);

            if (property is null)
                return NotFound();

            return Ok(PropertyMapper.ToResponse(property));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(
                "La propiedad cambió durante la actualización.");
        }
    }
}