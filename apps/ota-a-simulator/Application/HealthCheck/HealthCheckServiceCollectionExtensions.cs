namespace OtaASimulator.Application.HealthCheck;

public static class HealthCheckServiceCollectionExtensions
{
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
    {
        services.AddScoped<HealthCheckService>();

        return services;
    }
}
