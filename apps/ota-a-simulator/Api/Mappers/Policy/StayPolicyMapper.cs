using OtaASimulator.Domain;
using OtaASimulator.Models.Properties;

namespace OtaASimulator.Mappers;

public static class StayPolicyMapper
{
    public static void Apply(StayPolicyRequest request, StayPolicy entity)
    {
        entity.CheckInFrom = request.CheckInFrom;
        entity.CheckInUntil = request.CheckInUntil;
        entity.CheckOutFrom = request.CheckOutFrom;
        entity.CheckOutUntil = request.CheckOutUntil;
        entity.MinimumGuestAge = request.MinimumGuestAge;
        entity.ChildrenAllowed = request.ChildrenAllowed;
        entity.PetsAllowed = request.PetsAllowed;
        entity.SmokingAllowed = request.SmokingAllowed;
        entity.PartiesAllowed = request.PartiesAllowed;
    }

    public static StayPolicyResponse ToResponse(StayPolicy entity) => new(
        entity.CheckInFrom,
        entity.CheckInUntil,
        entity.CheckOutFrom,
        entity.CheckOutUntil,
        entity.MinimumGuestAge,
        entity.ChildrenAllowed,
        entity.PetsAllowed,
        entity.SmokingAllowed,
        entity.PartiesAllowed,
        entity.UpdatedAt);
}
