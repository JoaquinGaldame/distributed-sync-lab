namespace OtaASimulator.Domain;

public sealed class UnitFacility
{
    public long UnitId { get; set; }
    public int UnitFacilityTypeId { get; set; }

    public Unit Unit { get; set; } = null!;
    public UnitFacilityType UnitFacilityType { get; set; } = null!;
}
