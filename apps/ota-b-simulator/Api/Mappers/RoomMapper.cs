using OtaBSimulator.Api.Models.Rooms;
using OtaBSimulator.Domain;

namespace OtaBSimulator.Api.Mappers;

public static class RoomMapper
{
    public static RoomResponse ToResponse(Room x) => new(
        x.Id, x.RoomType.Code, x.Name, x.Description, x.AreaM2, x.FloorNumber, x.IsShared,
        x.DisplayOrder, x.CreatedAt, x.UpdatedAt,
        x.Beds.OrderBy(b => b.BedType.Code).Select(b => new BedResponse(b.BedType.Code, b.Quantity)).ToArray());

    public static void Apply(RoomRequest request, Room entity, RoomType roomType)
    {
        entity.RoomTypeId = roomType.Id;
        entity.Name = request.Name.Trim();
        entity.Description = request.Description;
        entity.AreaM2 = request.AreaM2;
        entity.FloorNumber = request.FloorNumber;
        entity.IsShared = request.IsShared;
        entity.DisplayOrder = request.DisplayOrder;
    }
}
