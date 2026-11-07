using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
namespace OtaASimulator.Application.Units;

public sealed class UnitFacilityService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<IReadOnlyList<int>> ReplaceAsync(long propertyId, long unitId, IReadOnlyList<int> ids, CancellationToken ct)
    {
        InputRules.Unique(ids, x => x, "unitFacilityTypeId");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockUnitAsync(propertyId, unitId, ct);
        if (await db.UnitFacilityTypes.CountAsync(x => ids.Contains(x.Id), ct) != ids.Count)
            throw ApiException.Invalid("La colección contiene instalaciones de unidad desconocidas.");
        var existing = await db.UnitFacilities.Where(x => x.UnitId == unitId).ToListAsync(ct);
        db.UnitFacilities.RemoveRange(existing.Where(x => !ids.Contains(x.UnitFacilityTypeId)));
        var current = existing.Select(x => x.UnitFacilityTypeId).ToHashSet();
        db.UnitFacilities.AddRange(ids.Where(x => !current.Contains(x)).Select(x => new UnitFacility { UnitId = unitId, UnitFacilityTypeId = x }));
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return ids.Order().ToArray();
    }
}
