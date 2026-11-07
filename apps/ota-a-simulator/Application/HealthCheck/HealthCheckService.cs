using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.HealthCheck.Models;
using OtaASimulator.Infrastructure.Persistence;

namespace OtaASimulator.Application.HealthCheck;

public sealed class HealthCheckService(
    OtaADbContext db,
    ILogger<HealthCheckService> logger)
{
    private const string Healthy = "healthy";
    private const string Unhealthy = "unhealthy";

    public HealthCheckResult CheckLiveness() =>
        CreateResult(Healthy, new HealthCheckComponentResult("api", Healthy));

    public async Task<HealthCheckResult> CheckReadinessAsync(CancellationToken ct)
    {
        try
        {
            var canConnect = await db.Database.CanConnectAsync(ct);
            var databaseStatus = canConnect ? Healthy : Unhealthy;

            return CreateResult(
                databaseStatus,
                new HealthCheckComponentResult("api", Healthy),
                new HealthCheckComponentResult("database", databaseStatus));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Falló la verificación de readiness de PostgreSQL.");

            return CreateResult(
                Unhealthy,
                new HealthCheckComponentResult("api", Healthy),
                new HealthCheckComponentResult("database", Unhealthy));
        }
    }

    private static HealthCheckResult CreateResult(
        string status,
        params HealthCheckComponentResult[] checks) =>
        new(status, DateTimeOffset.UtcNow, checks);
}
