using Microsoft.EntityFrameworkCore;
using OtaBSimulator.Infrastructure.Persistence;

namespace OtaBSimulator.Application.Health;

public sealed record HealthComponent(string Name, string Status);
public sealed record HealthResult(string Status, DateTimeOffset CheckedAt, IReadOnlyList<HealthComponent> Checks);

public sealed class HealthService(OtaBDbContext db, ILogger<HealthService> logger)
{
    public HealthResult Live() => new("healthy", DateTimeOffset.UtcNow, [new("api", "healthy")]);
    public async Task<HealthResult> Ready(CancellationToken ct)
    {
        try
        {
            var status = await db.Database.CanConnectAsync(ct) ? "healthy" : "unhealthy";
            return new(status, DateTimeOffset.UtcNow, [new("api", "healthy"), new("database", status)]);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falló la verificación de readiness de PostgreSQL.");
            return new("unhealthy", DateTimeOffset.UtcNow, [new("api", "healthy"), new("database", "unhealthy")]);
        }
    }
}
