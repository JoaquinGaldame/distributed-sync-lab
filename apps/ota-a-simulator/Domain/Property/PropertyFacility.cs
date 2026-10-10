namespace OtaASimulator.Domain;

public sealed class PropertyFacility
{
    public long PropertyId { get; set; }
    public string FacilityCode { get; set; } = "";

    public Property Property { get; set; } = null!;
    public Facility Facility { get; set; } = null!;
}
