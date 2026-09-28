using AcademicPms.Application;

namespace AcademicPms.Api.Validation;

public static class PropertyRequestValidator
{
    public static bool ValidId(string? id) =>
        !string.IsNullOrWhiteSpace(id)
        && id.Length <= 50;

    public static bool ValidSourceListingId(string? sourceListingId) =>
        sourceListingId is null
        || (!string.IsNullOrWhiteSpace(sourceListingId)
            && sourceListingId.Length <= 50);

    public static bool ValidState(PropertyState state) =>
        !string.IsNullOrWhiteSpace(state.Name)
        && state.Name.Length <= 255
        && state.Price >= 0
        && state.Price < 10_000_000_000m
        && decimal.Round(state.Price, 2) == state.Price
        && state.RoomType is
            "ENTIRE_HOME" or
            "PRIVATE_ROOM" or
            "HOTEL_ROOM" or
            "SHARED_ROOM"
        && !string.IsNullOrWhiteSpace(state.Neighbourhood)
        && state.Neighbourhood.Length <= 100
        && state.Latitude is >= -90m and <= 90m
        && state.Longitude is >= -180m and <= 180m
        && state.MinimumNights >= 1;
}