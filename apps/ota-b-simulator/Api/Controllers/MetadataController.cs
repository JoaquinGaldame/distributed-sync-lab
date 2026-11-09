using Microsoft.AspNetCore.Mvc;
using OtaBSimulator.Api.Models.Common;
using OtaBSimulator.Api.Models.Metadata;
using OtaBSimulator.Application.Metadata;

namespace OtaBSimulator.Api.Controllers;

[ApiController, Route("metadata")]
public sealed class MetadataController(MetadataService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<MetadataResponse>>> Get(CancellationToken ct) =>
        Ok(ApiResponse<MetadataResponse>.Success(await service.Get(ct), "Metadatos obtenidos correctamente."));
}
