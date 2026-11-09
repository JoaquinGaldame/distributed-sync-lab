using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Models.Properties;

namespace OtaASimulator.Application.Properties;

public sealed class PropertyFacilityService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<IReadOnlyList<PropertyFacilityResponse>> ConfigureAsync(
        long propertyId,
        IReadOnlyList<PropertyFacilityRequest> request,
        CancellationToken ct)
    {
        InputRules.Unique(request, x => x.FacilityId, "facilityId");

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockPropertyAsync(propertyId, ct);

        var facilityIds = request.Select(x => x.FacilityId).ToArray();
        var knownCount = await db.Facilities.CountAsync(x => facilityIds.Contains(x.Id), ct);
        if (knownCount != facilityIds.Length)
            throw ApiException.Invalid("La colección contiene instalaciones desconocidas.");

        var existing = await db.PropertyFacilities
            .Where(x => x.PropertyId == propertyId)
            .ToListAsync(ct);
        var existingById = existing.ToDictionary(x => x.FacilityId);

        var requestedIds = facilityIds.ToHashSet();
        db.PropertyFacilities.RemoveRange(existing.Where(x => !requestedIds.Contains(x.FacilityId)));

        foreach (var item in request)
        {
            if (item.Status == "Present" && !existingById.ContainsKey(item.FacilityId))
            {
                db.PropertyFacilities.Add(new PropertyFacility
                {
                    PropertyId = propertyId,
                    FacilityId = item.FacilityId
                });
            }
            else if (item.Status == "Missing" && existingById.TryGetValue(item.FacilityId, out var relation))
            {
                db.PropertyFacilities.Remove(relation);
            }
        }

        await db.SaveChangesAsync(ct);

        var configuredFacilities = await db.PropertyFacilities
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId)
            .OrderBy(x => x.FacilityId)
            .Select(x => new PropertyFacilityResponse(x.FacilityId, x.Facility.Code))
            .ToArrayAsync(ct);

        await tx.CommitAsync(ct);
        return configuredFacilities;
    }
}
