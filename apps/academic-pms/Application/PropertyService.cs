using AcademicPms.Domain;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AcademicPms.Application;

public sealed class PropertyService(PmsDbContext db)
{
    public async Task<Property?> CreateAsync(
        string id,
        string? sourceListingId,
        PropertyState state,
        CancellationToken cancellationToken)
    {
        if (await db.Properties.AnyAsync(x => x.Id == id, cancellationToken))
            return null;

        var now = DateTime.UtcNow;

        var property = new Property
        {
            Id = id,
            SourceListingId = sourceListingId,
            Name = state.Name,
            Price = state.Price,
            RoomType = state.RoomType,
            Neighbourhood = state.Neighbourhood,
            Latitude = state.Latitude,
            Longitude = state.Longitude,
            MinimumNights = state.MinimumNights,
            Version = 1,
            UpdatedAt = now
        };

        db.Properties.Add(property);
        db.PropertyChanges.Add(NewChange(property, now));

        await db.SaveChangesAsync(cancellationToken);
        return property;
    }

    public async Task<Property?> UpdateAsync(
        string id,
        PropertyState state,
        CancellationToken cancellationToken)
    {
        var property = await db.Properties.FindAsync(
            [id], cancellationToken);

        if (property is null)
            return null;

        // Un estado idéntico no crea una nueva versión.
        if (property.Name == state.Name
            && property.Price == state.Price
            && property.RoomType == state.RoomType
            && property.Neighbourhood == state.Neighbourhood
            && property.Latitude == state.Latitude
            && property.Longitude == state.Longitude
            && property.MinimumNights == state.MinimumNights)
        {
            return property;
        }

        property.Name = state.Name;
        property.Price = state.Price;
        property.RoomType = state.RoomType;
        property.Neighbourhood = state.Neighbourhood;
        property.Latitude = state.Latitude;
        property.Longitude = state.Longitude;
        property.MinimumNights = state.MinimumNights;
        property.Version++;
        property.UpdatedAt = DateTime.UtcNow;

        db.PropertyChanges.Add(
            NewChange(property, property.UpdatedAt));

        await db.SaveChangesAsync(cancellationToken);
        return property;
    }

    private static PropertyChange NewChange(
        Property property,
        DateTime occurredAt) => new()
    {
        EventId = Guid.NewGuid(),
        PropertyId = property.Id,
        Version = property.Version,
        OccurredAt = occurredAt,
        SourceListingId = property.SourceListingId,
        StateName = property.Name,
        StatePrice = property.Price,
        StateRoomType = property.RoomType,
        StateNeighbourhood = property.Neighbourhood,
        StateLatitude = property.Latitude,
        StateLongitude = property.Longitude,
        StateMinimumNights = property.MinimumNights
    };
}