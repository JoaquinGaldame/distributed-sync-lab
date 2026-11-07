using OtaASimulator.Domain;
using OtaASimulator.Models.Units;

namespace OtaASimulator.Mappers;

public static class BathroomMapper
{
    public static void Apply(BathroomRequest request, UnitBathroom entity)
    {
        entity.BathroomConfigurationId = request.BathroomConfigurationId;
        entity.Scope = request.Scope;
        entity.Quantity = request.Quantity;
    }

    public static BathroomResponse ToResponse(UnitBathroom entity) => new(
        entity.Id,
        entity.BathroomConfigurationId,
        entity.Scope,
        entity.Quantity);
}
