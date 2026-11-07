using OtaASimulator.Domain;
using OtaASimulator.Models.Properties;

namespace OtaASimulator.Mappers;

public static class ContactMapper
{
    public static void Apply(ContactRequest request, PropertyContact entity)
    {
        entity.ProfileTypeId = request.ProfileTypeId;
        entity.Name = request.Name;
        entity.Email = request.Email;
        entity.Phone = request.Phone;
        entity.LanguageId = request.LanguageId;
    }

    public static ContactResponse ToResponse(PropertyContact entity) => new(
        entity.Id,
        entity.ProfileTypeId,
        entity.Name,
        entity.Email,
        entity.Phone,
        entity.LanguageId,
        entity.CreatedAt,
        entity.UpdatedAt);
}
