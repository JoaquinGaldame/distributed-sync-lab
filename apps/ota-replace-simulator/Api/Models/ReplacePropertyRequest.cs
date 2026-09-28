namespace OtaReplaceSimulator.Api.Models;

public sealed record ReplacePropertyRequest(
    string Name,
    decimal Price,
    string? RoomType,
    string? Neighbourhood,
    decimal? Latitude,
    decimal? Longitude,
    int? MinimumNights,
    long SourceVersion);