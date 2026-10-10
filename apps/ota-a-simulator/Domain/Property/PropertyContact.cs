namespace OtaASimulator.Domain;

public sealed class PropertyContact
{
    public long Id { get; set; }
    public long PropertyId { get; set; }
    public int ProfileTypeId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int LanguageId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Property Property { get; set; } = null!;
    public ProfileType ProfileType { get; set; } = null!;
    public Language Language { get; set; } = null!;
}
