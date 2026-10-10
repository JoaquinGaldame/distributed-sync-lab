namespace OtaASimulator.Domain;

public sealed class PropertyDescription
{
    public long PropertyId { get; set; }
    public int LanguageId { get; set; }
    public string Description { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Property Property { get; set; } = null!;
    public Language Language { get; set; } = null!;
}
