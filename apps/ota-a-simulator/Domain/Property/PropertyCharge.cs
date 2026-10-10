namespace OtaASimulator.Domain;

public sealed class PropertyCharge
{
    public long Id { get; set; }
    public long PropertyId { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Calculation { get; set; } = "";
    public decimal Amount { get; set; }
    public bool IncludedInPrice { get; set; } = false;

    public Property Property { get; set; } = null!;
}
