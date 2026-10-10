namespace OtaASimulator.Domain;

public sealed class CancellationPolicy
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public int? FreeUntilDaysBefore { get; set; }
    public decimal PenaltyPercent { get; set; }
}
