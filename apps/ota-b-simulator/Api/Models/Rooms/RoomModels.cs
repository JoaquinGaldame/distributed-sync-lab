using System.ComponentModel.DataAnnotations;

namespace OtaBSimulator.Api.Models.Rooms;

public sealed record RoomRequest(
    [Required, StringLength(40)] string RoomTypeCode,
    [Required, StringLength(150, MinimumLength = 1)] string Name,
    string? Description,
    [Range(typeof(decimal), "0.01", "99999999.99")] decimal? AreaM2,
    int? FloorNumber,
    bool IsShared,
    [Range(0, int.MaxValue)] int DisplayOrder);
public sealed record BedRequest(
    [Required, StringLength(40)] string BedTypeCode,
    [Range(1, int.MaxValue)] int Quantity);
public sealed record BedResponse(string BedTypeCode, int Quantity);
public sealed record RoomResponse(Guid Id, string RoomTypeCode, string Name, string? Description, decimal? AreaM2, int? FloorNumber, bool IsShared, int DisplayOrder, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, IReadOnlyList<BedResponse> Beds);
