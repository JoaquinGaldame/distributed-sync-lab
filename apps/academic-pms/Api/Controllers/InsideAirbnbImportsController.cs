using AcademicPms.Api.Mappers;
using AcademicPms.Api.Models;
using AcademicPms.Application.Imports;
using Microsoft.AspNetCore.Mvc;

namespace AcademicPms.Api.Controllers;

[ApiController]
[Route("imports/inside-airbnb")]
public sealed class InsideAirbnbImportsController(
    InsideAirbnbImportService service,
    InsideAirbnbCalendarImportService calendarService) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult<InsideAirbnbImportResponse>> Import(
        [FromForm] IFormFile? file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Enviá el CSV en el campo form-data 'file'.");

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await service.ImportAsync(stream, ct);

            return Ok(InsideAirbnbImportMapper.ToResponse(result));
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("calendar")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(500_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
    public async Task<ActionResult<InsideAirbnbCalendarImportResponse>> ImportCalendar(
        [FromForm] IFormFile? file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Enviá calendar.csv en el campo form-data 'file'.");

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await calendarService.ImportAsync(stream, ct);

            return Ok(InsideAirbnbImportMapper.ToResponse(result));
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
