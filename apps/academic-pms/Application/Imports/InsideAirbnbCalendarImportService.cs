using System.Security.Cryptography;
using AcademicPms.Infrastructure.Imports;
using AcademicPms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace AcademicPms.Application.Imports;

public sealed class InsideAirbnbCalendarImportService(
    InsideAirbnbCalendarCsvReader reader,
    PmsDbContext db)
{
    private const string ExpectedSha256 =
        "610564cebd7c2224006ebc7d4d090c8949aa3fd1c299e41134113050a2ef5d38";

    private const int ExpectedOriginalRows = 10_835_026;
    private const int ExpectedSourceListings = 29_685;
    private const int ExpectedImportedRows = 10_179_851;
    private const int ExpectedExcludedRows = 655_175;
    private const int ExpectedPropertiesWithCalendar = 27_890;

    public async Task<InsideAirbnbCalendarImportResult> ImportAsync(
        Stream csv,
        CancellationToken ct)
    {
        if (!csv.CanSeek)
            throw new InvalidDataException("El archivo de calendario debe permitir lectura reposicionable.");

        if (!await db.Properties.AnyAsync(ct))
            throw new InvalidOperationException("Primero se debe importar listings.csv.");

        if (await db.CalendarDays.AnyAsync(ct))
            throw new InvalidOperationException("calendar_days ya contiene datos.");

        var sha256 = Convert
            .ToHexString(await SHA256.HashDataAsync(csv, ct))
            .ToLowerInvariant();

        if (!string.Equals(sha256, ExpectedSha256, StringComparison.Ordinal))
            throw new InvalidDataException("El calendario no coincide con el snapshot elegido.");

        csv.Position = 0;

        var propertyIds = await db.Properties
            .AsNoTracking()
            .Select(x => x.Id)
            .ToHashSetAsync(StringComparer.Ordinal, ct);

        var matchedPropertyIds = new HashSet<string>(StringComparer.Ordinal);
        var importedRows = 0;
        var excludedRows = 0;
        var importedAt = DateTime.UtcNow;

        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var connection = (NpgsqlConnection)db.Database.GetDbConnection();

        await using var importer = await connection.BeginBinaryImportAsync(
            """
            COPY calendar_days (
                property_id,
                date,
                is_available,
                minimum_nights,
                maximum_nights
            ) FROM STDIN (FORMAT BINARY)
            """,
            ct);

        var readResult = await reader.ReadAsync(
            csv,
            async (row, cancellationToken) =>
            {
                if (!propertyIds.Contains(row.PropertyId))
                {
                    excludedRows++;
                    return;
                }

                await importer.StartRowAsync(cancellationToken);
                await importer.WriteAsync(
                    row.PropertyId,
                    NpgsqlDbType.Varchar,
                    cancellationToken);
                await importer.WriteAsync(
                    row.Date,
                    NpgsqlDbType.Date,
                    cancellationToken);
                await importer.WriteAsync(
                    row.IsAvailable,
                    NpgsqlDbType.Boolean,
                    cancellationToken);
                await importer.WriteAsync(
                    row.MinimumNights,
                    NpgsqlDbType.Integer,
                    cancellationToken);
                await importer.WriteAsync(
                    row.MaximumNights,
                    NpgsqlDbType.Integer,
                    cancellationToken);

                importedRows++;
                matchedPropertyIds.Add(row.PropertyId);
            },
            ct);

        if (readResult.OriginalRows != ExpectedOriginalRows
            || readResult.SourceListings != ExpectedSourceListings
            || importedRows != ExpectedImportedRows
            || excludedRows != ExpectedExcludedRows
            || matchedPropertyIds.Count != ExpectedPropertiesWithCalendar)
        {
            throw new InvalidDataException(
                "El resultado de normalización del calendario no coincide con el esperado.");
        }

        await importer.CompleteAsync(ct);
        await transaction.CommitAsync(ct);

        return new InsideAirbnbCalendarImportResult(
            sha256,
            readResult.OriginalRows,
            importedRows,
            excludedRows,
            matchedPropertyIds.Count,
            readResult.MinimumDate,
            readResult.MaximumDate,
            importedAt);
    }
}
