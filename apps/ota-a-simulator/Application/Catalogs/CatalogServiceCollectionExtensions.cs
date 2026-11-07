using OtaASimulator.Application.Catalogs.BathroomConfigurations;
using OtaASimulator.Application.Catalogs.CancellationPolicies;
using OtaASimulator.Application.Catalogs.Currencies;
using OtaASimulator.Application.Catalogs.Facilities;
using OtaASimulator.Application.Catalogs.Languages;
using OtaASimulator.Application.Catalogs.ProfileTypes;
using OtaASimulator.Application.Catalogs.PropertyTypes;
using OtaASimulator.Application.Catalogs.UnitFacilityTypes;
using OtaASimulator.Application.Catalogs.UnitTypes;

namespace OtaASimulator.Application.Catalogs;

public static class CatalogServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogQueries(this IServiceCollection services)
    {
        services.AddScoped<PropertyTypeCatalogQuery>();
        services.AddScoped<UnitTypeCatalogQuery>();
        services.AddScoped<FacilityCatalogQuery>();
        services.AddScoped<UnitFacilityTypeCatalogQuery>();
        services.AddScoped<BathroomConfigurationCatalogQuery>();
        services.AddScoped<LanguageCatalogQuery>();
        services.AddScoped<CurrencyCatalogQuery>();
        services.AddScoped<ProfileTypeCatalogQuery>();
        services.AddScoped<CancellationPolicyCatalogQuery>();

        return services;
    }
}
