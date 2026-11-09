using OtaASimulator.Application.Availability;
using OtaASimulator.Application.Catalogs;
using OtaASimulator.Application.Common;
using OtaASimulator.Application.HealthCheck;
using OtaASimulator.Application.Properties;
using OtaASimulator.Application.RatePlans;
using OtaASimulator.Application.Units;

namespace OtaASimulator.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddOtaAApplication(this IServiceCollection services)
    {
        services.AddScoped<ResourceGuard>();
        services.AddCatalogQueries();
        services.AddHealthCheckServices();
        services.AddScoped<PropertyService>();
        services.AddScoped<LocationService>();
        services.AddScoped<ContactService>();
        services.AddScoped<DescriptionService>();
        services.AddScoped<PropertyFacilityService>();
        services.AddScoped<StayPolicyService>();
        services.AddScoped<ChargeService>();
        services.AddScoped<UnitService>();
        services.AddScoped<UnitFacilityService>();
        services.AddScoped<BathroomService>();
        services.AddScoped<RatePlanService>();
        services.AddScoped<AvailabilityService>();

        return services;
    }
}
