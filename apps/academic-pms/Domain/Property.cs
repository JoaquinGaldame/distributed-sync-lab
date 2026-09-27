public sealed class Property
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public long Version { get; set; }
    public DateTime UpdatedAt { get; set; }
}