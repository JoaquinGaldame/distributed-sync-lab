namespace OtaASimulator.Domain;

public sealed class RateCalendar
{
    public long RatePlanId { get; set; }
    public DateOnly StayDate { get; set; }
    public decimal NightlyPrice { get; set; }
    public bool Closed { get; set; } = false;
    public int? MinimumNightsOverride { get; set; }
    public int? MaximumNightsOverride { get; set; }
    public bool ClosedToArrival { get; set; } = false;
    public bool ClosedToDeparture { get; set; } = false;
    public DateTime UpdatedAt { get; set; }

    public RatePlan RatePlan { get; set; } = null!;
}
