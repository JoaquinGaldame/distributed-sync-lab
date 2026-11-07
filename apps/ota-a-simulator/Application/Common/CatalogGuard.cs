using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
namespace OtaASimulator.Application.Common;

public static class CatalogGuard
{
    public static async Task ExistsAsync<T>(IQueryable<T> query, Expression<Func<T, bool>> predicate, string field, CancellationToken ct) where T : class
    {
        if (!await query.AnyAsync(predicate, ct)) throw ApiException.Invalid($"Referencia desconocida: {field}.");
    }
}
