using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Catalogs.Models;
using OtaASimulator.Infrastructure.Persistence;

namespace OtaASimulator.Application.Catalogs.ProfileTypes;

public sealed class ProfileTypeCatalogQuery(OtaADbContext db)
{
    public async Task<IReadOnlyList<ProfileTypeResult>> ExecuteAsync(CancellationToken ct) =>
        await db.ProfileTypes
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new ProfileTypeResult(x.Id, x.Code, x.Name, x.Required))
            .ToArrayAsync(ct);
}
