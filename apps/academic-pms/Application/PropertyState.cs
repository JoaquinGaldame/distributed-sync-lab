namespace AcademicPms.Application;

public sealed record PropertyState(
    string Name,
    decimal Price,
    string RoomType,
    string Neighbourhood,
    decimal Latitude,
    decimal Longitude,
    int MinimumNights);