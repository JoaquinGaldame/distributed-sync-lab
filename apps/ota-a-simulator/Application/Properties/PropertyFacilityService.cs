using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
namespace OtaASimulator.Application.Properties;

public sealed class PropertyFacilityService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<IReadOnlyList<string>> ReplaceAsync(long propertyId, IReadOnlyList<string> codes, CancellationToken ct)
    {
        InputRules.Unique(codes, x => x, "facilityCode");
        if (codes.Any(x => string.IsNullOrWhiteSpace(x) || x.Length > 100)) throw ApiException.Invalid("Código de instalación inválido.");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockPropertyAsync(propertyId, ct);
        var known = await db.Facilities.Where(x => codes.Contains(x.Code)).Select(x => x.Code).ToListAsync(ct);
        if (known.Count != codes.Count) throw ApiException.Invalid("La colección contiene instalaciones desconocidas.");
        var existing = await db.PropertyFacilities.Where(x => x.PropertyId == propertyId).ToListAsync(ct);
        db.PropertyFacilities.RemoveRange(existing.Where(x => !codes.Contains(x.FacilityCode)));
        var current = existing.Select(x => x.FacilityCode).ToHashSet();
        db.PropertyFacilities.AddRange(codes.Where(x => !current.Contains(x)).Select(x => new PropertyFacility { PropertyId = propertyId, FacilityCode = x }));
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return codes.OrderBy(x => x, StringComparer.Ordinal).ToArray();
    }
}
