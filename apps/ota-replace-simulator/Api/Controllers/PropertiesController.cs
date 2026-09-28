using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtaReplaceSimulator.Api.Models;
using OtaReplaceSimulator.Api.Validation;
using OtaReplaceSimulator.Domain;
using OtaReplaceSimulator.Infrastructure.Persistence;

namespace OtaReplaceSimulator.Api.Controllers;

[ApiController]
[Route("properties")]
public sealed class PropertiesController(SimulatorDbContext db) : ControllerBase
{
    [HttpPut("{externalId}")]
    public async Task<ActionResult<ExternalPropertyResponse>> Replace(string externalId, [FromBody] ReplacePropertyRequest request, CancellationToken ct)
    {
        if (!ReplacePropertyValidator.IsValid(externalId, request))
            return BadRequest("ID externo o estado de propiedad inválido.");

        await db.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO external_properties (
                external_id, name, price, room_type, neighbourhood,
                latitude, longitude, minimum_nights, source_version
            )
            VALUES (
                {externalId}, {request.Name}, {request.Price},
                {request.RoomType}, {request.Neighbourhood},
                {request.Latitude}, {request.Longitude},
                {request.MinimumNights}, {request.SourceVersion}
            )
            ON CONFLICT (external_id) DO UPDATE SET
                name = EXCLUDED.name,
                price = EXCLUDED.price,
                room_type = EXCLUDED.room_type,
                neighbourhood = EXCLUDED.neighbourhood,
                latitude = EXCLUDED.latitude,
                longitude = EXCLUDED.longitude,
                minimum_nights = EXCLUDED.minimum_nights,
                source_version = EXCLUDED.source_version
            ", ct);

        var applied = new ExternalProperty
        {
            ExternalId = externalId,
            Name = request.Name,
            Price = request.Price,
            RoomType = request.RoomType,
            Neighbourhood = request.Neighbourhood,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            MinimumNights = request.MinimumNights,
            SourceVersion = request.SourceVersion
        };

        return Ok(ExternalPropertyResponse.From(applied));
    }

    [HttpGet("{externalId}")]
    public async Task<ActionResult<ExternalPropertyResponse>> GetById(string externalId, CancellationToken ct)
    {
        var property = await db.ExternalProperties
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ExternalId == externalId, ct);

        return property is null
            ? NotFound()
            : Ok(ExternalPropertyResponse.From(property));
    }

    [HttpGet]
    public async Task<ActionResult<ExternalPropertyPageResponse>> GetPage([FromQuery] int offset = 0, [FromQuery] int limit = 100, CancellationToken ct = default)
    {
        if (offset < 0 || limit is < 1 or > 500 || offset > int.MaxValue - limit)
            return BadRequest("offset o limit inválidos.");

        var rows = await db.ExternalProperties
            .AsNoTracking()
            .OrderBy(x => x.ExternalId)
            .Skip(offset)
            .Take(limit + 1)
            .ToListAsync(ct);

        var hasMore = rows.Count > limit;
        var items = rows
            .Take(limit)
            .Select(ExternalPropertyResponse.From)
            .ToList();

        return Ok(new ExternalPropertyPageResponse(
            items,
            hasMore ? offset + limit : null,
            hasMore));
    }
}