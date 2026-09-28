using AcademicPms.Domain;
using AcademicPms.Infrastructure.Imports;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AcademicPms.Application.Imports;

public sealed class InsideAirbnbImportService(
    InsideAirbnbCsvReader reader,
    PmsDbContext db)
{
    private const string ExpectedSha256 = "845472c69e282ba85caf651c918701f0ae81b86e123b238f1eec8ef176e28a00";

    public async Task<InsideAirbnbImportResult> ImportAsync(Stream csv, CancellationToken ct)
    {
        // Se lee y valida el archivo completo antes de escribir en la base.
        var dataset = await reader.ReadAsync(csv, ct);

        if (!string.Equals(dataset.Sha256,ExpectedSha256,StringComparison.Ordinal))
        {
            throw new InvalidDataException("El archivo no coincide con el snapshot elegido.");
        }

        if (dataset.OriginalRows != 29_685
            || dataset.ExcludedMissingPrice != 1_792
            || dataset.ExcludedMissingMinimumNights != 3
            || dataset.Properties.Count != 27_890)
        {
            throw new InvalidDataException("El resultado de normalización no coincide con el esperado.");
        }

        // La carga inicial necesita una base PMS vacía para ser reproducible.
        if (await db.Properties.AnyAsync(ct)
            || await db.PropertyChanges.AnyAsync(ct))
        {
            throw new InvalidOperationException("pms_db contiene propiedades o cambios. " + "La carga inicial requiere vaciar únicamente esas tablas.");
        }

        await using var transaction =
            await db.Database.BeginTransactionAsync(ct);

        var importedAt = DateTime.UtcNow;
        var pending = 0;

        foreach (var row in dataset.Properties)
        {
            ct.ThrowIfCancellationRequested();

            db.Properties.Add(new Property
            {
                Id = row.Id,
                SourceListingId = row.SourceListingId,
                Name = row.Name,
                Price = row.Price,
                RoomType = row.RoomType,
                Neighbourhood = row.Neighbourhood,
                Latitude = row.Latitude,
                Longitude = row.Longitude,
                MinimumNights = row.MinimumNights,
                Version = 1,
                UpdatedAt = importedAt
            });

            db.PropertyChanges.Add(new PropertyChange
            {
                EventId = Guid.NewGuid(),
                EventType = "PropertyChanged",
                PropertyId = row.Id,
                SourceListingId = row.SourceListingId,
                Version = 1,
                OccurredAt = importedAt,
                StateName = row.Name,
                StatePrice = row.Price,
                StateRoomType = row.RoomType,
                StateNeighbourhood = row.Neighbourhood,
                StateLatitude = row.Latitude,
                StateLongitude = row.Longitude,
                StateMinimumNights = row.MinimumNights
            });

            pending++;

            if (pending == 500)
            {
                await db.SaveChangesAsync(ct);
                db.ChangeTracker.Clear();
                pending = 0;
            }
        }

        if (pending > 0)
            await db.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);

        return new InsideAirbnbImportResult(
            dataset.Sha256,
            dataset.OriginalRows,
            dataset.ExcludedMissingPrice,
            dataset.ExcludedMissingMinimumNights,
            dataset.Properties.Count,
            importedAt);
    }
}