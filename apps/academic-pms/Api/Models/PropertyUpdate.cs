using AcademicPms.Application;

namespace AcademicPms.Api.Models;

public sealed record PropertyUpdate(
    string Name,
    decimal Price,
    string RoomType,
    string Neighbourhood,
    decimal Latitude,
    decimal Longitude,
    int MinimumNights)
{
    public PropertyState ToState() => new(
        Name,
        Price,
        RoomType,
        Neighbourhood,
        Latitude,
        Longitude,
        MinimumNights);
}