namespace OtaASimulator.Domain;

public sealed class UnitInventory
{
    public long UnitId { get; set; }
    public DateOnly StayDate { get; set; }
    public int AvailableUnits { get; set; }
    public bool Closed { get; set; } = false;
    public DateTime UpdatedAt { get; set; }

    public Unit Unit { get; set; } = null!;
}
