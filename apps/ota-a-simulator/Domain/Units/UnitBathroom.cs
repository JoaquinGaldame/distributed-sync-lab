namespace OtaASimulator.Domain;

public sealed class UnitBathroom
{
    public long Id { get; set; }
    public long UnitId { get; set; }
    public int BathroomConfigurationId { get; set; }
    public string Scope { get; set; } = "";
    public int Quantity { get; set; } = 1;

    public Unit Unit { get; set; } = null!;
    public BathroomConfiguration BathroomConfiguration { get; set; } = null!;
}
