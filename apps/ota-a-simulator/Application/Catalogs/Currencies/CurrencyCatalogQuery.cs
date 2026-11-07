using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Catalogs.Models;
using OtaASimulator.Infrastructure.Persistence;

namespace OtaASimulator.Application.Catalogs.Currencies;

public sealed class CurrencyCatalogQuery(OtaADbContext db)
{
    public async Task<IReadOnlyList<CatalogItemResult>> ExecuteAsync(CancellationToken ct) =>
        await db.CurrencyTypes
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new CatalogItemResult(x.Id, x.Code))
            .ToArrayAsync(ct);
}
