using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Catalogs.Models;
using OtaASimulator.Infrastructure.Persistence;

namespace OtaASimulator.Application.Catalogs.PropertyTypes;

public sealed class PropertyTypeCatalogQuery(OtaADbContext db)
{
    public async Task<IReadOnlyList<CatalogItemResult>> ExecuteAsync(CancellationToken ct) =>
        await db.PropertyTypes
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new CatalogItemResult(x.Id, x.Code))
            .ToArrayAsync(ct);
}
