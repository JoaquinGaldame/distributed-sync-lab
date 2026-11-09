using Microsoft.EntityFrameworkCore;
using OtaBSimulator.Api.Models.Metadata;
using OtaBSimulator.Infrastructure.Persistence;

namespace OtaBSimulator.Application.Metadata;

public sealed class MetadataService(OtaBDbContext db)
{
    public async Task<MetadataResponse> Get(CancellationToken ct) => new(
        await db.Currencies.AsNoTracking().OrderBy(x => x.Code).Select(x => new CurrencyMetadata(x.Id, x.Code, x.Name, x.DecimalPlaces)).ToListAsync(ct),
        await db.Languages.AsNoTracking().OrderBy(x => x.Code).Select(x => new NamedIdCatalogItem(x.Id, x.Code, x.Name)).ToListAsync(ct),
        await db.ListingCategories.AsNoTracking().OrderBy(x => x.Code).Select(x => new NamedIdCatalogItem(x.Id, x.Code, x.Name)).ToListAsync(ct),
        await db.RentalModes.AsNoTracking().OrderBy(x => x.Code).Select(x => new NamedCatalogItem(x.Code, x.Name)).ToListAsync(ct),
        await db.AmenityGroups.AsNoTracking().OrderBy(x => x.Code).Select(x => new NamedCatalogItem(x.Code, x.Name)).ToListAsync(ct),
        await db.RoomTypes.AsNoTracking().OrderBy(x => x.Code).Select(x => new NamedIdCatalogItem(x.Id, x.Code, x.Name)).ToListAsync(ct),
        await db.BedTypes.AsNoTracking().OrderBy(x => x.Code).Select(x => new NamedIdCatalogItem(x.Id, x.Code, x.Name)).ToListAsync(ct),
        await db.Amenities.AsNoTracking().OrderBy(x => x.Code).Select(x => new AmenityMetadata(x.Id, x.Code, x.GroupCode, x.Name)).ToListAsync(ct),
        await db.ContactRoles.AsNoTracking().OrderBy(x => x.Code).Select(x => new NamedCatalogItem(x.Code, x.Name)).ToListAsync(ct),
        await db.ListingStatuses.AsNoTracking().OrderBy(x => x.Code).Select(x => new NamedIdCatalogItem(x.Id, x.Code, x.Name)).ToListAsync(ct),
        await db.AvailabilityStates.AsNoTracking().OrderBy(x => x.Code).Select(x => new AvailabilityStateMetadata(x.Code, x.Name, x.IsOpen)).ToListAsync(ct),
        await db.CancellationTerms.AsNoTracking().OrderBy(x => x.Code).Select(x => new CancellationTermMetadata(x.Code, x.Name, x.FreeCancellationDaysBefore, x.RefundPercentAfterDeadline)).ToListAsync(ct),
        await db.FeeBases.AsNoTracking().OrderBy(x => x.Code).Select(x => new NamedCatalogItem(x.Code, x.Name)).ToListAsync(ct));
}
