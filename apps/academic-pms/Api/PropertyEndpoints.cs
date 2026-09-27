using AcademicPms.Application;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AcademicPms.Api;

public static class PropertyEndpoints
{
    public static void MapPropertyEndpoints(this WebApplication app)
    {
        app.MapPost("/properties", async (
            PropertyInput input,
            PropertyService service,
            CancellationToken ct) =>
        {
            if (!Valid(input) || string.IsNullOrWhiteSpace(input.Id)
                || input.Id.Length > 50)
                return Results.BadRequest("Id, name y price válidos son obligatorios.");

            var property = await service.CreateAsync(
                input.Id, input.Name, input.Price, ct);

            return property is null
                ? Results.Conflict("La propiedad ya existe.")
                : Results.Created($"/properties/{property.Id}", property);
        });

        app.MapGet("/properties/{id}", async (
            string id,
            PmsDbContext db,
            CancellationToken ct) =>
        {
            var property = await db.Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            return property is null
                ? Results.NotFound()
                : Results.Ok(property);
        });

        app.MapPut("/properties/{id}", async (
            string id,
            PropertyUpdate input,
            PropertyService service,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(input.Name)
                || input.Name.Length > 255
                || input.Price < 0)
                return Results.BadRequest("Name y price válidos son obligatorios.");

            try
            {
                var property = await service.UpdateAsync(
                    id, input.Name, input.Price, ct);

                return property is null
                    ? Results.NotFound()
                    : Results.Ok(property);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Results.Conflict(
                    "La propiedad cambió durante la actualización.");
            }
        });

        // Lectura inicial para inspeccionar los cambios del PMS.
        app.MapGet("/changes", async (
            PmsDbContext db,
            CancellationToken ct) =>
        {
            var changes = await db.PropertyChanges
                .AsNoTracking()
                .OrderBy(x => x.Sequence)
                .Take(100)
                .ToListAsync(ct);

            return Results.Ok(changes.Select(x => new
            {
                x.Sequence,
                Event = new
                {
                    x.EventId,
                    x.EventType,
                    x.PropertyId,
                    x.Version,
                    x.OccurredAt
                },
                DesiredState = new
                {
                    Name = x.StateName,
                    Price = x.StatePrice
                }
            }));
        });
    }

    private static bool Valid(PropertyInput input) =>
        !string.IsNullOrWhiteSpace(input.Name)
        && input.Name.Length <= 255
        && input.Price >= 0;
}

public sealed record PropertyInput(string Id, string Name, decimal Price);
public sealed record PropertyUpdate(string Name, decimal Price);