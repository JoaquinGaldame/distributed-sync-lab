using OtaReplaceSimulator.Api.Models;

namespace OtaReplaceSimulator.Api.Validation;

public static class ReplacePropertyValidator
{
    public static bool IsValid(string? externalId, ReplacePropertyRequest request) =>
        !string.IsNullOrWhiteSpace(externalId)
        && externalId.Length <= 100
        && !string.IsNullOrWhiteSpace(request.Name)
        && request.Name.Length <= 255
        && request.Price >= 0
        && request.Price < 10_000_000_000m
        && decimal.Round(request.Price, 2) == request.Price
        && request.RoomType is
            "ENTIRE_HOME" or "PRIVATE_ROOM" or "HOTEL_ROOM" or "SHARED_ROOM"
        && !string.IsNullOrWhiteSpace(request.Neighbourhood)
        && request.Neighbourhood.Length <= 100
        && request.Latitude is >= -90m and <= 90m
        && request.Longitude is >= -180m and <= 180m
        && request.MinimumNights is >= 1
        && request.SourceVersion >= 1;
}