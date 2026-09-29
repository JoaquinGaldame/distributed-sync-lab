using System.Globalization;
using System.Text;
using AcademicPms.Application.Imports;

namespace AcademicPms.Infrastructure.Imports;

public sealed class InsideAirbnbCalendarCsvReader
{
    private const string ExpectedHeader =
        "listing_id,date,available,minimum_nights,maximum_nights";

    public async Task<InsideAirbnbCalendarReadResult> ReadAsync(
        Stream source,
        Func<InsideAirbnbCalendarRow, CancellationToken, ValueTask> onRow,
        CancellationToken ct)
    {
        using var reader = new StreamReader(
            source,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 64 * 1024,
            leaveOpen: true);

        var header = await reader.ReadLineAsync(ct);

        if (!string.Equals(header, ExpectedHeader, StringComparison.Ordinal))
            throw new InvalidDataException("Encabezado de calendar.csv inválido.");

        var sourceListings = new HashSet<string>(StringComparer.Ordinal);
        var originalRows = 0;
        DateOnly? minimumDate = null;
        DateOnly? maximumDate = null;

        while (await reader.ReadLineAsync(ct) is { } line)
        {
            originalRows++;

            var fields = line.Split(',');

            if (fields.Length != 5)
                throw InvalidRow(originalRows, "cantidad de columnas inválida");

            var propertyId = fields[0].Trim();

            if (propertyId.Length == 0
                || propertyId.Length > 50
                || !propertyId.All(char.IsAsciiDigit))
            {
                throw InvalidRow(originalRows, "listing_id inválido");
            }

            if (!DateOnly.TryParseExact(
                    fields[1].Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                throw InvalidRow(originalRows, "date inválida");
            }

            var isAvailable = fields[2].Trim() switch
            {
                "t" => true,
                "f" => false,
                _ => throw InvalidRow(originalRows, "available inválido")
            };

            if (!int.TryParse(
                    fields[3].Trim(),
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var minimumNights)
                || minimumNights < 1)
            {
                throw InvalidRow(originalRows, "minimum_nights inválido");
            }

            if (!int.TryParse(
                    fields[4].Trim(),
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var maximumNights)
                || maximumNights < minimumNights)
            {
                throw InvalidRow(originalRows, "maximum_nights inválido");
            }

            sourceListings.Add(propertyId);
            minimumDate = minimumDate is null || date < minimumDate
                ? date
                : minimumDate;
            maximumDate = maximumDate is null || date > maximumDate
                ? date
                : maximumDate;

            await onRow(
                new InsideAirbnbCalendarRow(
                    propertyId,
                    date,
                    isAvailable,
                    minimumNights,
                    maximumNights),
                ct);
        }

        if (originalRows == 0 || minimumDate is null || maximumDate is null)
            throw new InvalidDataException("calendar.csv no contiene filas.");

        return new InsideAirbnbCalendarReadResult(
            originalRows,
            sourceListings.Count,
            minimumDate.Value,
            maximumDate.Value);
    }

    private static InvalidDataException InvalidRow(int row, string detail) =>
        new($"Fila {row} de calendar.csv: {detail}.");
}
