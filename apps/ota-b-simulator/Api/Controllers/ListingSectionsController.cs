using Microsoft.AspNetCore.Mvc;
using OtaBSimulator.Api.Models.Common;
using OtaBSimulator.Api.Models.Listings;
using OtaBSimulator.Application.Listings;

namespace OtaBSimulator.Api.Controllers;

[ApiController, Route("listings/{listingId:guid}")]
public sealed class ListingSectionsController(ListingSectionsService service) : ControllerBase
{
    [HttpPut("location")]
    public async Task<ActionResult<ApiResponse<LocationResponse>>> Location(Guid listingId, LocationRequest request, CancellationToken ct) =>
        Ok(ApiResponse<LocationResponse>.Success(await service.PutLocation(listingId, request, ct), "Ubicación actualizada correctamente."));

    [HttpPut("contacts")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ContactResponse>>>> Contacts(Guid listingId, IReadOnlyList<ContactRequest> request, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<ContactResponse>>.Success(await service.PutContacts(listingId, request, ct), "Contactos reemplazados correctamente."));

    [HttpPut("texts")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TextResponse>>>> Texts(Guid listingId, IReadOnlyList<TextRequest> request, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<TextResponse>>.Success(await service.PutTexts(listingId, request, ct), "Textos reemplazados correctamente."));

    [HttpPut("amenities")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<string>>>> Amenities(Guid listingId, IReadOnlyList<string> request, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<string>>.Success(await service.PutAmenities(listingId, request, ct), "Amenities reemplazados correctamente."));

    [HttpPut("house-rules")]
    public async Task<ActionResult<ApiResponse<HouseRulesResponse>>> HouseRules(Guid listingId, HouseRulesRequest request, CancellationToken ct) =>
        Ok(ApiResponse<HouseRulesResponse>.Success(await service.PutHouseRules(listingId, request, ct), "Reglas de estancia actualizadas correctamente."));

    [HttpPut("booking-rules")]
    public async Task<ActionResult<ApiResponse<BookingRulesResponse>>> BookingRules(Guid listingId, BookingRulesRequest request, CancellationToken ct) =>
        Ok(ApiResponse<BookingRulesResponse>.Success(await service.PutBookingRules(listingId, request, ct), "Condiciones comerciales actualizadas correctamente."));

    [HttpPut("base-rate")]
    public async Task<ActionResult<ApiResponse<PricingResponse>>> BaseRate(Guid listingId, PricingRequest request, CancellationToken ct) =>
        Ok(ApiResponse<PricingResponse>.Success(await service.PutPricing(listingId, request, ct), "Tarifa base actualizada correctamente."));

    [HttpPut("fees")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<FeeResponse>>>> Fees(Guid listingId, IReadOnlyList<FeeRequest> request, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<FeeResponse>>.Success(await service.PutFees(listingId, request, ct), "Cargos reemplazados correctamente."));
}
