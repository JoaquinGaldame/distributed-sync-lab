using Microsoft.AspNetCore.Mvc;
using OtaBSimulator.Api.Models.Common;
using OtaBSimulator.Api.Models.Rooms;
using OtaBSimulator.Application.Rooms;

namespace OtaBSimulator.Api.Controllers;

[ApiController, Route("listings/{listingId:guid}/rooms")]
public sealed class RoomsController(RoomService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RoomResponse>>>> List(Guid listingId, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<RoomResponse>>.Success(await service.List(listingId, ct), "Espacios obtenidos correctamente."));

    [HttpGet("{roomId:guid}")]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> Get(Guid listingId, Guid roomId, CancellationToken ct) =>
        Ok(ApiResponse<RoomResponse>.Success(await service.Get(listingId, roomId, ct), "Espacio obtenido correctamente."));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> Create(Guid listingId, RoomRequest request, CancellationToken ct)
    {
        var response = await service.Create(listingId, request, ct);
        return CreatedAtAction(nameof(Get), new { listingId, roomId = response.Id }, ApiResponse<RoomResponse>.Success(response, "Espacio creado correctamente."));
    }

    [HttpPut("{roomId:guid}")]
    public async Task<ActionResult<ApiResponse<RoomResponse>>> Update(Guid listingId, Guid roomId, RoomRequest request, CancellationToken ct) =>
        Ok(ApiResponse<RoomResponse>.Success(await service.Update(listingId, roomId, request, ct), "Espacio actualizado correctamente."));

    [HttpDelete("{roomId:guid}")]
    public async Task<ActionResult<ApiResponse<DeletedResourceResponse>>> Delete(Guid listingId, Guid roomId, CancellationToken ct)
    {
        await service.Delete(listingId, roomId, ct);
        return Ok(ApiResponse<DeletedResourceResponse>.Success(new(true, "room", roomId), "Espacio eliminado correctamente."));
    }

    [HttpPut("{roomId:guid}/beds")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BedResponse>>>> Beds(Guid listingId, Guid roomId, IReadOnlyList<BedRequest> request, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<BedResponse>>.Success(await service.PutBeds(listingId, roomId, request, ct), "Camas reemplazadas correctamente."));
}
