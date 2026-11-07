using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Mappers;
using OtaASimulator.Models.Properties;
namespace OtaASimulator.Application.Properties;

public sealed class StayPolicyService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<StayPolicyResponse> ReplaceAsync(long propertyId, StayPolicyRequest request, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await guard.LockPropertyAsync(propertyId, ct);
        var entity = await db.StayPolicies.SingleOrDefaultAsync(x => x.PropertyId == propertyId, ct);
        if (entity is null) { entity = new StayPolicy { PropertyId = propertyId }; db.StayPolicies.Add(entity); }
        StayPolicyMapper.Apply(request, entity);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return StayPolicyMapper.ToResponse(entity);
    }
}
