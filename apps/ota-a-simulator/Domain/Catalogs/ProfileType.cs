namespace OtaASimulator.Domain;

public sealed class ProfileType
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public bool Required { get; set; } = false;
}