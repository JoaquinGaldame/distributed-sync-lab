using Microsoft.EntityFrameworkCore;
using OtaBSimulator.Domain;
using OtaBSimulator.Infrastructure.Persistence;

namespace OtaBSimulator.Application.Common;

public static class RequestRules
{
    public static string Text(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value)) throw ApiException.Invalid($"El campo {field} no puede estar vacío.");
        return value.Trim();
    }

    public static void NoDuplicates(IEnumerable<string> values, string field)
    {
        if (values.GroupBy(x => x, StringComparer.Ordinal).Any(x => x.Count() > 1))
            throw ApiException.Invalid($"La colección contiene valores duplicados en {field}.");
    }
}

public sealed class ResourceGuard(OtaBDbContext db)
{
    public async Task<Listing> ListingAsync(Guid id, CancellationToken ct) =>
        await db.Listings.SingleOrDefaultAsync(x => x.Id == id, ct)
        ?? throw ApiException.NotFound("El anuncio solicitado no existe.");

    public async Task<Listing> LockedListingAsync(Guid id, CancellationToken ct) =>
        await db.Listings.FromSqlInterpolated($"SELECT * FROM public.listings WHERE id = {id} FOR UPDATE")
            .SingleOrDefaultAsync(ct)
        ?? throw ApiException.NotFound("El anuncio solicitado no existe.");

    public async Task<Room> RoomAsync(Guid listingId, Guid roomId, CancellationToken ct) =>
        await db.Rooms.Include(x => x.RoomType).Include(x => x.Beds).ThenInclude(x => x.BedType)
            .SingleOrDefaultAsync(x => x.Id == roomId && x.ListingId == listingId, ct)
        ?? throw ApiException.NotFound("El espacio solicitado no existe en el anuncio indicado.");
}

public sealed class CatalogResolver(OtaBDbContext db)
{
    public async Task<ListingCategory> Category(string code, CancellationToken ct) =>
        await db.ListingCategories.SingleOrDefaultAsync(x => x.Code == code, ct) ?? Invalid<ListingCategory>("categoryCode");
    public async Task<Language> Language(string code, string field, CancellationToken ct) =>
        await db.Languages.SingleOrDefaultAsync(x => x.Code == code, ct) ?? Invalid<Language>(field);
    public async Task<ListingStatus> Status(string code, CancellationToken ct) =>
        await db.ListingStatuses.SingleOrDefaultAsync(x => x.Code == code, ct) ?? Invalid<ListingStatus>("statusCode");
    public async Task<RoomType> RoomType(string code, CancellationToken ct) =>
        await db.RoomTypes.SingleOrDefaultAsync(x => x.Code == code, ct) ?? Invalid<RoomType>("roomTypeCode");

    public async Task ValidateListingCodes(string rentalMode, string currency, CancellationToken ct)
    {
        if (!await db.RentalModes.AnyAsync(x => x.Code == rentalMode, ct)) Invalid<object>("rentalModeCode");
        if (!await db.Currencies.AnyAsync(x => x.Code == currency, ct)) Invalid<object>("currencyCode");
    }

    private static T Invalid<T>(string field) => throw ApiException.Invalid($"El valor de {field} no existe en el catálogo correspondiente.");
}
