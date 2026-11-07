using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Mappers;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Application.Properties;

public sealed class ContactService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<IReadOnlyList<ContactResponse>> ReplaceAsync(long propertyId, IReadOnlyList<ContactRequest> request, CancellationToken ct)
    {
        InputRules.Unique(request, item => item.ProfileTypeId, "ProfileTypeId");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockPropertyAsync(propertyId, ct);
        foreach (var item in request)
        {
            await CatalogGuard.ExistsAsync(db.ProfileTypes, x => x.Id == item.ProfileTypeId, "profileTypeId", ct);
            await CatalogGuard.ExistsAsync(db.Languages, x => x.Id == item.LanguageId, "languageId", ct);
        }
        var existing = await db.PropertyContacts.Where(x => x.PropertyId == propertyId).ToListAsync(ct);
        var desiredKeys = request.Select(item => item.ProfileTypeId).ToHashSet();
        db.PropertyContacts.RemoveRange(existing.Where(x => !desiredKeys.Contains(x.ProfileTypeId)));
        var byKey = existing.ToDictionary(x => x.ProfileTypeId);
        var result = new List<PropertyContact>();
        foreach (var item in request)
        {
            if (!byKey.TryGetValue(item.ProfileTypeId, out var entity))
            {
                entity = new PropertyContact { PropertyId = propertyId };
                db.PropertyContacts.Add(entity);
            }
            ContactMapper.Apply(item, entity);
            result.Add(entity);
        }
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return result.Select(ContactMapper.ToResponse).ToArray();
    }
}
