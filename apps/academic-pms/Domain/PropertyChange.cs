namespace AcademicPms.Domain;

public sealed class PropertyChange
{
    public long Sequence { get; set; }
    public Guid EventId { get; set; }
    public string PropertyId { get; set; } = "";
    public string EventType { get; set; } = "PropertyChanged";
    public DateTime OccurredAt { get; set; }

    // Estado de la propiedad correspondiente a ESTA versión.
    public string StateName { get; set; } = "";
    public decimal StatePrice { get; set; }

    public string? SourceListingId { get; set; }
    public string? StateRoomType { get; set; }
    public string? StateNeighbourhood { get; set; }
    public decimal? StateLatitude { get; set; }
    public decimal? StateLongitude { get; set; }
    public int? StateMinimumNights { get; set; }
    public long Version { get; set; }
}