namespace OtaASimulator.Application.HealthCheck.Models;

public sealed record HealthCheckComponentResult(
    string Name,
    string Status);
