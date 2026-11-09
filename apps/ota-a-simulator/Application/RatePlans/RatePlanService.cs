using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Mappers;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.RatePlans;
namespace OtaASimulator.Application.RatePlans;

public sealed class RatePlanService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<PagedResponse<RatePlanResponse>> ListAsync(long propertyId, long unitId, int page, int pageSize, CancellationToken ct)
    {
        await guard.UnitAsync(propertyId, unitId, ct);
        var query = db.RatePlans
            .AsNoTracking()
            .Include(x => x.CancellationPolicy)
            .Where(x => x.PropertyId == propertyId && x.UnitId == unitId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new(items.Select(RatePlanMapper.ToResponse).ToArray(), page, pageSize, total);
    }

    public async Task<RatePlanResponse> GetAsync(long propertyId, long unitId, long ratePlanId, CancellationToken ct)
    {
        var entity = await db.RatePlans
            .AsNoTracking()
            .Include(x => x.CancellationPolicy)
            .SingleOrDefaultAsync(x => x.Id == ratePlanId && x.PropertyId == propertyId && x.UnitId == unitId, ct)
            ?? throw ApiException.NotFound("Plan inexistente en la unidad y propiedad indicadas.");
        return RatePlanMapper.ToResponse(entity);
    }

    public async Task<RatePlanResponse> CreateAsync(long propertyId, long unitId, RatePlanRequest request, CancellationToken ct)
    {
        await guard.UnitAsync(propertyId, unitId, ct);
        await ValidateAsync(propertyId, null, request, ct);
        var entity = new RatePlan { PropertyId = propertyId, UnitId = unitId };
        RatePlanMapper.Apply(request, entity);
        db.RatePlans.Add(entity);
        await db.SaveChangesAsync(ct);
        await db.Entry(entity).Reference(x => x.CancellationPolicy).LoadAsync(ct);
        return RatePlanMapper.ToResponse(entity);
    }

    public async Task<RatePlanResponse> UpdateAsync(long propertyId, long unitId, long ratePlanId, RatePlanRequest request, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var entity = (await db.RatePlans.FromSqlInterpolated($"SELECT * FROM public.rate_plans WHERE id = {ratePlanId} AND property_id = {propertyId} AND unit_id = {unitId} FOR UPDATE").ToListAsync(ct)).SingleOrDefault()
            ?? throw ApiException.NotFound("Plan inexistente en la unidad y propiedad indicadas.");
        await ValidateAsync(propertyId, ratePlanId, request, ct);
        if (await db.RateCalendars.AnyAsync(x => x.RatePlanId == ratePlanId
            && (x.MaximumNightsOverride ?? request.MaximumNights) != null
            && (x.MaximumNightsOverride ?? request.MaximumNights) < (x.MinimumNightsOverride ?? request.MinimumNights), ct))
            throw ApiException.Conflict("Las restricciones propuestas contradicen el calendario existente.");
        RatePlanMapper.Apply(request, entity);
        await db.SaveChangesAsync(ct);
        await db.Entry(entity).Reference(x => x.CancellationPolicy).LoadAsync(ct);
        await tx.CommitAsync(ct);
        return RatePlanMapper.ToResponse(entity);
    }

    public async Task DeleteAsync(long propertyId, long unitId, long ratePlanId, CancellationToken ct)
    {
        var entity = await guard.RatePlanAsync(propertyId, unitId, ratePlanId, ct);
        db.RatePlans.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    private async Task ValidateAsync(long propertyId, long? ratePlanId, RatePlanRequest request, CancellationToken ct)
    {
        await CatalogGuard.ExistsAsync(db.CancellationPolicies, x => x.Id == request.CancellationPolicyId, "cancellationPolicyId", ct);
        if (await db.RatePlans.AnyAsync(x => x.PropertyId == propertyId && x.Id != ratePlanId && x.ExternalCode == request.ExternalCode, ct))
            throw ApiException.Conflict("El código externo del plan ya existe en la propiedad.");
    }
}
