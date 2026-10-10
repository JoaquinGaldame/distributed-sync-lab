namespace OtaASimulator.Domain;

public sealed class StayPolicy
{
    public long PropertyId { get; set; }
    public TimeOnly CheckInFrom { get; set; }
    public TimeOnly CheckInUntil { get; set; }
    public TimeOnly CheckOutFrom { get; set; }
    public TimeOnly CheckOutUntil { get; set; }
    public int MinimumGuestAge { get; set; } = 18;
    public bool ChildrenAllowed { get; set; } = true;
    public bool PetsAllowed { get; set; } = false;
    public bool SmokingAllowed { get; set; } = false;
    public bool PartiesAllowed { get; set; } = false;
    public DateTime UpdatedAt { get; set; }

    public Property Property { get; set; } = null!;
}
