using AcademicPms.Domain;

namespace AcademicPms.Api.Models;

public sealed record PropertyChangePageResponse(
    IReadOnlyList<PropertyChangeResponse> Items,
    long NextAfter,
    bool HasMore);

public sealed record PropertyChangeResponse(
    long Sequence,
    string? SourceListingId,
    PropertyChangedEventResponse Event,
    PropertyDesiredStateResponse DesiredState)
{
    public static PropertyChangeResponse From(PropertyChange change) => new(
        change.Sequence,
        change.SourceListingId,
        new PropertyChangedEventResponse(
            change.EventId,
            change.EventType,
            change.PropertyId,
            change.Version,
            change.OccurredAt),
        new PropertyDesiredStateResponse(
            change.StateName,
            change.StatePrice,
            change.StateRoomType,
            change.StateNeighbourhood,
            change.StateLatitude,
            change.StateLongitude,
            change.StateMinimumNights));
}

public sealed record PropertyChangedEventResponse(
    Guid EventId,
    string EventType,
    string PropertyId,
    long Version,
    DateTime OccurredAt);

public sealed record PropertyDesiredStateResponse(
    string Name,
    decimal Price,
    string? RoomType,
    string? Neighbourhood,
    decimal? Latitude,
    decimal? Longitude,
    int? MinimumNights);