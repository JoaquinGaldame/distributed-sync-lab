using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Mappers;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Application.Properties;

public sealed class ChargeService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<IReadOnlyList<ChargeResponse>> ReplaceAsync(long propertyId, IReadOnlyList<ChargeRequest> request, CancellationToken ct)
    {
        InputRules.Unique(request, item => item.Code, "Code");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockPropertyAsync(propertyId, ct);
        var existing = await db.PropertyCharges.Where(x => x.PropertyId == propertyId).ToListAsync(ct);
        var desiredKeys = request.Select(item => item.Code).ToHashSet();
        db.PropertyCharges.RemoveRange(existing.Where(x => !desiredKeys.Contains(x.Code)));
        var byKey = existing.ToDictionary(x => x.Code);
        var result = new List<PropertyCharge>();
        foreach (var item in request)
        {
            if (!byKey.TryGetValue(item.Code, out var entity))
            {
                entity = new PropertyCharge { PropertyId = propertyId };
                db.PropertyCharges.Add(entity);
            }
            ChargeMapper.Apply(item, entity);
            result.Add(entity);
        }
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return result.Select(ChargeMapper.ToResponse).ToArray();
    }
}
