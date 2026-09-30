using AcademicPms.Domain;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AcademicPms.Application;

public enum PublicationUpsertStatus
{
    Created,
    Updated,
    PropertyNotFound,
    ChannelNotFound
}

public sealed record PublicationUpsertResult(
    PublicationUpsertStatus Status,
    Publication? Publication);

public sealed class PublicationService(PmsDbContext db)
{
    public async Task<PublicationUpsertResult> UpsertAsync(
        string propertyId,
        long channelId,
        string? externalPropertyId,
        bool published,
        CancellationToken cancellationToken)
    {
        if (!await db.Properties.AnyAsync(
                x => x.Id == propertyId,
                cancellationToken))
        {
            return new(
                PublicationUpsertStatus.PropertyNotFound,
                null);
        }

        if (!await db.Channels.AnyAsync(
                x => x.Id == channelId,
                cancellationToken))
        {
            return new(
                PublicationUpsertStatus.ChannelNotFound,
                null);
        }

        var publication = await db.Publications.FindAsync(
            [propertyId, channelId], cancellationToken);

        if (publication is null)
        {
            var now = DateTime.UtcNow;
            publication = new Publication
            {
                PropertyId = propertyId,
                ChannelId = channelId,
                ExternalPropertyId = externalPropertyId,
                Published = published,
                CreatedAt = now,
                UpdatedAt = now
            };

            db.Publications.Add(publication);
            await db.SaveChangesAsync(cancellationToken);

            return new(PublicationUpsertStatus.Created, publication);
        }

        if (publication.ExternalPropertyId != externalPropertyId
            || publication.Published != published)
        {
            publication.ExternalPropertyId = externalPropertyId;
            publication.Published = published;
            publication.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
        }

        return new(PublicationUpsertStatus.Updated, publication);
    }
}
