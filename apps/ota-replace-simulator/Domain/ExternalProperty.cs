namespace OtaReplaceSimulator.Domain;

public sealed class ExternalProperty
{
    public string ExternalId { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string? RoomType { get; set; }
    public string? Neighbourhood { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? MinimumNights { get; set; }
    public long SourceVersion { get; set; }
}