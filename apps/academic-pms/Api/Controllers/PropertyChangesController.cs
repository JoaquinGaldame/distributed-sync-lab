using AcademicPms.Api.Mappers;
using AcademicPms.Api.Models;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicPms.Api.Controllers;

[ApiController]
[Route("changes")]
public sealed class PropertyChangesController(PmsDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PropertyChangePageResponse>> Get([FromQuery] long? after, [FromQuery] int? limit, CancellationToken ct)
    {
        if (after is < 0 || limit is < 1 or > 100)
        {
            return BadRequest(
                "after debe ser >= 0 y limit debe estar entre 1 y 100.");
        }

        var cursor = after ?? 0;
        var pageSize = limit ?? 50;

        var rows = await db.PropertyChanges
            .AsNoTracking()
            .Where(x => x.Sequence > cursor)
            .OrderBy(x => x.Sequence)
            .Take(pageSize + 1)
            .ToListAsync(ct);

        var page = rows
            .Take(pageSize)
            .Select(PropertyMapper.ToResponse)
            .ToArray();

        return Ok(new PropertyChangePageResponse(
            page,
            page.Length == 0 ? cursor : page[^1].Sequence,
            rows.Count > pageSize));
    }
}