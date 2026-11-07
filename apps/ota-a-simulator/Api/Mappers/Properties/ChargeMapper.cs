using OtaASimulator.Domain;
using OtaASimulator.Models.Properties;

namespace OtaASimulator.Mappers;

public static class ChargeMapper
{
    public static void Apply(ChargeRequest request, PropertyCharge entity)
    {
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Calculation = request.Calculation;
        entity.Amount = request.Amount;
        entity.IncludedInPrice = request.IncludedInPrice;
    }

    public static ChargeResponse ToResponse(PropertyCharge entity) => new(
        entity.Id,
        entity.Code,
        entity.Name,
        entity.Calculation,
        entity.Amount,
        entity.IncludedInPrice);
}
