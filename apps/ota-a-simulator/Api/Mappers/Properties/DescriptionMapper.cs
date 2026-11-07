using OtaASimulator.Domain;
using OtaASimulator.Models.Properties;

namespace OtaASimulator.Mappers;

public static class DescriptionMapper
{
    public static void Apply(DescriptionRequest request, PropertyDescription entity)
    {
        entity.LanguageId = request.LanguageId;
        entity.Description = request.Description;
    }

    public static DescriptionResponse ToResponse(PropertyDescription entity) => new(
        entity.LanguageId,
        entity.Description,
        entity.CreatedAt,
        entity.UpdatedAt);
}
