namespace AcademicPms.Domain;

public sealed class PropertyChange
{
    public long Sequence { get; set; }
    public Guid EventId { get; set; }
    public string PropertyId { get; set; } = "";
    public long Version { get; set; }
    public string EventType { get; set; } = "PropertyChanged";
    public DateTime OccurredAt { get; set; }

    // Estado de la propiedad correspondiente a ESTA versión.
    public string StateName { get; set; } = "";
    public decimal StatePrice { get; set; }
}