using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Mappers;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.Units;
namespace OtaASimulator.Application.Units;

public sealed class UnitService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<PagedResponse<UnitResponse>> ListAsync(long propertyId, int page, int pageSize, CancellationToken ct)
    {
        await guard.PropertyAsync(propertyId, ct);
        var query = db.Units.AsNoTracking().Where(x => x.PropertyId == propertyId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new(items.Select(UnitMapper.ToResponse).ToArray(), page, pageSize, total);
    }

    public async Task<UnitDetailResponse> GetAsync(long propertyId, long unitId, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead, ct);
        var entity = await guard.UnitAsync(propertyId, unitId, ct);
        var facilities = await db.UnitFacilities
            .AsNoTracking()
            .Where(x => x.UnitId == unitId)
            .OrderBy(x => x.UnitFacilityTypeId)
            .Select(x => new UnitFacilityResponse(x.UnitFacilityTypeId, x.UnitFacilityType.Code))
            .ToListAsync(ct);
        var bathrooms = await db.UnitBathrooms.AsNoTracking().Where(x => x.UnitId == unitId).OrderBy(x => x.BathroomConfigurationId).ThenBy(x => x.Scope).ToListAsync(ct);
        await tx.CommitAsync(ct);
        return new(UnitMapper.ToResponse(entity), facilities, bathrooms.Select(BathroomMapper.ToResponse).ToArray());
    }

    public async Task<UnitResponse> CreateAsync(long propertyId, UnitRequest request, CancellationToken ct)
    {
        await guard.PropertyAsync(propertyId, ct);
        await ValidateAsync(propertyId, null, request, ct);
        var entity = new Unit { PropertyId = propertyId };
        UnitMapper.Apply(request, entity);
        db.Units.Add(entity);
        await db.SaveChangesAsync(ct);
        return UnitMapper.ToResponse(entity);
    }

    public async Task<UnitResponse> UpdateAsync(long propertyId, long unitId, UnitRequest request, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var entity = await guard.LockUnitAsync(propertyId, unitId, ct);
        await ValidateAsync(propertyId, unitId, request, ct);
        if (await db.UnitInventories.AnyAsync(x => x.UnitId == unitId && x.AvailableUnits > request.TotalUnits, ct))
            throw ApiException.Conflict("La capacidad propuesta contradice el inventario existente.");
        UnitMapper.Apply(request, entity);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return UnitMapper.ToResponse(entity);
    }

    public async Task DeleteAsync(long propertyId, long unitId, CancellationToken ct)
    {
        var entity = await guard.UnitAsync(propertyId, unitId, ct);
        db.Units.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    private async Task ValidateAsync(long propertyId, long? unitId, UnitRequest request, CancellationToken ct)
    {
        await CatalogGuard.ExistsAsync(db.UnitTypes, x => x.Id == request.UnitTypeId, "unitTypeId", ct);
        if (await db.Units.AnyAsync(x => x.PropertyId == propertyId && x.Id != unitId && x.ExternalCode == request.ExternalCode, ct))
            throw ApiException.Conflict("El código externo de la unidad ya existe en la propiedad.");
    }
}
