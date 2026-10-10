namespace OtaASimulator.Domain;

public sealed class RatePlan
{
    public long Id { get; set; }
    public long PropertyId { get; set; }
    public long UnitId { get; set; }
    public string ExternalCode { get; set; } = "";
    public string Name { get; set; } = "";
    public int CancellationPolicyId { get; set; }

    public int MinimumNights { get; set; } = 1;
    public int? MaximumNights { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Property Property { get; set; } = null!;
    public Unit Unit { get; set; } = null!;
    public CancellationPolicy CancellationPolicy { get; set; } = null!;
}
