using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Catalogs.Models;
using OtaASimulator.Infrastructure.Persistence;

namespace OtaASimulator.Application.Catalogs.UnitFacilityTypes;

public sealed class UnitFacilityTypeCatalogQuery(OtaADbContext db)
{
    public async Task<IReadOnlyList<NamedCatalogItemResult>> ExecuteAsync(CancellationToken ct) =>
        await db.UnitFacilityTypes
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new NamedCatalogItemResult(x.Id, x.Code, x.Name))
            .ToArrayAsync(ct);
}
