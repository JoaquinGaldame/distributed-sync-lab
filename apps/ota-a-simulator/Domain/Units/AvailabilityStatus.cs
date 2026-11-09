namespace OtaASimulator.Domain;

public enum AvailabilityStatus : short
{
    Available = 0,
    Closed = 1,
    Reserved = 2
}

public static class AvailabilityStatusExtensions
{
    public static string ToApiStatus(this AvailabilityStatus status) => status switch
    {
        AvailabilityStatus.Available => "AVAILABLE",
        AvailabilityStatus.Closed => "CLOSED",
        AvailabilityStatus.Reserved => "RESERVED",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Estado de disponibilidad desconocido.")
    };
}
