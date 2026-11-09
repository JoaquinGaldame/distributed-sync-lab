using System.Data;
using Microsoft.EntityFrameworkCore;
using OtaBSimulator.Api.Models.Calendar;
using OtaBSimulator.Application.Common;
using OtaBSimulator.Domain;
using OtaBSimulator.Infrastructure.Persistence;

namespace OtaBSimulator.Application.Calendar;

public sealed class CalendarService(OtaBDbContext db, ResourceGuard guard)
{
    public async Task<CalendarWriteResponse> PutPrices(Guid listingId, PricePeriodsRequest request, CancellationToken ct)
    {
        var days = Expand(request.Periods, x => x.From, x => x.Until).SelectMany((period, index) => period.Select(date => (date, request.Periods[index].AmountMinor))).ToArray();
        await using var tx = await db.Database.BeginTransactionAsync(ct); await guard.LockedListingAsync(listingId, ct);
        var dates = days.Select(x => x.date).ToArray();
        var existing = await db.DailyPrices.Where(x => x.ListingId == listingId && dates.Contains(x.StayDate)).ToDictionaryAsync(x => x.StayDate, ct);
        foreach (var (date, amount) in days)
        {
            if (existing.TryGetValue(date, out var row)) row.AmountMinor = amount;
            else db.DailyPrices.Add(new DailyPrice { ListingId = listingId, StayDate = date, AmountMinor = amount });
        }
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return new(listingId, days.Length);
    }

    public async Task<CalendarWriteResponse> PutAvailability(Guid listingId, AvailabilityPeriodsRequest request, CancellationToken ct)
    {
        var periods = Expand(request.Periods, x => x.From, x => x.Until);
        var codes = request.Periods.Select(x => x.StateCode).Distinct().ToArray();
        if (await db.AvailabilityStates.CountAsync(x => codes.Contains(x.Code), ct) != codes.Length)
            throw ApiException.Invalid("Uno o más stateCode no existen en el catálogo; UNCONFIGURED no es persistible.");
        var days = periods.SelectMany((period, index) => period.Select(date => (date, request.Periods[index].StateCode))).ToArray();
        await using var tx = await db.Database.BeginTransactionAsync(ct); await guard.LockedListingAsync(listingId, ct);
        var dates = days.Select(x => x.date).ToArray();
        var existing = await db.DailyAvailability.Where(x => x.ListingId == listingId && dates.Contains(x.StayDate)).ToDictionaryAsync(x => x.StayDate, ct);
        foreach (var (date, state) in days)
        {
            if (existing.TryGetValue(date, out var row)) row.StateCode = state;
            else db.DailyAvailability.Add(new DailyAvailability { ListingId = listingId, StayDate = date, StateCode = state });
        }
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return new(listingId, days.Length);
    }

    public Task<CalendarDeleteResponse> DeletePrices(Guid id, DateOnly from, DateOnly until, CancellationToken ct) => Delete(id, from, until, true, ct);
    public Task<CalendarDeleteResponse> DeleteAvailability(Guid id, DateOnly from, DateOnly until, CancellationToken ct) => Delete(id, from, until, false, ct);

    private async Task<CalendarDeleteResponse> Delete(Guid id, DateOnly from, DateOnly until, bool prices, CancellationToken ct)
    {
        ValidateRange(from, until);
        await using var tx = await db.Database.BeginTransactionAsync(ct); await guard.LockedListingAsync(id, ct);
        var deleted = prices
            ? await db.DailyPrices.Where(x => x.ListingId == id && x.StayDate >= from && x.StayDate < until).ExecuteDeleteAsync(ct)
            : await db.DailyAvailability.Where(x => x.ListingId == id && x.StayDate >= from && x.StayDate < until).ExecuteDeleteAsync(ct);
        await tx.CommitAsync(ct); return new(id, deleted, from, until);
    }

    public async Task<CalendarResponse> Get(Guid id, DateOnly from, DateOnly until, CancellationToken ct)
    {
        ValidateRange(from, until);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, ct);
        var listing = await db.Listings.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw ApiException.NotFound("El anuncio solicitado no existe.");
        var basePrice = await db.ListingPricing.AsNoTracking().Where(x => x.ListingId == id).Select(x => (long?)x.BaseNightlyAmountMinor).SingleOrDefaultAsync(ct);
        var prices = await db.DailyPrices.AsNoTracking().Where(x => x.ListingId == id && x.StayDate >= from && x.StayDate < until).ToDictionaryAsync(x => x.StayDate, ct);
        var availability = await db.DailyAvailability.AsNoTracking().Where(x => x.ListingId == id && x.StayDate >= from && x.StayDate < until).ToDictionaryAsync(x => x.StayDate, ct);
        var days = Dates(from, until).Select(date =>
        {
            var hasOverride = prices.TryGetValue(date, out var price);
            return new CalendarDayResponse(date, hasOverride ? price!.AmountMinor : basePrice,
                hasOverride ? "OVERRIDE" : basePrice.HasValue ? "BASE" : "UNCONFIGURED",
                availability.TryGetValue(date, out var state) ? state.StateCode : "UNCONFIGURED");
        }).ToArray();
        await tx.CommitAsync(ct); return new(id, listing.CurrencyCode, from, until, days);
    }

    private static IReadOnlyList<IReadOnlyList<DateOnly>> Expand<T>(IReadOnlyList<T>? source, Func<T, DateOnly> from, Func<T, DateOnly> until)
    {
        if (source is null || source.Count is < 1 or > 730 || source.Any(x => x is null)) throw ApiException.Invalid("periods debe contener entre 1 y 730 intervalos sin elementos null.");
        var ordered = source.Select((value, original) => (value, original, from: from(value), until: until(value))).OrderBy(x => x.from).ToArray();
        var total = 0;
        for (var i = 0; i < ordered.Length; i++)
        {
            ValidateRange(ordered[i].from, ordered[i].until);
            if (i > 0 && ordered[i].from < ordered[i - 1].until) throw ApiException.Invalid("Los intervalos no pueden superponerse.");
            total = checked(total + ordered[i].until.DayNumber - ordered[i].from.DayNumber);
            if (total > 730) throw ApiException.Invalid("El total de fechas expandidas no puede superar 730.");
        }
        var byOriginal = new IReadOnlyList<DateOnly>[source.Count];
        foreach (var x in ordered) byOriginal[x.original] = Dates(x.from, x.until).ToArray();
        return byOriginal;
    }

    private static void ValidateRange(DateOnly from, DateOnly until)
    {
        var nights = until.DayNumber - from.DayNumber;
        if (nights is < 1 or > 730) throw ApiException.Invalid("El intervalo debe contener entre 1 y 730 noches y until debe ser posterior a from.");
    }

    private static IEnumerable<DateOnly> Dates(DateOnly from, DateOnly until)
    {
        for (var date = from; date < until; date = date.AddDays(1)) yield return date;
    }
}
