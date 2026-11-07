using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Mappers;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Application.Properties;

public sealed class DescriptionService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<IReadOnlyList<DescriptionResponse>> ReplaceAsync(long propertyId, IReadOnlyList<DescriptionRequest> request, CancellationToken ct)
    {
        InputRules.Unique(request, item => item.LanguageId, "LanguageId");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockPropertyAsync(propertyId, ct);
        foreach (var item in request)
        {
            await CatalogGuard.ExistsAsync(db.Languages, x => x.Id == item.LanguageId, "languageId", ct);
        }
        var existing = await db.PropertyDescriptions.Where(x => x.PropertyId == propertyId).ToListAsync(ct);
        var desiredKeys = request.Select(item => item.LanguageId).ToHashSet();
        db.PropertyDescriptions.RemoveRange(existing.Where(x => !desiredKeys.Contains(x.LanguageId)));
        var byKey = existing.ToDictionary(x => x.LanguageId);
        var result = new List<PropertyDescription>();
        foreach (var item in request)
        {
            if (!byKey.TryGetValue(item.LanguageId, out var entity))
            {
                entity = new PropertyDescription { PropertyId = propertyId };
                db.PropertyDescriptions.Add(entity);
            }
            DescriptionMapper.Apply(item, entity);
            result.Add(entity);
        }
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return result.Select(DescriptionMapper.ToResponse).ToArray();
    }
}
