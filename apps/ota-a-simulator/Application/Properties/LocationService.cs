using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Mappers;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Application.Properties;

public sealed class LocationService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<LocationResponse> ReplaceAsync(long propertyId, LocationRequest request, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockPropertyAsync(propertyId, ct);
        var entity = await db.PropertyLocations.SingleOrDefaultAsync(x => x.PropertyId == propertyId, ct);
        if (entity is null) { entity = new PropertyLocation { PropertyId = propertyId }; db.PropertyLocations.Add(entity); }
        LocationMapper.Apply(request, entity);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return LocationMapper.ToResponse(entity);
    }
}
