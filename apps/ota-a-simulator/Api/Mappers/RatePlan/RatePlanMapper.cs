using OtaASimulator.Domain;
using OtaASimulator.Models.RatePlans;

namespace OtaASimulator.Mappers;

public static class RatePlanMapper
{
    public static void Apply(RatePlanRequest request, RatePlan entity)
    {
        entity.ExternalCode = request.ExternalCode;
        entity.Name = request.Name;
        entity.CancellationPolicyId = request.CancellationPolicyId;
        entity.MinimumNights = request.MinimumNights;
        entity.MaximumNights = request.MaximumNights;
        entity.IsActive = request.IsActive;
    }

    public static RatePlanResponse ToResponse(RatePlan entity) => new(
        entity.Id,
        entity.PropertyId,
        entity.UnitId,
        entity.ExternalCode,
        entity.Name,
        new CancellationPolicyResponse(
            entity.CancellationPolicy.Id,
            entity.CancellationPolicy.Code,
            entity.CancellationPolicy.Name,
            entity.CancellationPolicy.FreeUntilDaysBefore,
            entity.CancellationPolicy.PenaltyPercent),
        entity.MinimumNights,
        entity.MaximumNights,
        entity.IsActive,
        entity.CreatedAt,
        entity.UpdatedAt);
}
