using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Models.Units;
namespace OtaASimulator.Application.Units;

public sealed class UnitFacilityService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<IReadOnlyList<UnitFacilityResponse>> ConfigureAsync(
        long propertyId,
        long unitId,
        IReadOnlyList<UnitFacilityRequest> request,
        CancellationToken ct)
    {
        InputRules.Unique(request, x => x.UnitFacilityTypeId, "unitFacilityTypeId");

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockUnitAsync(propertyId, unitId, ct);

        var unitFacilityTypeIds = request.Select(x => x.UnitFacilityTypeId).ToArray();
        var knownCount = await db.UnitFacilityTypes.CountAsync(x => unitFacilityTypeIds.Contains(x.Id), ct);
        if (knownCount != unitFacilityTypeIds.Length)
            throw ApiException.Invalid("La colección contiene instalaciones de unidad desconocidas.");

        var existing = await db.UnitFacilities
            .Where(x => x.UnitId == unitId)
            .ToListAsync(ct);
        var existingById = existing.ToDictionary(x => x.UnitFacilityTypeId);

        var requestedIds = unitFacilityTypeIds.ToHashSet();
        db.UnitFacilities.RemoveRange(existing.Where(x => !requestedIds.Contains(x.UnitFacilityTypeId)));

        foreach (var item in request)
        {
            if (item.Status == "PRESENT" && !existingById.ContainsKey(item.UnitFacilityTypeId))
            {
                db.UnitFacilities.Add(new UnitFacility
                {
                    UnitId = unitId,
                    UnitFacilityTypeId = item.UnitFacilityTypeId
                });
            }
            else if (item.Status == "MISSING" && existingById.TryGetValue(item.UnitFacilityTypeId, out var relation))
            {
                db.UnitFacilities.Remove(relation);
            }
        }

        await db.SaveChangesAsync(ct);

        var configuredFacilities = await db.UnitFacilities
            .AsNoTracking()
            .Where(x => x.UnitId == unitId)
            .OrderBy(x => x.UnitFacilityTypeId)
            .Select(x => new UnitFacilityResponse(x.UnitFacilityTypeId, x.UnitFacilityType.Code))
            .ToArrayAsync(ct);

        await tx.CommitAsync(ct);
        return configuredFacilities;
    }
}
