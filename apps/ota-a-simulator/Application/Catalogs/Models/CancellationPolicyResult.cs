namespace OtaASimulator.Application.Catalogs.Models;

public sealed record CancellationPolicyResult(
    int Id,
    string Code,
    string Name,
    int? FreeUntilDaysBefore,
    decimal PenaltyPercent);
