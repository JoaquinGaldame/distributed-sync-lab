using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Mappers;
using OtaASimulator.Models.Units;
namespace OtaASimulator.Application.Units;

public sealed class BathroomService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<IReadOnlyList<BathroomResponse>> ReplaceAsync(long propertyId, long unitId, IReadOnlyList<BathroomRequest> request, CancellationToken ct)
    {
        InputRules.Unique(request, item => (item.BathroomConfigurationId, item.Scope), "configuración y ámbito");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockUnitAsync(propertyId, unitId, ct);
        foreach (var item in request)
        {
            await CatalogGuard.ExistsAsync(db.BathroomConfigurations, x => x.Id == item.BathroomConfigurationId, "bathroomConfigurationId", ct);
        }
        var existing = await db.UnitBathrooms.Where(x => x.UnitId == unitId).ToListAsync(ct);
        var desiredKeys = request.Select(item => (item.BathroomConfigurationId, item.Scope)).ToHashSet();
        db.UnitBathrooms.RemoveRange(existing.Where(x => !desiredKeys.Contains((x.BathroomConfigurationId, x.Scope))));
        var byKey = existing.ToDictionary(x => (x.BathroomConfigurationId, x.Scope));
        var result = new List<UnitBathroom>();
        foreach (var item in request)
        {
            if (!byKey.TryGetValue((item.BathroomConfigurationId, item.Scope), out var entity))
            {
                entity = new UnitBathroom { UnitId = unitId };
                db.UnitBathrooms.Add(entity);
            }
            BathroomMapper.Apply(item, entity);
            result.Add(entity);
        }
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return result.Select(BathroomMapper.ToResponse).ToArray();
    }
}
