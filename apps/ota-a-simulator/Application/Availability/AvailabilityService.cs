using Microsoft.EntityFrameworkCore;
using OtaASimulator.Application.Common;
using OtaASimulator.Domain;
using OtaASimulator.Infrastructure.Persistence;
using OtaASimulator.Models.Availability;

namespace OtaASimulator.Application.Availability;

public sealed class AvailabilityService(OtaADbContext db, ResourceGuard guard)
{
    public async Task<AvailabilityResponse> SyncAsync(AvailabilitySyncRequest request, CancellationToken ct)
    {
        var dates = request.Days.Select(x => x.Date).ToArray();
        if (dates.Distinct().Count() != dates.Length)
            throw ApiException.Invalid("Days no puede contener fechas duplicadas.");

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var unit = await guard.LockUnitAsync(request.PropertyId, request.UnitId, ct);
        await guard.RatePlanAsync(request.PropertyId, request.UnitId, request.RatePlanId, ct);

        var inventories = await db.UnitInventories
            .Where(x => x.UnitId == request.UnitId && dates.Contains(x.StayDate))
            .ToDictionaryAsync(x => x.StayDate, ct);
        var calendars = await db.RateCalendars
            .Where(x => x.RatePlanId == request.RatePlanId && dates.Contains(x.StayDate))
            .ToDictionaryAsync(x => x.StayDate, ct);

        foreach (var day in request.Days)
        {
            if (day.Price.HasValue)
            {
                if (!calendars.TryGetValue(day.Date, out var calendar))
                {
                    calendar = new RateCalendar
                    {
                        RatePlanId = request.RatePlanId,
                        StayDate = day.Date
                    };
                    calendars.Add(day.Date, calendar);
                    db.RateCalendars.Add(calendar);
                }

                calendar.NightlyPrice = day.Price.Value;
            }

            if (day.AvailableUnits.HasValue && day.Closed.HasValue)
            {
                if (day.AvailableUnits.Value > unit.TotalUnits)
                    throw ApiException.Invalid($"La disponibilidad de {day.Date} supera las {unit.TotalUnits} unidades configuradas.");

                if (!inventories.TryGetValue(day.Date, out var inventory))
                {
                    inventory = new UnitInventory
                    {
                        UnitId = request.UnitId,
                        StayDate = day.Date
                    };
                    inventories.Add(day.Date, inventory);
                    db.UnitInventories.Add(inventory);
                }

                inventory.AvailableUnits = day.AvailableUnits.Value;
                inventory.Closed = day.Closed.Value;
            }
        }

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return BuildResponse(request.PropertyId, request.UnitId, request.RatePlanId, dates, inventories, calendars);
    }

    public async Task<AvailabilityResponse> ListAsync(AvailabilityListRequest request, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead, ct);
        await guard.RatePlanAsync(request.PropertyId, request.UnitId, request.RatePlanId, ct);

        DateOnly endDate;
        try
        {
            endDate = request.StartDate.AddDays(request.Days - 1);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw ApiException.Invalid("El rango solicitado excede las fechas admitidas.");
        }

        var inventories = await db.UnitInventories
            .AsNoTracking()
            .Where(x => x.UnitId == request.UnitId && x.StayDate >= request.StartDate && x.StayDate <= endDate)
            .ToDictionaryAsync(x => x.StayDate, ct);
        var calendars = await db.RateCalendars
            .AsNoTracking()
            .Where(x => x.RatePlanId == request.RatePlanId && x.StayDate >= request.StartDate && x.StayDate <= endDate)
            .ToDictionaryAsync(x => x.StayDate, ct);
        var dates = inventories.Keys
            .Concat(calendars.Keys)
            .Distinct()
            .OrderBy(date => date)
            .ToArray();

        if (dates.Length == 0)
            throw ApiException.AvailabilityNotConfigured(
                "No existen precios ni disponibilidad configurados dentro del período solicitado.");

        await tx.CommitAsync(ct);

        return BuildResponse(request.PropertyId, request.UnitId, request.RatePlanId, dates, inventories, calendars);
    }

    private static AvailabilityResponse BuildResponse(
        long propertyId,
        long unitId,
        long ratePlanId,
        IEnumerable<DateOnly> dates,
        IReadOnlyDictionary<DateOnly, UnitInventory> inventories,
        IReadOnlyDictionary<DateOnly, RateCalendar> calendars)
    {
        var days = dates.Select(date =>
        {
            inventories.TryGetValue(date, out var inventory);
            calendars.TryGetValue(date, out var calendar);

            return new AvailabilityDayResponse(
                date,
                calendar?.NightlyPrice,
                inventory?.AvailableUnits,
                inventory is null ? null : (short)inventory.Closed,
                inventory?.Closed.ToApiStatus());
        }).ToArray();

        return new AvailabilityResponse(propertyId, unitId, ratePlanId, days);
    }
}
