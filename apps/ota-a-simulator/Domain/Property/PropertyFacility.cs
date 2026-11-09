namespace OtaASimulator.Domain;

public sealed class PropertyFacility
{
    public long PropertyId { get; set; }
    public int FacilityId { get; set; }

    public Property Property { get; set; } = null!;
    public Facility Facility { get; set; } = null!;
}
