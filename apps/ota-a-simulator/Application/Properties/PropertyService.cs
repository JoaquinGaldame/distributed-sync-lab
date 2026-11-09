using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Mappers;
using OtaASimulator.Models.Common;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Application.Properties;

public sealed class PropertyService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<PagedResponse<PropertyResponse>> ListAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = db.Properties.AsNoTracking();
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new(items.Select(PropertyMapper.ToResponse).ToArray(), page, pageSize, total);
    }

    public async Task<PropertyResponse> CreateAsync(PropertyRequest request, CancellationToken ct)
    {
        await ValidateAsync(request, ct);
        if (await db.Properties.AnyAsync(x => x.ExternalCode == request.ExternalCode, ct))
            throw ApiException.Conflict("El código externo de la propiedad ya existe.");
        var entity = new Property();
        PropertyMapper.Apply(request, entity);
        db.Properties.Add(entity);
        await db.SaveChangesAsync(ct);
        return PropertyMapper.ToResponse(entity);
    }

    public async Task<PropertyResponse> UpdateAsync(long propertyId, PropertyRequest request, CancellationToken ct)
    {
        var entity = await guard.PropertyAsync(propertyId, ct);
        await ValidateAsync(request, ct);
        if (await db.Properties.AnyAsync(x => x.Id != propertyId && x.ExternalCode == request.ExternalCode, ct))
            throw ApiException.Conflict("El código externo de la propiedad ya existe.");
        PropertyMapper.Apply(request, entity);
        await db.SaveChangesAsync(ct);
        return PropertyMapper.ToResponse(entity);
    }

    public async Task DeleteAsync(long propertyId, CancellationToken ct)
    {
        var entity = await guard.PropertyAsync(propertyId, ct);
        db.Properties.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task<PropertyDetailResponse> GetAsync(long propertyId, CancellationToken ct)
    {
        // Todas las secciones se leen desde la misma instantánea de PostgreSQL.
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead, ct);
        var entity = await guard.PropertyAsync(propertyId, ct);
        var location = await db.PropertyLocations.AsNoTracking().SingleOrDefaultAsync(x => x.PropertyId == propertyId, ct);
        var contacts = await db.PropertyContacts.AsNoTracking().Where(x => x.PropertyId == propertyId).OrderBy(x => x.ProfileTypeId).ToListAsync(ct);
        var descriptions = await db.PropertyDescriptions.AsNoTracking().Where(x => x.PropertyId == propertyId).OrderBy(x => x.LanguageId).ToListAsync(ct);
        var facilities = await db.PropertyFacilities
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId)
            .OrderBy(x => x.FacilityId)
            .Select(x => new PropertyFacilityResponse(x.FacilityId, x.Facility.Code))
            .ToListAsync(ct);
        var policy = await db.StayPolicies.AsNoTracking().SingleOrDefaultAsync(x => x.PropertyId == propertyId, ct);
        var charges = await db.PropertyCharges.AsNoTracking().Where(x => x.PropertyId == propertyId).OrderBy(x => x.Code).ToListAsync(ct);
        await tx.CommitAsync(ct);
        return new(PropertyMapper.ToResponse(entity), location is null ? null : LocationMapper.ToResponse(location),
            contacts.Select(ContactMapper.ToResponse).ToArray(), descriptions.Select(DescriptionMapper.ToResponse).ToArray(), facilities,
            policy is null ? null : StayPolicyMapper.ToResponse(policy), charges.Select(ChargeMapper.ToResponse).ToArray());
    }

    private async Task ValidateAsync(PropertyRequest request, CancellationToken ct)
    {
        await CatalogGuard.ExistsAsync(db.PropertyTypes, x => x.Id == request.PropertyTypeId, "propertyTypeId", ct);
        await CatalogGuard.ExistsAsync(db.CurrencyTypes, x => x.Id == request.CurrencyId, "currencyId", ct);
        await CatalogGuard.ExistsAsync(db.Languages, x => x.Id == request.PrimaryLanguageId, "primaryLanguageId", ct);
    }
}
