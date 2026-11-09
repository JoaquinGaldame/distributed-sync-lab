using OtaBSimulator.Application.Calendar;
using OtaBSimulator.Application.Common;
using OtaBSimulator.Application.Health;
using OtaBSimulator.Application.Listings;
using OtaBSimulator.Application.Metadata;
using OtaBSimulator.Application.Rooms;

namespace OtaBSimulator.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddOtaBApplication(this IServiceCollection services)
    {
        services.AddScoped<ResourceGuard>();
        services.AddScoped<CatalogResolver>();
        services.AddScoped<ListingService>();
        services.AddScoped<ListingSectionsService>();
        services.AddScoped<RoomService>();
        services.AddScoped<CalendarService>();
        services.AddScoped<MetadataService>();
        services.AddScoped<HealthService>();
        return services;
    }
}
