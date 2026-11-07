using OtaASimulator.Domain;
using OtaASimulator.Models.Units;

namespace OtaASimulator.Mappers;

public static class UnitMapper
{
    public static void Apply(UnitRequest request, Unit entity)
    {
        entity.ExternalCode = request.ExternalCode;
        entity.Name = request.Name;
        entity.UnitTypeId = request.UnitTypeId;
        entity.TotalUnits = request.TotalUnits;
        entity.MaximumOccupancy = request.MaximumOccupancy;
        entity.MaximumAdults = request.MaximumAdults;
        entity.MaximumChildren = request.MaximumChildren;
        entity.IsActive = request.IsActive;
    }

    public static UnitResponse ToResponse(Unit entity) => new(
        entity.Id,
        entity.PropertyId,
        entity.ExternalCode,
        entity.Name,
        entity.UnitTypeId,
        entity.TotalUnits,
        entity.MaximumOccupancy,
        entity.MaximumAdults,
        entity.MaximumChildren,
        entity.IsActive,
        entity.CreatedAt,
        entity.UpdatedAt);
}
