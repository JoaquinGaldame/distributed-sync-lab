using System.ComponentModel.DataAnnotations;

namespace OtaBSimulator.Api.Models.Calendar;

public sealed record PricePeriod(DateOnly From, DateOnly Until, [Range(1, long.MaxValue)] long AmountMinor);
public sealed record PricePeriodsRequest([Required, MinLength(1), MaxLength(730)] IReadOnlyList<PricePeriod> Periods);
public sealed record AvailabilityPeriod(DateOnly From, DateOnly Until, [Required, StringLength(20)] string StateCode);
public sealed record AvailabilityPeriodsRequest([Required, MinLength(1), MaxLength(730)] IReadOnlyList<AvailabilityPeriod> Periods);
public sealed record CalendarWriteResponse(Guid ListingId, int DaysUpdated);
public sealed record CalendarDeleteResponse(Guid ListingId, int DaysDeleted, DateOnly From, DateOnly Until);
public sealed record CalendarDayResponse(DateOnly Date, long? AmountMinor, string PriceSource, string StateCode);
public sealed record CalendarResponse(Guid ListingId, string CurrencyCode, DateOnly From, DateOnly Until, IReadOnlyList<CalendarDayResponse> Days);
