using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Catalogs.Models;
using OtaASimulator.Infrastructure.Persistence;

namespace OtaASimulator.Application.Catalogs.CancellationPolicies;

public sealed class CancellationPolicyCatalogQuery(OtaADbContext db)
{
    public async Task<IReadOnlyList<CancellationPolicyResult>> ExecuteAsync(CancellationToken ct) =>
        await db.CancellationPolicies
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new CancellationPolicyResult(
                x.Id,
                x.Code,
                x.Name,
                x.FreeUntilDaysBefore,
                x.PenaltyPercent))
            .ToArrayAsync(ct);
}
