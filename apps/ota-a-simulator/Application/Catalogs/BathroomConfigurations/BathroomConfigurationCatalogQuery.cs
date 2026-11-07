using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Catalogs.Models;
using OtaASimulator.Infrastructure.Persistence;

namespace OtaASimulator.Application.Catalogs.BathroomConfigurations;

public sealed class BathroomConfigurationCatalogQuery(OtaADbContext db)
{
    public async Task<IReadOnlyList<CatalogItemResult>> ExecuteAsync(CancellationToken ct) =>
        await db.BathroomConfigurations
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new CatalogItemResult(x.Id, x.Code))
            .ToArrayAsync(ct);
}
