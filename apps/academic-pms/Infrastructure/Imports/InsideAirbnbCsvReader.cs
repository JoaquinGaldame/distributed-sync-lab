using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using AcademicPms.Application.Imports;
using Microsoft.VisualBasic.FileIO;

namespace AcademicPms.Infrastructure.Imports;

public sealed class InsideAirbnbCsvReader
{
    public async Task<InsideAirbnbDataset> ReadAsync(
        Stream source,
        CancellationToken ct)
    {
        using var buffer = new MemoryStream();
        await source.CopyToAsync(buffer, ct);

        var bytes = buffer.ToArray();
        var sha256 = Convert
            .ToHexString(SHA256.HashData(bytes))
            .ToLowerInvariant();

        using var parser = new TextFieldParser(
            new MemoryStream(bytes),
            Encoding.UTF8)
        {
            TextFieldType = FieldType.Delimited,
            HasFieldsEnclosedInQuotes = true,
            TrimWhiteSpace = false
        };

        parser.SetDelimiters(",");

        var header = parser.ReadFields()
            ?? throw new InvalidDataException("El CSV está vacío.");

        int Column(string name)
        {
            var index = Array.IndexOf(header, name);

            if (index < 0)
                throw new InvalidDataException(
                    $"Falta la columna '{name}'.");

            return index;
        }

        var idColumn = Column("id");
        var nameColumn = Column("name");
        var priceColumn = Column("price");
        var roomTypeColumn = Column("room_type");
        var neighbourhoodColumn = Column("neighbourhood");
        var latitudeColumn = Column("latitude");
        var longitudeColumn = Column("longitude");
        var minimumNightsColumn = Column("minimum_nights");

        var properties = new List<InsideAirbnbProperty>();
        var seenIds = new HashSet<string>(StringComparer.Ordinal);

        var originalRows = 0;
        var missingPrice = 0;
        var missingMinimumNights = 0;

        while (!parser.EndOfData)
        {
            ct.ThrowIfCancellationRequested();

            var fields = parser.ReadFields()
                ?? throw new InvalidDataException(
                    $"No se pudo leer la fila {originalRows + 1}.");

            originalRows++;

            if (fields.Length != header.Length)
                throw new InvalidDataException(
                    $"Cantidad de columnas inválida en la fila {originalRows}.");

            string Field(int index) => fields[index].Trim();

            var sourceId = Field(idColumn);

            if (sourceId.Length == 0
                || sourceId.Length > 47
                || !sourceId.All(char.IsAsciiDigit)
                || !seenIds.Add(sourceId))
            {
                throw new InvalidDataException(
                    $"ID vacío, inválido o duplicado en la fila {originalRows}.");
            }

            var rawPrice = Field(priceColumn);

            if (rawPrice.Length == 0)
            {
                missingPrice++;
                continue;
            }

            var rawMinimumNights = Field(minimumNightsColumn);

            if (rawMinimumNights.Length == 0)
            {
                missingMinimumNights++;
                continue;
            }

            var name = Field(nameColumn);
            var neighbourhood = Field(neighbourhoodColumn);

            var roomType = Field(roomTypeColumn) switch
            {
                "Entire home/apt" => "ENTIRE_HOME",
                "Private room" => "PRIVATE_ROOM",
                "Hotel room" => "HOTEL_ROOM",
                "Shared room" => "SHARED_ROOM",
                _ => throw new InvalidDataException(
                    $"room_type desconocido en la fila {originalRows}.")
            };

            const NumberStyles decimalStyle =
                NumberStyles.AllowLeadingSign |
                NumberStyles.AllowDecimalPoint;

            if (!decimal.TryParse(
                    rawPrice,
                    decimalStyle,
                    CultureInfo.InvariantCulture,
                    out var price)
                || !decimal.TryParse(
                    Field(latitudeColumn),
                    decimalStyle,
                    CultureInfo.InvariantCulture,
                    out var latitude)
                || !decimal.TryParse(
                    Field(longitudeColumn),
                    decimalStyle,
                    CultureInfo.InvariantCulture,
                    out var longitude)
                || !int.TryParse(
                    rawMinimumNights,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var minimumNights))
            {
                throw new InvalidDataException(
                    $"Número inválido en la fila {originalRows}.");
            }

            if (string.IsNullOrWhiteSpace(name)
                || name.Length > 255
                || string.IsNullOrWhiteSpace(neighbourhood)
                || neighbourhood.Length > 100
                || price < 0
                || price >= 10_000_000_000m
                || decimal.Round(price, 2) != price
                || latitude is < -90m or > 90m
                || longitude is < -180m or > 180m
                || minimumNights < 1)
            {
                throw new InvalidDataException(
                    $"Datos fuera del modelo en la fila {originalRows}.");
            }

            properties.Add(new InsideAirbnbProperty(
                sourceId,
                sourceId,
                name,
                price,
                roomType,
                neighbourhood,
                latitude,
                longitude,
                minimumNights));
        }

        properties.Sort((a, b) =>
            StringComparer.Ordinal.Compare(
                a.SourceListingId,
                b.SourceListingId));

        return new InsideAirbnbDataset(
            sha256,
            originalRows,
            missingPrice,
            missingMinimumNights,
            properties);
    }
}
