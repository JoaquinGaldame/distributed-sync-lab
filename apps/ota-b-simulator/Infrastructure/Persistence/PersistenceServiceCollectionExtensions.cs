using Microsoft.EntityFrameworkCore;

namespace OtaBSimulator.Infrastructure.Persistence;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddOtaBPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OtaBDb");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Falta configurar ConnectionStrings__OtaBDb.");
        services.AddDbContext<OtaBDbContext>(options => options.UseNpgsql(connectionString));
        return services;
    }
}
