using AcademicPms.Api.Models;
using AcademicPms.Application;
using AcademicPms.Domain;

namespace AcademicPms.Api.Mappers;

public static class PropertyMapper
{
    public static PropertyState ToState(PropertyInput input) => new(
        input.Name,
        input.Price,
        input.RoomType,
        input.Neighbourhood,
        input.Latitude,
        input.Longitude,
        input.MinimumNights);

    public static PropertyState ToState(PropertyUpdate input) => new(
        input.Name,
        input.Price,
        input.RoomType,
        input.Neighbourhood,
        input.Latitude,
        input.Longitude,
        input.MinimumNights);

    public static PropertyResponse ToResponse(Property property) => new(
        property.Id,
        property.SourceListingId,
        property.Name,
        property.Price,
        property.RoomType,
        property.Neighbourhood,
        property.Latitude,
        property.Longitude,
        property.MinimumNights,
        property.Version,
        property.UpdatedAt);

    public static PropertyChangeResponse ToResponse(PropertyChange change) => new(
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