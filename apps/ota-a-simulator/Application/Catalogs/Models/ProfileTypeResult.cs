namespace OtaASimulator.Application.Catalogs.Models;

public sealed record ProfileTypeResult(
    int Id,
    string Code,
    string Name,
    bool Required);
