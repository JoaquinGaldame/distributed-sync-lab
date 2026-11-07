using Microsoft.EntityFrameworkCore;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
namespace OtaASimulator.Application.Common;

public sealed class ResourceGuard(OtaADbContext db)
{
    public async Task<Property> PropertyAsync(long propertyId, CancellationToken ct) =>
        await db.Properties.SingleOrDefaultAsync(x => x.Id == propertyId, ct)
        ?? throw ApiException.NotFound("Propiedad inexistente.");

    // Serializa reemplazos de secciones de la misma propiedad. Requiere una transacción activa.
    public async Task<Property> LockPropertyAsync(long propertyId, CancellationToken ct) =>
        (await db.Properties.FromSqlInterpolated($"SELECT * FROM public.properties WHERE id = {propertyId} FOR UPDATE").ToListAsync(ct)).SingleOrDefault()
        ?? throw ApiException.NotFound("Propiedad inexistente.");

    public async Task<Unit> UnitAsync(long propertyId, long unitId, CancellationToken ct) =>
        await db.Units.SingleOrDefaultAsync(x => x.Id == unitId && x.PropertyId == propertyId, ct)
        ?? throw ApiException.NotFound("Unidad inexistente en la propiedad indicada.");

    public async Task<Unit> LockUnitAsync(long propertyId, long unitId, CancellationToken ct) =>
        (await db.Units.FromSqlInterpolated($"SELECT * FROM public.units WHERE id = {unitId} AND property_id = {propertyId} FOR UPDATE").ToListAsync(ct)).SingleOrDefault()
        ?? throw ApiException.NotFound("Unidad inexistente en la propiedad indicada.");

    public async Task<RatePlan> RatePlanAsync(long propertyId, long unitId, long planId, CancellationToken ct) =>
        await db.RatePlans.SingleOrDefaultAsync(x => x.Id == planId && x.PropertyId == propertyId && x.UnitId == unitId, ct)
        ?? throw ApiException.NotFound("Plan inexistente en la unidad y propiedad indicadas.");
}
