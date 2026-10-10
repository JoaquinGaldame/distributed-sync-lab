namespace OtaASimulator.Domain;

public sealed class Unit
{
    public long Id { get; set; }
    public long PropertyId { get; set; }
    public string ExternalCode { get; set; } = "";
    public string Name { get; set; } = "";
    public int UnitTypeId { get; set; }
    public int TotalUnits { get; set; }
    public int MaximumOccupancy { get; set; }
    public int MaximumAdults { get; set; }
    public int MaximumChildren { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Property Property { get; set; } = null!;
    public UnitType UnitType { get; set; } = null!;
}
