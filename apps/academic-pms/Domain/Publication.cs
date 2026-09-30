namespace AcademicPms.Domain;

public sealed class Publication
{
    public string PropertyId { get; set; } = "";
    public long ChannelId { get; set; }
    public string? ExternalPropertyId { get; set; }
    public bool Published { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
