using OtaASimulator.Domain;
using OtaASimulator.Models.Properties;

namespace OtaASimulator.Mappers;

public static class PropertyMapper
{
    public static void Apply(PropertyRequest request, Property entity)
    {
        entity.ExternalCode = request.ExternalCode;
        entity.Name = request.Name;
        entity.PropertyTypeId = request.PropertyTypeId;
        entity.CurrencyId = request.CurrencyId;
        entity.PrimaryLanguageId = request.PrimaryLanguageId;
        entity.Timezone = request.Timezone;
        entity.Status = request.Status;
    }

    public static PropertyResponse ToResponse(Property entity) => new(
        entity.Id,
        entity.ExternalCode,
        entity.Name,
        entity.PropertyTypeId,
        entity.CurrencyId,
        entity.PrimaryLanguageId,
        entity.Timezone,
        entity.Status,
        entity.CreatedAt,
        entity.UpdatedAt);
}
