public sealed class Property
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? SourceListingId { get; set; }
    public string? RoomType { get; set; }
    public string? Neighbourhood { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? MinimumNights { get; set; }
    public long Version { get; set; }
}