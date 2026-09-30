using AcademicPms.Domain;

namespace AcademicPms.Api.Models;

public sealed record PublicationResponse(
    string PropertyId,
    long ChannelId,
    string? ExternalPropertyId,
    bool Published,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static PublicationResponse From(Publication publication) => new(
        publication.PropertyId,
        publication.ChannelId,
        publication.ExternalPropertyId,
        publication.Published,
        publication.CreatedAt,
        publication.UpdatedAt);
}
