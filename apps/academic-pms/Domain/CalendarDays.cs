namespace AcademicPms.Domain;

public sealed class CalendarDays
{
    public string PropertyId { get; set; } = "";
    public DateOnly Date { get; set; }
    public bool IsAvailable { get; set; }
    public int MinimumNights { get; set; }
    public int MaximumNights { get; set; }
}
