using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OtaBSimulator.Api.Models.Common;
using OtaBSimulator.Api.Models.Listings;
using OtaBSimulator.Application.Listings;

namespace OtaBSimulator.Api.Controllers;

[ApiController, Route("listings")]
public sealed class ListingsController(ListingService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<ListingResponse>>>> List(
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 50, CancellationToken ct = default) =>
        Ok(ApiResponse<PagedResponse<ListingResponse>>.Success(await service.ListAsync(page, pageSize, ct), "Anuncios obtenidos correctamente."));

    [HttpGet("{listingId:guid}")]
    public async Task<ActionResult<ApiResponse<ListingDetailResponse>>> Get(Guid listingId, CancellationToken ct) =>
        Ok(ApiResponse<ListingDetailResponse>.Success(await service.GetAsync(listingId, ct), "Anuncio obtenido correctamente."));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ListingResponse>>> Create(ListingRequest request, CancellationToken ct)
    {
        var response = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { listingId = response.Id }, ApiResponse<ListingResponse>.Success(response, "Anuncio creado correctamente."));
    }

    [HttpPut("{listingId:guid}")]
    public async Task<ActionResult<ApiResponse<ListingResponse>>> Update(Guid listingId, ListingRequest request, CancellationToken ct) =>
        Ok(ApiResponse<ListingResponse>.Success(await service.UpdateAsync(listingId, request, ct), "Anuncio actualizado correctamente."));

    [HttpDelete("{listingId:guid}")]
    public async Task<ActionResult<ApiResponse<DeletedResourceResponse>>> Delete(Guid listingId, CancellationToken ct)
    {
        await service.DeleteAsync(listingId, ct);
        return Ok(ApiResponse<DeletedResourceResponse>.Success(new(true, "listing", listingId), "Anuncio eliminado correctamente."));
    }
}
