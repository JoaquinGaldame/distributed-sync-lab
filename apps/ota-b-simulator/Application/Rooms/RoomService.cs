using Microsoft.EntityFrameworkCore;
using OtaBSimulator.Api.Mappers;
using OtaBSimulator.Api.Models.Rooms;
using OtaBSimulator.Application.Common;
using OtaBSimulator.Domain;
using OtaBSimulator.Infrastructure.Persistence;

namespace OtaBSimulator.Application.Rooms;

public sealed class RoomService(OtaBDbContext db, ResourceGuard guard, CatalogResolver catalogs)
{
    public async Task<IReadOnlyList<RoomResponse>> List(Guid listingId, CancellationToken ct)
    {
        await guard.ListingAsync(listingId, ct);
        var rooms = await Query().Where(x => x.ListingId == listingId).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id).ToListAsync(ct);
        return rooms.Select(RoomMapper.ToResponse).ToArray();
    }

    public async Task<RoomResponse> Get(Guid listingId, Guid roomId, CancellationToken ct) => RoomMapper.ToResponse(await guard.RoomAsync(listingId, roomId, ct));

    public async Task<RoomResponse> Create(Guid listingId, RoomRequest request, CancellationToken ct)
    {
        Validate(request); var type = await catalogs.RoomType(request.RoomTypeCode, ct);
        await using var tx = await db.Database.BeginTransactionAsync(ct); await guard.LockedListingAsync(listingId, ct);
        var room = new Room { ListingId = listingId }; RoomMapper.Apply(request, room, type); db.Rooms.Add(room);
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return RoomMapper.ToResponse(await Query().SingleAsync(x => x.Id == room.Id, ct));
    }

    public async Task<RoomResponse> Update(Guid listingId, Guid roomId, RoomRequest request, CancellationToken ct)
    {
        Validate(request); var type = await catalogs.RoomType(request.RoomTypeCode, ct);
        await using var tx = await db.Database.BeginTransactionAsync(ct); await guard.LockedListingAsync(listingId, ct);
        var room = await guard.RoomAsync(listingId, roomId, ct); RoomMapper.Apply(request, room, type);
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return RoomMapper.ToResponse(await Query().SingleAsync(x => x.Id == room.Id, ct));
    }

    public async Task Delete(Guid listingId, Guid roomId, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct); await guard.LockedListingAsync(listingId, ct);
        db.Rooms.Remove(await guard.RoomAsync(listingId, roomId, ct)); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
    }

    public async Task<IReadOnlyList<BedResponse>> PutBeds(Guid listingId, Guid roomId, IReadOnlyList<BedRequest>? requests, CancellationToken ct)
    {
        if (requests is null || requests.Any(x => x is null)) throw ApiException.Invalid("beds debe ser una colección y no admite elementos null.");
        RequestRules.NoDuplicates(requests.Select(x => x.BedTypeCode), "bedTypeCode");
        if (requests.Any(x => x.Quantity <= 0)) throw ApiException.Invalid("Todas las cantidades de camas deben ser positivas.");
        var codes = requests.Select(x => x.BedTypeCode).Distinct().ToArray();
        var types = await db.BedTypes.Where(x => codes.Contains(x.Code)).ToDictionaryAsync(x => x.Code, ct);
        if (types.Count != codes.Length) throw ApiException.Invalid("Uno o más bedTypeCode no existen en el catálogo.");
        await using var tx = await db.Database.BeginTransactionAsync(ct); await guard.LockedListingAsync(listingId, ct);
        var room = await guard.RoomAsync(listingId, roomId, ct);
        db.RoomBeds.RemoveRange(room.Beds);
        var beds = requests.Select(x => new RoomBed { RoomId = roomId, BedTypeId = types[x.BedTypeCode].Id, BedType = types[x.BedTypeCode], Quantity = x.Quantity }).ToArray();
        db.RoomBeds.AddRange(beds); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return beds.OrderBy(x => x.BedType.Code).Select(x => new BedResponse(x.BedType.Code, x.Quantity)).ToArray();
    }

    private IQueryable<Room> Query() => db.Rooms.AsNoTracking().Include(x => x.RoomType).Include(x => x.Beds).ThenInclude(x => x.BedType);
    private static void Validate(RoomRequest request) { RequestRules.Text(request.Name, "name"); }
}
