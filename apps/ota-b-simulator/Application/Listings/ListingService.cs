using System.Data;
using Microsoft.EntityFrameworkCore;
using OtaBSimulator.Api.Mappers;
using OtaBSimulator.Api.Models.Common;
using OtaBSimulator.Api.Models.Listings;
using OtaBSimulator.Application.Common;
using OtaBSimulator.Domain;
using OtaBSimulator.Infrastructure.Persistence;

namespace OtaBSimulator.Application.Listings;

public sealed class ListingService(OtaBDbContext db, ResourceGuard guard, CatalogResolver catalogs)
{
    public async Task<PagedResponse<ListingResponse>> ListAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = db.Listings.AsNoTracking();
        var total = await query.LongCountAsync(ct);
        var items = await BaseQuery().OrderBy(x => x.CreatedAt).ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new(items.Select(ListingMapper.ToResponse).ToArray(), page, pageSize, total);
    }

    public async Task<ListingResponse> CreateAsync(ListingRequest request, CancellationToken ct)
    {
        var refs = await ValidateAsync(request, ct);
        var entity = new Listing();
        ListingMapper.Apply(request, entity, refs.Category, refs.Primary, refs.Secondary, refs.Status);
        db.Listings.Add(entity);
        await db.SaveChangesAsync(ct);
        return ListingMapper.ToResponse(await BaseQuery().SingleAsync(x => x.Id == entity.Id, ct));
    }

    public async Task<ListingResponse> UpdateAsync(Guid id, ListingRequest request, CancellationToken ct)
    {
        var refs = await ValidateAsync(request, ct);
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var entity = await guard.LockedListingAsync(id, ct);
        if (!string.Equals(entity.CurrencyCode, request.CurrencyCode, StringComparison.Ordinal))
            throw ApiException.Conflict("La moneda del anuncio es inmutable y no puede modificarse.");
        ListingMapper.Apply(request, entity, refs.Category, refs.Primary, refs.Secondary, refs.Status);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return ListingMapper.ToResponse(await BaseQuery().SingleAsync(x => x.Id == id, ct));
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await guard.ListingAsync(id, ct);
        db.Listings.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task<ListingDetailResponse> GetAsync(Guid id, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, ct);
        var x = await DetailQuery().SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw ApiException.NotFound("El anuncio solicitado no existe.");
        var response = new ListingDetailResponse(
            ListingMapper.ToResponse(x),
            x.Location is null ? null : ListingMapper.ToResponse(x.Location),
            x.Contacts.OrderBy(y => y.RoleCode).Select(ListingMapper.ToResponse).ToArray(),
            x.Texts.OrderBy(y => y.Language.Code).Select(ListingMapper.ToResponse).ToArray(),
            x.Amenities.OrderBy(y => y.Amenity.Code).Select(y => y.Amenity.Code).ToArray(),
            x.HouseRules is null ? null : ListingMapper.ToResponse(x.HouseRules),
            x.BookingRules is null ? null : ListingMapper.ToResponse(x.BookingRules),
            x.Pricing is null ? null : ListingMapper.ToResponse(x.Pricing),
            x.Fees.OrderBy(y => y.Code).Select(ListingMapper.ToResponse).ToArray(),
            x.Rooms.OrderBy(y => y.DisplayOrder).ThenBy(y => y.Id).Select(RoomMapper.ToResponse).ToArray());
        await tx.CommitAsync(ct);
        return response;
    }

    private IQueryable<Listing> BaseQuery() => db.Listings.AsNoTracking()
        .Include(x => x.Category).Include(x => x.RentalMode).Include(x => x.Currency)
        .Include(x => x.PrimaryLanguage).Include(x => x.SecondaryLanguage).Include(x => x.Status);

    private IQueryable<Listing> DetailQuery() => BaseQuery().AsSplitQuery()
        .Include(x => x.Location).Include(x => x.Contacts)
        .Include(x => x.Texts).ThenInclude(x => x.Language)
        .Include(x => x.Amenities).ThenInclude(x => x.Amenity)
        .Include(x => x.HouseRules).Include(x => x.BookingRules).Include(x => x.Pricing).Include(x => x.Fees)
        .Include(x => x.Rooms).ThenInclude(x => x.RoomType)
        .Include(x => x.Rooms).ThenInclude(x => x.Beds).ThenInclude(x => x.BedType);

    private async Task<(ListingCategory Category, Language Primary, Language? Secondary, ListingStatus Status)> ValidateAsync(ListingRequest request, CancellationToken ct)
    {
        RequestRules.Text(request.Title, "title");
        RequestRules.Text(request.Timezone, "timezone");
        try { _ = TimeZoneInfo.FindSystemTimeZoneById(request.Timezone); }
        catch (TimeZoneNotFoundException) { throw ApiException.Invalid("El valor de timezone no identifica una zona horaria válida."); }
        catch (InvalidTimeZoneException) { throw ApiException.Invalid("El valor de timezone no identifica una zona horaria válida."); }
        await catalogs.ValidateListingCodes(request.RentalModeCode, request.CurrencyCode, ct);
        var category = await catalogs.Category(request.CategoryCode, ct);
        var primary = await catalogs.Language(request.PrimaryLanguageCode, "primaryLanguageCode", ct);
        var secondary = request.SecondaryLanguageCode is null ? null : await catalogs.Language(request.SecondaryLanguageCode, "secondaryLanguageCode", ct);
        var status = await catalogs.Status(request.StatusCode, ct);
        return (category, primary, secondary, status);
    }
}
