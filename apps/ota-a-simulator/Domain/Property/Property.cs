namespace OtaASimulator.Domain;

public sealed class Property
{
    public long Id { get; set; }
    public string ExternalCode { get; set; } = "";
    public string Name { get; set; } = "";
    public int PropertyTypeId { get; set; }
    public int CurrencyId { get; set; }
    public int PrimaryLanguageId { get; set; }
    public string Timezone { get; set; } = "";
    public string Status { get; set; } = "draft";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public PropertyType PropertyType { get; set; } = null!;
    public CurrencyType CurrencyType { get; set; } = null!;
    public Language PrimaryLanguage { get; set; } = null!;
}
