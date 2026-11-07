using Microsoft.AspNetCore.Mvc;
using OtaASimulator.Application.Catalogs.BathroomConfigurations;
using OtaASimulator.Application.Catalogs.CancellationPolicies;
using OtaASimulator.Application.Catalogs.Currencies;
using OtaASimulator.Application.Catalogs.Facilities;
using OtaASimulator.Application.Catalogs.Languages;
using OtaASimulator.Application.Catalogs.Models;
using OtaASimulator.Application.Catalogs.ProfileTypes;
using OtaASimulator.Application.Catalogs.PropertyTypes;
using OtaASimulator.Application.Catalogs.UnitFacilityTypes;
using OtaASimulator.Application.Catalogs.UnitTypes;
using OtaASimulator.Models.Common;

namespace OtaASimulator.Controllers;

[ApiController]
[Route("catalogs")]
public sealed class CatalogsController(
    PropertyTypeCatalogQuery propertyTypes,
    UnitTypeCatalogQuery unitTypes,
    FacilityCatalogQuery facilities,
    UnitFacilityTypeCatalogQuery unitFacilityTypes,
    BathroomConfigurationCatalogQuery bathroomConfigurations,
    LanguageCatalogQuery languages,
    CurrencyCatalogQuery currencies,
    ProfileTypeCatalogQuery profileTypes,
    CancellationPolicyCatalogQuery cancellationPolicies) : ControllerBase
{
    [HttpGet("property-types")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CatalogItemResult>>>> GetPropertyTypes(CancellationToken ct) =>
        CatalogResponse(await propertyTypes.ExecuteAsync(ct), "Tipos de propiedad obtenidos correctamente.");

    [HttpGet("unit-types")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CatalogItemResult>>>> GetUnitTypes(CancellationToken ct) =>
        CatalogResponse(await unitTypes.ExecuteAsync(ct), "Tipos de unidad obtenidos correctamente.");

    [HttpGet("facilities")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NamedCatalogItemResult>>>> GetFacilities(CancellationToken ct) =>
        CatalogResponse(await facilities.ExecuteAsync(ct), "Instalaciones de propiedad obtenidas correctamente.");

    [HttpGet("unit-facility-types")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NamedCatalogItemResult>>>> GetUnitFacilityTypes(CancellationToken ct) =>
        CatalogResponse(await unitFacilityTypes.ExecuteAsync(ct), "Instalaciones de unidad obtenidas correctamente.");

    [HttpGet("bathroom-configurations")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CatalogItemResult>>>> GetBathroomConfigurations(CancellationToken ct) =>
        CatalogResponse(await bathroomConfigurations.ExecuteAsync(ct), "Configuraciones de baño obtenidas correctamente.");

    [HttpGet("languages")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NamedCatalogItemResult>>>> GetLanguages(CancellationToken ct) =>
        CatalogResponse(await languages.ExecuteAsync(ct), "Idiomas obtenidos correctamente.");

    [HttpGet("currencies")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CatalogItemResult>>>> GetCurrencies(CancellationToken ct) =>
        CatalogResponse(await currencies.ExecuteAsync(ct), "Monedas obtenidas correctamente.");

    [HttpGet("profile-types")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProfileTypeResult>>>> GetProfileTypes(CancellationToken ct) =>
        CatalogResponse(await profileTypes.ExecuteAsync(ct), "Tipos de perfil obtenidos correctamente.");

    [HttpGet("cancellation-policies")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CancellationPolicyResult>>>> GetCancellationPolicies(CancellationToken ct) =>
        CatalogResponse(await cancellationPolicies.ExecuteAsync(ct), "Políticas de cancelación obtenidas correctamente.");

    private ActionResult<ApiResponse<T>> CatalogResponse<T>(T data, string message) =>
        Ok(ApiResponse<T>.Success(data, message));
}
