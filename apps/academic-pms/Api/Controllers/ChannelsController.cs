using AcademicPms.Api.Models;
using AcademicPms.Api.Validation;
using AcademicPms.Application;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicPms.Api.Controllers;

[ApiController]
[Route("channels")]
public sealed class ChannelsController(
    ChannelService service,
    PmsDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ChannelResponse>>> GetAll(CancellationToken ct)
    {
        var channels = await db.Channels
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(ct);

        return Ok(channels.Select(ChannelResponse.From));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ChannelResponse>> GetById(long id, CancellationToken ct)
    {
        var channel = await db.Channels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return channel is null
            ? NotFound()
            : Ok(ChannelResponse.From(channel));
    }

    [HttpPost]
    public async Task<ActionResult<ChannelResponse>> Create([FromBody] ChannelInput input, CancellationToken ct)
    {
        var code = input.Code?.Trim();
        var name = input.Name?.Trim();

        if (!ChannelRequestValidator.ValidCode(code)
            || !ChannelRequestValidator.ValidName(name))
        {
            return BadRequest("Código o nombre de canal inválidos.");
        }

        try
        {
            var channel = await service.CreateAsync(
                code!, name!, input.IsEnabled, ct);

            if (channel is null)
                return Conflict("Ya existe un canal con ese código.");

            return CreatedAtAction(
                nameof(GetById),
                new { id = channel.Id },
                ChannelResponse.From(channel));
        }
        catch (DbUpdateException)
        {
            return Conflict("Ya existe un canal con ese código.");
        }
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ChannelResponse>> Update(long id, [FromBody] ChannelUpdate input, CancellationToken ct)
    {
        var name = input.Name?.Trim();

        if (id <= 0 || !ChannelRequestValidator.ValidName(name))
            return BadRequest("Id o nombre de canal inválidos.");

        var channel = await service.UpdateAsync(
            id, name!, input.IsEnabled, ct);

        return channel is null
            ? NotFound()
            : Ok(ChannelResponse.From(channel));
    }
}
