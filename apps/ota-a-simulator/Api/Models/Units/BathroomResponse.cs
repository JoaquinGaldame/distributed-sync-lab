namespace OtaASimulator.Models.Units;

public sealed record BathroomResponse(
    long Id,
    int BathroomConfigurationId,
    string Scope,
    int Quantity);
