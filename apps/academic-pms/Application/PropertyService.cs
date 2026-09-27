using AcademicPms.Domain;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AcademicPms.Application;

public sealed class PropertyService(PmsDbContext db)
{
    public async Task<Property?> CreateAsync(string id, string name, decimal price, CancellationToken cancellationToken)
    {
        if (await db.Properties.AnyAsync(x => x.Id == id, cancellationToken))
            return null;

        var now = DateTime.UtcNow;

        var property = new Property
        {
            Id = id,
            Name = name,
            Price = price,
            Version = 1,
            UpdatedAt = now
        };

        db.Properties.Add(property);
        db.PropertyChanges.Add(NewChange(property, now));

        await db.SaveChangesAsync(cancellationToken);
        return property;
    }

    public async Task<Property?> UpdateAsync( string id, string name, decimal price, CancellationToken cancellationToken)
    {
        var property = await db.Properties.FindAsync(
            [id], cancellationToken);

        if (property is null)
            return null;

        // Repetir exactamente el mismo estado no genera una versión nueva.
        if (property.Name == name && property.Price == price)
            return property;

        property.Name = name;
        property.Price = price;
        property.Version++;
        property.UpdatedAt = DateTime.UtcNow;

        db.PropertyChanges.Add(
            NewChange(property, property.UpdatedAt));

        await db.SaveChangesAsync(cancellationToken);
        return property;
    }

    private static PropertyChange NewChange( Property property, DateTime occurredAt) => new()
    {
        EventId = Guid.NewGuid(),
        PropertyId = property.Id,
        Version = property.Version,
        OccurredAt = occurredAt,
        StateName = property.Name,
        StatePrice = property.Price
    };
}