using Microsoft.EntityFrameworkCore;

namespace OtaASimulator.Infrastructure.Persistence;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddOtaAPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OtaADb");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Falta configurar ConnectionStrings__OtaADb.");

        services.AddDbContext<OtaADbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
