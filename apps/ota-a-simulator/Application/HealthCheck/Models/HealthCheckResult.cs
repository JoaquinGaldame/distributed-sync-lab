namespace OtaASimulator.Application.HealthCheck.Models;

public sealed record HealthCheckResult(
    string Status,
    DateTimeOffset CheckedAtUtc,
    IReadOnlyList<HealthCheckComponentResult> Checks);
